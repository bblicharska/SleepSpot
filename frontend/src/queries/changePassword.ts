import { API_BASE_URL } from "../types/types";

export const changePassword = async (
  token: string | null,
  oldPassword: string,
  newPassword: string,
  userId?: string,
) =>  {
  const response = await fetch(`${API_BASE_URL}/api/users/${userId}/change-password`, {
    method: "PUT",
    headers: {
      Authorization: `Bearer ${token}`,
      "Content-Type": "application/json",
    },
    body: JSON.stringify({
      currentPassword: oldPassword,
      newPassword,
      confirmNewPassword: newPassword,
    }),
  });

  if (!response.ok) {
    const text = await response.text();
    let message = "Password change failed";
    try {
      const data = JSON.parse(text);
      message = data.message || message;
    } catch {
      message = text || message;
    }
    throw new Error(message);
  }

  // Only parse JSON if content exists
  if (response.status !== 204) {
    return await response.json();
  }

  return null; // or void
};