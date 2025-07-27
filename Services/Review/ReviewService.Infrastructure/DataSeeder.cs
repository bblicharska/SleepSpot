using ReviewService.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviewService.Infrastructure
{
    public class DataSeeder
    {
        private readonly ReviewDbContext _context;

        public DataSeeder(ReviewDbContext context)
        {
            _context = context;
        }

        public void Seed()
        {

            try
            {
                if (_context.Database.CanConnect())
                {

                    if (!_context.Reviews.Any())
                    {

                        var properties = new List<Review>
                    {
                        new Review
                        {
                            Id = Guid.NewGuid(),
                            ReviewerId =  new Guid("49dbeb15-3962-4686-e0cc-08ddb10005d3"),
                            PropertyId = new Guid("140c0d86-df4a-4c1c-8cb2-b563c2361307"),
                            Rating=5,
                            Comment = "Very nice and kind owner.",
                            CreatedAt = DateTime.Now.AddDays(-2)
                        },

                        new Review
                        {
                           Id = Guid.NewGuid(),
                            ReviewerId =  new Guid("49dbeb15-3962-4686-e0cc-08ddb10005d3"),
                            RoomId = new Guid("c53a92a3-1043-4e3b-873b-d8fa55cc90b2"),
                            Rating=5,
                            Comment = "Really recommend!",
                            CreatedAt = DateTime.Now.AddDays(-3)
                        },
                    };

                        // Dodanie przykładowych nieruchomości do bazy danych
                        _context.Reviews.AddRange(properties);
                        _context.SaveChanges(); // Zapisanie danych w bazie
                        Console.WriteLine("Seeding completed successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Reviews already exist. Skipping seeding.");
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
