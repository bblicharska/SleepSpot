import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import {
  Box,
  Typography,
  CircularProgress,
  Paper,
  Divider,
  Grid,
  Chip,
  Dialog,
  DialogContent,
  IconButton,
} from "@mui/material";
import CloseIcon from "@mui/icons-material/Close";
import { PropertyMap } from "./PropertyMap";
import AttachMoneyIcon from "@mui/icons-material/AttachMoney";
import SquareFootIcon from "@mui/icons-material/SquareFoot";
import CheckCircleIcon from "@mui/icons-material/CheckCircle";

interface PropertyImageDto {
  id: string;
  imageUrl: string;
  originalFileName: string;
  isPrimary: boolean;
  displayOrder: number;
}

interface RoomDto {
  id: string;
  name: string;
  description: string;
  pricePerMonth: number;
  areaInSquareMeters: number;
  capacity: number;
  isAvailable: boolean;
  createdAt: string;
}

interface Property {
  id: string;
  name: string;
  description: string;
  address: string;
  pricePerMonth: number;
  areaInSquareMeters: number;
  isEntirePlaceRentable: boolean;
  images: PropertyImageDto[];
  rooms: RoomDto[];
  ownerId: string;
}

export const PropertyDetailsPage = () => {
  const { id } = useParams();
  const [property, setProperty] = useState<Property | null>(null);
  const [loading, setLoading] = useState(true);
  const [selectedImageIndex, setSelectedImageIndex] = useState<number | null>(
    null
  ); // NEW

  useEffect(() => {
    const loadProperty = async () => {
      try {
        const res = await fetch(`http://localhost:5000/api/properties/${id}`);
        const data = await res.json();
        setProperty(data);
      } catch (err) {
        console.error("Failed to load property", err);
      } finally {
        setLoading(false);
      }
    };

    loadProperty();
  }, [id]);

  if (loading) return <CircularProgress />;
  if (!property)
    return <Typography variant="h6">Property not found</Typography>;

  const sortedImages = [...property.images].sort(
    (a, b) => a.displayOrder - b.displayOrder
  ); // NEW

  const getImageUrl = (url: string) => {
    if (!url) return "/placeholder-image.jpg";
    if (url.startsWith("http")) return url;
    if (url.startsWith("/")) return `http://localhost:5000${url}`;
    return `http://localhost:5000/uploads/properties/${url}`;
  };

  return (
    <Box sx={{ px: { xs: 2, md: 6 }, py: 4 }}>
      {" "}
      {/* Wider padding */}
      <Typography variant="h4" fontWeight={600} gutterBottom>
        {property.name}
      </Typography>
      <Typography variant="subtitle1" color="text.secondary" paragraph>
        {property.description}
      </Typography>
      {/* Image gallery */}
      {sortedImages.length > 0 && (
        <Grid container spacing={2} sx={{ my: 3 }}>
          {sortedImages.map((img, index) => (
            <Grid size={{ xs: 6, sm: 4, md: 3 }} key={img.id}>
              <Box
                component="img"
                src={getImageUrl(img.imageUrl)}
                alt={img.originalFileName}
                sx={{
                  width: "100%",
                  height: 180,
                  objectFit: "cover",
                  borderRadius: 2,
                  cursor: "pointer",
                  boxShadow: 1,
                  transition: "transform 0.2s",
                  "&:hover": {
                    transform: "scale(1.02)",
                  },
                }}
                onClick={() => setSelectedImageIndex(index)}
              />
            </Grid>
          ))}
        </Grid>
      )}
      {/* Image modal */}
      <Dialog
        open={selectedImageIndex !== null}
        onClose={() => setSelectedImageIndex(null)}
        fullWidth
        maxWidth="md"
      >
        <DialogContent sx={{ position: "relative", p: 0 }}>
          <IconButton
            onClick={() => setSelectedImageIndex(null)}
            sx={{
              position: "absolute",
              top: 8,
              right: 8,
              zIndex: 1,
              background: "rgba(0,0,0,0.5)",
              color: "white",
            }}
          >
            <CloseIcon />
          </IconButton>
          {selectedImageIndex !== null && (
            <Box
              component="img"
              src={getImageUrl(sortedImages[selectedImageIndex].imageUrl)}
              alt=""
              sx={{
                width: "100%",
                maxHeight: "80vh",
                objectFit: "contain",
              }}
            />
          )}
        </DialogContent>
      </Dialog>
      <Divider sx={{ my: 3 }} />
      <Grid container spacing={4}>
        <Grid size={{ xs: 12, md: 7 }}>
          <Paper elevation={2} sx={{ p: 3 }}>
            <Typography variant="h6" gutterBottom>
              Property Info
            </Typography>
            <Chip
              icon={<AttachMoneyIcon />}
              label={`${property.pricePerMonth} PLN/month`}
              color="primary"
              sx={{ mr: 1, mb: 1 }}
            />
            <Chip
              icon={<SquareFootIcon />}
              label={`${property.areaInSquareMeters} m²`}
              color="secondary"
              sx={{ mr: 1, mb: 1 }}
            />
            {property.isEntirePlaceRentable && (
              <Chip
                icon={<CheckCircleIcon />}
                label="Entire place"
                color="success"
                sx={{ mb: 1 }}
              />
            )}
          </Paper>

          <Box sx={{ mt: 3 }}>
            <PropertyMap address={property.address} />
          </Box>
        </Grid>

        <Grid size={{ xs: 12, md: 5 }}>
          <Paper elevation={2} sx={{ p: 3 }}>
            <Typography variant="h6" gutterBottom>
              Rooms
            </Typography>
            {property.rooms.length === 0 ? (
              <Typography>No rooms listed</Typography>
            ) : (
              property.rooms.map((room) => (
                <Box key={room.id} sx={{ mb: 3 }}>
                  <Typography fontWeight={600}>{room.name}</Typography>
                  <Typography variant="body2" color="text.secondary">
                    {room.description}
                  </Typography>
                  <Chip
                    label={`${room.pricePerMonth} PLN`}
                    size="small"
                    sx={{ mr: 1, mt: 1 }}
                  />
                  <Chip
                    label={`${room.areaInSquareMeters} m²`}
                    size="small"
                    sx={{ mr: 1, mt: 1 }}
                  />
                  <Chip
                    label={`Capacity: ${room.capacity}`}
                    size="small"
                    sx={{ mr: 1, mt: 1 }}
                  />
                  <Chip
                    label={room.isAvailable ? "Available" : "Unavailable"}
                    color={room.isAvailable ? "success" : "default"}
                    size="small"
                    sx={{ mt: 1 }}
                  />
                </Box>
              ))
            )}
          </Paper>
        </Grid>
      </Grid>
    </Box>
  );
};
