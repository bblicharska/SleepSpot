import { API_BASE_URL, Property } from "../types/types";

export const fetchProperties = async (): Promise<Property[]> => {
  const response = await fetch(`${API_BASE_URL}/api/properties`);
  
  if (!response.ok) {
    throw new Error(`Failed to fetch properties: ${response.status} ${response.statusText}`);
  }
  
  return response.json();
};