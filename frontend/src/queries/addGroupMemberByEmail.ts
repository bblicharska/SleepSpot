import { API_BASE_URL, AddMemberByEmailRequest } from "../types/types";

export const addGroupMemberByEmail = async (data: AddMemberByEmailRequest) => {
  const response = await fetch(`${API_BASE_URL}/api/groups/members/by-email`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(data),
  });
  if (!response.ok) {
    const text = await response.text();
    throw new Error(text || "Failed to add member");
  }
};