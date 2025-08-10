import { API_BASE_URL, RoomApplicationDto } from "../types/types";

export const fetchListingApplications = async (listingId: string): Promise<RoomApplicationDto[]> => {
  const response = await fetch(`${API_BASE_URL}/api/groups/applications/listing/${listingId}`);
  if (!response.ok) {
    throw new Error(`HTTP error! status: ${response.status}`);
  }
  return response.json();
};