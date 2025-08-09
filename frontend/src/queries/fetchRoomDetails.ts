import { RoomWithPropertyDetailsDto } from "../components/RoomDetailsPage";
import { API_BASE_URL } from "../types/types";

export const fetchRoomDetails = async (roomId: string): Promise<RoomWithPropertyDetailsDto> => {
  const response = await fetch(`${API_BASE_URL}/gateway/room-details/${roomId}`);
  
  if (!response.ok) {
    throw new Error(`Failed to fetch room details: ${response.statusText}`);
  }
  
  const data = await response.json();
  return data;
};