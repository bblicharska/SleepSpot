using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyService.Application.Dto
{
    public class AvailabilityUpdateDto
    {
        public bool IsAvailable { get; set; }
        public DateTime AvailableSince { get; set; }
    }
}
