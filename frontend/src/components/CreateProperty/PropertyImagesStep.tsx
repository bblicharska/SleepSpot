import React from "react";
import { Box, Button, Typography, Alert } from "@mui/material";
import { PhotoCamera as PhotoCameraIcon } from "@mui/icons-material";
import { ImageGallery } from "./ImageGallery";
import { PropertyImageDto } from "../../types/types";

interface PropertyImagesStepProps {
  images: PropertyImageDto[];
  errors: Record<string, string>;
  fileInputRef: React.RefObject<HTMLInputElement | null>;
  onImageUpload: (event: React.ChangeEvent<HTMLInputElement>) => void;
  onRemoveImage: (imageId: string) => void;
  onSetPrimaryImage: (imageId: string) => void;
}

export const PropertyImagesStep: React.FC<PropertyImagesStepProps> = ({
  images,
  errors,
  fileInputRef,
  onImageUpload,
  onRemoveImage,
  onSetPrimaryImage,
}) => {
  return (
    <Box sx={{ mt: 3 }}>
      <Box sx={{ display: "flex", alignItems: "center", mb: 3 }}>
        <Button
          variant="outlined"
          startIcon={<PhotoCameraIcon />}
          onClick={() => fileInputRef.current?.click()}
          sx={{ mr: 2 }}
        >
          Upload Images
        </Button>
        <Typography variant="body2" color="text.secondary">
          Upload high-quality images of your property
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
      {errors.images && (
        <Alert severity="error" sx={{ mb: 2 }}>
          {errors.images}
        </Alert>
      )}
      <ImageGallery
        images={images}
        onRemoveImage={onRemoveImage}
        onSetPrimaryImage={onSetPrimaryImage}
      />
    </Box>
  );
};
