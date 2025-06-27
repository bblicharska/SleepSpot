import React, { useEffect, useState } from "react";
import { Box, Tabs, Tab, Grid } from "@mui/material";
import { PropertyCard } from "./PropertyCard";
import { RoomCard } from "./RoomCard";
import { useNavigate } from "react-router-dom";

interface Image {
  id: string;
  imageUrl: string;
  originalFileName: string;
  isPrimary: boolean;
  displayOrder: number;
}

interface Room {
  id: string;
  name: string;
  description: string;
  pricePerMonth: number;
  areaInSquareMeters: number;
  capacity: number;
  isAvailable: boolean;
  propertyName: string;
}

interface Property {
  id: string;
  name: string;
  description: string;
  address: string;
  pricePerMonth: number;
  areaInSquareMeters: number;
  isEntirePlaceRentable: boolean;
  images: Image[]; // Updated to use Image interface
  rooms: Omit<Room, "propertyName">[];
}

export const PropertiesPage = () => {
  const [properties, setProperties] = useState<Property[]>([]);
  const [tabIndex, setTabIndex] = useState(0);
  const navigate = useNavigate();

  useEffect(() => {
    fetch("/api/properties")
      .then((res) => res.json())
      .then((data) => setProperties(data))
      .catch((err) => console.error("Error fetching properties:", err));
  }, []);

  console.log("Properties:", properties);

  const entirePlaces = properties.filter((p) => p.isEntirePlaceRentable);
  const allRooms: Room[] = properties.flatMap((p) =>
    p.rooms.map((room) => ({
      ...room,
      propertyName: p.name,
    }))
  );

  const handleTabChange = (_event: React.SyntheticEvent, newValue: number) => {
    setTabIndex(newValue);
  };

  // Handler for navigating to details
  const handleViewDetails = (type: "property" | "room", id: string) => {
    if (type === "property") {
      navigate(`/property/${id}`);
    } else {
      navigate(`/room/${id}`);
    }
  };

  // Helper function to get the primary image or first image
  const getPrimaryImage = (images: Image[]): string => {
    if (!images || images.length === 0) return "/placeholder-image.jpg";

    // Find primary image
    const primaryImage = images.find((img) => img.isPrimary);
    if (primaryImage) return primaryImage.imageUrl;

    // Sort by display order and return first
    const sortedImages = [...images].sort(
      (a, b) => a.displayOrder - b.displayOrder
    );
    return sortedImages[0]?.imageUrl || "/placeholder-image.jpg";
  };

  return (
    <Box>
      <Tabs value={tabIndex} onChange={handleTabChange} sx={{ mb: 2 }}>
        <Tab label="Entire Apartments" />
        <Tab label="Rooms" />
      </Tabs>

      {tabIndex === 0 && (
        <Grid container spacing={2}>
          {entirePlaces.map((p) => (
            <Grid size={{ xs: 12, sm: 4, md: 4 }} key={p.id}>
              <PropertyCard
                name={p.name}
                description={p.description}
                image={getPrimaryImage(p.images)}
                images={p.images} // Pass all images for potential carousel/gallery
                address={p.address}
                price={p.pricePerMonth}
                area={p.areaInSquareMeters}
                onViewDetails={() => handleViewDetails("property", p.id)}
              />
            </Grid>
          ))}
        </Grid>
      )}

      {tabIndex === 1 && (
        <Grid container spacing={2}>
          {allRooms.map((room) => (
            <Grid size={{ xs: 12, sm: 4, md: 4 }} key={room.id}>
              <RoomCard
                name={room.name}
                description={room.description}
                price={room.pricePerMonth}
                area={room.areaInSquareMeters}
                capacity={room.capacity}
                address={p.address}
                onViewDetails={() => handleViewDetails("room", room.id)}
              />
            </Grid>
          ))}
        </Grid>
      )}
    </Box>
  );
};
