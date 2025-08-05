import {
  GroupListingDto,
  PagedResult,
  GroupListingFilters,
  API_BASE_URL
} from "../types/types";

export const fetchListings = async (
  filters: GroupListingFilters
): Promise<{
  listings: GroupListingDto[];
  pagedResult: PagedResult<GroupListingDto> | null;
}> => {;
  const queryParams = new URLSearchParams();

  Object.entries(filters).forEach(([key, value]) => {
    if (value !== undefined && value !== null && value !== "") {
      queryParams.append(key, value.toString());
    }
  });

  if (filters.noPropertyYet) {
    queryParams.append("hasProperty", "false");
    queryParams.append("hasRoom", "false");
    queryParams.delete("noPropertyYet");
  }

  const url = queryParams.toString()
    ? `${API_BASE_URL}/api/groups/listings?${queryParams.toString()}`
    : `${API_BASE_URL}/api/groups/listings`;

  const res = await fetch(url, {
    headers: { "Content-Type": "application/json" },
  });

  if (!res.ok) throw new Error(`Status: ${res.status}`);

  const data = await res.json();
  
  if (data.items) {
    return {
      listings: data.items,
      pagedResult: data,
    };
  } else {
    let filteredData = data;
    if (filters.noPropertyYet) {
      filteredData = data.filter(
        (listing: GroupListingDto) =>
          !listing.property &&
          !listing.room &&
          !listing.propertyAlreadyRented
      );
    }
    return {
      listings: filteredData,
      pagedResult: null,
    };
  }
};