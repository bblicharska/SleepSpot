using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RentalService.Application.Interfaces;
using RentalService.Domain.Contracts;
using RentalService.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentalService.Infrastructure.Workers
{
    public class ExpiredRentalWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ExpiredRentalWorker> _logger;
        private readonly TimeSpan _interval;

        public ExpiredRentalWorker(IServiceProvider serviceProvider, IConfiguration configuration, ILogger<ExpiredRentalWorker> logger)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            var seconds = configuration.GetValue<int?>("ExpiredRentalWorker:IntervalSeconds") ?? 60;
            if (seconds <= 0) seconds = 60;
            _interval = TimeSpan.FromSeconds(seconds);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("ExpiredRentalWorker starting. Interval: {IntervalSeconds}s", _interval.TotalSeconds);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var uow = scope.ServiceProvider.GetRequiredService<IRentalUnitOfWork>();
                    var propertyClient = scope.ServiceProvider.GetRequiredService<IPropertyClient>();
                    var repo = uow.RentalAgreementRepository;

                    var now = DateTime.UtcNow;

                    IEnumerable<RentalAgreement> expired;
                    try
                    {
                        expired = await repo.GetExpiredActiveRentalsAsync(now);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to read expired rentals from repository.");
                        expired = Array.Empty<RentalAgreement>();
                    }

                    foreach (var rental in expired)
                    {
                        if (stoppingToken.IsCancellationRequested) break;

                        try
                        {
                            var endDate = rental.EndDate ?? now;

                            var terminated = await repo.TryTerminateAsync(rental.Id, endDate);
                            if (!terminated)
                            {
                                _logger.LogDebug("Rental {RentalId} was already processed by another actor or not Active.", rental.Id);
                                continue;
                            }

                            try
                            {
                                await uow.CommitAsync();
                            }
                            catch (Exception exCommit)
                            {
                                _logger.LogError(exCommit, "Commit failed after TryTerminateAsync for rental {RentalId}.", rental.Id);
                            }

                            try
                            {
                                if (rental.RoomId.HasValue)
                                {
                                    _logger.LogInformation("Making room {RoomId} available (rental {RentalId}).", rental.RoomId, rental.Id);
                                    await propertyClient.UpdateRoomAvailabilityAsync(rental.RoomId.Value, true, endDate);
                                }
                                else
                                {
                                    _logger.LogInformation("Making property {PropertyId} available (rental {RentalId}).", rental.PropertyId, rental.Id);
                                    await propertyClient.UpdatePropertyAvailabilityAsync(rental.PropertyId, true, endDate);
                                }

                                _logger.LogInformation("Expired rental processed successfully: {RentalId}", rental.Id);
                            }
                            catch (Exception exProp)
                            {
                                _logger.LogError(exProp, "Failed to synchronize property availability for rental {RentalId}. Rental is terminated in DB, but property might be inconsistent.", rental.Id);
                            }
                        }
                        catch (Exception exRental)
                        {
                            _logger.LogError(exRental, "Unhandled error while processing expired rental {RentalId}.", rental.Id);
                        }
                    }
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "ExpiredRentalWorker iteration failed unexpectedly.");
                }

                try
                {
                    await Task.Delay(_interval, stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
            }

            _logger.LogInformation("ExpiredRentalWorker stopping.");
        }

    }
}

