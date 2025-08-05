import { API_BASE_URL } from "../types/types";

export const fetchListingDetails = async (id: string) => {
        const response  = await fetch(
          `${API_BASE_URL}/gateway/group-listing-details/${id}`
        );

     if (!response.ok) throw new Error("Failed to fetch listing");
       
       
     return response.json();
    };