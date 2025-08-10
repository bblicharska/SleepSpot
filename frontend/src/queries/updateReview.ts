import { API_BASE_URL } from "../types/types";

export const updateReview = async (
  reviewId: string,
  updateData: { id: string; rating: number; comment: string },
  token: string
) => {
  const response = await fetch(`${API_BASE_URL}/api/reviews/${reviewId}`, {
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${token}`,
    },
    body: JSON.stringify(updateData),
  });

  if (!response.ok) {
    throw new Error('Failed to update review');
  }

  const contentType = response.headers.get('Content-Type');
  if (contentType && contentType.includes('application/json')) {
    const text = await response.text();
    if (text) {
      return JSON.parse(text);
    }
  }

  return updateData;
};