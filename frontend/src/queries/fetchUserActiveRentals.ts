import { API_BASE_URL, RentalAgreementDto } from "../types/types";

export const fetchUserActiveRentals = async (
  userId: string
): Promise<RentalAgreementDto[]> => {
  const response = await fetch(
    `${API_BASE_URL}/api/rentals/user/${userId}/active`
  );

  if (!response.ok) {
    throw new Error("Failed to fetch user rentals");
  }

  return response.json();
};