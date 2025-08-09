import { API_BASE_URL, CreateGroupDto } from "../types/types";

export const createGroup = async (groupData: CreateGroupDto) => {
  const response = await fetch(`${API_BASE_URL}/api/groups`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(groupData),
  });

  if (!response.ok) {
    let errorMessage = `Failed to create group (status ${response.status})`;
    let invalidEmails: string[] = [];

    const contentType = response.headers.get("content-type") || "";

    try {
      if (contentType.includes("application/json")) {
        const errorData = await response.json();

        // preferred shapes:
        if (errorData.message) errorMessage = errorData.message;
        else if (errorData.title) errorMessage = errorData.title;
        else if (errorData.errors) {
          // ASP.NET validation errors object
          errorMessage = Object.values(errorData.errors).flat().join(", ");
        }

        // If backend returns a dedicated invalidEmails array -> use it
        if (Array.isArray(errorData.invalidEmails) && errorData.invalidEmails.length) {
          invalidEmails = errorData.invalidEmails.map((e: any) => String(e).trim());
        }

        // Some servers may put them inside errors.memberEmails
        if (!invalidEmails.length && errorData.errors?.memberEmails) {
          invalidEmails = [].concat(errorData.errors.memberEmails).map((e: any) => String(e).trim());
        }
      } else {
        // fallback: try reading text (HTML or plain text)
        const text = await response.text();
        if (text) errorMessage = text;
      }
    } catch (parseErr) {
      // ignore parse errors, keep default message
      console.warn("Failed to parse error response", parseErr);
    }

    // attach invalidEmails to the Error object so callers can check it
    const err: any = new Error(errorMessage);
    err.invalidEmails = invalidEmails;
    throw err;
  }

  return response.json();
};
