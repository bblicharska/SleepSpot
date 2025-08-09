import { API_BASE_URL } from "../types/types";
import { CreateGroupListingDto } from "../types/types";

export const createGroupListing = async (listingData: CreateGroupListingDto) => {
    const response = await fetch(`${API_BASE_URL}/api/groups/listings`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(listingData),
    });

    if (!response.ok) throw new Error("Failed to create listing");

    return response.json();
  };