using AutoMapper;
using RentalService.Application.Dto;
using RentalService.Application.Interfaces;
using RentalService.Domain.Contracts;
using RentalService.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentalService.Application.Services
{
    public class RentalService : IRentalService
    {
        private readonly IRentalUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserClient _userClient;
        private readonly IGroupClient _groupClient;
        private readonly IPropertyClient _propertyClient;

        public RentalService(IRentalUnitOfWork unitOfWork, IMapper mapper, IUserClient userClient, IPropertyClient propertyClient, IGroupClient groupClient)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userClient = userClient;
            _groupClient = groupClient;
            _propertyClient = propertyClient;
        }

        public async Task<IEnumerable<RentalAgreementDto>> GetAllAsync()
        {
            var agreements = await _unitOfWork.RentalAgreementRepository.GetAllAsync();
            var agreementDtos = _mapper.Map<List<RentalAgreementDto>>(agreements);

            var tasks = agreementDtos.Select(async dto =>
            {
                if (dto.PropertyId.HasValue)
                {
                    dto.Property = await _propertyClient.GetPropertyByIdAsync(dto.PropertyId.Value);
                }
                if (dto.RoomId.HasValue)
                {
                    dto.Room = await _propertyClient.GetRoomByIdAsync(dto.RoomId.Value);
                }
                if (dto.GroupId.HasValue)
                {
                    dto.Group = await _groupClient.GetGroupByIdAsync(dto.GroupId.Value);
                }
                if (dto.UserId.HasValue)
                {
                    dto.User = await _userClient.GetUserByIdAsync(dto.UserId.Value);
                }

                return dto;
            });

            return await Task.WhenAll(tasks);
        }


        public async Task<RentalAgreementDto> GetByIdAsync(Guid id)
        {
            var agreement = await _unitOfWork.RentalAgreementRepository.GetByIdAsync(id);
            if (agreement == null) return null;

            var dto = _mapper.Map<RentalAgreementDto>(agreement);

            dto.Property = await _propertyClient.GetPropertyByIdAsync(agreement.PropertyId);

            if (agreement.RoomId.HasValue)
            {
                dto.Room = await _propertyClient.GetRoomByIdAsync(agreement.RoomId.Value);
            }
            if (dto.GroupId.HasValue)
            {
                dto.Group = await _groupClient.GetGroupByIdAsync(dto.GroupId.Value);
            }
            if (agreement.UserId.HasValue)
            {
                dto.User = await _userClient.GetUserByIdAsync(agreement.UserId.Value);
            }

            return dto;
        }

        public async Task<RentalAgreementDto> CreateAsync(CreateRentalAgreementDto dto)
        {
            var agreement = _mapper.Map<RentalAgreement>(dto);
            await _unitOfWork.RentalAgreementRepository.AddAsync(agreement);
            await _unitOfWork.CommitAsync();
            return _mapper.Map<RentalAgreementDto>(agreement);
        }

        public async Task<RentalAgreementDto> UpdateAsync(Guid id, UpdateRentalAgreementDto dto)
        {
            var agreement = await _unitOfWork.RentalAgreementRepository.GetByIdAsync(id);
            if (agreement == null) return null;

            _mapper.Map(dto, agreement);
            _unitOfWork.RentalAgreementRepository.Update(agreement);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<RentalAgreementDto>(agreement);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var agreement = await _unitOfWork.RentalAgreementRepository.GetByIdAsync(id);
            if (agreement == null) return false;

            _unitOfWork.RentalAgreementRepository.Remove(agreement);
            await _unitOfWork.CommitAsync();
            return true;
        }

        public async Task<IEnumerable<RentalAgreementDto>> GetActiveByUserIdAsync(Guid userId)
        {
            var agreements = await _unitOfWork.RentalAgreementRepository.GetActiveByUserIdAsync(userId);
            var agreementDtos = _mapper.Map<List<RentalAgreementDto>>(agreements);

            var tasks = agreementDtos.Select(async dto =>
            {

                if (dto.PropertyId.HasValue)
                {
                    dto.Property = await _propertyClient.GetPropertyByIdAsync(dto.PropertyId.Value);
                }
                if (dto.RoomId.HasValue)
                {
                    dto.Room = await _propertyClient.GetRoomByIdAsync(dto.RoomId.Value);
                }
                if (dto.GroupId.HasValue)
                {
                    dto.Group = await _groupClient.GetGroupByIdAsync(dto.GroupId.Value);
                }
                if (dto.UserId.HasValue)
                {
                    dto.User = await _userClient.GetUserByIdAsync(dto.UserId.Value);
                }

                return dto;
            });

            return await Task.WhenAll(tasks);
        }

        public async Task<IEnumerable<RentalAgreementDto>> GetActiveByGroupIdAsync(Guid groupId)
        {
            var agreements = await _unitOfWork.RentalAgreementRepository.GetActiveByGroupIdAsync(groupId);
            var agreementDtos = _mapper.Map<List<RentalAgreementDto>>(agreements);

            var tasks = agreementDtos.Select(async dto =>
            {

                if (dto.PropertyId.HasValue)
                {
                    dto.Property = await _propertyClient.GetPropertyByIdAsync(dto.PropertyId.Value);
                }
                if (dto.RoomId.HasValue)
                {
                    dto.Room = await _propertyClient.GetRoomByIdAsync(dto.RoomId.Value);
                }
                if (dto.GroupId.HasValue)
                {
                    dto.Group = await _groupClient.GetGroupByIdAsync(dto.GroupId.Value);
                }
                if (dto.UserId.HasValue)
                {
                    dto.User = await _userClient.GetUserByIdAsync(dto.UserId.Value);
                }

                return dto;
            });

            return await Task.WhenAll(tasks);
        }

        public async Task<RentalAgreementDto?> ActivateRentalAsync(Guid rentalAgreementId)
        {
            var agreement = await _unitOfWork.RentalAgreementRepository.GetByIdAsync(rentalAgreementId);
            if (agreement == null) return null;

            if (string.Equals(agreement.Status.ToString(), "Active", StringComparison.OrdinalIgnoreCase))
            {
                return _mapper.Map<RentalAgreementDto>(agreement);
            }

            agreement.Status = RentalAgreementStatus.Active;
            _unitOfWork.RentalAgreementRepository.Update(agreement);

            DateTime? newAvailableSince = agreement.EndDate;

            if (agreement.RoomId.HasValue)
            {
                await _propertyClient.UpdateRoomAvailabilityAsync(agreement.RoomId.Value, false, newAvailableSince);
            }
            else if (agreement.PropertyId != Guid.Empty && agreement.PropertyId != default(Guid))
            {
                await _propertyClient.UpdatePropertyAvailabilityAsync(agreement.PropertyId, false, newAvailableSince);
            }

            await _unitOfWork.CommitAsync();

            var dto = _mapper.Map<RentalAgreementDto>(agreement);

            if (dto.PropertyId.HasValue)
                dto.Property = await _propertyClient.GetPropertyByIdAsync(dto.PropertyId.Value);

            if (dto.RoomId.HasValue)
                dto.Room = await _propertyClient.GetRoomByIdAsync(dto.RoomId.Value);

            return dto;
        }

        public async Task<RentalAgreementDto?> TerminateRentalAsync(Guid rentalAgreementId)
        {
            var now = DateTime.UtcNow;

            var current = await _unitOfWork.RentalAgreementRepository.GetByIdAsync(rentalAgreementId);
            if (current == null) return null;

            if (current.Status == RentalAgreementStatus.Terminated)
            {
                return _mapper.Map<RentalAgreementDto>(current);
            }

            if (current.Status == RentalAgreementStatus.Active)
            {
                var terminated = await _unitOfWork.RentalAgreementRepository.TryTerminateAsync(rentalAgreementId, now);

                if (!terminated)
                {
                    var refreshed = await _unitOfWork.RentalAgreementRepository.GetByIdAsync(rentalAgreementId);
                    return _mapper.Map<RentalAgreementDto>(refreshed);
                }

                await _unitOfWork.CommitAsync();

                var rental = await _unitOfWork.RentalAgreementRepository.GetByIdAsync(rentalAgreementId);
                if (rental == null) return null;

                try
                {
                    if (rental.RoomId.HasValue)
                    {
                        await _propertyClient.UpdateRoomAvailabilityAsync(rental.RoomId.Value, true, now);
                    }
                    else
                    {
                        await _propertyClient.UpdatePropertyAvailabilityAsync(rental.PropertyId, true, now);

                    }
                }
                catch (Exception ex)
                {
                }

                var dto = _mapper.Map<RentalAgreementDto>(rental);
                if (dto.PropertyId.HasValue)
                    dto.Property = await _propertyClient.GetPropertyByIdAsync(dto.PropertyId.Value);
                if (dto.RoomId.HasValue)
                    dto.Room = await _propertyClient.GetRoomByIdAsync(dto.RoomId.Value);

                return dto;
            }

            current.Status = RentalAgreementStatus.Terminated;
            current.EndDate = now;
            _unitOfWork.RentalAgreementRepository.Update(current);
            await _unitOfWork.CommitAsync();

            var dto2 = _mapper.Map<RentalAgreementDto>(current);
            if (dto2.PropertyId.HasValue)
                dto2.Property = await _propertyClient.GetPropertyByIdAsync(dto2.PropertyId.Value);
            if (dto2.RoomId.HasValue)
                dto2.Room = await _propertyClient.GetRoomByIdAsync(dto2.RoomId.Value);

            return dto2;
        }


        public async Task<RentalAgreementDto?> DeclineRentalAsync(Guid rentalAgreementId)
        {
            var agreement = await _unitOfWork.RentalAgreementRepository.GetByIdAsync(rentalAgreementId);
            if (agreement == null) return null;

            if (string.Equals(agreement.Status.ToString(), "Active", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Cannot decline an active rental agreement.");

            if (string.Equals(agreement.Status.ToString(), "Declined", StringComparison.OrdinalIgnoreCase))
                return _mapper.Map<RentalAgreementDto>(agreement);

            agreement.Status = RentalAgreementStatus.Declined;

            _unitOfWork.RentalAgreementRepository.Update(agreement);
            await _unitOfWork.CommitAsync();

            var dto = _mapper.Map<RentalAgreementDto>(agreement);

            return dto;
        }

    }
}
