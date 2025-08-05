using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Dto
{
    public class GroupListingQueryParams
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? PreferredCity { get; set; }
        public decimal? MinBudget { get; set; }
        public decimal? MaxBudget { get; set; }
        public int? MinRoommates { get; set; }
        public int? MaxRoommates { get; set; }
        public bool? HasProperty { get; set; }
        public bool? HasRoom { get; set; }
        public string? SearchTerm { get; set; } 
        public string SortBy { get; set; } = "CreatedAt";
        public string SortOrder { get; set; } = "desc"; 
    }
}
