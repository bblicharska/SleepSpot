import React from "react";
import {
  Card,
  CardContent,
  Typography,
  Box,
  Chip,
  IconButton,
} from "@mui/material";
import {
  Edit as EditIcon,
  Delete as DeleteIcon,
  People as PeopleIcon,
} from "@mui/icons-material";
import { RoomDto } from "../../types/types";

interface RoomCardProps {
  room: RoomDto;
  index: number;
  onEdit: (index: number) => void;
  onDelete: (index: number) => void;
}

export const RoomCard: React.FC<RoomCardProps> = ({
  room,
  index,
  onEdit,
  onDelete,
}) => {
  return (
    <Card>
      <CardContent>
        <Typography variant="h6" gutterBottom>
          {room.name}
        </Typography>
        <Typography variant="body2" color="text.secondary" gutterBottom>
          {room.description}
        </Typography>
        <Box
          sx={{
            display: "flex",
            justifyContent: "space-between",
            my: 1,
          }}
        >
          <Typography variant="body2">€{room.pricePerMonth}/month</Typography>
          <Typography variant="body2">{room.areaInSquareMeters}m²</Typography>
        </Box>
        <Box
          sx={{
            display: "flex",
            justifyContent: "space-between",
            alignItems: "center",
          }}
        >
          <Chip
            label={`${room.capacity} person${room.capacity > 1 ? "s" : ""}`}
            size="small"
            icon={<PeopleIcon />}
          />
          <Box>
            <IconButton size="small" onClick={() => onEdit(index)}>
              <EditIcon />
            </IconButton>
            <IconButton
              size="small"
              onClick={() => onDelete(index)}
              color="error"
            >
              <DeleteIcon />
            </IconButton>
          </Box>
        </Box>
      </CardContent>
    </Card>
  );
};
