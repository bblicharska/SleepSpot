import React, { useState } from "react";
import {
  Card,
  CardContent,
  CardMedia,
  Typography,
  Box,
  Chip,
  Button,
  CardActions,
  IconButton,
  Badge,
} from "@mui/material";
import AttachMoneyIcon from "@mui/icons-material/AttachMoney";
import SquareFootIcon from "@mui/icons-material/SquareFoot";
import ChevronLeftIcon from "@mui/icons-material/ChevronLeft";
import ChevronRightIcon from "@mui/icons-material/ChevronRight";
import PhotoLibraryIcon from "@mui/icons-material/PhotoLibrary";

interface Image {
  id: string;
  imageUrl: string;
  originalFileName: string;
  isPrimary: boolean;
  displayOrder: number;
}

interface PropertyCardProps {
  name: string;
  description: string;
  image: string; // Primary image URL
  images?: Image[]; // All images for carousel
  address: string;
  price: number;
  area: number;
  onViewDetails: () => void;
}

export const PropertyCard: React.FC<PropertyCardProps> = ({
  name,
  description,
  image,
  images = [],
  address,
  price,
  area,
  onViewDetails,
}) => {
  const [currentImageIndex, setCurrentImageIndex] = useState(0);
  const [imageError, setImageError] = useState(false);

  console.log("PropertyCard images:", images);
  console.log("Primary image:", image);

  // Sort images by display order for carousel
  const sortedImages = [...images].sort(
    (a, b) => a.displayOrder - b.displayOrder
  );
  const hasMultipleImages = sortedImages.length > 1;

  const handlePrevImage = (e: React.MouseEvent) => {
    e.stopPropagation();
    setCurrentImageIndex((prev) =>
      prev === 0 ? sortedImages.length - 1 : prev - 1
    );
  };

  const handleNextImage = (e: React.MouseEvent) => {
    e.stopPropagation();
    setCurrentImageIndex((prev) =>
      prev === sortedImages.length - 1 ? 0 : prev + 1
    );
  };

  // Helper function to construct proper image URL
  const getImageUrl = (imagePath: string) => {
    if (!imagePath) return "/placeholder-image.jpg";

    // If it's already a full URL, return as is
    if (imagePath.startsWith("http")) return imagePath;

    // If it starts with /, construct full URL through API Gateway
    if (imagePath.startsWith("/")) {
      // Since your API Gateway is on localhost:5000, construct the full URL
      return `http://localhost:5000${imagePath}`;
    }

    // For relative paths, add the uploads prefix
    return `http://localhost:5000/uploads/properties/${imagePath}`;
  };

  const getCurrentImage = () => {
    if (sortedImages.length === 0) {
      return getImageUrl(image);
    }
    return getImageUrl(sortedImages[currentImageIndex]?.imageUrl || image);
  };

  const handleImageError = () => {
    console.error("Failed to load image:", getCurrentImage());
    setImageError(true);
  };

  return (
    <Card
      sx={{
        width: 400,
        height: "auto",
        display: "flex",
        flexDirection: "column",
        transition: "transform 0.2s ease-in-out, box-shadow 0.2s ease-in-out",
        "&:hover": {
          transform: "translateY(-4px)",
          boxShadow: "0 8px 25px rgba(0,0,0,0.15)",
        },
      }}
    >
      <Box
        sx={{
          position: "relative",
          height: 220,
          overflow: "hidden",
          borderRadius: 2,
        }}
      >
        <CardMedia
          component="img"
          height="220"
          image={imageError ? "/placeholder-image.jpg" : getCurrentImage()}
          alt={name}
          onError={handleImageError}
          sx={{
            objectFit: "cover",
            transition: "opacity 0.3s ease-in-out",
            borderTopLeftRadius: 8,
            borderTopRightRadius: 8,
          }}
        />

        {/* Image count badge */}
        {hasMultipleImages && (
          <Box
            sx={{
              position: "absolute",
              top: 8,
              right: 8,
              zIndex: 2,
              backgroundColor: "rgba(0,0,0,0.7)",
              color: "white",
              padding: "4px 8px",
              borderRadius: "12px",
              display: "flex",
              alignItems: "center",
              gap: 0.5,
              fontSize: 10,
              fontWeight: 600,
            }}
          >
            <PhotoLibraryIcon sx={{ fontSize: 12 }} />
            {sortedImages.length}
          </Box>
        )}

        {/* Navigation arrows */}
        {hasMultipleImages && (
          <>
            <IconButton
              onClick={handlePrevImage}
              sx={{
                position: "absolute",
                left: 4,
                top: "50%",
                transform: "translateY(-50%)",
                backgroundColor: "rgba(255,255,255,0.8)",
                color: "rgba(0,0,0,0.7)",
                width: 32,
                height: 32,
                "&:hover": {
                  backgroundColor: "rgba(255,255,255,0.95)",
                },
                transition: "all 0.2s ease-in-out",
              }}
            >
              <ChevronLeftIcon fontSize="small" />
            </IconButton>

            <IconButton
              onClick={handleNextImage}
              sx={{
                position: "absolute",
                right: 4,
                top: "50%",
                transform: "translateY(-50%)",
                backgroundColor: "rgba(255,255,255,0.8)",
                color: "rgba(0,0,0,0.7)",
                width: 32,
                height: 32,
                "&:hover": {
                  backgroundColor: "rgba(255,255,255,0.95)",
                },
                transition: "all 0.2s ease-in-out",
              }}
            >
              <ChevronRightIcon fontSize="small" />
            </IconButton>
          </>
        )}

        {/* Image dots indicator */}
        {hasMultipleImages && (
          <Box
            sx={{
              position: "absolute",
              bottom: 8,
              left: "50%",
              transform: "translateX(-50%)",
              display: "flex",
              gap: 0.5,
              backgroundColor: "rgba(0,0,0,0.5)",
              borderRadius: "12px",
              padding: "4px 8px",
            }}
          >
            {sortedImages.map((_, index) => (
              <Box
                key={index}
                sx={{
                  width: 6,
                  height: 6,
                  borderRadius: "50%",
                  backgroundColor:
                    index === currentImageIndex
                      ? "white"
                      : "rgba(255,255,255,0.5)",
                  transition: "background-color 0.2s ease-in-out",
                }}
              />
            ))}
          </Box>
        )}
      </Box>

      <CardContent
        sx={{
          flexGrow: 1,
          display: "grid",
          gridTemplateRows: "auto auto 1fr auto",
          gap: 1,
          paddingBottom: 1,
        }}
      >
        <Typography
          variant="h6"
          gutterBottom
          sx={{ margin: 0, fontWeight: 600 }}
        >
          {name}
        </Typography>

        <Box
          sx={{ minHeight: "40px", display: "flex", alignItems: "flex-start" }}
        >
          <Typography variant="body2" color="text.secondary">
            {description}
          </Typography>
        </Box>

        <Box sx={{ display: "flex", alignItems: "flex-start" }}>
          <Typography variant="body2" color="text.secondary">
            <strong>Address:</strong> {address}
          </Typography>
        </Box>

        <Box
          sx={{ display: "flex", gap: 1, flexWrap: "wrap", alignSelf: "end" }}
        >
          <Chip
            icon={<AttachMoneyIcon />}
            label={`${price} PLN/month`}
            color="primary"
            variant="outlined"
            size="small"
          />
          <Chip
            icon={<SquareFootIcon />}
            label={`${area} m²`}
            color="secondary"
            variant="outlined"
            size="small"
          />
          <Chip label="Entire apartment" color="success" size="small" />
        </Box>
      </CardContent>

      <CardActions sx={{ padding: 2, paddingTop: 1, paddingBottom: 2 }}>
        <Button
          variant="contained"
          fullWidth
          onClick={onViewDetails}
          sx={{
            textTransform: "none",
            borderRadius: 2,
            fontWeight: 600,
            fontSize: "0.95rem",
            padding: "12px 24px",
            background: "linear-gradient(45deg, #8E44AD 30%, #AF7AC5 90%)",
            boxShadow: "0 3px 5px 2px rgba(142, 68, 173, .3)",
            transition: "all 0.3s ease-in-out",
            "&:hover": {
              background: "linear-gradient(45deg, #6A1B9A 30%, #8E44AD 90%)",
              boxShadow: "0 6px 10px 4px rgba(142, 68, 173, .3)",
              transform: "translateY(-2px)",
            },
            "&:active": {
              transform: "translateY(0px)",
            },
          }}
        >
          View Details
        </Button>
      </CardActions>
    </Card>
  );
};
