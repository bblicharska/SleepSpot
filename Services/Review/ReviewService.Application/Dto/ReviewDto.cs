using Shared.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviewService.Application.Dto
{
    public class ReviewDto
    {
        public Guid Id { get; set; }
        public Guid ReviewerId { get; set; }
        public Guid ReviewedId { get; set; }
        public string ReviewedRole { get; set; }
        public Guid PropertyId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }

        public UserDto? Reviewer { get; set; }
        public UserDto? Reviewed { get; set; }
    }
}
