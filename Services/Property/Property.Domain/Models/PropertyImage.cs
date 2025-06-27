using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyService.Domain.Models
{
    public class PropertyImage
    {
        public Guid Id { get; set; }
        public Guid PropertyId { get; set; }
        public string ImageUrl { get; set; }
        public string OriginalFileName { get; set; }
        public bool IsPrimary { get; set; } = false;
        public int DisplayOrder { get; set; }
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
        public Property Property { get; set; }
    }
}
