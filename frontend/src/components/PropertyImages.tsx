import React, { useState, useEffect } from "react";
import {
  Box,
  Card,
  CardMedia,
  CardActions,
  Button,
  IconButton,
  Typography,
  Grid,
  Alert,
  CircularProgress,
  Chip,
  Paper,
  styled,
} from "@mui/material";
import {
  CloudUpload,
  Delete,
  Star,
  StarBorder,
  PhotoCamera,
} from "@mui/icons-material";
import { API_BASE_URL } from "../types/types";

interface UploadAreaProps {
  isDragOver: boolean;
}

const UploadArea = styled(Paper)<UploadAreaProps>(({ theme, isDragOver }) => ({
  border: `2px dashed ${
    isDragOver ? theme.palette.primary.main : theme.palette.grey[300]
  }`,
  borderRadius: theme.shape.borderRadius,
  padding: theme.spacing(6),
  textAlign: "center",
  cursor: "pointer",
  transition: "all 0.3s ease",
  backgroundColor: isDragOver ? theme.palette.action.hover : "transparent",
  "&:hover": {
    borderColor: theme.palette.primary.main,
    backgroundColor: theme.palette.action.hover,
  },
}));

const ImageCard = styled(Card)(({ theme }) => ({
  position: "relative",
  "&:hover .image-overlay": {
    opacity: 1,
  },
}));

const ImageOverlay = styled(Box)(({ theme }) => ({
  position: "absolute",
  top: 0,
  left: 0,
  right: 0,
  bottom: 0,
  backgroundColor: "rgba(0, 0, 0, 0.5)",
  display: "flex",
  alignItems: "center",
  justifyContent: "center",
  opacity: 0,
  transition: "opacity 0.3s ease",
  borderRadius: theme.shape.borderRadius,
}));

const HiddenInput = styled("input")({
  display: "none",
});

