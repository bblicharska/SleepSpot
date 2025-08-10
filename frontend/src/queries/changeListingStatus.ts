import { API_BASE_URL } from "../types/types";

export const changeListingStatus = async (appId: string, status: string = "Accepted"
      ) => {
        const response = await fetch(
        `${API_BASE_URL}/api/groups/applications/${appId}/status`,
        {
          method: "PUT",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify(status),
        }
      );
      
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
      
        return JSON.stringify(status);
      };