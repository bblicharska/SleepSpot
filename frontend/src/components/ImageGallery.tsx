import React, { useState } from "react";
import { Grid, Box, Dialog, DialogContent, IconButton } from "@mui/material";
import CloseIcon from "@mui/icons-material/Close";
import { API_BASE_URL, PropertyImageDto } from "../types/types";

interface ImageGalleryProps {
  images: PropertyImageDto[];
  title?: string;
}

export const ImageGallery = ({ images, title }: ImageGalleryProps) => {
  const [selectedImageIndex, setSelectedImageIndex] = useState<number | null>(
    null
  );

  const sortedImages = [...images].sort(
    (a, b) => a.displayOrder - b.displayOrder
  );

  const getImageUrl = (url?: string) => {
    if (!url) return "/placeholder-image.jpg";
    if (url.startsWith("http")) return url;
    if (url.startsWith("/")) return `${API_BASE_URL}${url}`;
    return `${API_BASE_URL}/uploads/properties/${url}`;
  };

  if (sortedImages.length === 0) return null;

  return (
    <>
      <Grid container spacing={2} sx={{ my: 3 }}>
        {sortedImages.map((img, index) => (
          <Grid size={{ xs: 6, sm: 4, md: 3 }} key={img.id}>
            <Box
              component="img"
              src={getImageUrl(img.imageUrl)}
              alt={img.originalFileName}
              sx={{
                width: "100%",
                height: 180,
                objectFit: "cover",
                borderRadius: 2,
                cursor: "pointer",
                boxShadow: "0 4px 20px rgba(142, 68, 173, 0.2)",
                transition: "all 0.3s ease",
                "&:hover": {
                  transform: "scale(1.02)",
                  boxShadow: "0 8px 30px rgba(142, 68, 173, 0.3)",
                },
              }}
              onClick={() => setSelectedImageIndex(index)}
            />
          </Grid>
        ))}
      </Grid>

      <Dialog
        open={selectedImageIndex !== null}
        onClose={() => setSelectedImageIndex(null)}
        fullWidth
        maxWidth="md"
      >
        <DialogContent sx={{ position: "relative", p: 0 }}>
          <IconButton
            onClick={() => setSelectedImageIndex(null)}
            sx={{
              position: "absolute",
              top: 8,
              right: 8,
              zIndex: 1,
              background: "linear-gradient(45deg, #8E44AD 30%, #AF7AC5 90%)",
              color: "white",
              "&:hover": {
                background: "linear-gradient(45deg, #6A1B9A 30%, #8E44AD 90%)",
              },
            }}
          >
            <CloseIcon />
          </IconButton>
          {selectedImageIndex !== null && (
            <Box
              component="img"
              src={getImageUrl(sortedImages[selectedImageIndex].imageUrl)}
              alt={title || ""}
              sx={{
                width: "100%",
                maxHeight: "80vh",
                objectFit: "contain",
              }}
            />
          )}
        </DialogContent>
      </Dialog>
    </>
  );
};
