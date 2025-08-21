using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentalService.Application.Dto
{
    public class CreateRentalAgreementDto
    {
        public Guid? PropertyId { get; set; }

        public Guid? RoomId { get; set; }

        public Guid? GroupId { get; set; }
        public Guid? UserId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
        public RentalAgreementStatus Status { get; set; } = RentalAgreementStatus.Pending;

        [Required]
        [Range(0, double.MaxValue)]
        public decimal MonthlyRent { get; set; }
    }
}
