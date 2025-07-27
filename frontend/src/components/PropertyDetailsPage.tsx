import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import {
  Box,
  Typography,
  Paper,
  Divider,
  Grid,
  Avatar,
  Button,
} from "@mui/material";
import { PropertyMap } from "./PropertyMap";
import EmailIcon from "@mui/icons-material/Email";
import { Property } from "../types/types";
import { fetchPropertyDetails } from "../queries/fetchPropertyDetails";
import { useAuth } from "./AuthContext";
import { LoadingComponent } from "./LoadingComponent";
import { ImageGallery } from "./ImageGallery";
import { DetailedDescription } from "./DetailedDescription";
import { ReviewSection } from "./ReviewSection";
import { EntitySummaryCard } from "./EntitySummaryCard";

export const PropertyDetailsPage = () => {
  const { id } = useParams();
  const [property, setProperty] = useState<Property | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const loadProperty = async () => {
      try {
        const data = await fetchPropertyDetails(id!);
        setProperty(data);
      } catch (err) {
        console.error("Failed to load property", err);
      } finally {
        setLoading(false);
      }
    };
    loadProperty();
  }, [id]);

  const handleReviewsUpdate = async () => {
    if (property) {
      try {
        const updatedProperty = await fetchPropertyDetails(property.id);
        setProperty(updatedProperty);
      } catch (error) {
        console.error("Error updating reviews:", error);
      }
    }
  };

  if (loading) return <LoadingComponent text="Loading property details..." />;
  if (!property)
    return <Typography variant="h6">Property not found</Typography>;

  // Calculate average rating with safety checks
  const averageRating =
    property.reviews && property.reviews.length > 0
      ? property.reviews.reduce((sum, review) => {
          const rating =
            typeof review.rating === "number" && !isNaN(review.rating)
              ? review.rating
              : 0;
          return sum + rating;
        }, 0) / property.reviews.length
      : 0;

  const getInitials = (firstName: string, lastName: string) => {
    return `${firstName.charAt(0)}${lastName.charAt(0)}`.toUpperCase();
  };

  const handleContactOwner = () => {
    if (property?.owner?.email) {
      window.location.href = `mailto:${property.owner.email}?subject=Inquiry about ${property.name}`;
    }
  };

  return (
    <Box sx={{ px: { xs: 2, md: 6 }, py: 4 }}>
      <Typography variant="h4" fontWeight={600} gutterBottom>
        {property.name}
      </Typography>

      {property.address && (
        <Typography variant="body1" color="text.secondary" paragraph>
          📍 {property.address}
        </Typography>
      )}
      {/* Reviews Section */}
      <ReviewSection
        reviews={property.reviews || []}
        entityId={property.id}
        entityType="property"
        onReviewsUpdate={handleReviewsUpdate}
      />

      <Typography variant="subtitle1" color="text.secondary" paragraph>
        {property.description}
      </Typography>

      {/* Image Gallery */}
      <ImageGallery images={property.images} title={property.name} />

      {/* Map Section */}
      <Box sx={{ my: 4 }}>
        <PropertyMap address={property.address} />
      </Box>

      <Divider sx={{ my: 3, borderColor: "rgba(142, 68, 173, 0.2)" }} />

      <Grid container spacing={4}>
        <Grid size={{ xs: 12, md: 8 }}>
          {/* Detailed Description */}
          {property.detailedDescription && (
            <DetailedDescription
              title="About This Property"
              description={property.detailedDescription}
            />
          )}
        </Grid>

        <Grid size={{ xs: 12, md: 4 }}>
          {/* Property Summary Sidebar using the common component */}
          <EntitySummaryCard
            entityType="property"
            name={property.name}
            pricePerMonth={property.pricePerMonth}
            areaInSquareMeters={property.areaInSquareMeters}
            averageRating={averageRating}
            reviewsCount={property.reviews?.length || 0}
            isEntirePlaceRentable={property.isEntirePlaceRentable}
            isAvailable={property.isAvailable}
            createdAt={property.createdAt}
          />

          {/* Owner Information Card */}
          {property.owner && (
            <Paper
              elevation={3}
              sx={{
                p: 4,
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
                  background:
                    "linear-gradient(45deg, #8E44AD 30%, #AF7AC5 90%)",
                  backgroundClip: "text",
                  WebkitBackgroundClip: "text",
                  WebkitTextFillColor: "transparent",
                }}
              >
                Property Owner
              </Typography>

              <Box sx={{ display: "flex", alignItems: "center", mb: 3 }}>
                <Avatar
                  sx={{
                    mr: 3,
                    background:
                      "linear-gradient(45deg, #8E44AD 30%, #AF7AC5 90%)",
                    width: 56,
                    height: 56,
                    fontSize: "1.5rem",
                    fontWeight: 600,
                  }}
                >
                  {getInitials(
                    property.owner.firstName,
                    property.owner.lastName
                  )}
                </Avatar>
                <Box>
                  <Typography
                    variant="h6"
                    sx={{
                      fontWeight: 600,
                      color: "text.primary",
                      mb: 0.5,
                    }}
                  >
                    {property.owner.firstName} {property.owner.lastName}
                  </Typography>
                  {property.owner.role && (
                    <Typography
                      variant="caption"
                      sx={{
                        color: "#8E44AD",
                        backgroundColor: "rgba(142, 68, 173, 0.1)",
                        px: 1,
                        py: 0.2,
                        borderRadius: 1,
                        fontSize: "0.75rem",
                        fontWeight: 600,
                        textTransform: "uppercase",
                        mb: 0.5,
                        display: "inline-block",
                      }}
                    >
                      {property.owner.role}
                    </Typography>
                  )}
                  <Box sx={{ display: "flex", alignItems: "center" }}>
                    <EmailIcon
                      sx={{
                        mr: 1,
                        fontSize: "1.1rem",
                        color: "#8E44AD",
                      }}
                    />
                    <Typography
                      variant="body2"
                      sx={{
                        color: "#8E44AD",
                        cursor: "pointer",
                        textDecoration: "underline",
                        "&:hover": { color: "#6A1B9A" },
                      }}
                      onClick={handleContactOwner}
                    >
                      {property.owner.email}
                    </Typography>
                  </Box>
                </Box>
              </Box>

              <Divider sx={{ my: 2, borderColor: "rgba(142, 68, 173, 0.3)" }} />

              <Button
                variant="outlined"
                fullWidth
                startIcon={<EmailIcon />}
                onClick={handleContactOwner}
                sx={{
                  borderColor: "#8E44AD",
                  color: "#8E44AD",
                  fontWeight: 600,
                  py: 1.2,
                  borderRadius: 2,
                  textTransform: "none",
                  "&:hover": {
                    borderColor: "#6A1B9A",
                    backgroundColor: "rgba(142, 68, 173, 0.1)",
                    color: "#6A1B9A",
                  },
                  transition: "all 0.3s ease",
                }}
              >
                Send Email
              </Button>
            </Paper>
          )}
        </Grid>
      </Grid>
    </Box>
  );
};
