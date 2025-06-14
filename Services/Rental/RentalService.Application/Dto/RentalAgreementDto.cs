using Shared.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentalService.Application.Dto
{
    public class RentalAgreementDto
    {
        public Guid Id { get; set; }

        public Guid PropertyId { get; set; }
        public Guid? RoomId { get; set; }

        public Guid? GroupId { get; set; }
        public Guid? UserId { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public decimal MonthlyRent { get; set; }
        public string Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public PropertyDto Property { get; set; }
        public RoomDto Room { get; set; }
        public GroupDto Group { get; set; }
        public UserDto User { get; set; }
    }
}
