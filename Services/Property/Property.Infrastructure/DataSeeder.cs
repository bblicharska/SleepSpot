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
                            Id = new Guid("58d744b5-77e7-497a-8a92-24eee8dcfb8a"),
                            Name = "Villa in the City Center",
                            Description = "A beautiful villa with a stunning view.",
                            Address = "Marszałkowska 84, Warsaw, Poland",
                            PricePerMonth = 2500.00m,
                            AreaInSquareMeters = 180.5m,
                            IsEntirePlaceRentable = true,
                            OwnerId =  new Guid("d93a381b-3b00-44c4-2165-08dd6f90dd1f"),
                            Images = new List<PropertyImage>
{
    new PropertyImage
    {
        Id = Guid.NewGuid(),
        ImageUrl = "/uploads/properties/property1.jpg",
        OriginalFileName = "image1.jpg",
        IsPrimary = true,
        DisplayOrder = 1,
        UploadedAt = DateTime.UtcNow
    },
    new PropertyImage
    {
        Id = Guid.NewGuid(),
        ImageUrl ="/uploads/properties/property2.jpg",
        OriginalFileName = "image2.jpg",
        IsPrimary = false,
        DisplayOrder = 2,
        UploadedAt = DateTime.UtcNow
    }
},
                            Rooms = new List<Room>
                            {
                                new Room
                                {
                                    Id = new Guid("3716234f-ea57-467b-91ed-78368106bd24"),
                                    Name = "Master Bedroom",
                                    Description = "Spacious room with private bathroom.",
                                    PricePerMonth = 900.00m,
                                    AreaInSquareMeters = 30.0m,
                                    Capacity = 2
                                },
                                new Room
                                {
                                    Id = new Guid("663d12c7-dbf9-4c1a-bfc4-efca876610a9"),
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
                            Id = new Guid("140c0d86-df4a-4c1c-8cb2-b563c2361307"),
                            Name = "Beachside Apartment",
                            Description = "A cozy apartment right next to the beach.",
                            Address = "Długi Targ 1, Gdansk, Poland",
                            PricePerMonth = 1800.00m,
                            AreaInSquareMeters = 85.0m,
                            IsEntirePlaceRentable = true,
                            OwnerId = new Guid("ea679e1b-4f2c-4108-2166-08dd6f90dd1f"),
                            Images = new List<PropertyImage>
{
    new PropertyImage
    {
        Id = Guid.NewGuid(),
        ImageUrl =  "/uploads/properties/property1.jpg",
        OriginalFileName = "image1.jpg",
        IsPrimary = true,
        DisplayOrder = 1,
        UploadedAt = DateTime.UtcNow
    },
    new PropertyImage
    {
        Id = Guid.NewGuid(),
        ImageUrl =  "/uploads/properties/property2.jpg",
        OriginalFileName = "image2.jpg",
        IsPrimary = false,
        DisplayOrder = 2,
        UploadedAt = DateTime.UtcNow
    }
},
                            Rooms = new List<Room>()
                        },
                        new Property
                        {
                            Id = new Guid("5cb0bd81-7624-4c09-8be2-447a638f53e6"),
                            Name = "City Center Loft",
                            Description = "A modern loft located in the heart of the city.",
                            Address = "Rynek Główny 5, Krakow, Poland",
                            PricePerMonth = 2200.00m,
                            AreaInSquareMeters = 65.5m,
                            IsEntirePlaceRentable = true,
                            OwnerId = new Guid("d21c93e3-8245-442d-f898-08dd6f9adc78"),
                            Images = new List<PropertyImage>
{
    new PropertyImage
    {
        Id = Guid.NewGuid(),
        ImageUrl =  "/uploads/properties/property1.jpg",
        OriginalFileName = "image1.jpg",
        IsPrimary = true,
        DisplayOrder = 1,
        UploadedAt = DateTime.UtcNow
    },
    new PropertyImage
    {
        Id = Guid.NewGuid(),
        ImageUrl =  "/uploads/properties/property2.jpg",
        OriginalFileName = "image2.jpg",
        IsPrimary = false,
        DisplayOrder = 2,
        UploadedAt = DateTime.UtcNow
    }
},
                            Rooms = new List<Room>
                            {
                                new Room
                                {
                                    Id = new Guid("c53a92a3-1043-4e3b-873b-d8fa55cc90b2"),
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
