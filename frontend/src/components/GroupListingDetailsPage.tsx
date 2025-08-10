import { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { useAuth } from "./AuthContext";
import {
  Box,
  Typography,
  Chip,
  Avatar,
  Paper,
  Stack,
  Divider,
  Button,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
  Alert,
  Snackbar,
  IconButton,
} from "@mui/material";
import {
  Euro as EuroIcon,
  LocationOn as LocationOnIcon,
  CalendarToday as CalendarTodayIcon,
  People as PeopleIcon,
  Hotel as HotelIcon,
  Send as SendIcon,
  Delete as DeleteIcon,
} from "@mui/icons-material";
import { LoadingComponent } from "../components/LoadingComponent";
import { PropertyMap } from "../components/PropertyMap";
import { ImageGallery } from "../components/ImageGallery";
import { DeleteConfirmationDialog } from "../components/DeleteConfirmationDialog";
import {
  GroupListingDto,
  GroupMemberDto,
  RoomApplicationDto,
} from "../types/types";
import { fetchListingDetails } from "../queries/fetchListingDetails";
import { deleteGroupListing } from "../queries/deleteListing";
import { checkDeletePermissionsForListings } from "../utils/canUserDeleteListing";

export const GroupListingDetailsPage = () => {
  const { id } = useParams();
  const { user } = useAuth();
  const navigate = useNavigate();
  const [listing, setListing] = useState<GroupListingDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [applicationDialogOpen, setApplicationDialogOpen] = useState(false);
  const [applicationMessage, setApplicationMessage] = useState("");
  const [submittingApplication, setSubmittingApplication] = useState(false);
  const [snackbarOpen, setSnackbarOpen] = useState(false);
  const [snackbarMessage, setSnackbarMessage] = useState("");
  const [snackbarSeverity, setSnackbarSeverity] = useState<"success" | "error">(
    "success"
  );
  const [listingToDelete, setListingToDelete] = useState<string | null>(null);
  const [deleteLoading, setDeleteLoading] = useState(false);
  const [canDelete, setCanDelete] = useState(false);
  const [isGroupMember, setIsGroupMember] = useState(false);
  const [groupMembersCache, setGroupMembersCache] = useState<{
    [groupId: string]: GroupMemberDto[];
  }>({});

  const updateMembersCache = (groupId: string, members: GroupMemberDto[]) => {
    setGroupMembersCache((prev) => ({
      ...prev,
      [groupId]: members,
    }));
  };

  useEffect(() => {
    const fetchDetails = async () => {
      try {
        const data = await fetchListingDetails(id!);
        setListing(data);

        if (user?.userId) {
          const deletableListings = await checkDeletePermissionsForListings(
            [data],
            user.userId,
            groupMembersCache,
            updateMembersCache
          );
          setCanDelete(deletableListings.has(data.id));

          if (data.group) {
            const isMember = data.group.members.some(
              (member: GroupMemberDto) => member.userId === user.userId
            );
            setIsGroupMember(isMember);
          }
        }
      } catch (err) {
        console.error(err);
      } finally {
        setLoading(false);
      }
    };
    fetchDetails();
  }, [id, user?.userId, groupMembersCache]);

  const formatDate = (date: string) =>
    new Date(date).toLocaleDateString("en-GB");

  const handleApplicationSubmit = async () => {
    if (!applicationMessage || !applicationMessage.trim()) {
      setSnackbarMessage("Please enter a message with your application");
      setSnackbarSeverity("error");
      setSnackbarOpen(true);
      return;
    }

    setSubmittingApplication(true);

    try {
      const applicationDto: RoomApplicationDto = {
        id: crypto.randomUUID(),
        listingId: id!,
        applicantUserId: user?.userId ?? "",
        message: applicationMessage.trim(),
        status: "Pending",
        createdAt: new Date().toISOString(),
      };

      const response = await fetch(
        "http://localhost:5000/api/groups/applications",
        {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
          body: JSON.stringify(applicationDto),
        }
      );

      if (response.ok) {
        setSnackbarMessage("Application submitted successfully!");
        setSnackbarSeverity("success");
        setSnackbarOpen(true);
        setApplicationDialogOpen(false);
        setApplicationMessage("");
      } else {
        throw new Error(`HTTP error! status: ${response.status}`);
      }
    } catch (error) {
      console.error("Error submitting application:", error);
      setSnackbarMessage("Failed to submit application. Please try again.");
      setSnackbarSeverity("error");
      setSnackbarOpen(true);
    } finally {
      setSubmittingApplication(false);
    }
  };

  const handleDeleteListing = async (listingId: string) => {
    try {
      setDeleteLoading(true);

      await deleteGroupListing(listingId);

      setSnackbarMessage("Listing deleted successfully");
      setSnackbarSeverity("success");
      setSnackbarOpen(true);

      setTimeout(() => {
        navigate("/group-listings");
      }, 1500);
    } catch (err) {
      setSnackbarMessage(
        err instanceof Error ? err.message : "Failed to delete listing"
      );
      setSnackbarSeverity("error");
      setSnackbarOpen(true);
    } finally {
      setDeleteLoading(false);
      setListingToDelete(null);
    }
  };

  const handleDeleteClick = () => {
    setListingToDelete(listing?.id || null);
  };

  const handleDeleteConfirm = () => {
    if (listingToDelete) {
      handleDeleteListing(listingToDelete);
    }
  };

  const handleDeleteCancel = () => {
    setListingToDelete(null);
  };

  const handleCloseDialog = () => {
    setApplicationDialogOpen(false);
    setApplicationMessage("");
  };

  const handleCloseSnackbar = () => {
    setSnackbarOpen(false);
  };

  if (loading) return <LoadingComponent text="Loading listing details..." />;

  if (!listing) return <Typography variant="h6">Listing not found</Typography>;

  const { property, room } = listing;
  const hasProperty = !!property && !room;
  const hasRoom = !!room;

  return (
    <Box sx={{ px: { xs: 2, md: 6 }, py: 4 }}>
      <Box
        display="flex"
        justifyContent="space-between"
        alignItems="flex-start"
        mb={2}
      >
        <Typography variant="h4" fontWeight={600} gutterBottom>
          {listing.title}
        </Typography>
        <Stack direction="row" spacing={1}>
          {canDelete && (
            <IconButton
              onClick={handleDeleteClick}
              disabled={deleteLoading}
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
          {listing.status === "Active" && !isGroupMember && (
            <Button
              variant="contained"
              startIcon={<SendIcon />}
              onClick={() => setApplicationDialogOpen(true)}
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
            >
              Send Application
            </Button>
          )}
        </Stack>
      </Box>
      <Stack direction="row" spacing={2} mb={2} flexWrap="wrap">
        <Chip
          label={listing.status}
          color={listing.status === "Active" ? "success" : "default"}
        />
        <Stack direction="row" alignItems="center" spacing={1}>
          <LocationOnIcon fontSize="small" />
          <Typography variant="body2">{listing.preferredCity}</Typography>
        </Stack>
        <Stack direction="row" alignItems="center" spacing={1}>
          <PeopleIcon fontSize="small" />
          <Typography variant="body2">
            {listing.desiredRoommatesCount} roommates wanted
          </Typography>
        </Stack>
        {listing.maxBudgetPerPerson && (
          <Stack direction="row" alignItems="center" spacing={1}>
            <EuroIcon fontSize="small" />
            <Typography variant="body2">
              Max {listing.maxBudgetPerPerson} PLN/person
            </Typography>
          </Stack>
        )}
        <Stack direction="row" alignItems="center" spacing={1}>
          <CalendarTodayIcon fontSize="small" />
          <Typography variant="body2">
            Posted: {formatDate(listing.createdAt)}
          </Typography>
        </Stack>
      </Stack>
      <Typography variant="subtitle1" color="text.secondary" paragraph>
        {listing.description}
      </Typography>
      <Divider sx={{ my: 3 }} />
      {listing.group && (
        <Paper sx={{ p: 3, mb: 4 }}>
          <Typography variant="h6" gutterBottom>
            Group Information
          </Typography>
          <Typography>
            <strong>Name:</strong> {listing.group.name}
          </Typography>
          <Typography>
            <strong>Description:</strong> {listing.group.description}
          </Typography>
          <Typography>
            <strong>Created:</strong> {formatDate(listing.group.createdAt)}
          </Typography>
          <Divider sx={{ my: 2 }} />
          <Typography variant="subtitle1" gutterBottom>
            Members
          </Typography>
          <Stack spacing={1}>
            {listing.group.members.map((member) => (
              <Box key={member.id} display="flex" alignItems="center" gap={2}>
                <Avatar>{member.user?.firstName?.[0] || "?"}</Avatar>
                <Box>
                  <Typography>
                    {member.user?.firstName} {member.user?.lastName}
                  </Typography>
                  <Typography variant="caption" color="text.secondary">
                    {member.role} • Joined {formatDate(member.joinedAt)}
                  </Typography>
                </Box>
              </Box>
            ))}
          </Stack>
        </Paper>
      )}
      {hasRoom && (
        <Paper sx={{ p: 3, mb: 4 }}>
          <Typography variant="h6" gutterBottom>
            <HotelIcon sx={{ mr: 1, verticalAlign: "middle" }} />
            Room Information
          </Typography>
          <Typography>
            <strong>Name:</strong> {room?.name}
          </Typography>
          <Typography>
            <strong>Description:</strong> {room?.description}
          </Typography>
          <Typography>
            <strong>Price:</strong> {room?.pricePerMonth} PLN/month
          </Typography>
          <Typography>
            <strong>Size:</strong> {room?.areaInSquareMeters} m²
          </Typography>
          <Typography>
            <strong>Capacity:</strong> {room?.capacity}{" "}
            {room?.capacity === 1 ? "person" : "people"}
          </Typography>
          {room?.detailedDescription && (
            <Typography sx={{ mt: 1 }}>
              <strong>Details:</strong> {room?.detailedDescription}
            </Typography>
          )}
          {room?.images.length > 0 && (
            <Box mt={3}>
              <ImageGallery images={room?.images} title={room?.name} />
            </Box>
          )}
          <Box sx={{ my: 4 }}>
            <PropertyMap address={room.propertyAddress} />
          </Box>
        </Paper>
      )}
      {hasProperty && (
        <Paper sx={{ p: 3, mb: 4 }}>
          <Typography variant="h6" gutterBottom>
            Property Information
          </Typography>
          <Typography>
            <strong>Name:</strong> {property.name}
          </Typography>
          <Typography>
            <strong>Address:</strong> {property.address}
          </Typography>
          <Typography>
            <strong>Description:</strong> {property.description}
          </Typography>
          <Typography>
            <strong>Size:</strong> {property.areaInSquareMeters} m²
          </Typography>
          <Typography>
            <strong>Type:</strong>{" "}
            {property.isEntirePlaceRentable ? "Entire Place" : "Shared Space"}
          </Typography>
          {property.detailedDescription && (
            <Typography sx={{ mt: 1 }}>
              <strong>Details:</strong> {property.detailedDescription}
            </Typography>
          )}
          {property.images.length > 0 && (
            <Box mt={3}>
              <ImageGallery images={property.images} title={property.name} />
            </Box>
          )}
          <Box sx={{ my: 4 }}>
            <PropertyMap address={property.address} />
          </Box>
        </Paper>
      )}
      <Dialog
        open={applicationDialogOpen}
        onClose={handleCloseDialog}
        maxWidth="sm"
        fullWidth
      >
        <DialogTitle>Send Room Application</DialogTitle>
        <DialogContent>
          <Typography variant="body2" color="text.secondary" paragraph>
            Send an application to join this group listing. Include a message to
            introduce yourself and explain why you'd be a good fit.
          </Typography>
          <TextField
            autoFocus
            margin="dense"
            label="Application Message"
            type="text"
            fullWidth
            multiline
            rows={4}
            variant="outlined"
            value={applicationMessage}
            onChange={(e) => setApplicationMessage(e.target.value)}
            placeholder="Hi! I'm interested in joining your group. I'm a..."
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={handleCloseDialog}>Cancel</Button>
          <Button
            onClick={handleApplicationSubmit}
            variant="contained"
            disabled={
              submittingApplication || !applicationMessage.trim() || !user
            }
            startIcon={<SendIcon />}
            sx={{
              backgroundColor: "#8B5CF6",
              color: "white",
              "&:hover": {
                backgroundColor: "#7C3AED",
              },
              "&:active": {
                backgroundColor: "#6D28D9",
              },
              "&:disabled": {
                backgroundColor: "#D1D5DB",
                color: "#9CA3AF",
              },
            }}
          >
            {submittingApplication ? "Sending..." : "Send Application"}
          </Button>
        </DialogActions>
      </Dialog>
      <DeleteConfirmationDialog
        open={!!listingToDelete}
        onClose={handleDeleteCancel}
        onConfirm={handleDeleteConfirm}
        title="Delete Group Listing"
        message="Are you sure you want to delete this group listing? This action cannot be undone."
        loading={deleteLoading}
        variant="standard"
      />

      <Snackbar
        open={snackbarOpen}
        autoHideDuration={6000}
        onClose={handleCloseSnackbar}
        anchorOrigin={{ vertical: "bottom", horizontal: "center" }}
      >
        <Alert
          onClose={handleCloseSnackbar}
          severity={snackbarSeverity}
          sx={{ width: "100%" }}
        >
          {snackbarMessage}
        </Alert>
      </Snackbar>
    </Box>
  );
};
