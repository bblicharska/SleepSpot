import { API_BASE_URL } from "../types/types";

  export const getImageUrl = (imagePath: string) => {
    if (!imagePath) return "/placeholder-image.jpg";

    if (imagePath.startsWith("http")) return imagePath;

    if (imagePath.startsWith("/")) {
      return `${API_BASE_URL}${imagePath}`;
    }

    return `${API_BASE_URL}/uploads/properties/${imagePath}`;
  };