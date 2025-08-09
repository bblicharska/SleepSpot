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
} from "@mui/material";
import AttachMoneyIcon from "@mui/icons-material/AttachMoney";
import SquareFootIcon from "@mui/icons-material/SquareFoot";
import PeopleIcon from "@mui/icons-material/People";
import LocationOnIcon from "@mui/icons-material/LocationOn";
import ChevronLeftIcon from "@mui/icons-material/ChevronLeft";
import ChevronRightIcon from "@mui/icons-material/ChevronRight";
import PhotoLibraryIcon from "@mui/icons-material/PhotoLibrary";
import { PropertyImageDto } from "../types/types";
import DeleteIcon from "@mui/icons-material/Delete";
import { getImageUrl } from "../queries/getImageUrl";

interface BaseCardProps {
  title: string;
  description: string;
  image?: string;
  images?: PropertyImageDto[];
  address: string;
  price: number;
  area: number;
  capacity?: number;
  cardType: "property" | "room";
  onViewDetails: () => void;
  canDelete?: boolean;
  onDelete?: () => void;
}

export const BaseCard: React.FC<BaseCardProps> = ({
  title,
  description,
  image,
  images = [],
  address,
  price,
  area,
  capacity,
  cardType,
  onViewDetails,
  canDelete = false,
  onDelete,
}) => {
  const [currentImageIndex, setCurrentImageIndex] = useState(0);
  const [imageError, setImageError] = useState(false);

  const sortedImages = [...images].sort(
    (a, b) => a.displayOrder - b.displayOrder
  );
  const hasMultipleImages = sortedImages.length > 1;
  const hasAnyImage = sortedImages.length > 0 || (image && image.trim() !== "");

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

  const getCurrentImage = () => {
    if (sortedImages.length === 0) {
      return image ? getImageUrl(image) : "";
    }
    return getImageUrl(
      sortedImages[currentImageIndex]?.imageUrl || image || ""
    );
  };

  const handleImageError = () => {
    console.error("Failed to load image:", getCurrentImage());
    setImageError(true);
  };

  const generateChips = () => {
    const chips = [
      <Chip
        key="price"
        icon={<AttachMoneyIcon />}
        label={`${price} PLN/month`}
        color="primary"
        variant="outlined"
        size="small"
      />,
      <Chip
        key="area"
        icon={<SquareFootIcon />}
        label={`${area} m²`}
        color="secondary"
        variant="outlined"
        size="small"
      />,
    ];

    if (cardType === "property") {
      chips.push(
        <Chip
          key="type"
          label="Entire apartment"
          color="success"
          size="small"
        />
      );
    } else if (cardType === "room" && capacity) {
      chips.push(
        <Chip
          key="capacity"
          icon={<PeopleIcon />}
          label={`max ${capacity} ${capacity > 1 ? "people" : "person"}`}
          color="info"
          variant="outlined"
          size="small"
        />
      );
    }

    return chips;
  };

  return (
    <Card
      sx={{
        position: "relative",
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
      {canDelete && (
        <IconButton
          aria-label="delete"
          onClick={(e) => {
            e.stopPropagation();
            if (onDelete) onDelete();
          }}
          sx={{
            position: "absolute",
            top: 8,
            left: 8,
            color: "rgba(255, 0, 0, 0.8)",
            backgroundColor: "rgba(255,255,255,0.8)",
            "&:hover": {
              backgroundColor: "rgba(255,0,0,0.15)",
              color: "red",
            },
            zIndex: 10,
          }}
        >
          <DeleteIcon />
        </IconButton>
      )}
      <Box
        sx={{
          position: "relative",
          height: 220,
          overflow: "hidden",
          borderRadius: 2,
        }}
      >
        {hasAnyImage && !imageError ? (
          <>
            <CardMedia
              component="img"
              height="220"
              image={getCurrentImage()}
              alt={title}
              onError={handleImageError}
              sx={{
                objectFit: "cover",
                transition: "opacity 0.3s ease-in-out",
                borderTopLeftRadius: 8,
                borderTopRightRadius: 8,
              }}
            />
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
          </>
        ) : (
          <Box
            sx={{
              height: 220,
              backgroundColor: "#eee",
              display: "flex",
              justifyContent: "center",
              alignItems: "center",
              color: "#999",
              fontStyle: "italic",
              fontSize: 18,
              borderTopLeftRadius: 8,
              borderTopRightRadius: 8,
            }}
          >
            No Image Available
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
          {title}
        </Typography>
        <Box
          sx={{ minHeight: "40px", display: "flex", alignItems: "flex-start" }}
        >
          <Typography variant="body2" color="text.secondary">
            {description}
          </Typography>
        </Box>
        <Box sx={{ display: "flex", alignItems: "center", gap: 0.5 }}>
          <LocationOnIcon sx={{ fontSize: 18, color: "text.secondary" }} />
          <Typography variant="caption" color="text.secondary">
            {address}
          </Typography>
        </Box>
        <Box
          sx={{ display: "flex", gap: 1, flexWrap: "wrap", alignSelf: "end" }}
        >
          {generateChips()}
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
