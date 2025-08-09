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

  const propertyAverageRating =
    property.reviews && property.reviews.length > 0
      ? property.reviews.reduce((sum, review) => {
          const rating =
            typeof review.rating === "number" && !isNaN(review.rating)
              ? review.rating
              : 0;
          return sum + rating;
        }, 0) / property.reviews.length
      : 0;

  const ownerAverageRating =
    property.owner?.reviews && property.owner.reviews.length > 0
      ? property.owner.reviews.reduce((sum, review) => {
          const rating =
            typeof review.rating === "number" && !isNaN(review.rating)
              ? review.rating
              : 0;
          return sum + rating;
        }, 0) / property.owner.reviews.length
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
          {property.address}
        </Typography>
      )}
      <Typography variant="subtitle1" color="text.secondary" paragraph>
        {property.description}
      </Typography>
      <Box sx={{ mb: 4 }}>
        <Typography
          variant="h5"
          sx={{
            mb: 3,
            fontWeight: 600,
            background: "linear-gradient(45deg, #8E44AD 30%, #AF7AC5 90%)",
            backgroundClip: "text",
            WebkitBackgroundClip: "text",
            WebkitTextFillColor: "transparent",
          }}
        >
          Property Reviews
        </Typography>
        <ReviewSection
          reviews={property.reviews || []}
          entityId={property.id}
          entityType="property"
          onReviewsUpdate={handleReviewsUpdate}
        />
      </Box>
      <ImageGallery images={property.images} title={property.name} />
      <Box sx={{ my: 4 }}>
        <PropertyMap address={property.address} />
      </Box>
      <Divider sx={{ my: 3, borderColor: "rgba(142, 68, 173, 0.2)" }} />
      <Grid container spacing={4}>
        <Grid size={{ xs: 12, md: 8 }}>
          {property.detailedDescription && (
            <DetailedDescription
              title="About This Property"
              description={property.detailedDescription}
            />
          )}
        </Grid>
        <Grid size={{ xs: 12, md: 4 }}>
          <EntitySummaryCard
            entityType="property"
            name={property.name}
            pricePerMonth={property.pricePerMonth}
            areaInSquareMeters={property.areaInSquareMeters}
            averageRating={propertyAverageRating}
            reviewsCount={property.reviews?.length || 0}
            isEntirePlaceRentable={property.isEntirePlaceRentable}
            isAvailable={property.isAvailable}
            createdAt={property.createdAt}
          />
        </Grid>
      </Grid>
      {property.owner && (
        <Box sx={{ mb: 4 }}>
          <Paper
            elevation={3}
            sx={{
              p: 4,
              borderRadius: 3,
              background:
                "linear-gradient(135deg, rgba(142, 68, 173, 0.08) 0%, rgba(175, 122, 197, 0.08) 100%)",
              border: "1px solid rgba(142, 68, 173, 0.2)",
              mb: 3,
            }}
          >
            <Typography
              variant="h5"
              sx={{
                mb: 3,
                fontWeight: 600,
                background: "linear-gradient(45deg, #8E44AD 30%, #AF7AC5 90%)",
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
                {getInitials(property.owner.firstName, property.owner.lastName)}
              </Avatar>
              <Box sx={{ flexGrow: 1 }}>
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
                <Box sx={{ display: "flex", alignItems: "center", mt: 1 }}>
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
              <Button
                variant="outlined"
                startIcon={<EmailIcon />}
                onClick={handleContactOwner}
                sx={{
                  borderColor: "#8E44AD",
                  color: "#8E44AD",
                  fontWeight: 600,
                  textTransform: "none",
                  "&:hover": {
                    borderColor: "#6A1B9A",
                    backgroundColor: "rgba(142, 68, 173, 0.1)",
                    color: "#6A1B9A",
                  },
                  transition: "all 0.3s ease",
                }}
              >
                Contact Owner
              </Button>
            </Box>
            <Divider sx={{ mb: 2 }} />
            <ReviewSection
              reviews={property.owner.reviews || []}
              entityId={property.owner.id || property.ownerId}
              entityType="owner"
              onReviewsUpdate={handleReviewsUpdate}
            />
          </Paper>
        </Box>
      )}
    </Box>
  );
};
