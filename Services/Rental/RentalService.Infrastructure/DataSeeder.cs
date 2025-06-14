using RentalService.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RentalService.Infrastructure
{
    public class DataSeeder
    {
        private readonly RentalDbContext _context;

        public DataSeeder(RentalDbContext context)
        {
            _context = context;
        }

        public void Seed()
        {
            try
            {
                if (_context.Database.CanConnect())
                {
                    if (!_context.RentalAgreements.Any())
                    {
                        var agreements = new List<RentalAgreement>
                        {
                            new RentalAgreement
                            {
                                Id = Guid.NewGuid(),
                                PropertyId = new Guid("58d744b5-77e7-497a-8a92-24eee8dcfb8a"),
                                RoomId = null, // cała nieruchomość
                                GroupId = new Guid("65857791-7319-4bd2-82e4-34f53812ffb6"),
                                UserId = new Guid("d93a381b-3b00-44c4-2165-08dd6f90dd1f"),
                                StartDate = DateTime.UtcNow,
                                EndDate = null,
                                MonthlyRent = 4500,
                                Status = RentalAgreementStatus.Active,
                                CreatedAt = DateTime.UtcNow
                            },
                            new RentalAgreement
                            {
                                Id = Guid.NewGuid(),
                                PropertyId = new Guid("5cb0bd81-7624-4c09-8be2-447a638f53e6"),
                                RoomId = new Guid("c53a92a3-1043-4e3b-873b-d8fa55cc90b2"),
                                GroupId = null,
                                UserId = new Guid("ea679e1b-4f2c-4108-2166-08dd6f90dd1f"),
                                StartDate = DateTime.UtcNow.AddDays(-10),
                                EndDate = DateTime.UtcNow.AddMonths(6),
                                MonthlyRent = 1600,
                                Status = RentalAgreementStatus.Active,
                                CreatedAt = DateTime.UtcNow
                            }
                        };

                        _context.RentalAgreements.AddRange(agreements);
                        _context.SaveChanges();

                        Console.WriteLine("Seeding completed successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Rental agreements already exist. Skipping seeding.");
                    }
                }
                else
                {
                    Console.WriteLine("Cannot connect to the database. Skipping migrations and seeding.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred during seeding: {ex.Message}");
                throw;
            }
        }
    }
}
