import { API_BASE_URL, Property } from "../types/types";


export const fetchPropertyDetails = async (id: string): Promise<Property> => {
  const response = await fetch(`${API_BASE_URL}/gateway/property-details/${id}`);
  
  if (!response.ok) {
    throw new Error('Failed to fetch property details');
  }
  
  return response.json();
};