using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviewService.Application.Dto
{
    public class ReviewCreateDto
    {
        public Guid ReviewerId { get; set; }
        public Guid? PropertyId { get; set; }
        public Guid? RoomId { get; set; }
        public Guid? OwnerId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
    }
}
