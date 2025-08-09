import { API_BASE_URL } from "../types/types"

export const fetchOwnerReviews = async (ownerId: string) => {
  try {
    const response = await fetch(`${API_BASE_URL}/api/reviews/owner/${ownerId}`, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
      },
    });

    if (!response.ok) {
      throw new Error(`Failed to fetch owner reviews: ${response.statusText}`);
    }

    const data = await response.json();
    return data;
  } catch (error) {
    console.error('Error fetching owner reviews:', error);
    throw error;
  }
};