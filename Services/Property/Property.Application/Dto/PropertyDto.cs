using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyService.Application.Dto
{
    public class PropertyDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public decimal PricePerNight { get; set; }
        public List<string> Images { get; set; }  // Assuming you're using URLs for the images
        public int Capacity { get; set; }
        public Guid OwnerId { get; set; }
    }
}
