using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentalService.Domain.Models
{
    public class RentalAgreement
    {
        public Guid Id { get; set; }

        public Guid PropertyId { get; set; }
        public Guid? RoomId { get; set; }

        public Guid? GroupId { get; set; }
        public Guid? UserId { get; set; } 

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public decimal MonthlyRent { get; set; }
        public RentalAgreementStatus Status { get; set; } = RentalAgreementStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

public enum RentalAgreementStatus
{
    Active,
    Terminated,
    Pending
}