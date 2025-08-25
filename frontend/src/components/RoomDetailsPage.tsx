import { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import {
  Box,
  Typography,
  Divider,
  Grid,
  Paper,
  Avatar,
  Button,
  IconButton,
  Chip,
} from "@mui/material";
import EmailIcon from "@mui/icons-material/Email";
import DeleteIcon from "@mui/icons-material/Delete";
import SendIcon from "@mui/icons-material/Send";
import CalendarTodayIcon from "@mui/icons-material/CalendarToday";
import { fetchRoomDetails } from "../queries/fetchRoomDetails";
import { LoadingComponent } from "./LoadingComponent";
import { ImageGallery } from "./ImageGallery";
import { DetailedDescription } from "./DetailedDescription";
import { ReviewSection } from "./ReviewSection";
import { PropertyMap } from "./PropertyMap";
import { EntitySummaryCard } from "./EntitySummaryCard";
import { OtherRoomsSection } from "./OtherRoomsSection";
import { DeleteConfirmationDialog } from "./DeleteConfirmationDialog";
import { useAuth } from "./AuthContext";
import { PropertyImageDto, ReviewDto } from "../types/types";
import { deleteRoom } from "../queries/deleteRoom";
import { RentalRequestModal } from "./RentalRequestModal"; // <-- added

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
  reviews?: ReviewDto[];
}

export interface RoomWithPropertyDetailsDto {
  id: string;
  name: string;
  description: string;
  detailedDescription?: string;
  pricePerMonth: number;
  areaInSquareMeters: number;
  capacity: number;
  isAvailable: boolean;
  availableSince?: string;
  images: PropertyImageDto[];
  createdAt: string;
  propertyId: string;
  propertyName: string;
  propertyAddress: string;
  ownerId?: string;
  otherRoomsInProperty: RoomSummaryDto[];
  owner?: OwnerInfoDto;
  reviews: ReviewDto[];
}

const getInitials = (firstName: string, lastName: string): string => {
  return `${firstName?.charAt(0) || ""}${
    lastName?.charAt(0) || ""
  }`.toUpperCase();
};

