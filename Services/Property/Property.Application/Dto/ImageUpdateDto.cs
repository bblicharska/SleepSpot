using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyService.Application.Dto
{
    public class ImageUpdateDto
    {
        public string? Url { get; set; }  // For existing images
        public IFormFile? File { get; set; }  // For new uploads
        public bool IsPrimary { get; set; }
        public int DisplayOrder { get; set; }
        public bool ToDelete { get; set; }  // Flag for deletion
    }
}
