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
        DetailedDescription = "This spacious city-center villa is perfect for families or professionals seeking long-term accommodation. Fully furnished with modern appliances including a dishwasher, washing machine, large refrigerator, oven, microwave, and flat-screen TV. High-speed fiber internet (600 Mbps) and central heating included. The property features 4 bedrooms, 3 bathrooms, a large kitchen with a dining area, a living room, a garage, and a private garden. Pets are allowed. Located in a quiet neighborhood near tram and bus stops (5 min walk), supermarkets (Biedronka, Carrefour), schools, and medical centers. Secure, gated property with outdoor lighting and security cameras.",
        isAvailable = true,
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
                UploadedAt = DateTime.UtcNow,
                PropertyId = new Guid("58d744b5-77e7-497a-8a92-24eee8dcfb8a")
            },
            new PropertyImage
            {
                Id = Guid.NewGuid(),
                ImageUrl = "/uploads/properties/property2.jpg",
                OriginalFileName = "image2.jpg",
                IsPrimary = false,
                DisplayOrder = 2,
                UploadedAt = DateTime.UtcNow,
                PropertyId = new Guid("58d744b5-77e7-497a-8a92-24eee8dcfb8a")
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
                Capacity = 2,
                CreatedAt = DateTime.UtcNow,
                Images = new List<PropertyImage>
                {
                    new PropertyImage
                    {
                        Id = Guid.NewGuid(),
                        ImageUrl = "/uploads/properties/room1.jpg",
                        OriginalFileName = "room1.jpg",
                        IsPrimary = true,
                        DisplayOrder = 1,
                        UploadedAt = DateTime.UtcNow,
                        RoomId = new Guid("3716234f-ea57-467b-91ed-78368106bd24")
                    }
                }
            },
            new Room
            {
                Id = new Guid("663d12c7-dbf9-4c1a-bfc4-efca876610a9"),
                Name = "Guest Room",
                Description = "Comfortable guest room.",
                PricePerMonth = 700.00m,
                AreaInSquareMeters = 20.0m,
                Capacity = 1,
                CreatedAt = DateTime.UtcNow
            }
        }
    },
    new Property
    {
        Id = new Guid("140c0d86-df4a-4c1c-8cb2-b563c2361307"),
        Name = "Beachside Apartment",
        Description = "A cozy apartment right next to the beach.",
        DetailedDescription = "Cozy and bright apartment located just 100 meters from the beach — perfect for long-term stays. The apartment comes fully furnished with a double bed, sofa, dining table, and wardrobes. Kitchen includes a built-in oven, microwave, induction cooktop, dishwasher, and fridge-freezer. Equipped with AC for the summer months and central heating for winter. Fiber-optic internet available (400 Mbps). No pets allowed. Close to the tram stop (2 min walk), local grocery stores, pharmacy, and seaside promenade. Quiet and safe area with walking and cycling paths. Ideal for singles or couples.",
        isAvailable = true,
        Address = "Długi Targ 1, Gdansk, Poland",
        PricePerMonth = 1800.00m,
        AreaInSquareMeters = 85.0m,
        IsEntirePlaceRentable = true,
        OwnerId = new Guid("49dbeb15-3962-4686-e0cc-08ddb10005d3"),
        Images = new List<PropertyImage>
        {
            new PropertyImage
            {
                Id = Guid.NewGuid(),
                ImageUrl =  "/uploads/properties/property2.jpg",
                OriginalFileName = "image2.jpg",
                IsPrimary = true,
                DisplayOrder = 1,
                UploadedAt = DateTime.UtcNow,
                PropertyId = new Guid("140c0d86-df4a-4c1c-8cb2-b563c2361307")
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
        isAvailable = true,
        PricePerMonth = 2200.00m,
        AreaInSquareMeters = 65.5m,
        IsEntirePlaceRentable = false,
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
                UploadedAt = DateTime.UtcNow,
                PropertyId = new Guid("5cb0bd81-7624-4c09-8be2-447a638f53e6")
            },
            new PropertyImage
            {
                Id = Guid.NewGuid(),
                ImageUrl =  "/uploads/properties/property2.jpg",
                OriginalFileName = "image2.jpg",
                IsPrimary = false,
                DisplayOrder = 2,
                UploadedAt = DateTime.UtcNow,
                PropertyId = new Guid("5cb0bd81-7624-4c09-8be2-447a638f53e6")
            }
        },
        Rooms = new List<Room>
        {
            new Room
            {
                Id = new Guid("c53a92a3-1043-4e3b-873b-d8fa55cc90b2"),
                Name = "Loft Bedroom",
                Description = "Open bedroom space in loft style.",
                DetailedDescription = "Loft-style open bedroom space with a queen-size bed (140x200 cm), wall-mounted shelves, a large closet, and a work desk. Positioned on the mezzanine level with views over the living area. Well-ventilated, quiet, and perfect for remote work. Soundproof windows, blackout curtains, and warm lighting for comfort. Wi-Fi coverage is strong throughout the apartment. Access to shared kitchen and bathroom downstairs.",
                PricePerMonth = 1100.00m,
                AreaInSquareMeters = 25.0m,
                Capacity = 1,
                CreatedAt = DateTime.UtcNow,
                Images = new List<PropertyImage>
                {
                    new PropertyImage
                    {
                        Id = Guid.NewGuid(),
                        ImageUrl = "/uploads/properties/room2.jpg",
                        OriginalFileName = "room2.jpg",
                        IsPrimary = true,
                        DisplayOrder = 1,
                        UploadedAt = DateTime.UtcNow,
                        RoomId = new Guid("3716234f-ea57-467b-91ed-78368106bd24")
                    }
                }
            },
             new Room
            {
                Id = new Guid("3716234f-ea57-467b-91ed-78368106bd26"),
                Name = "Modern Bedroom",
                Description = "Open bedroom space in loft style.",
                DetailedDescription = "Modern-style open bedroom space with a queen-size bed (140x200 cm), wall-mounted shelves, a large closet, and a work desk. Positioned on the mezzanine level with views over the living area. Well-ventilated, quiet, and perfect for remote work. Soundproof windows, blackout curtains, and warm lighting for comfort. Wi-Fi coverage is strong throughout the apartment. Access to shared kitchen and bathroom downstairs.",
                PricePerMonth = 1200.00m,
                AreaInSquareMeters = 20.0m,
                Capacity = 1,
                CreatedAt = DateTime.UtcNow,
                Images = new List<PropertyImage>
                {
                    new PropertyImage
                    {
                        Id = Guid.NewGuid(),
                        ImageUrl = "/uploads/properties/room1.jpg",
                        OriginalFileName = "room1.jpg",
                        IsPrimary = true,
                        DisplayOrder = 1,
                        UploadedAt = DateTime.UtcNow,
                        RoomId = new Guid("3716234f-ea57-467b-91ed-78368106bd26")
                    }
                }
            },
             new Room
            {
                Id = new Guid("3716234f-ea57-467b-91ed-78368106bd27"),
                Name = "Large bedroom with balcony",
                Description = "Open bedroom space in loft style.",
                DetailedDescription = "Modern-style open bedroom space with a queen-size bed (140x200 cm), wall-mounted shelves, a large closet, and a work desk. Positioned on the mezzanine level with views over the living area. Well-ventilated, quiet, and perfect for remote work. Soundproof windows, blackout curtains, and warm lighting for comfort. Wi-Fi coverage is strong throughout the apartment. Access to shared kitchen and bathroom downstairs.",
                PricePerMonth = 1500.00m,
                AreaInSquareMeters = 30.0m,
                IsAvailable = false,
                Capacity = 1,
                CreatedAt = DateTime.UtcNow,
                Images = new List<PropertyImage>
                {
                    new PropertyImage
                    {
                        Id = Guid.NewGuid(),
                        ImageUrl = "/uploads/properties/room2.jpg",
                        OriginalFileName = "room2.jpg",
                        IsPrimary = true,
                        DisplayOrder = 1,
                        UploadedAt = DateTime.UtcNow,
                        RoomId = new Guid("3716234f-ea57-467b-91ed-78368106bd27")
                    }
                }
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
