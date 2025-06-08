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
                            ReviewerId =  new Guid("f162943b-a81c-4bd5-a9a1-08dd9d619286"),
                            ReviewedId =  new Guid("d93a381b-3b00-44c4-2165-08dd6f90dd1f"),
                            ReviewedRole = "Landlord",
                            PropertyId = new Guid("b5dabd55-3d2d-4551-8f7b-1af1307067a0"),
                            Rating=5,
                            Comment = "Very nice and kind person.",
                            CreatedAt = DateTime.Now.AddDays(-2)
                        },

                        new Review
                        {
                           Id = Guid.NewGuid(),
                            ReviewerId =  new Guid("d93a381b-3b00-44c4-2165-08dd6f90dd1f"),
                            ReviewedId =  new Guid("f162943b-a81c-4bd5-a9a1-08dd9d619286"),
                            ReviewedRole = "Tenant",
                            PropertyId = new Guid("4af29e12-58bd-45b0-a3f1-47fdc6a97593"),
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
