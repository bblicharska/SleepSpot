import { API_BASE_URL } from "../types/types";

export const fetchUserGroups = async (userId?: string) => {
    const response = await fetch(`${API_BASE_URL}/api/groups/user/${userId}`);

    if (!response.ok) throw new Error("Failed to fetch user groups");

    return response.json();
};