import { Property, RoomSearchFilterDto, PropertyFilterDto, RoomFilterDto, API_BASE_URL } from "../types/types";

export const searchProperties = async (filters: PropertyFilterDto): Promise<Property[]> => {
  const queryParams = new URLSearchParams();
  
  if (filters.location) queryParams.append('location', filters.location);
  if (filters.minPrice !== undefined) queryParams.append('minPrice', filters.minPrice.toString());
  if (filters.maxPrice !== undefined) queryParams.append('maxPrice', filters.maxPrice.toString());
  if (filters.minArea !== undefined) queryParams.append('minArea', filters.minArea.toString());
  if (filters.maxArea !== undefined) queryParams.append('maxArea', filters.maxArea.toString());
  if (filters.isAvailable !== undefined) queryParams.append('isAvailable', filters.isAvailable.toString());
  if (filters.isEntirePlaceRentable !== undefined) queryParams.append('isEntirePlaceRentable', filters.isEntirePlaceRentable.toString());
  if (filters.availableSince) queryParams.append('availableSince', filters.availableSince);

  const response = await fetch(`${API_BASE_URL}/api/properties/search?${queryParams}`);
  
  if (!response.ok) {
    throw new Error('Failed to search properties');
  }
  
  return response.json();
};

export const searchRooms = async (filters: RoomSearchFilterDto): Promise<RoomFilterDto[]> => {
  const queryParams = new URLSearchParams();
  
  if (filters.location) queryParams.append('location', filters.location);
  if (filters.minPrice !== undefined) queryParams.append('minPrice', filters.minPrice.toString());
  if (filters.maxPrice !== undefined) queryParams.append('maxPrice', filters.maxPrice.toString());
  if (filters.minArea !== undefined) queryParams.append('minArea', filters.minArea.toString());
  if (filters.maxArea !== undefined) queryParams.append('maxArea', filters.maxArea.toString());
  if (filters.minCapacity !== undefined) queryParams.append('minCapacity', filters.minCapacity.toString());
  if (filters.isAvailable !== undefined) queryParams.append('isAvailable', filters.isAvailable.toString());
if (filters.availableSince) queryParams.append('availableSince', filters.availableSince);

  const response = await fetch(`${API_BASE_URL}/api/properties/rooms/search?${queryParams}`);
  
  if (!response.ok) {
    throw new Error('Failed to search rooms');
  }
  
  return response.json();
};