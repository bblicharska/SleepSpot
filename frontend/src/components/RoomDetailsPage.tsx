import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import {
  Box,
  Typography,
  Divider,
  Grid,
  Paper,
  Avatar,
  Button,
} from "@mui/material";
import EmailIcon from "@mui/icons-material/Email";
import { fetchRoomDetails } from "../queries/fetchRoomDetails";
import { useAuth } from "./AuthContext";
import { LoadingComponent } from "./LoadingComponent";
import { ImageGallery } from "./ImageGallery";
import { DetailedDescription } from "./DetailedDescription";
import { ReviewSection } from "./ReviewSection";
import { PropertyMap } from "./PropertyMap";
import { EntitySummaryCard } from "./EntitySummaryCard";
import { OtherRoomsSection } from "./OtherRoomsSection";
import { PropertyImageDto, ReviewDto } from "../types/types";

export interface RoomSummaryDto {
  id: string;
  name: string;
  description: string;
  pricePerMonth: number;
  areaInSquareMeters: number;
  capacity: number;
  isAvailable: boolean;
  mainImage?: string;
}

interface OwnerInfoDto {
  firstName: string;
  lastName: string;
  email: string;
}

export interface RoomWithPropertyDetailsDto {
  // Room data
  id: string;
  name: string;
  description: string;
  detailedDescription?: string;
  pricePerMonth: number;
  areaInSquareMeters: number;
  capacity: number;
  isAvailable: boolean;
  images: PropertyImageDto[];
  createdAt: string;

  // Property data
  propertyId: string;
  propertyName: string;
  propertyAddress: string;
  ownerId?: string;

  // Other rooms in this property
  otherRoomsInProperty: RoomSummaryDto[];

  // These fields will be populated in Gateway
  owner?: OwnerInfoDto;
  reviews: ReviewDto[];
}

// Helper function to get initials
const getInitials = (firstName: string, lastName: string): string => {
  return `${firstName?.charAt(0) || ""}${
    lastName?.charAt(0) || ""
  }`.toUpperCase();
};

export const RoomDetailsPage = () => {
  const { user } = useAuth();
  const { id } = useParams();
  const [roomDetails, setRoomDetails] =
    useState<RoomWithPropertyDetailsDto | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const loadRoom = async () => {
      try {
        const data = await fetchRoomDetails(id!);
        setRoomDetails(data);
      } catch (err) {
        console.error("Failed to load room", err);
      } finally {
        setLoading(false);
      }
    };
    loadRoom();
  }, [id]);

  const handleReviewsUpdate = async () => {
    if (roomDetails?.id) {
      try {
        const updatedRoomDetails = await fetchRoomDetails(roomDetails.id);
        setRoomDetails(updatedRoomDetails);
      } catch (error) {
        console.error("Error updating reviews:", error);
      }
    }
  };

  const handleContactOwner = () => {
    if (roomDetails?.owner?.email) {
      window.location.href = `mailto:${roomDetails.owner.email}`;
    }
  };

  const handleBookRoom = () => {
    console.log("Booking room:", roomDetails?.id);
  };

  if (loading) return <LoadingComponent text="Loading room details..." />;
  if (!roomDetails) {
    return <Typography variant="h6">Room not found</Typography>;
  }

  // Calculate average rating with safety checks
  const averageRating =
    roomDetails.reviews && roomDetails.reviews.length > 0
      ? roomDetails.reviews.reduce((sum, review) => {
          const rating =
            typeof review.rating === "number" && !isNaN(review.rating)
              ? review.rating
              : 0;
          return sum + rating;
        }, 0) / roomDetails.reviews.length
      : 0;

  return (
    <Box sx={{ px: { xs: 2, md: 6 }, py: 4 }}>
      <Typography variant="h4" fontWeight={600} gutterBottom>
        {roomDetails.name}
      </Typography>

      {/* Property Information */}
      {roomDetails.propertyName && (
        <Typography variant="h6" color="text.secondary" gutterBottom>
          {roomDetails.propertyName}
        </Typography>
      )}

      {roomDetails.propertyAddress && (
        <Typography variant="body1" color="text.secondary" paragraph>
          📍 {roomDetails.propertyAddress}
        </Typography>
      )}

      {/* Reviews Section */}
      <ReviewSection
        reviews={roomDetails.reviews || []}
        entityId={roomDetails.id}
        entityType="room"
        onReviewsUpdate={handleReviewsUpdate}
      />

      <Typography variant="subtitle1" color="text.secondary" paragraph>
        {roomDetails.description}
      </Typography>

      {/* Image Gallery */}
      <ImageGallery
        images={roomDetails.images ?? []}
        title={roomDetails.name}
      />

      {/* Map Section */}
      {roomDetails.propertyAddress && (
        <Box sx={{ my: 4 }}>
          <PropertyMap address={roomDetails.propertyAddress} />
        </Box>
      )}

      <Divider sx={{ my: 3, borderColor: "rgba(142, 68, 173, 0.2)" }} />

      <Grid container spacing={4}>
        <Grid size={{ xs: 12, md: 8 }}>
          {/* Detailed Description */}
          {roomDetails.detailedDescription && (
            <DetailedDescription
              title="About This Room"
              description={roomDetails.detailedDescription}
            />
          )}

          {/* Other Rooms in Property */}
          {roomDetails.otherRoomsInProperty &&
            roomDetails.otherRoomsInProperty.length > 0 && (
              <OtherRoomsSection
                rooms={roomDetails.otherRoomsInProperty}
                propertyName={roomDetails.propertyName}
                currentRoomId={roomDetails.id}
              />
            )}
        </Grid>

        <Grid size={{ xs: 12, md: 4 }}>
          {/* Room Summary Sidebar */}
          <EntitySummaryCard
            entityType="room"
            name={roomDetails.name}
            pricePerMonth={roomDetails.pricePerMonth}
            areaInSquareMeters={roomDetails.areaInSquareMeters}
            averageRating={averageRating}
            reviewsCount={roomDetails.reviews?.length || 0}
            createdAt={roomDetails.createdAt}
            capacity={roomDetails.capacity}
            isAvailable={roomDetails.isAvailable}
            onBookRoom={handleBookRoom}
          />

          {/* Owner Information Card */}
          {roomDetails.owner && (
            <Paper
              elevation={3}
              sx={{
                p: 4,
                borderRadius: 3,
                background:
                  "linear-gradient(135deg, rgba(142, 68, 173, 0.08) 0%, rgba(175, 122, 197, 0.08) 100%)",
                border: "1px solid rgba(142, 68, 173, 0.2)",
                mt: 3,
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
                    roomDetails.owner.firstName,
                    roomDetails.owner.lastName
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
                    {roomDetails.owner.firstName} {roomDetails.owner.lastName}
                  </Typography>
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
                      {roomDetails.owner.email}
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
