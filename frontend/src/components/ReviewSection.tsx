import React, { useState } from "react";
import {
  Box,
  Typography,
  Button,
  Rating,
  Dialog,
  DialogContent,
  DialogTitle,
  DialogActions,
  TextField,
  CircularProgress,
  Grid,
  Card,
  CardContent,
  Avatar,
  IconButton,
  Tooltip,
  Alert,
  Snackbar,
} from "@mui/material";
import RateReviewIcon from "@mui/icons-material/RateReview";
import CloseIcon from "@mui/icons-material/Close";
import DeleteIcon from "@mui/icons-material/Delete";
import EditIcon from "@mui/icons-material/Edit";
import PersonIcon from "@mui/icons-material/Person";
import { useAuth } from "./AuthContext";
import { postReview } from "../queries/postReview";
import { deleteReview } from "../queries/deleteReview";
import { updateReview } from "../queries/updateReview";
import { ReviewDto, ReviewFormData } from "../types/types";

interface ReviewSectionProps {
  reviews: ReviewDto[];
  entityId: string;
  entityType: "property" | "room" | "owner";
  onReviewsUpdate: () => void;
}

export const ReviewSection = ({
  reviews,
  entityId,
  entityType,
  onReviewsUpdate,
}: ReviewSectionProps) => {
  const { user, token } = useAuth();
  const [localReviews, setLocalReviews] = useState<ReviewDto[]>(reviews || []);
  const [reviewsModalOpen, setReviewsModalOpen] = useState(false);
  const [addReviewModalOpen, setAddReviewModalOpen] = useState(false);
  const [editReviewModalOpen, setEditReviewModalOpen] = useState(false);
  const [reviewForm, setReviewForm] = useState<ReviewFormData>({
    rating: 0,
    comment: "",
  });
  const [editingReview, setEditingReview] = useState<ReviewDto | null>(null);
  const [submittingReview, setSubmittingReview] = useState(false);
  const [updatingReview, setUpdatingReview] = useState(false);
  const [deletingReviewId, setDeletingReviewId] = useState<string | null>(null);
  const [snackbar, setSnackbar] = useState({
    open: false,
    message: "",
    severity: "success" as "success" | "error",
  });

  // Calculate average rating from local reviews
  const averageRating = React.useMemo(() => {
    if (!localReviews || localReviews.length === 0) return 0;
    const total = localReviews.reduce((sum, review) => sum + review.rating, 0);
    return total / localReviews.length;
  }, [localReviews]);

  // Update local reviews when props change
  React.useEffect(() => {
    setLocalReviews(reviews || []);
  }, [reviews]);

  // Get entity display name for UI
  const getEntityDisplayName = () => {
    switch (entityType) {
      case "property":
        return "property";
      case "room":
        return "room";
      case "owner":
        return "owner";
      default:
        return "entity";
    }
  };

  // Get review placeholder text
  const getReviewPlaceholder = () => {
    switch (entityType) {
      case "property":
        return "Share your experience with this property...";
      case "room":
        return "Share your experience with this room...";
      case "owner":
        return "Share your experience with this property owner...";
      default:
        return "Share your experience...";
    }
  };

  const handleSubmitReview = async () => {
    if (!user || !token) {
      setSnackbar({
        open: true,
        message: "Please log in to submit review",
        severity: "error",
      });
      return;
    }

    if (reviewForm.rating === 0 || !reviewForm.comment.trim()) {
      setSnackbar({
        open: true,
        message: "Please provide both a rating and a comment",
        severity: "error",
      });
      return;
    }

    setSubmittingReview(true);

    try {
      // Map entityType to the correct field name for the API
      const reviewData: any = {
        rating: reviewForm.rating,
        comment: reviewForm.comment.trim(),
        reviewerId: user.userId,
      };

      // Add the correct ID field based on entityType
      switch (entityType) {
        case "property":
          reviewData.propertyId = entityId;
          break;
        case "room":
          reviewData.roomId = entityId;
          break;
        case "owner":
          reviewData.ownerId = entityId;
          break;
        default:
          throw new Error("Invalid entity type");
      }

      const newReview = await postReview(reviewData, token);

      // Immediately update local state for instant UI feedback
      setLocalReviews((prevReviews) => [newReview, ...prevReviews]);

      // Close modal first to prevent UI glitch
      setReviewForm({ rating: 0, comment: "" });
      setAddReviewModalOpen(false);

      // Call the callback to refresh data from parent (in background)
      onReviewsUpdate();

      setSnackbar({
        open: true,
        message: "Review submitted successfully!",
        severity: "success",
      });
    } catch (error) {
      console.error("Error submitting review:", error);
      setSnackbar({
        open: true,
        message:
          error instanceof Error ? error.message : "Failed to submit review",
        severity: "error",
      });
    } finally {
      setSubmittingReview(false);
    }
  };

  const handleUpdateReview = async () => {
    if (!user || !token || !editingReview) {
      setSnackbar({
        open: true,
        message: "Please log in to update review",
        severity: "error",
      });
      return;
    }

    if (reviewForm.rating === 0 || !reviewForm.comment.trim()) {
      setSnackbar({
        open: true,
        message: "Please provide both a rating and a comment",
        severity: "error",
      });
      return;
    }

    setUpdatingReview(true);

    try {
      const updateData = {
        id: editingReview.id,
        rating: reviewForm.rating,
        comment: reviewForm.comment.trim(),
      };

      // Make the API call
      await updateReview(editingReview.id, updateData, token);

      // Immediately update local state with the new data
      setLocalReviews((prevReviews) =>
        prevReviews.map((review) =>
          review.id === editingReview.id
            ? {
                ...review,
                rating: reviewForm.rating,
                comment: reviewForm.comment.trim(),
              }
            : review
        )
      );

      // Close modal and reset form immediately after successful update
      setEditReviewModalOpen(false);
      setReviewForm({ rating: 0, comment: "" });
      setEditingReview(null);

      // Show success message
      setSnackbar({
        open: true,
        message: "Review updated successfully!",
        severity: "success",
      });

      // Refresh data from parent in the background (optional)
      try {
        await onReviewsUpdate();
      } catch (refreshError) {
        console.warn("Failed to refresh reviews from parent:", refreshError);
        // Don't show error to user since the local update was successful
      }
    } catch (error) {
      console.error("Error updating review:", error);

      setSnackbar({
        open: true,
        message:
          error instanceof Error ? error.message : "Failed to update review",
        severity: "error",
      });
    } finally {
      setUpdatingReview(false);
    }
  };

  const handleEditReview = (review: ReviewDto) => {
    setEditingReview(review);
    setReviewForm({
      rating: review.rating,
      comment: review.comment,
    });
    setEditReviewModalOpen(true);
  };

  const handleDeleteReview = async (reviewId: string) => {
    if (!token) {
      setSnackbar({
        open: true,
        message: "Please log in to delete review",
        severity: "error",
      });
      return;
    }

    setDeletingReviewId(reviewId);
    try {
      await deleteReview(reviewId, token);

      // Immediately update local state for instant UI feedback
      setLocalReviews((prevReviews) =>
        prevReviews.filter((review) => review.id !== reviewId)
      );

      // Call the callback to refresh data from parent (in background)
      onReviewsUpdate();

      setSnackbar({
        open: true,
        message: "Review deleted successfully!",
        severity: "success",
      });
    } catch (error) {
      console.error("Error deleting review:", error);
      setSnackbar({
        open: true,
        message:
          error instanceof Error ? error.message : "Failed to delete review",
        severity: "error",
      });
    } finally {
      setDeletingReviewId(null);
    }
  };

  const resetReviewForm = () => {
    setReviewForm({ rating: 0, comment: "" });
    setEditingReview(null);
  };

  const isUserReview = (reviewerId: string) => {
    return user && user.userId === reviewerId;
  };

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString("en-US", {
      year: "numeric",
      month: "long",
      day: "numeric",
    });
  };

  const getInitials = (firstName: string, lastName: string) => {
    return `${firstName.charAt(0)}${lastName.charAt(0)}`.toUpperCase();
  };

  return (
    <>
      <Box
        sx={{
          display: "flex",
          alignItems: "center",
          mb: 2,
          flexWrap: "wrap",
          gap: 2,
        }}
      >
        {localReviews && localReviews.length > 0 && (
          <Box sx={{ display: "flex", alignItems: "center" }}>
            <Rating
              value={averageRating}
              precision={0.1}
              readOnly
              size="large"
              sx={{
                "& .MuiRating-iconFilled": {
                  color: "#8E44AD",
                },
              }}
            />
            <Typography variant="h6" sx={{ ml: 1, mr: 2, fontWeight: 600 }}>
              {averageRating.toFixed(1)}
            </Typography>
            <Typography
              variant="body2"
              sx={{
                cursor: "pointer",
                textDecoration: "underline",
                color: "#8E44AD",
                "&:hover": { color: "#6A1B9A" },
              }}
              onClick={() => setReviewsModalOpen(true)}
            >
              ({localReviews.length} review
              {localReviews.length !== 1 ? "s" : ""})
            </Typography>
          </Box>
        )}

        {user ? (
          <Button
            variant="outlined"
            startIcon={<RateReviewIcon />}
            onClick={() => setAddReviewModalOpen(true)}
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
            }}
          >
            Write a Review
          </Button>
        ) : (
          <Button
            variant="outlined"
            startIcon={<RateReviewIcon />}
            onClick={() => {
              setSnackbar({
                open: true,
                message: "Please log in to write a review",
                severity: "error",
              });
            }}
            sx={{
              borderColor: "#ccc",
              color: "#999",
              fontWeight: 600,
              textTransform: "none",
              "&:hover": {
                borderColor: "#bbb",
                backgroundColor: "rgba(0, 0, 0, 0.04)",
              },
            }}
          >
            Write a Review (Login Required)
          </Button>
        )}
      </Box>

      {/* Add Review Modal */}
      <Dialog
        open={addReviewModalOpen}
        onClose={() => {
          setAddReviewModalOpen(false);
          resetReviewForm();
        }}
        fullWidth
        maxWidth="sm"
        PaperProps={{
          sx: {
            borderRadius: 3,
          },
        }}
      >
        <DialogTitle
          sx={{
            background: "linear-gradient(45deg, #8E44AD 30%, #AF7AC5 90%)",
            color: "white",
            fontWeight: 600,
          }}
        >
          <Box sx={{ display: "flex", alignItems: "center" }}>
            <RateReviewIcon sx={{ mr: 2 }} />
            Write a Review for {getEntityDisplayName()}
          </Box>
        </DialogTitle>
        <DialogContent sx={{ p: 4 }}>
          <Box sx={{ mb: 3 }}>
            <Typography variant="subtitle1" sx={{ mb: 2, fontWeight: 600 }}>
              Rating *
            </Typography>
            <Rating
              value={reviewForm.rating}
              onChange={(_, newValue) =>
                setReviewForm((prev) => ({ ...prev, rating: newValue || 0 }))
              }
              size="large"
              sx={{
                "& .MuiRating-iconFilled": {
                  color: "#8E44AD",
                },
                "& .MuiRating-iconHover": {
                  color: "#6A1B9A",
                },
              }}
            />
          </Box>
          <Box>
            <Typography variant="subtitle1" sx={{ mb: 2, fontWeight: 600 }}>
              Comment *
            </Typography>
            <TextField
              multiline
              rows={4}
              fullWidth
              placeholder={getReviewPlaceholder()}
              variant="outlined"
              value={reviewForm.comment}
              onChange={(e) =>
                setReviewForm((prev) => ({ ...prev, comment: e.target.value }))
              }
              sx={{
                "& .MuiOutlinedInput-root": {
                  "&:hover fieldset": {
                    borderColor: "#8E44AD",
                  },
                  "&.Mui-focused fieldset": {
                    borderColor: "#8E44AD",
                  },
                },
              }}
            />
          </Box>
        </DialogContent>
        <DialogActions sx={{ p: 3, pt: 0 }}>
          <Button
            onClick={() => {
              setAddReviewModalOpen(false);
              resetReviewForm();
            }}
            color="inherit"
            sx={{ textTransform: "none" }}
          >
            Cancel
          </Button>
          <Button
            onClick={handleSubmitReview}
            disabled={
              submittingReview ||
              reviewForm.rating === 0 ||
              !reviewForm.comment.trim()
            }
            variant="contained"
            sx={{
              background: "linear-gradient(45deg, #8E44AD 30%, #AF7AC5 90%)",
              textTransform: "none",
              fontWeight: 600,
              px: 4,
              "&:hover": {
                background: "linear-gradient(45deg, #6A1B9A 30%, #8E44AD 90%)",
              },
              "&:disabled": {
                background: "#ccc",
              },
            }}
            startIcon={
              submittingReview ? (
                <CircularProgress size={16} color="inherit" />
              ) : null
            }
          >
            {submittingReview ? "Submitting..." : "Submit Review"}
          </Button>
        </DialogActions>
      </Dialog>

      {/* Edit Review Modal */}
      <Dialog
        open={editReviewModalOpen}
        onClose={() => {
          setEditReviewModalOpen(false);
          resetReviewForm();
        }}
        fullWidth
        maxWidth="sm"
        PaperProps={{
          sx: {
            borderRadius: 3,
          },
        }}
      >
        <DialogTitle
          sx={{
            background: "linear-gradient(45deg, #8E44AD 30%, #AF7AC5 90%)",
            color: "white",
            fontWeight: 600,
          }}
        >
          <Box sx={{ display: "flex", alignItems: "center" }}>
            <EditIcon sx={{ mr: 2 }} />
            Edit Review
          </Box>
        </DialogTitle>
        <DialogContent sx={{ p: 4 }}>
          <Box sx={{ mb: 3 }}>
            <Typography variant="subtitle1" sx={{ mb: 2, fontWeight: 600 }}>
              Rating *
            </Typography>
            <Rating
              value={reviewForm.rating}
              onChange={(_, newValue) =>
                setReviewForm((prev) => ({ ...prev, rating: newValue || 0 }))
              }
              size="large"
              sx={{
                "& .MuiRating-iconFilled": {
                  color: "#8E44AD",
                },
                "& .MuiRating-iconHover": {
                  color: "#6A1B9A",
                },
              }}
            />
          </Box>
          <Box>
            <Typography variant="subtitle1" sx={{ mb: 2, fontWeight: 600 }}>
              Comment *
            </Typography>
            <TextField
              multiline
              rows={4}
              fullWidth
              placeholder={getReviewPlaceholder()}
              variant="outlined"
              value={reviewForm.comment}
              onChange={(e) =>
                setReviewForm((prev) => ({ ...prev, comment: e.target.value }))
              }
              sx={{
                "& .MuiOutlinedInput-root": {
                  "&:hover fieldset": {
                    borderColor: "#8E44AD",
                  },
                  "&.Mui-focused fieldset": {
                    borderColor: "#8E44AD",
                  },
                },
              }}
            />
          </Box>
        </DialogContent>
        <DialogActions sx={{ p: 3, pt: 0 }}>
          <Button
            onClick={() => {
              setEditReviewModalOpen(false);
              resetReviewForm();
            }}
            color="inherit"
            sx={{ textTransform: "none" }}
          >
            Cancel
          </Button>
          <Button
            onClick={handleUpdateReview}
            disabled={
              updatingReview ||
              reviewForm.rating === 0 ||
              !reviewForm.comment.trim()
            }
            variant="contained"
            sx={{
              background: "linear-gradient(45deg, #8E44AD 30%, #AF7AC5 90%)",
              textTransform: "none",
              fontWeight: 600,
              px: 4,
              "&:hover": {
                background: "linear-gradient(45deg, #6A1B9A 30%, #8E44AD 90%)",
              },
              "&:disabled": {
                background: "#ccc",
              },
            }}
            startIcon={
              updatingReview ? (
                <CircularProgress size={16} color="inherit" />
              ) : null
            }
          >
            {updatingReview ? "Updating..." : "Update Review"}
          </Button>
        </DialogActions>
      </Dialog>

      {/* Reviews Modal */}
      <Dialog
        open={reviewsModalOpen}
        onClose={() => setReviewsModalOpen(false)}
        fullWidth
        maxWidth="md"
        PaperProps={{
          sx: {
            borderRadius: 3,
            maxHeight: "80vh",
          },
        }}
      >
        <DialogContent sx={{ p: 0 }}>
          <Box
            sx={{
              p: 4,
              background: "linear-gradient(45deg, #8E44AD 30%, #AF7AC5 90%)",
              color: "white",
              position: "sticky",
              top: 0,
              zIndex: 1,
            }}
          >
            <Box
              sx={{
                display: "flex",
                justifyContent: "space-between",
                alignItems: "center",
              }}
            >
              <Box>
                <Typography variant="h5" fontWeight={600}>
                  {getEntityDisplayName().charAt(0).toUpperCase() +
                    getEntityDisplayName().slice(1)}{" "}
                  Reviews
                </Typography>
                <Box sx={{ display: "flex", alignItems: "center", mt: 1 }}>
                  <Rating
                    value={averageRating}
                    precision={0.1}
                    readOnly
                    sx={{
                      "& .MuiRating-iconFilled": {
                        color: "#FFD700",
                      },
                    }}
                  />
                  <Typography
                    variant="h6"
                    sx={{ ml: 1, mr: 2, fontWeight: 600 }}
                  >
                    {averageRating.toFixed(1)}
                  </Typography>
                  <Typography variant="body2" sx={{ opacity: 0.9 }}>
                    ({localReviews?.length} review
                    {localReviews?.length !== 1 ? "s" : ""})
                  </Typography>
                </Box>
              </Box>
              <IconButton
                onClick={() => setReviewsModalOpen(false)}
                sx={{
                  backgroundColor: "rgba(255, 255, 255, 0.2)",
                  color: "white",
                  "&:hover": { backgroundColor: "rgba(255, 255, 255, 0.3)" },
                }}
              >
                <CloseIcon />
              </IconButton>
            </Box>
          </Box>

          <Box
            sx={{ p: 4, maxHeight: "calc(80vh - 140px)", overflowY: "auto" }}
          >
            {localReviews && localReviews.length > 0 ? (
              <Grid container spacing={3}>
                {localReviews.map((review) => (
                  <Grid size={{ xs: 12 }} key={review.id}>
                    <Card
                      elevation={2}
                      sx={{
                        borderRadius: 3,
                        border: "1px solid rgba(142, 68, 173, 0.2)",
                        "&:hover": {
                          boxShadow: "0 8px 25px rgba(142, 68, 173, 0.15)",
                          transform: "translateY(-2px)",
                          transition: "all 0.3s ease",
                        },
                      }}
                    >
                      <CardContent sx={{ p: 3 }}>
                        <Box
                          sx={{
                            display: "flex",
                            alignItems: "flex-start",
                            mb: 2,
                          }}
                        >
                          <Avatar
                            sx={{
                              mr: 3,
                              background:
                                "linear-gradient(45deg, #8E44AD 30%, #AF7AC5 90%)",
                              width: 48,
                              height: 48,
                              fontSize: "1.2rem",
                              fontWeight: 600,
                            }}
                          >
                            {review.reviewer ? (
                              getInitials(
                                review.reviewer.firstName,
                                review.reviewer.lastName
                              )
                            ) : (
                              <PersonIcon />
                            )}
                          </Avatar>
                          <Box sx={{ flexGrow: 1 }}>
                            <Box
                              sx={{
                                display: "flex",
                                alignItems: "center",
                                justifyContent: "space-between",
                                mb: 1,
                              }}
                            >
                              <Typography variant="subtitle1" fontWeight={600}>
                                {review.reviewer
                                  ? `${review.reviewer.firstName} ${review.reviewer.lastName}`
                                  : "Anonymous User"}
                              </Typography>
                              {isUserReview(review.reviewerId) && (
                                <Box sx={{ display: "flex", gap: 1 }}>
                                  <Tooltip title="Edit your review">
                                    <IconButton
                                      onClick={() => handleEditReview(review)}
                                      sx={{
                                        color: "#8E44AD",
                                        "&:hover": {
                                          backgroundColor:
                                            "rgba(142, 68, 173, 0.1)",
                                        },
                                      }}
                                      size="small"
                                    >
                                      <EditIcon fontSize="small" />
                                    </IconButton>
                                  </Tooltip>
                                  <Tooltip title="Delete your review">
                                    <IconButton
                                      onClick={() =>
                                        handleDeleteReview(review.id)
                                      }
                                      disabled={deletingReviewId === review.id}
                                      sx={{
                                        color: "#f44336",
                                        "&:hover": {
                                          backgroundColor:
                                            "rgba(244, 67, 54, 0.1)",
                                        },
                                        "&:disabled": {
                                          color: "#ccc",
                                        },
                                      }}
                                      size="small"
                                    >
                                      {deletingReviewId === review.id ? (
                                        <CircularProgress
                                          size={16}
                                          color="inherit"
                                        />
                                      ) : (
                                        <DeleteIcon fontSize="small" />
                                      )}
                                    </IconButton>
                                  </Tooltip>
                                </Box>
                              )}
                            </Box>
                            <Box
                              sx={{
                                display: "flex",
                                alignItems: "center",
                                mb: 2,
                              }}
                            >
                              <Rating
                                value={review.rating}
                                size="small"
                                readOnly
                                sx={{
                                  "& .MuiRating-iconFilled": {
                                    color: "#8E44AD",
                                  },
                                }}
                              />
                              <Typography
                                variant="caption"
                                color="text.secondary"
                                sx={{ ml: 2 }}
                              >
                                {formatDate(review.createdAt)}
                              </Typography>
                            </Box>
                            <Typography
                              variant="body1"
                              sx={{
                                lineHeight: 1.6,
                                color: "text.primary",
                              }}
                            >
                              {review.comment}
                            </Typography>
                          </Box>
                        </Box>
                      </CardContent>
                    </Card>
                  </Grid>
                ))}
              </Grid>
            ) : (
              <Box sx={{ textAlign: "center", py: 4 }}>
                <Typography variant="h6" color="text.secondary">
                  No reviews yet
                </Typography>
                <Typography
                  variant="body2"
                  color="text.secondary"
                  sx={{ mt: 1 }}
                >
                  Be the first to leave a review for this{" "}
                  {getEntityDisplayName()}!
                </Typography>
              </Box>
            )}
          </Box>
        </DialogContent>
      </Dialog>

      <Snackbar
        open={snackbar.open}
        autoHideDuration={6000}
        onClose={() => setSnackbar((prev) => ({ ...prev, open: false }))}
        anchorOrigin={{ vertical: "bottom", horizontal: "left" }}
      >
        <Alert
          onClose={() => setSnackbar((prev) => ({ ...prev, open: false }))}
          severity={snackbar.severity}
          sx={{ width: "100%" }}
        >
          {snackbar.message}
        </Alert>
      </Snackbar>
    </>
  );
};
