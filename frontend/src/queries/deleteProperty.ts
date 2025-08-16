import { API_BASE_URL } from "../types/types";

export const deleteProperty = async (propertyId: string) => {

    const response = await fetch(
        `${API_BASE_URL}/api/properties/${propertyId}`,
        {
          method: "DELETE",
          headers: {
            "Content-Type": "application/json",
          },
        }
      );

if (!response.ok) {
    const errorText = await response.text();
    throw new Error(errorText || 'Failed to delete property');
  }

  return response;
}