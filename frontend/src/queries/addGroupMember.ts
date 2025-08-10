import { API_BASE_URL } from "../types/types";

export const addGroupMember = async (groupId: string, userId: string) => {
  const response = await fetch(`${API_BASE_URL}/api/groups/members`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({
      groupId,
      userId,
      joinedAt: new Date().toISOString(),
      role: "Member",
    }),
  });
  if (!response.ok) {
    const text = await response.text();
    throw new Error(text || "Failed to add member to group");
  }
};