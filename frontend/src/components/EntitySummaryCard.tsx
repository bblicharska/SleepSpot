import React from "react";
import { Box, Typography, Paper, Divider, Chip } from "@mui/material";
import PersonIcon from "@mui/icons-material/Person";
import CheckCircleIcon from "@mui/icons-material/CheckCircle";
import CancelIcon from "@mui/icons-material/Cancel";
import CalendarTodayIcon from "@mui/icons-material/CalendarToday";

interface EntitySummaryCardProps {
  entityType: "property" | "room";
  pricePerMonth: number;
  areaInSquareMeters: number;
  averageRating?: number;
  reviewsCount?: number;
  createdAt?: string;
  availableSince?: string;
  isEntirePlaceRentable?: boolean;
  capacity?: number;
  isAvailable?: boolean;
  showReviews?: boolean;
  showAvailability?: boolean;
  showCreatedDate?: boolean;
  showAvailableSince?: boolean;
}

export const EntitySummaryCard: React.FC<EntitySummaryCardProps> = ({
  entityType,
  pricePerMonth,
  areaInSquareMeters,
  averageRating = 0,
  reviewsCount = 0,
  createdAt,
  availableSince,
  isEntirePlaceRentable,
  capacity,
  isAvailable,
  showReviews = true,
  showAvailability = true,
  showCreatedDate = true,
  showAvailableSince = true,
}) => {
  const isRoom = entityType === "room";
  const isProperty = entityType === "property";

  const formatAvailableSince = (dateString: string) => {
    const date = new Date(dateString);
    const today = new Date();

    today.setHours(0, 0, 0, 0);
    const availableDate = new Date(date);
    availableDate.setHours(0, 0, 0, 0);

    const diffTime = availableDate.getTime() - today.getTime();
    const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));

    const formattedDate = date.toLocaleDateString("en-US", {
      year: "numeric",
      month: "short",
      day: "numeric",
    });

    if (diffDays < 0) {
      return {
        text: `Since ${formattedDate}`,
        fullText: `Available since ${formattedDate}`,
        status: "available",
      };
    } else if (diffDays === 0) {
      return {
        text: "From today",
        fullText: "Available from today",
        status: "available",
      };
    } else {
      return {
        text: `From ${formattedDate}`,
        fullText: `Available from ${formattedDate}`,
        status: "future",
      };
    }
  };

  const availabilityInfo = availableSince
    ? formatAvailableSince(availableSince)
    : null;

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
      {showAvailability && isAvailable !== undefined && (
        <Box
          sx={{
            display: "flex",
            justifyContent: "space-between",
            alignItems: "center",
            mb: 2,
          }}
        >
          <Typography variant="body2" color="text.secondary">
            Status:
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
      {showAvailableSince && availabilityInfo && (
        <Box
          sx={{
            display: "flex",
            justifyContent: "space-between",
            alignItems: "center",
            mb: 2,
          }}
        >
          <Typography variant="body2" color="text.secondary">
            Available:
          </Typography>
          <Chip
            icon={<CalendarTodayIcon />}
            label={availabilityInfo.text}
            size="small"
            variant="outlined"
            title={availabilityInfo.fullText}
            sx={{
              borderColor:
                availabilityInfo.status === "available" ? "#4CAF50" : "#FF9800",
              color:
                availabilityInfo.status === "available" ? "#4CAF50" : "#FF9800",
              backgroundColor:
                availabilityInfo.status === "available"
                  ? "rgba(76, 175, 80, 0.08)"
                  : "rgba(255, 152, 0, 0.08)",
              fontWeight: 600,
              "& .MuiChip-icon": {
                color:
                  availabilityInfo.status === "available"
                    ? "#4CAF50"
                    : "#FF9800",
              },
            }}
          />
        </Box>
      )}
      {showReviews && (
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
      )}
      {showCreatedDate && createdAt && (
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
    </Paper>
  );
};