export const PropertyImages = ({ propertyId }: { propertyId: string }) => {
  const [images, setImages] = useState<PropertyImage[]>([]);
  const [uploading, setUploading] = useState(false);
  const [error, setError] = useState("");
  const [dragOver, setDragOver] = useState(false);

  useEffect(() => {
    if (propertyId) {
      loadImages();
    }
  }, [propertyId]);

  const loadImages = async () => {
    try {
      const response = await fetch(
        `${API_BASE_URL}/api/properties/${propertyId}/images`
      );
      if (response.ok) {
        const data = await response.json();
        setImages(data);
      }
    } catch (error) {
      console.error("Error loading images:", error);
    }
  };

  interface HandleFileUploadFiles extends FileList {}

  interface NewImage {
    id: string;
    imageUrl: string;
    originalFileName: string;
    isPrimary: boolean;
    [key: string]: any;
  }

  const handleFileUpload = async (
    files: HandleFileUploadFiles | null
  ): Promise<void> => {
    if (!files || files.length === 0) return;

    setUploading(true);
    setError("");

    const formData = new FormData();
    Array.from(files).forEach((file: File) => formData.append("files", file));

    try {
      const response: Response = await fetch(
        `${API_BASE_URL}/api/properties/${propertyId}/images`,
        {
          method: "POST",
          body: formData,
        }
      );

      if (response.ok) {
        const newImages: NewImage[] = await response.json();
        setImages((prev: NewImage[]) => [...prev, ...newImages]);
      } else {
        const errorText: string = await response.text();
        setError(errorText || "Upload failed");
      }
    } catch (error) {
      setError("Upload failed");
    } finally {
      setUploading(false);
    }
  };

  interface PropertyImage {
    id: string;
    imageUrl: string;
    originalFileName: string;
    isPrimary: boolean;
    [key: string]: any; // For any additional fields
  }

  interface FileInputChangeEvent extends React.ChangeEvent<HTMLInputElement> {}

  const handleFileInputChange = (event: FileInputChangeEvent) => {
    handleFileUpload(event.target.files);
    event.target.value = ""; // Reset file input
  };

  interface DragEventHandler {
    (e: React.DragEvent<HTMLDivElement>): void;
  }

  const handleDragOver: DragEventHandler = (e) => {
    e.preventDefault();
    setDragOver(true);
  };

  interface DragLeaveEvent extends React.DragEvent<HTMLDivElement> {}

  const handleDragLeave: (e: DragLeaveEvent) => void = (e) => {
    e.preventDefault();
    setDragOver(false);
  };

  const handleDrop = (e: React.DragEvent<HTMLDivElement>) => {
    e.preventDefault();
    setDragOver(false);
    handleFileUpload(e.dataTransfer.files);
  };

  interface DeleteImageResponse {
    ok: boolean;
  }

  const deleteImage = async (imageId: string): Promise<void> => {
    try {
      const response: DeleteImageResponse & Response = await fetch(
        `${API_BASE_URL}/api/properties/images/${imageId}`,
        {
          method: "DELETE",
        }
      );

      if (response.ok) {
        setImages((prev: PropertyImage[]) =>
          prev.filter((img: PropertyImage) => img.id !== imageId)
        );
      } else {
        setError("Failed to delete image");
      }
    } catch (error) {
      setError("Failed to delete image");
    }
  };

  interface SetPrimaryResponse extends Response {}

  const setPrimary = async (imageId: string): Promise<void> => {
    try {
      const response: SetPrimaryResponse = await fetch(
        `${API_BASE_URL}/api/properties/images/${imageId}/primary`,
        {
          method: "PUT",
        }
      );

      if (response.ok) {
        setImages((prev: PropertyImage[]) =>
          prev.map((img: PropertyImage) => ({
            ...img,
            isPrimary: img.id === imageId,
          }))
        );
      } else {
        setError("Failed to set primary image");
      }
    } catch (error) {
      setError("Failed to set primary image");
    }
  };

  return (
    <Box sx={{ mt: 2 }}>
      {/* Upload Section */}
      <UploadArea
        elevation={0}
        isDragOver={dragOver}
        onDragOver={handleDragOver}
        onDragLeave={handleDragLeave}
        onDrop={handleDrop}
      >
        <Box
          sx={{
            display: "flex",
            flexDirection: "column",
            alignItems: "center",
          }}
        >
          {uploading ? (
            <CircularProgress size={48} sx={{ mb: 2 }} />
          ) : (
            <CloudUpload
              sx={{ fontSize: 48, color: "text.secondary", mb: 2 }}
            />
          )}

          <Typography variant="h6" gutterBottom>
            {uploading ? "Uploading..." : "Upload Property Images"}
          </Typography>

          <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
            Drag and drop images here or click to browse
          </Typography>

          <label htmlFor="image-upload">
            <HiddenInput
              id="image-upload"
              type="file"
              multiple
              accept="image/*"
              onChange={handleFileInputChange}
              disabled={uploading}
            />
            <Button
              variant="contained"
              component="span"
              startIcon={<PhotoCamera />}
              disabled={uploading}
            >
              Select Images
            </Button>
          </label>

          <Typography variant="caption" color="text.secondary" sx={{ mt: 1 }}>
            PNG, JPG, GIF up to 5MB each
          </Typography>
        </Box>
      </UploadArea>

      {/* Error Alert */}
      {error && (
        <Alert severity="error" onClose={() => setError("")} sx={{ mt: 2 }}>
          {error}
        </Alert>
      )}

      {/* Images Section */}
      {images.length > 0 && (
        <Box sx={{ mt: 4 }}>
          <Typography variant="h6" gutterBottom>
            Images ({images.length})
          </Typography>

          <Grid container spacing={2}>
            {images.map((image) => (
              <Grid size={{ xs: 12, sm: 4, md: 4 }} key={image.id}>
                <ImageCard>
                  <Box sx={{ position: "relative" }}>
                    <CardMedia
                      component="img"
                      height="200"
                      image={image.imageUrl}
                      alt={image.originalFileName}
                      sx={{ objectFit: "cover" }}
                    />

                    {/* Primary Badge */}
                    {image.isPrimary && (
                      <Chip
                        label="Primary"
                        color="warning"
                        size="small"
                        sx={{
                          position: "absolute",
                          top: 8,
                          left: 8,
                          fontWeight: "bold",
                        }}
                      />
                    )}

                    {/* Overlay with Actions */}
                    <ImageOverlay className="image-overlay">
                      <Box sx={{ display: "flex", gap: 1 }}>
                        <IconButton
                          onClick={() => setPrimary(image.id)}
                          sx={{
                            backgroundColor: image.isPrimary
                              ? "warning.main"
                              : "background.paper",
                            color: image.isPrimary
                              ? "warning.contrastText"
                              : "text.primary",
                            "&:hover": {
                              backgroundColor: image.isPrimary
                                ? "warning.dark"
                                : "grey.100",
                            },
                          }}
                          title={
                            image.isPrimary ? "Primary image" : "Set as primary"
                          }
                        >
                          {image.isPrimary ? <Star /> : <StarBorder />}
                        </IconButton>

                        <IconButton
                          onClick={() => deleteImage(image.id)}
                          sx={{
                            backgroundColor: "error.main",
                            color: "error.contrastText",
                            "&:hover": {
                              backgroundColor: "error.dark",
                            },
                          }}
                          title="Delete image"
                        >
                          <Delete />
                        </IconButton>
                      </Box>
                    </ImageOverlay>
                  </Box>
                </ImageCard>
              </Grid>
            ))}
          </Grid>
        </Box>
      )}

      {/* Empty State */}
      {images.length === 0 && !uploading && (
        <Box
          sx={{
            textAlign: "center",
            py: 8,
            color: "text.secondary",
          }}
        >
          <PhotoCamera sx={{ fontSize: 64, mb: 2, opacity: 0.5 }} />
          <Typography variant="h6" gutterBottom>
            No images uploaded yet
          </Typography>
          <Typography variant="body2">
            Upload some images to showcase your property
          </Typography>
        </Box>
      )}
    </Box>
  );
};
