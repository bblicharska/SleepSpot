using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentalService.Application.Dto
{
    public class UpdateRentalAgreementDto
    {
        [Required]
        public Guid Id { get; set; }
        public DateTime? EndDate { get; set; }
        [Required]
        [Range(0, double.MaxValue)]
        public decimal MonthlyRent { get; set; }
        [Required]
        public string Status { get; set; }
    }
}
