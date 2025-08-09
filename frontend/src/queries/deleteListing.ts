import { API_BASE_URL } from "../types/types";

export const deleteGroupListing = async (listingId: string): Promise<void> => {
  try {
    const response = await fetch(
      `${API_BASE_URL}/api/groups/listings/${listingId}`,
      {
        method: "DELETE",
        headers: {
          Authorization: `Bearer ${localStorage.getItem("token")}`,
          "Content-Type": "application/json",
        },
      }
    );

    if (!response.ok) {
      throw new Error("Failed to delete listing");
    }
  } catch (error) {
    console.error("Error deleting group listing:", error);
    throw error instanceof Error ? error : new Error("Failed to delete listing");
  }
};