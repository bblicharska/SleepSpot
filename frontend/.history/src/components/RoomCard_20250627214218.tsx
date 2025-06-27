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
import ChevronLeftIcon from "@mui/icons-material/ChevronLeft";
import ChevronRightIcon from "@mui/icons-material/ChevronRight";

interface Image {
  id: string;
  imageUrl: string;
  originalFileName: string;
  isPrimary: boolean;
  displayOrder: number;
}

interface RoomCardProps {
  name: string;
  description: string;
  price: number;
  area: number;
  capacity: number;
  onViewDetails: () => void;
  images?: Image[];
}

export const RoomCard: React.FC<RoomCardProps> = ({
  name,
  description,
  price,
  area,
  capacity,
  onViewDetails,
  images = [],
}) => {
  const [imageIndex, setImageIndex] = useState(0);

  const sortedImages = [...images].sort(
    (a, b) => a.displayOrder - b.displayOrder
  );
  const hasImages = sortedImages.length > 0;

  const getImageUrl = (path: string) => {
    if (!path) return "/placeholder-image.jpg";
    if (path.startsWith("http")) return path;
    return `http://localhost:5000${path}`;
  };

  const handlePrev = (e: React.MouseEvent) => {
    e.stopPropagation();
    setImageIndex((prev) => (prev === 0 ? sortedImages.length - 1 : prev - 1));
  };

  const handleNext = (e: React.MouseEvent) => {
    e.stopPropagation();
    setImageIndex((prev) => (prev === sortedImages.length - 1 ? 0 : prev + 1));
  };

  const displayImage = hasImages
    ? getImageUrl(sortedImages[imageIndex].imageUrl)
    : "/placeholder-image.jpg";

  return (
    <Card
      sx={{
        width: 320,
        display: "flex",
        flexDirection: "column",
        transition: "0.3s",
        "&:hover": {
          boxShadow: 6,
          transform: "translateY(-4px)",
        },
      }}
    >
      <Box sx={{ position: "relative", height: 180 }}>
        <CardMedia
          component="img"
          image={displayImage}
          alt={name}
          height="180"
          sx={{ objectFit: "cover" }}
        />
        {hasImages && sortedImages.length > 1 && (
          <>
            <IconButton
              onClick={handlePrev}
              sx={{
                position: "absolute",
                top: "50%",
                left: 8,
                backgroundColor: "rgba(255,255,255,0.8)",
                transform: "translateY(-50%)",
              }}
            >
              <ChevronLeftIcon fontSize="small" />
            </IconButton>
            <IconButton
              onClick={handleNext}
              sx={{
                position: "absolute",
                top: "50%",
                right: 8,
                backgroundColor: "rgba(255,255,255,0.8)",
                transform: "translateY(-50%)",
              }}
            >
              <ChevronRightIcon fontSize="small" />
            </IconButton>
          </>
        )}
      </Box>

      <CardContent sx={{ flexGrow: 1, display: "grid", gap: 1 }}>
        <Typography variant="h6" gutterBottom sx={{ fontWeight: 600 }}>
          {name}
        </Typography>

        <Typography
          variant="body2"
          color="text.secondary"
          sx={{ minHeight: "40px" }}
        >
          {description}
        </Typography>

        <Box
          sx={{ display: "flex", gap: 1, flexWrap: "wrap", alignSelf: "end" }}
        >
          <Chip
            icon={<AttachMoneyIcon />}
            label={`${price} PLN/month`}
            color="primary"
            size="small"
            variant="outlined"
          />
          <Chip
            icon={<SquareFootIcon />}
            label={`${area} m²`}
            color="secondary"
            size="small"
            variant="outlined"
          />
          <Chip
            icon={<PeopleIcon />}
            label={`max ${capacity} ${capacity > 1 ? "people" : "person"}`}
            color="info"
            size="small"
            variant="outlined"
          />
        </Box>
      </CardContent>

      <CardActions sx={{ p: 2, pt: 1 }}>
        <Button
          variant="contained"
          fullWidth
          onClick={onViewDetails}
          sx={{
            textTransform: "none",
            fontWeight: 600,
            borderRadius: 2,
            background: "linear-gradient(45deg, #8E44AD 30%, #AF7AC5 90%)",
            "&:hover": {
              background: "linear-gradient(45deg, #6A1B9A 30%, #8E44AD 90%)",
            },
          }}
        >
          View Details
        </Button>
      </CardActions>
    </Card>
  );
};
