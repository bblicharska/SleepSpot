import { API_BASE_URL, CreateGroupDto } from "../types/types";

export const createGroup = async (groupData: CreateGroupDto) => {
  const response = await fetch(`${API_BASE_URL}/api/groups`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(groupData),
  });

  const contentType = response.headers.get("content-type") || "";
  const isJson = contentType.includes("application/json");

  if (!response.ok) {
    let errorMessage = `Failed to create group (status ${response.status})`;
    let invalidEmails: string[] = [];

    try {
      if (isJson) {
        const errorData = await response.json();

        if (errorData.message) errorMessage = errorData.message;
        else if (errorData.title) errorMessage = errorData.title;
        else if (errorData.errors) {
          errorMessage = Object.values(errorData.errors).flat().join(", ");
        }

        if (Array.isArray(errorData.invalidEmails)) {
          invalidEmails = errorData.invalidEmails.map((e: any) => String(e).trim());
        } else if (errorData.errors?.memberEmails) {
          invalidEmails = []
            .concat(errorData.errors.memberEmails)
            .map((e: any) => String(e).trim());
        }
      } else {
        const text = await response.text();
        if (text) errorMessage = text;
      }
    } catch (parseErr) {
      console.warn("Failed to parse error response", parseErr);
    }

    const err: any = new Error(errorMessage);
    err.invalidEmails = invalidEmails;
    throw err;
  }

  if (response.status === 204) {
    return null; }
  if (isJson) {
    try {
      return await response.json();
    } catch {
      return null; 
    }
  }
  return await response.text(); 
};

