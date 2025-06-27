import React from "react";
import {
  Card,
  CardContent,
  Typography,
  Box,
  Chip,
  Button,
  CardActions,
} from "@mui/material";
import AttachMoneyIcon from "@mui/icons-material/AttachMoney";
import SquareFootIcon from "@mui/icons-material/SquareFoot";
import PeopleIcon from "@mui/icons-material/People";
import LocationOnIcon from "@mui/icons-material/LocationOn";

export const RoomCard = ({
  name,
  description,
  price,
  area,
  capacity,
  address,
  onViewDetails,
}: {
  name: string;
  description: string;
  price: number;
  area: number;
  capacity: number;
  address: string;
  onViewDetails: () => void;
}) => {
  return (
    <Card
      sx={{
        width: "100%",
        height: "100%",
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
          height: 220,
          backgroundColor: "#eee",
          display: "flex",
          justifyContent: "center",
          alignItems: "center",
          color: "#999",
          fontStyle: "italic",
          fontSize: 18,
        }}
      >
        No Image Available
      </Box>

      <CardContent
        sx={{
          flexGrow: 1,
          display: "grid",
          gridTemplateRows: "auto auto auto 1fr",
          gap: 1,
        }}
      >
        <Typography variant="h6" sx={{ fontWeight: 600 }}>
          {name}
        </Typography>

        {/* 🏠 Address */}
        <Box sx={{ display: "flex", alignItems: "center", gap: 0.5 }}>
          <LocationOnIcon sx={{ fontSize: 18, color: "text.secondary" }} />
          <Typography variant="caption" color="text.secondary">
            {address}
          </Typography>
        </Box>

        {/* Description block with fixed height */}
        <Box sx={{ minHeight: 40 }}>
          <Typography variant="body2" color="text.secondary">
            {description}
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
          <Chip
            icon={<PeopleIcon />}
            label={`max ${capacity} ${capacity > 1 ? "people" : "person"}`}
            color="info"
            variant="outlined"
            size="small"
          />
        </Box>
      </CardContent>

      <CardActions sx={{ padding: 2, pt: 1 }}>
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
