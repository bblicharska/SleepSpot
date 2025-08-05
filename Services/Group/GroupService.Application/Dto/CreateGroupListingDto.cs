using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroupService.Application.Dto
{
    public class CreateGroupListingDto
    {
        public Guid GroupId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int DesiredRoommatesCount { get; set; }
        public string PreferredCity { get; set; }
        public bool PropertyAlreadyRented { get; set; }
        public decimal? MaxBudgetPerPerson { get; set; }
        public Guid? PropertyId { get; set; }
        public Guid? RoomId { get; set; }
    }

}
