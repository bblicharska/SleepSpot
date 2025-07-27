import { API_BASE_URL, ReviewDto } from "../types/types";

interface SubmitReviewParams {
reviewerId: string;
  rating: number;
  comment: string;
   roomId?: string; 
  propertyId?: string;
}

export const postReview = async (
  reviewData: SubmitReviewParams,
  token: string
): Promise<ReviewDto> => {
  const response = await fetch(`${API_BASE_URL}/api/reviews`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      "Authorization": `Bearer ${token}`, // Add the authorization header
    },
    body: JSON.stringify(reviewData),
  });

  if (!response.ok) {
    const errorData = await response.json();
    throw new Error(errorData.message || "Failed to submit review");
  }

  return response.json();
};