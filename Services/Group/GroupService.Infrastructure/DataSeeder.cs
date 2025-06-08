using GroupService.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroupService.Infrastructure
{
    public class DataSeeder
    {
        private readonly GroupDbContext _context;

        public DataSeeder(GroupDbContext context)
        {
            _context = context;
        }

        public void Seed()
        {
            try
            {
                if (_context.Database.CanConnect())
                {
                    if (!_context.Groups.Any())
                    {
                        var group1 = new Group
                        {
                            Id = Guid.NewGuid(),
                            Name = "Erasmus Kraków",
                            Description = "Szukamy osoby do wspólnego mieszkania w centrum",
                            CreatedByUserId = new Guid("d93a381b-3b00-44c4-2165-08dd6f90dd1f"),
                            CreatedAt = DateTime.UtcNow,
                            Members = new List<GroupMember>
                            {
                                new GroupMember
                                {
                                    Id = Guid.NewGuid(),
                                    UserId = new Guid("ea679e1b-4f2c-4108-2166-08dd6f90dd1f"),
                                    Role = GroupRole.Admin
                                },
                                new GroupMember
                                {
                                    Id = Guid.NewGuid(),
                                    UserId = new Guid("d21c93e3-8245-442d-f898-08dd6f9adc78"),
                                    Role = GroupRole.Member
                                },
                            }
                        };

                        var group2 = new Group
                        {
                            Id = Guid.NewGuid(),
                            Name = "Studentki AGH",
                            Description = "Szukamy współlokatorki do 3-osobowego mieszkania",
                            CreatedByUserId = new Guid("3d9ff833-a4c3-4a0e-08ad-08dd94c7b620"),
                            CreatedAt = DateTime.UtcNow,
                            Members = new List<GroupMember>
                            {
                                new GroupMember
                                {
                                    Id = Guid.NewGuid(),
                                    UserId = new Guid("f162943b-a81c-4bd5-a9a1-08dd9d619286"),
                                    Role = GroupRole.Admin
                                },
                                new GroupMember
                                {
                                    Id = Guid.NewGuid(),
                                    UserId = new Guid("ea679e1b-4f2c-4108-2166-08dd6f90dd1f"),
                                    Role = GroupRole.Member
                                },
                            }
                        };

                        var groups = new List<Group>
                        {
                            group1,
                            group2
                        };

                        var listings = new List<GroupListing>
                        {
                            new GroupListing
                            {
                                Id = Guid.NewGuid(),
                                Group = group1,
                                Title = "Pokój w centrum Krakowa",
                                Description = "Duże mieszkanie z balkonem, szukamy jednej osoby",
                                DesiredRoommatesCount = 1,
                                Status = ListingStatus.Active,
                                PreferredCity = "Kraków",
                                MaxBudgetPerPerson = 1500,
                                CreatedAt = DateTime.UtcNow,
                                PropertyId = new Guid("58d744b5-77e7-497a-8a92-24eee8dcfb8a")
                            },
                            new GroupListing
                            {
                                Id = Guid.NewGuid(),
                                Group = group2,
                                Title = "Pokój w centrum Krakowa",
                                Description = "Duże mieszkanie z balkonem, szukamy jednej osoby",
                                DesiredRoommatesCount = 1,
                                Status = ListingStatus.Active,
                                PreferredCity = "Kraków",
                                MaxBudgetPerPerson = 1500,
                                CreatedAt = DateTime.UtcNow
                            },
                        };

                        _context.Groups.AddRange(groups);
                        _context.GroupListings.AddRange(listings);

                        _context.SaveChanges(); // Zapisanie danych w bazie
                        Console.WriteLine("Seeding completed successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Properties already exist. Skipping seeding.");
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
                // Rzucanie wyjątku, jeśli konieczne
                throw;
            }
        }
    }
}