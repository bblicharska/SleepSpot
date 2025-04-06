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
                            PricePerNight = 200.50m,
                            Images = new List<PropertyImage>
                            {
                                new PropertyImage { ImageUrl = "https://example.com/image1.jpg" },
                                new PropertyImage { ImageUrl = "https://example.com/image2.jpg" }
                            },
                            Capacity = 6,
                            OwnerId = Guid.NewGuid() // Przykładowy właściciel
                        },

                        new Property
                        {
                            Id = Guid.NewGuid(),
                            Name = "Beachside Apartment",
                            Description = "A cozy apartment right next to the beach.",
                            Address = "456 Ocean Blvd, Beach Town",
                            PricePerNight = 150.00m,
                            Images = new List<PropertyImage>
                            {
                                new PropertyImage { ImageUrl = "https://example.com/image1.jpg" },
                                new PropertyImage { ImageUrl = "https://example.com/image2.jpg" }
                            },
                            Capacity = 4,
                            OwnerId = Guid.NewGuid() // Przykładowy właściciel
                        },

                        new Property
                        {
                            Id = Guid.NewGuid(),
                            Name = "City Center Loft",
                            Description = "A modern loft located in the heart of the city.",
                            Address = "789 Downtown St, City Center",
                            PricePerNight = 120.75m,
                            Images = new List<PropertyImage>
                            {
                                new PropertyImage { ImageUrl = "https://example.com/image1.jpg" },
                                new PropertyImage { ImageUrl = "https://example.com/image2.jpg" }
                            },
                            Capacity = 2,
                            OwnerId = Guid.NewGuid() // Przykładowy właściciel
                        }
                    };

                    // Dodanie przykładowych nieruchomości do bazy danych
                    _context.Properties.AddRange(properties);
                    _context.SaveChanges(); // Zapisanie danych w bazie
                }
            }
        }
    }
}
