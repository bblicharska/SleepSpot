using Microsoft.EntityFrameworkCore;
using PropertyService.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyService.Infrastructure
{
    public class DataSeeder
    {
        private readonly PropertyDbContext _context;

        public DataSeeder(PropertyDbContext context)
        {
            _context = context;
        }

        public void Seed()
        {

            try
            {
                if (_context.Database.CanConnect())
            {

                if (!_context.Properties.Any())
                {

                    var properties = new List<Property>
                    {
                        new Property
                        {
                            Id = Guid.NewGuid(),
                            Name = "Villa in the Mountains",
                            Description = "A beautiful villa with a stunning view of the mountains.",
                            Address = "123 Mountain Road, Mountain City",
                            PricePerMonth = 2500.00m,
                            AreaInSquareMeters = 180.5m,
                            IsEntirePlaceRentable = true,
                            OwnerId =  new Guid("d93a381b-3b00-44c4-2165-08dd6f90dd1f"),
                            Images = new List<PropertyImage>
                            {
                                new PropertyImage { ImageUrl = "https://example.com/image1.jpg" },
                                new PropertyImage { ImageUrl = "https://example.com/image2.jpg" }
                            },
                            Rooms = new List<Room>
                            {
                                new Room
                                {
                                    Id = Guid.NewGuid(),
                                    Name = "Master Bedroom",
                                    Description = "Spacious room with private bathroom.",
                                    PricePerMonth = 900.00m,
                                    AreaInSquareMeters = 30.0m,
                                    Capacity = 2
                                },
                                new Room
                                {
                                    Id = Guid.NewGuid(),
                                    Name = "Guest Room",
                                    Description = "Comfortable guest room.",
                                    PricePerMonth = 700.00m,
                                    AreaInSquareMeters = 20.0m,
                                    Capacity = 1
                                }
                            }
                        },
                        new Property
                        {
                            Id = Guid.NewGuid(),
                            Name = "Beachside Apartment",
                            Description = "A cozy apartment right next to the beach.",
                            Address = "456 Ocean Blvd, Beach Town",
                            PricePerMonth = 1800.00m,
                            AreaInSquareMeters = 85.0m,
                            IsEntirePlaceRentable = true,
                            OwnerId = new Guid("ea679e1b-4f2c-4108-2166-08dd6f90dd1f"),
                            Images = new List<PropertyImage>
                            {
                                new PropertyImage { ImageUrl = "https://example.com/image3.jpg" },
                                new PropertyImage { ImageUrl = "https://example.com/image4.jpg" }
                            },
                            Rooms = new List<Room>()
                        },
                        new Property
                        {
                            Id = Guid.NewGuid(),
                            Name = "City Center Loft",
                            Description = "A modern loft located in the heart of the city.",
                            Address = "789 Downtown St, City Center",
                            PricePerMonth = 2200.00m,
                            AreaInSquareMeters = 65.5m,
                            IsEntirePlaceRentable = true,
                            OwnerId = new Guid("d21c93e3-8245-442d-f898-08dd6f9adc78"),
                            Images = new List<PropertyImage>
                            {
                                new PropertyImage { ImageUrl = "https://example.com/image5.jpg" },
                                new PropertyImage { ImageUrl = "https://example.com/image6.jpg" }
                            },
                            Rooms = new List<Room>
                            {
                                new Room
                                {
                                    Id = Guid.NewGuid(),
                                    Name = "Loft Bedroom",
                                    Description = "Open bedroom space in loft style.",
                                    PricePerMonth = 1100.00m,
                                    AreaInSquareMeters = 25.0m,
                                    Capacity = 1
                                }
                            }
                        }
                    };
                    // Dodanie przykładowych nieruchomości do bazy danych
                    _context.Properties.AddRange(properties);
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
