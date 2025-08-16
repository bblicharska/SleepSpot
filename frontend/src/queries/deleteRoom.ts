import { API_BASE_URL } from "../types/types";

export const deleteRoom = async (roomId: string) => {

    const response = await fetch(
        `${API_BASE_URL}/api/properties/rooms/${roomId}`,
        {
          method: "DELETE",
          headers: {
            "Content-Type": "application/json",
          },
        }
      );

if (!response.ok) {
    const errorText = await response.text();
    throw new Error(errorText || 'Failed to delete room');
  }

  return response;
}