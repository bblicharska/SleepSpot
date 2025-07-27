import React from "react";
import {
  Grid,
  Card,
  CardMedia,
  CardContent,
  Typography,
  IconButton,
  Chip,
  Box,
} from "@mui/material";
import { Delete as DeleteIcon } from "@mui/icons-material";
import { PropertyImageDto } from "../../types/types";

interface ImageGalleryProps {
  images: PropertyImageDto[];
  onRemoveImage: (imageId: string) => void;
  onSetPrimaryImage: (imageId: string) => void;
}

export const ImageGallery: React.FC<ImageGalleryProps> = ({
  images,
  onRemoveImage,
  onSetPrimaryImage,
}) => {
  return (
    <Grid container spacing={2}>
      {images.map((image) => (
        <Grid size={{ xs: 12, sm: 6, md: 4 }} key={image.id}>
          <Card>
            <CardMedia
              component="img"
              height="200"
              image={image.imageUrl}
              alt={image.originalFileName}
            />
            <CardContent>
              <Box
                sx={{
                  display: "flex",
                  justifyContent: "space-between",
                  alignItems: "center",
                }}
              >
                <Typography variant="caption" noWrap>
                  {image.originalFileName}
                </Typography>
                <Box>
                  <IconButton
                    size="small"
                    onClick={() => onSetPrimaryImage(image.id)}
                    color={image.isPrimary ? "primary" : "default"}
                  >
                    {image.isPrimary ? (
                      <Chip label="Primary" size="small" color="primary" />
                    ) : (
                      <Chip
                        label="Set Primary"
                        size="small"
                        variant="outlined"
                      />
                    )}
                  </IconButton>
                  <IconButton
                    size="small"
                    onClick={() => onRemoveImage(image.id)}
                    color="error"
                  >
                    <DeleteIcon />
                  </IconButton>
                </Box>
              </Box>
            </CardContent>
          </Card>
        </Grid>
      ))}
    </Grid>
  );
};