export const RoomDetailsPage = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const { user } = useAuth();
  const [roomDetails, setRoomDetails] =
    useState<RoomWithPropertyDetailsDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
  const [deleting, setDeleting] = useState(false);

  const [rentalModalOpen, setRentalModalOpen] = useState(false);

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
      const subject = encodeURIComponent(`Inquiry about ${roomDetails.name}`);
      window.location.href = `mailto:${roomDetails.owner.email}?subject=${subject}`;
    }
  };

  const handleDeleteRoom = async () => {
    if (!roomDetails) return;

    setDeleting(true);
    try {
      await deleteRoom(roomDetails.id);
      navigate("/rooms");
    } catch (error) {
      console.error("Error deleting room:", error);
      alert(
        error instanceof Error
          ? error.message
          : "Failed to delete room. Please try again."
      );
    } finally {
      setDeleting(false);
      setDeleteDialogOpen(false);
    }
  };

  const formatAvailableSince = (dateString: string) => {
    const date = new Date(dateString);
    const today = new Date();

    today.setHours(0, 0, 0, 0);
    const availableDate = new Date(date);
    availableDate.setHours(0, 0, 0, 0);

    const diffTime = availableDate.getTime() - today.getTime();
    const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));

    const formattedDate = date.toLocaleDateString("en-US", {
      year: "numeric",
      month: "short",
      day: "numeric",
    });

    if (diffDays < 0) {
      return { text: `Available since ${formattedDate}`, status: "available" };
    } else if (diffDays === 0) {
      return { text: "Available from today", status: "available" };
    } else {
      return { text: `Available from ${formattedDate}`, status: "future" };
    }
  };

  const canDelete = roomDetails && roomDetails.ownerId === user?.userId;

  if (loading) return <LoadingComponent text="Loading room details..." />;

  if (!roomDetails) {
    return <Typography variant="h6">Room not found</Typography>;
  }

  const roomAverageRating =
    roomDetails.reviews && roomDetails.reviews.length > 0
      ? roomDetails.reviews.reduce((sum, review) => {
          const rating =
            typeof review.rating === "number" && !isNaN(review.rating)
              ? review.rating
              : 0;
          return sum + rating;
        }, 0) / roomDetails.reviews.length
      : 0;

  const ownerAverageRating =
    roomDetails.owner?.reviews && roomDetails.owner.reviews.length > 0
      ? roomDetails.owner.reviews.reduce((sum, review) => {
          const rating =
            typeof review.rating === "number" && !isNaN(review.rating)
              ? review.rating
              : 0;
          return sum + rating;
        }, 0) / roomDetails.owner.reviews.length
      : 0;

  const availabilityInfo = roomDetails.availableSince
    ? formatAvailableSince(roomDetails.availableSince)
    : null;

  return (
    <Box sx={{ px: { xs: 2, md: 6 }, py: 4, position: "relative" }}>
      <Box
        display="flex"
        justifyContent="space-between"
        alignItems="flex-start"
        mb={2}
      >
        <Typography variant="h4" fontWeight={600} gutterBottom>
          {roomDetails.name}
        </Typography>
        {canDelete && (
          <IconButton
            onClick={() => setDeleteDialogOpen(true)}
            sx={{
              color: "rgba(255, 0, 0, 0.8)",
              backgroundColor: "rgba(255,255,255,0.8)",
              border: "1px solid rgba(255, 0, 0, 0.3)",
              "&:hover": {
                backgroundColor: "rgba(255,0,0,0.15)",
                color: "red",
                borderColor: "red",
              },
            }}
          >
            <DeleteIcon />
          </IconButton>
        )}
        {roomDetails.ownerId !== user?.userId && (
          <Button
            variant="contained"
            startIcon={<SendIcon />}
            sx={{
              flexShrink: 0,
              backgroundColor: "#8B5CF6",
              color: "white",
              "&:hover": {
                backgroundColor: "#7C3AED",
              },
              "&:active": {
                backgroundColor: "#6D28D9",
              },
            }}
            onClick={() => setRentalModalOpen(true)}
          >
            Send Rental Request
          </Button>
        )}
      </Box>
      <Box sx={{ mb: 3 }}>
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
        {availabilityInfo && (
          <Box sx={{ mb: 2 }}>
            <Chip
              icon={<CalendarTodayIcon />}
              label={availabilityInfo.text}
              variant="outlined"
              sx={{
                color:
                  availabilityInfo.status === "available"
                    ? "#2E7D32"
                    : "#ED6C02",
                borderColor:
                  availabilityInfo.status === "available"
                    ? "#2E7D32"
                    : "#ED6C02",
                backgroundColor:
                  availabilityInfo.status === "available"
                    ? "rgba(46, 125, 50, 0.08)"
                    : "rgba(237, 108, 2, 0.08)",
                fontWeight: 600,
                "& .MuiChip-icon": {
                  color:
                    availabilityInfo.status === "available"
                      ? "#2E7D32"
                      : "#ED6C02",
                },
              }}
            />
          </Box>
        )}
        <Typography variant="subtitle1" color="text.secondary" paragraph>
          {roomDetails.description}
        </Typography>
      </Box>
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
          Room Reviews
        </Typography>
        <ReviewSection
          reviews={roomDetails.reviews || []}
          entityId={roomDetails.id}
          entityType="room"
          onReviewsUpdate={handleReviewsUpdate}
        />
      </Box>
      <ImageGallery
        images={roomDetails.images ?? []}
        title={roomDetails.name}
      />
      {roomDetails.propertyAddress && (
        <Box sx={{ my: 4 }}>
          <PropertyMap address={roomDetails.propertyAddress} />
        </Box>
      )}
      <Divider sx={{ my: 3, borderColor: "rgba(142, 68, 173, 0.2)" }} />
      <Grid container spacing={4}>
        <Grid size={{ xs: 12, md: 8 }}>
          {roomDetails.detailedDescription && (
            <DetailedDescription
              title="About This Room"
              description={roomDetails.detailedDescription}
            />
          )}
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
          <EntitySummaryCard
            entityType="room"
            pricePerMonth={roomDetails.pricePerMonth}
            areaInSquareMeters={roomDetails.areaInSquareMeters}
            averageRating={roomAverageRating}
            reviewsCount={roomDetails.reviews?.length || 0}
            createdAt={roomDetails.createdAt}
            capacity={roomDetails.capacity}
            isAvailable={roomDetails.isAvailable}
            availableSince={roomDetails.availableSince}
          />
        </Grid>
      </Grid>
      {roomDetails.owner && (
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
                {getInitials(
                  roomDetails.owner.firstName,
                  roomDetails.owner.lastName
                )}
              </Avatar>
              <Box sx={{ flexGrow: 1 }}>
                <Typography
                  variant="h6"
                  sx={{ fontWeight: 600, color: "text.primary", mb: 0.5 }}
                >
                  {roomDetails.owner.firstName} {roomDetails.owner.lastName}
                </Typography>
                {roomDetails.owner.reviews &&
                  roomDetails.owner.reviews.length > 0 && (
                    <Box sx={{ display: "flex", alignItems: "center", mb: 1 }}>
                      <Typography
                        variant="body2"
                        sx={{ color: "#8E44AD", fontWeight: 600, mr: 1 }}
                      >
                        Owner Rating:
                      </Typography>
                      <Typography
                        variant="body2"
                        sx={{ color: "#8E44AD", fontWeight: 600 }}
                      >
                        {ownerAverageRating.toFixed(1)} (
                        {roomDetails.owner.reviews.length} review
                        {roomDetails.owner.reviews.length !== 1 ? "s" : ""})
                      </Typography>
                    </Box>
                  )}
                <Box sx={{ display: "flex", alignItems: "center" }}>
                  <EmailIcon
                    sx={{ mr: 1, fontSize: "1.1rem", color: "#8E44AD" }}
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
              reviews={roomDetails.owner.reviews || []}
              entityId={roomDetails.ownerId || ""}
              entityType="owner"
              onReviewsUpdate={handleReviewsUpdate}
            />
          </Paper>
        </Box>
      )}
      <DeleteConfirmationDialog
        open={deleteDialogOpen}
        onClose={() => setDeleteDialogOpen(false)}
        onConfirm={handleDeleteRoom}
        title="Delete Room"
        message={`Are you sure you want to delete "${roomDetails.name}"? This action cannot be undone.`}
        variant="gradient"
        loading={deleting}
      />
      <RentalRequestModal
        open={rentalModalOpen}
        onClose={() => setRentalModalOpen(false)}
        propertyId={roomDetails.propertyId}
        roomId={roomDetails.id}
        currentUserId={user?.userId ?? null}
      />
    </Box>
  );
};
