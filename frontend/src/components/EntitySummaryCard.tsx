import React from "react";
import { Box, Typography, Paper, Divider, Chip, Button } from "@mui/material";
import PersonIcon from "@mui/icons-material/Person";
import CheckCircleIcon from "@mui/icons-material/CheckCircle";
import CancelIcon from "@mui/icons-material/Cancel";

interface EntitySummaryCardProps {
  entityType: "property" | "room";
  name: string;
  pricePerMonth: number;
  areaInSquareMeters: number;
  averageRating: number;
  reviewsCount: number;
  createdAt?: string;
  // Property specific props
  isEntirePlaceRentable?: boolean;
  // Room specific props
  capacity?: number;
  isAvailable?: boolean;
  onBookRoom?: () => void;
}

export const EntitySummaryCard: React.FC<EntitySummaryCardProps> = ({
  entityType,
  name,
  pricePerMonth,
  areaInSquareMeters,
  averageRating,
  reviewsCount,
  createdAt,
  isEntirePlaceRentable,
  capacity,
  isAvailable,
  onBookRoom,
}) => {
  const isRoom = entityType === "room";
  const isProperty = entityType === "property";

  return (
    <Paper
      elevation={3}
      sx={{
        p: 4,
        mb: 4,
        position: "sticky",
        top: 20,
        borderRadius: 3,
        background:
          "linear-gradient(135deg, rgba(142, 68, 173, 0.08) 0%, rgba(175, 122, 197, 0.08) 100%)",
        border: "1px solid rgba(142, 68, 173, 0.2)",
      }}
    >
      <Typography
        variant="h6"
        gutterBottom
        sx={{
          fontWeight: 600,
          mb: 3,
          background: "linear-gradient(45deg, #8E44AD 30%, #AF7AC5 90%)",
          backgroundClip: "text",
          WebkitBackgroundClip: "text",
          WebkitTextFillColor: "transparent",
        }}
      >
        {isRoom ? "Room Summary" : "Property Summary"}
      </Typography>

      <Box sx={{ mb: 3 }}>
        <Typography
          variant="h4"
          sx={{
            background: "linear-gradient(45deg, #8E44AD 30%, #AF7AC5 90%)",
            backgroundClip: "text",
            WebkitBackgroundClip: "text",
            WebkitTextFillColor: "transparent",
            fontWeight: 700,
          }}
        >
          {pricePerMonth} PLN
        </Typography>
        <Typography variant="body2" color="text.secondary">
          per month
        </Typography>
      </Box>

      <Divider sx={{ my: 2, borderColor: "rgba(142, 68, 173, 0.3)" }} />

      {/* Size */}
      <Box
        sx={{
          display: "flex",
          justifyContent: "space-between",
          alignItems: "center",
          mb: 2,
        }}
      >
        <Typography variant="body2" color="text.secondary">
          Size:
        </Typography>
        <Chip
          label={`${areaInSquareMeters} m²`}
          size="small"
          sx={{
            background: "linear-gradient(45deg, #8E44AD 30%, #AF7AC5 90%)",
            color: "white",
            fontWeight: 600,
          }}
        />
      </Box>

      {/* Room specific: Capacity */}
      {isRoom && capacity && (
        <Box
          sx={{
            display: "flex",
            justifyContent: "space-between",
            alignItems: "center",
            mb: 2,
          }}
        >
          <Typography variant="body2" color="text.secondary">
            Capacity:
          </Typography>
          <Chip
            icon={<PersonIcon />}
            label={`${capacity} ${capacity === 1 ? "person" : "people"}`}
            size="small"
            variant="outlined"
            sx={{
              borderColor: "#8E44AD",
              color: "#8E44AD",
              fontWeight: 600,
            }}
          />
        </Box>
      )}

      {/* Property specific: Type */}
      {isProperty && isEntirePlaceRentable !== undefined && (
        <Box
          sx={{
            display: "flex",
            justifyContent: "space-between",
            alignItems: "center",
            mb: 2,
          }}
        >
          <Typography variant="body2" color="text.secondary">
            Type:
          </Typography>
          <Chip
            label={isEntirePlaceRentable ? "Entire Place" : "Shared Space"}
            size="small"
            variant="outlined"
            sx={{
              borderColor: "#8E44AD",
              color: "#8E44AD",
              fontWeight: 600,
            }}
          />
        </Box>
      )}

      {/* Room specific: Availability */}
      {isAvailable !== undefined && (
        <Box
          sx={{
            display: "flex",
            justifyContent: "space-between",
            alignItems: "center",
            mb: 2,
          }}
        >
          <Typography variant="body2" color="text.secondary">
            Availability:
          </Typography>
          <Chip
            icon={isAvailable ? <CheckCircleIcon /> : <CancelIcon />}
            label={isAvailable ? "Available" : "Not Available"}
            size="small"
            sx={{
              background: isAvailable
                ? "linear-gradient(45deg, #4CAF50 30%, #81C784 90%)"
                : "linear-gradient(45deg, #F44336 30%, #E57373 90%)",
              color: "white",
              fontWeight: 600,
            }}
          />
        </Box>
      )}

      {/* Reviews */}
      <Box
        sx={{
          display: "flex",
          justifyContent: "space-between",
          alignItems: "center",
          mb: 3,
        }}
      >
        <Typography variant="body2" color="text.secondary">
          Reviews:
        </Typography>
        <Box sx={{ display: "flex", alignItems: "center" }}>
          <Typography
            variant="body2"
            sx={{
              ml: 1,
              color: "#8E44AD",
              fontWeight: 600,
            }}
          >
            {averageRating > 0 ? averageRating.toFixed(1) : "No reviews"} (
            {reviewsCount})
          </Typography>
        </Box>
      </Box>

      {/* Created Date (for rooms) */}
      {createdAt && (
        <Box
          sx={{
            display: "flex",
            justifyContent: "space-between",
            alignItems: "center",
            mb: 3,
          }}
        >
          <Typography variant="body2" color="text.secondary">
            Created:
          </Typography>
          <Typography
            variant="body2"
            sx={{
              color: "#8E44AD",
              fontWeight: 600,
            }}
          >
            {new Date(createdAt).toLocaleDateString()}
          </Typography>
        </Box>
      )}

      {/* Book Room Button (for rooms only) */}
      {isRoom && onBookRoom && isAvailable !== undefined && (
        <Button
          variant="contained"
          fullWidth
          onClick={onBookRoom}
          disabled={!isAvailable}
          sx={{
            background: isAvailable
              ? "linear-gradient(45deg, #8E44AD 30%, #AF7AC5 90%)"
              : "rgba(0, 0, 0, 0.12)",
            color: "white",
            fontWeight: 600,
            py: 1.5,
            borderRadius: 2,
            textTransform: "none",
            fontSize: "1rem",
            "&:hover": {
              background: isAvailable
                ? "linear-gradient(45deg, #6A1B9A 30%, #8E44AD 90%)"
                : "rgba(0, 0, 0, 0.12)",
            },
            "&:disabled": {
              color: "rgba(0, 0, 0, 0.26)",
            },
            transition: "all 0.3s ease",
          }}
        >
          {isAvailable ? "Book This Room" : "Room Not Available"}
        </Button>
      )}
    </Paper>
  );
};
