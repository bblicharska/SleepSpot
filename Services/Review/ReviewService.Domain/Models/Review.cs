using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviewService.Domain.Models
{
    public class Review
    {
        public Guid Id { get; set; }
        public Guid ReviewerId { get; set; } // ID z UserService  
        public Guid ReviewedId { get; set; } // ID z UserService  
        public string ReviewedRole { get; set; } // "Landlord" lub "Tenant" (kopia z UserService)  

        public Guid PropertyId { get; set; }
        
        [Range(1, 5)]
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}
