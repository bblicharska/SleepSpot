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
        public string? Url { get; set; }
        public IFormFile? File { get; set; }
        public bool IsPrimary { get; set; }
        public int DisplayOrder { get; set; }
        public bool ToDelete { get; set; }
    }
}
