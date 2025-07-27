import React from "react";
import { Box, Typography, Button, Alert, Grid } from "@mui/material";
import { Add as AddIcon } from "@mui/icons-material";
import { RoomCard } from "./RoomCard";
import { RoomDto } from "../../types/types";

interface RoomsStepProps {
  rooms: RoomDto[];
  isEntirePlaceRentable: boolean;
  onAddRoom: () => void;
  onEditRoom: (index: number) => void;
  onDeleteRoom: (index: number) => void;
}

export const RoomsStep: React.FC<RoomsStepProps> = ({
  rooms,
  isEntirePlaceRentable,
  onAddRoom,
  onEditRoom,
  onDeleteRoom,
}) => {
  return (
    <Box sx={{ mt: 3 }}>
      <Box
        sx={{
          display: "flex",
          justifyContent: "space-between",
          alignItems: "center",
          mb: 3,
        }}
      >
        <Typography variant="h6">Rooms</Typography>
        <Button
          variant="outlined"
          startIcon={<AddIcon />}
          onClick={onAddRoom}
          disabled={isEntirePlaceRentable}
        >
          Add Room
        </Button>
      </Box>

      {isEntirePlaceRentable && (
        <Alert severity="info" sx={{ mb: 2 }}>
          Since this is an entire place rental, you don't need to add individual
          rooms.
        </Alert>
      )}

      <Grid container spacing={2}>
        {rooms.map((room, index) => (
          <Grid size={{ xs: 12, sm: 6, md: 4 }} key={room.id}>
            <RoomCard
              room={room}
              index={index}
              onEdit={onEditRoom}
              onDelete={onDeleteRoom}
            />
          </Grid>
        ))}
      </Grid>
    </Box>
  );
};
