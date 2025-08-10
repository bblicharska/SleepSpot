import { API_BASE_URL } from "../types/types";

export const deleteGroup = async (groupId: string)  => {
  const response = await fetch(`${API_BASE_URL}/api/groups/${groupId}`, {
    method: "DELETE",
  });
  if (!response.ok) {
    const text = await response.text();
    throw new Error(text || "Failed to delete group");
  }
};