import { API_BASE_URL, GroupListingDto } from "../types/types";

export const fetchGroupListings = async (groupId: string): Promise<GroupListingDto[]> => {
  const response = await fetch(`${API_BASE_URL}/api/groups/${groupId}/listings`);
  if (!response.ok) {
    throw new Error(`HTTP error! status: ${response.status}`);
  }
  return response.json();
};