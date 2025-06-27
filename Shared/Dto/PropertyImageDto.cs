using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Dto
{
        public class PropertyImageDto
        {
            public Guid Id { get; set; }
            public string ImageUrl { get; set; }
            public string OriginalFileName { get; set; }
            public bool IsPrimary { get; set; }
            public int DisplayOrder { get; set; }
        }
}
