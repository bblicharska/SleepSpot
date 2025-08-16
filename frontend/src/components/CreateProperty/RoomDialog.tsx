import React from "react";
import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  Grid,
  TextField,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  FormControlLabel,
  Switch,
  Divider,
  Box,
  Typography,
  InputAdornment,
} from "@mui/material";
import {
  Bed as BedIcon,
  PriceCheck as PriceCheckIcon,
  Square as SquareIcon,
  People as PeopleIcon,
  PhotoCamera as PhotoCameraIcon,
} from "@mui/icons-material";
import { ImageGallery } from "./ImageGallery";
import { RoomDto } from "../../types/types";

interface RoomDialogProps {
  open: boolean;
  room: RoomDto;
  isEditing: boolean;
  fileInputRef: React.RefObject<HTMLInputElement | null>;
  onClose: () => void;
  onSave: () => void;
  onRoomChange: (field: keyof RoomDto, value: any) => void;
  onImageUpload: (event: React.ChangeEvent<HTMLInputElement>) => void;
  onRemoveImage: (imageId: string) => void;
  onSetPrimaryImage: (imageId: string) => void;
}

export const RoomDialog: React.FC<RoomDialogProps> = ({
  open,
  room,
  isEditing,
  fileInputRef,
  onClose,
  onSave,
  onRoomChange,
  onImageUpload,
  onRemoveImage,
  onSetPrimaryImage,
}) => {
  return (
    <Dialog open={open} onClose={onClose} maxWidth="md" fullWidth>
      <DialogTitle>{isEditing ? "Edit Room" : "Add New Room"}</DialogTitle>
      <DialogContent>
        <Grid container spacing={2} sx={{ mt: 1 }}>
          <Grid size={{ xs: 12 }}>
            <TextField
              fullWidth
              label="Room Name"
              value={room.name}
              onChange={(e) => onRoomChange("name", e.target.value)}
              InputProps={{
                startAdornment: (
                  <InputAdornment position="start">
                    <BedIcon />
                  </InputAdornment>
                ),
              }}
            />
          </Grid>

          <Grid size={{ xs: 12 }}>
            <TextField
              fullWidth
              multiline
              rows={2}
              label="Description"
              value={room.description}
              onChange={(e) => onRoomChange("description", e.target.value)}
            />
          </Grid>

          <Grid size={{ xs: 12 }}>
            <TextField
              fullWidth
              multiline
              rows={3}
              label="Detailed Description (Optional)"
              value={room.detailedDescription}
              onChange={(e) =>
                onRoomChange("detailedDescription", e.target.value)
              }
            />
          </Grid>

          <Grid size={{ xs: 12, sm: 4 }}>
            <TextField
              fullWidth
              type="number"
              label="Price Per Month (€)"
              value={room.pricePerMonth}
              onChange={(e) =>
                onRoomChange("pricePerMonth", parseFloat(e.target.value) || 0)
              }
              InputProps={{
                startAdornment: (
                  <InputAdornment position="start">
                    <PriceCheckIcon />
                  </InputAdornment>
                ),
              }}
            />
          </Grid>

          <Grid size={{ xs: 12, sm: 4 }}>
            <TextField
              fullWidth
              type="number"
              label="Area (m²)"
              value={room.areaInSquareMeters}
              onChange={(e) =>
                onRoomChange(
                  "areaInSquareMeters",
                  parseFloat(e.target.value) || 0
                )
              }
              InputProps={{
                startAdornment: (
                  <InputAdornment position="start">
                    <SquareIcon />
                  </InputAdornment>
                ),
              }}
            />
          </Grid>

          <Grid size={{ xs: 12, sm: 4 }}>
            <FormControl fullWidth>
              <InputLabel>Capacity</InputLabel>
              <Select
                value={room.capacity}
                onChange={(e) => onRoomChange("capacity", e.target.value)}
                startAdornment={
                  <InputAdornment position="start">
                    <PeopleIcon />
                  </InputAdornment>
                }
              >
                {[1, 2, 3, 4].map((num) => (
                  <MenuItem key={num} value={num}>
                    {num} person{num > 1 ? "s" : ""}
                  </MenuItem>
                ))}
              </Select>
            </FormControl>
          </Grid>
          <Grid size={{ xs: 12, sm: 6 }}>
            <TextField
              fullWidth
              type="date"
              label="Available Since"
              value={room.availableSince?.split("T")[0]}
              onChange={(e) =>
                onRoomChange(
                  "availableSince",
                  new Date(e.target.value).toISOString()
                )
              }
              InputLabelProps={{ shrink: true }}
            />
          </Grid>
          <Grid size={{ xs: 12 }}>
            <FormControlLabel
              control={
                <Switch
                  checked={room.isAvailable}
                  onChange={(e) =>
                    onRoomChange("isAvailable", e.target.checked)
                  }
                  color="primary"
                />
              }
              label="Available for rent"
            />
          </Grid>
          <Grid size={{ xs: 12 }}>
            <Divider sx={{ my: 2 }} />
            <Box sx={{ display: "flex", alignItems: "center", mb: 2 }}>
              <Button
                variant="outlined"
                startIcon={<PhotoCameraIcon />}
                onClick={() => fileInputRef.current?.click()}
                sx={{ mr: 2 }}
              >
                Upload Room Images
              </Button>
              <Typography variant="body2" color="text.secondary">
                Add images specific to this room
              </Typography>
            </Box>

            <input
              ref={fileInputRef}
              type="file"
              multiple
              accept="image/*"
              onChange={onImageUpload}
              style={{ display: "none" }}
            />

            <ImageGallery
              images={room.images}
              onRemoveImage={onRemoveImage}
              onSetPrimaryImage={onSetPrimaryImage}
            />
          </Grid>
        </Grid>
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose}>Cancel</Button>
        <Button variant="contained" onClick={onSave}>
          {isEditing ? "Update Room" : "Add Room"}
        </Button>
      </DialogActions>
    </Dialog>
  );
};
