import { API_BASE_URL } from "../types/types";
import { GroupMemberDto } from "../types/types";

export const fetchGroupMembers = async (
  groupId: string
): Promise<GroupMemberDto[]> => {
  try {
    const response = await fetch(
      `${API_BASE_URL}/api/groups/${groupId}/members`,
      {
        headers: {
          Authorization: `Bearer ${localStorage.getItem("token")}`,
          "Content-Type": "application/json",
        },
      }
    );

    if (!response.ok) {
      throw new Error("Failed to fetch group members");
    }

    const members: GroupMemberDto[] = await response.json();
    return members;
  } catch (error) {
    console.error("Error fetching group members:", error);
    throw error instanceof Error ? error : new Error("Failed to fetch group members");
  }
};