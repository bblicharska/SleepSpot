using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyService.Domain.Enums
{
    public enum PropertySortBy
    {
        CreatedAt,
        Name,
        Price,
        Area,
        Address
    }

    public enum RoomSortBy
    {
        CreatedAt,
        Name,
        Price,
        Area,
        Capacity,
        PropertyName,
        PropertyAddress
    }

    public enum SortDirection
    {
        Ascending,
        Descending
    }
}
