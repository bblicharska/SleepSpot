import { API_BASE_URL } from "../types/types";

export const deleteGroupMember = async (memberId: string) => {
  const res = await fetch(`${API_BASE_URL}/api/groups/members/${memberId}`, {
    method: "DELETE",
    headers: { "Content-Type": "application/json" },
  });

  if (!res.ok) {
    const text = await res.text();
    throw new Error(text || "Failed to remove group member");
  }

  return true;
};