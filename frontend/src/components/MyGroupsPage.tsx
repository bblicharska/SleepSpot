import React, { useEffect, useState, useCallback } from "react";
import {
  Container,
  Typography,
  Card,
  CardContent,
  CardHeader,
  Avatar,
  Grid,
  CircularProgress,
  Box,
  Chip,
  List,
  ListItem,
  ListItemAvatar,
  ListItemText,
  Button,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  IconButton,
  TextField,
  MenuItem,
  Snackbar,
  Alert,
  Divider,
} from "@mui/material";
import CloseIcon from "@mui/icons-material/Close";
import DeleteIcon from "@mui/icons-material/Delete";
import PersonAddIcon from "@mui/icons-material/PersonAdd";
import MailOutlineIcon from "@mui/icons-material/MailOutline";
import { useAuth } from "./AuthContext";
import {
  GroupDto,
  GroupListingDto,
  RoomApplicationDto,
  RentalAgreementDto,
  API_BASE_URL,
} from "../types/types";
import { Link } from "react-router-dom";
import { DeleteConfirmationDialog } from "./DeleteConfirmationDialog";
import { fetchUserGroups } from "../queries/fetchUserGroups";
import { deleteGroup } from "../queries/deleteGroup";
import { fetchGroupListings } from "../queries/fetchGroupListings";
import { fetchListingApplications } from "../queries/fetchListingApplications";
import { addGroupMemberByEmail } from "../queries/addGroupMemberByEmail";
import { addGroupMember } from "../queries/addGroupMember";
import { changeListingStatus } from "../queries/changeListingStatus";
import { deleteGroupMember } from "../queries/deleteGroupMember";

export const MyGroupsPage: React.FC = () => {
  const { user } = useAuth();
  const [groups, setGroups] = useState<
    (GroupDto & { listings: GroupListingDto[] })[]
  >([]);
  const [loading, setLoading] = useState(true);
  const [applications, setApplications] = useState<
    Record<string, RoomApplicationDto[]>
  >({});
  const [modalOpen, setModalOpen] = useState(false);
  const [selectedListing, setSelectedListing] =
    useState<GroupListingDto | null>(null);
  const [loadingApplications, setLoadingApplications] = useState(false);
  const [openDeleteModal, setOpenDeleteModal] = useState(false);
  const [groupPendingDeleteId, setGroupPendingDeleteId] = useState<
    string | null
  >(null);
  const [openAddMemberDialog, setOpenAddMemberDialog] = useState(false);
  const [addMemberGroupId, setAddMemberGroupId] = useState<string | null>(null);
  const [newMemberEmail, setNewMemberEmail] = useState("");
  const [newMemberRole, setNewMemberRole] = useState<"Member" | "Admin">(
    "Member"
  );
  const [addingMemberLoading, setAddingMemberLoading] = useState(false);
  const [submitAttempted, setSubmitAttempted] = useState(false);
  const [snackbar, setSnackbar] = useState<{
    open: boolean;
    message: string;
    severity: "success" | "error";
  }>({ open: false, message: "", severity: "success" });
  const [groupRentals, setGroupRentals] = useState<
    Record<string, RentalAgreementDto[]>
  >({});
  const [rentalsModalOpen, setRentalsModalOpen] = useState(false);
  const [selectedGroupId, setSelectedGroupId] = useState<string | null>(null);
  const [loadingRentals, setLoadingRentals] = useState(false);

  // New states for member removal
  const [memberPendingRemoveId, setMemberPendingRemoveId] = useState<
    string | null
  >(null);
  const [memberPendingRemoveName, setMemberPendingRemoveName] = useState<
    string | null
  >(null);
  const [memberRemoveDialogOpen, setMemberRemoveDialogOpen] = useState(false);
  const [removingMemberId, setRemovingMemberId] = useState<string | null>(null);

  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

  const showSnackbar = (message: string, severity: "success" | "error") => {
    setSnackbar({ open: true, message, severity });
  };

  const fetchGroupsWithListingsAndApps = useCallback(async () => {
    if (!user?.userId) return;
    setLoading(true);
    try {
      const groupsData = await fetchUserGroups(user.userId);

      const groupsWithListings = await Promise.all(
        groupsData.map(async (group: GroupDto) => {
          let listings: GroupListingDto[] = [];
          try {
            listings = await fetchGroupListings(group.id);

            for (const listing of listings) {
              try {
                const apps = await fetchListingApplications(listing.id);
                setApplications((prev) => ({
                  ...prev,
                  [listing.id]: apps,
                }));
              } catch (err) {
                console.error(
                  `Error fetching applications for ${listing.id}`,
                  err
                );
              }
            }
          } catch (err) {
            console.error(
              `Error fetching listings for group ${group.id}:`,
              err
            );
          }
          return { ...group, listings };
        })
      );

      setGroups(groupsWithListings);
    } catch (error) {
      console.error("Error fetching groups:", error);
      showSnackbar("Failed to load groups", "error");
    } finally {
      setLoading(false);
    }
  }, [user?.userId]);

  const fetchGroupRentals = async (groupId: string) => {
    setLoadingRentals(true);
    try {
      const res = await fetch(`${API_BASE_URL}/api/rentals/group/${groupId}`);
      if (!res.ok) throw new Error("Failed to fetch rentals");
      const data: RentalAgreementDto[] = await res.json();

      setGroupRentals((prev) => ({ ...prev, [groupId]: data }));
      setSelectedGroupId(groupId);
      setRentalsModalOpen(true);
    } catch (err: any) {
      console.error("Error fetching rentals", err);
      showSnackbar(err?.message || "Failed to fetch rentals", "error");
    } finally {
      setLoadingRentals(false);
    }
  };

  useEffect(() => {
    fetchGroupsWithListingsAndApps();
  }, [fetchGroupsWithListingsAndApps]);

  const openApplicationsModal = (listing: GroupListingDto) => {
    setSelectedListing(listing);
    setModalOpen(true);
  };

  const closeModal = () => {
    setModalOpen(false);
    setSelectedListing(null);
  };

  const handleAccept = async (app: RoomApplicationDto) => {
    try {
      await changeListingStatus(app.id, "Accepted");
      await addGroupMember(selectedListing!.groupId, app.applicantUserId);

      setApplications((prev) => ({
        ...prev,
        [selectedListing!.id]: prev[selectedListing!.id].map((a) =>
          a.id === app.id ? { ...a, status: "Accepted" } : a
        ),
      }));
      showSnackbar("Application accepted", "success");
      // refresh groups to show new member
      await fetchGroupsWithListingsAndApps();
    } catch (err) {
      console.error("Error accepting application:", err);
      showSnackbar("Error accepting application", "error");
    }
  };

  const handleReject = async (app: RoomApplicationDto) => {
    try {
      await changeListingStatus(app.id, "Rejected");

      setApplications((prev) => ({
        ...prev,
        [selectedListing!.id]: prev[selectedListing!.id].map((a) =>
          a.id === app.id ? { ...a, status: "Rejected" } : a
        ),
      }));
      showSnackbar("Application rejected", "success");
    } catch (err) {
      console.error("Error rejecting application:", err);
      showSnackbar("Error rejecting application", "error");
    }
  };

  const confirmDeleteGroup = (groupId: string) => {
    setGroupPendingDeleteId(groupId);
    setOpenDeleteModal(true);
  };

  const handleDeleteGroup = async (groupId: string) => {
    try {
      await deleteGroup(groupId);
      setGroups((prev) => prev.filter((g) => g.id !== groupId));
      showSnackbar("Group deleted", "success");
    } catch (err: any) {
      console.error("Error deleting group:", err);
      showSnackbar(err?.message || "Failed to delete group", "error");
    } finally {
      setOpenDeleteModal(false);
      setGroupPendingDeleteId(null);
    }
  };

  const openAddMemberForGroup = (groupId: string) => {
    setAddMemberGroupId(groupId);
    setNewMemberEmail("");
    setNewMemberRole("Member");
    setSubmitAttempted(false);
    setOpenAddMemberDialog(true);
  };

  const handleAddMember = async () => {
    if (!addMemberGroupId) return;

    setSubmitAttempted(true);

    const email = newMemberEmail.trim();
    const isValid = emailRegex.test(email);

    if (!email || !isValid) {
      showSnackbar("Please enter a valid email address", "error");
      return;
    }

    try {
      setAddingMemberLoading(true);
      await addGroupMemberByEmail({
        email,
        role: newMemberRole,
        groupId: addMemberGroupId,
      });

      await fetchGroupsWithListingsAndApps();

      showSnackbar("Member added", "success");
      setOpenAddMemberDialog(false);
      setSubmitAttempted(false);
      setNewMemberEmail("");
    } catch (err: any) {
      console.error("Add member error", err);
      showSnackbar(err?.message || "Failed to add member", "error");
    } finally {
      setAddingMemberLoading(false);
    }
  };

  // --- New: member removal flow ---
  const openRemoveMemberDialog = (memberId: string, memberName: string) => {
    setMemberPendingRemoveId(memberId);
    setMemberPendingRemoveName(memberName);
    setMemberRemoveDialogOpen(true);
  };

  const handleRemoveMemberConfirmed = async () => {
    if (!memberPendingRemoveId) return;
    const id = memberPendingRemoveId;
    try {
      setRemovingMemberId(id);
      await deleteGroupMember(id);

      // update local groups state: find which group had the member and remove it
      setGroups((prev) =>
        prev.map((g) => ({
          ...g,
          members: g.members
            ? g.members.filter((m: any) => m.id !== id)
            : g.members,
        }))
      );

      showSnackbar("Member removed from group", "success");
    } catch (err: any) {
      console.error("Failed to remove member:", err);
      showSnackbar(err?.message || "Failed to remove member", "error");
    } finally {
      setRemovingMemberId(null);
      setMemberPendingRemoveId(null);
      setMemberPendingRemoveName(null);
      setMemberRemoveDialogOpen(false);
    }
  };

  if (loading) {
    return (
      <Box display="flex" justifyContent="center" mt={5}>
        <CircularProgress />
      </Box>
    );
  }

  const emailIsValid = emailRegex.test(newMemberEmail.trim());
  const showEmailError =
    submitAttempted && (!newMemberEmail.trim() || !emailIsValid);
  const emailErrorMessage = !newMemberEmail.trim()
    ? "Email is required"
    : !emailIsValid
    ? "Enter a valid email address"
    : "";

  const isCurrentUserAdmin = (group: any) =>
    group.members?.some(
      (m: any) => m.userId === user?.userId && m.role === "Admin"
    );

  return (
    <>
      <Container maxWidth="md" sx={{ mt: 4 }}>
        <Typography variant="h4" gutterBottom>
          My Groups
        </Typography>
        {groups.length === 0 ? (
          <Typography variant="body1" color="text.secondary">
            You are not part of any groups yet.
          </Typography>
        ) : (
          <Grid container spacing={3}>
            {groups.map((group) => (
              <Grid size={{ xs: 12 }} key={group.id}>
                <Card>
                  <CardHeader
                    avatar={
                      <Avatar>{group.name.charAt(0).toUpperCase()}</Avatar>
                    }
                    title={group.name}
                    subheader={`Created on ${new Date(
                      group.createdAt
                    ).toLocaleDateString()}`}
                    action={
                      isCurrentUserAdmin(group) && (
                        <>
                          <IconButton
                            color="primary"
                            onClick={() => openAddMemberForGroup(group.id)}
                            title="Add member by email"
                          >
                            <PersonAddIcon />
                          </IconButton>

                          <IconButton
                            color="error"
                            onClick={() => confirmDeleteGroup(group.id)}
                            title="Delete group"
                          >
                            <DeleteIcon />
                          </IconButton>
                        </>
                      )
                    }
                  />
                  <CardContent>
                    <Typography variant="body1" gutterBottom>
                      {group.description || "No description provided."}
                    </Typography>

                    <Typography variant="subtitle2" gutterBottom>
                      Members:
                    </Typography>

                    <List dense>
                      {group.members.map((member: any) => {
                        const memberName =
                          `${member.user?.firstName || ""} ${
                            member.user?.lastName || ""
                          }`.trim() ||
                          member.user?.email ||
                          "Unknown";
                        const isSelf = member.userId === user?.userId;
                        const canRemove = isCurrentUserAdmin(group) && !isSelf; // admin can remove others, but not themselves

                        return (
                          <ListItem
                            key={member.id}
                            alignItems="flex-start"
                            secondaryAction={
                              canRemove ? (
                                <IconButton
                                  edge="end"
                                  aria-label="remove-member"
                                  onClick={() =>
                                    openRemoveMemberDialog(
                                      member.id,
                                      memberName
                                    )
                                  }
                                  disabled={removingMemberId === member.id}
                                  title="Remove member"
                                  size="small"
                                >
                                  {removingMemberId === member.id ? (
                                    <CircularProgress size={18} />
                                  ) : (
                                    <DeleteIcon fontSize="small" />
                                  )}
                                </IconButton>
                              ) : null
                            }
                          >
                            <ListItemAvatar>
                              <Avatar>
                                {member.user?.firstName?.charAt(0)}
                                {member.user?.lastName?.charAt(0)}
                              </Avatar>
                            </ListItemAvatar>
                            <ListItemText
                              primary={
                                <Typography variant="body1" fontWeight="bold">
                                  {memberName} - {member.role}
                                </Typography>
                              }
                              secondary={
                                member.user?.email && (
                                  <Box
                                    display="flex"
                                    alignItems="center"
                                    mt={0.3}
                                  >
                                    <MailOutlineIcon
                                      fontSize="small"
                                      sx={{ mr: 0.5 }}
                                    />
                                    <Typography
                                      variant="body2"
                                      color="text.secondary"
                                      sx={{ wordBreak: "break-all" }}
                                    >
                                      {member.user.email}
                                    </Typography>
                                  </Box>
                                )
                              }
                            />
                          </ListItem>
                        );
                      })}
                    </List>

                    <Typography variant="subtitle2" gutterBottom sx={{ mt: 2 }}>
                      Listings:
                    </Typography>
                    <Box display="flex" flexDirection="column" gap={1}>
                      {group.listings.map((listing) => {
                        const appCount =
                          applications[listing.id]?.filter(
                            (a) => a.status === "Pending"
                          )?.length ?? 0;
                        const buttonColor =
                          appCount === 0 ? "error" : "success";

                        return (
                          <Box
                            key={listing.id}
                            display="flex"
                            alignItems="center"
                            gap={1}
                          >
                            <Chip
                              label={listing.title}
                              clickable
                              component={Link}
                              to={`/group-listings/${listing.id}`}
                              color="primary"
                              variant="outlined"
                            />
                            <Button
                              size="small"
                              variant="contained"
                              color={buttonColor}
                              onClick={() => openApplicationsModal(listing)}
                            >
                              Applications ({appCount})
                            </Button>
                          </Box>
                        );
                      })}
                    </Box>
                    <Typography variant="subtitle2" gutterBottom sx={{ mt: 2 }}>
                      Rentals:
                    </Typography>
                    <Box>
                      <Button
                        size="small"
                        variant="contained"
                        onClick={() => fetchGroupRentals(group.id)}
                      >
                        View Rentals
                      </Button>
                    </Box>
                  </CardContent>
                </Card>
              </Grid>
            ))}
          </Grid>
        )}
      </Container>

      {/* Add member dialog */}
      <Dialog
        open={openAddMemberDialog}
        onClose={() => {
          setOpenAddMemberDialog(false);
          setSubmitAttempted(false);
        }}
        maxWidth="sm"
        fullWidth
      >
        <DialogTitle>Add member by email</DialogTitle>
        <DialogContent>
          <Box sx={{ display: "flex", flexDirection: "column", gap: 2, mt: 1 }}>
            <TextField
              label="Email"
              type="email"
              value={newMemberEmail}
              onChange={(e) => setNewMemberEmail(e.target.value)}
              fullWidth
              autoFocus
              error={showEmailError}
              helperText={showEmailError ? emailErrorMessage : ""}
            />
            <TextField
              select
              label="Role"
              value={newMemberRole}
              onChange={(e) => setNewMemberRole(e.target.value as any)}
            >
              <MenuItem value="Member">Member</MenuItem>
              <MenuItem value="Admin">Admin</MenuItem>
            </TextField>
          </Box>
        </DialogContent>
        <DialogActions>
          <Button
            onClick={() => {
              setOpenAddMemberDialog(false);
              setSubmitAttempted(false);
            }}
            disabled={addingMemberLoading}
          >
            Cancel
          </Button>
          <Button
            onClick={handleAddMember}
            variant="contained"
            disabled={addingMemberLoading}
          >
            {addingMemberLoading ? "Adding..." : "Add member"}
          </Button>
        </DialogActions>
      </Dialog>

      {/* Delete group confirmation */}
      <DeleteConfirmationDialog
        open={openDeleteModal}
        onClose={() => {
          setOpenDeleteModal(false);
          setGroupPendingDeleteId(null);
        }}
        onConfirm={async () => {
          if (groupPendingDeleteId)
            await handleDeleteGroup(groupPendingDeleteId);
        }}
        title="Delete Group"
        message="Are you sure you want to delete this group? All related listings will be permanently removed."
        loading={loading}
        variant="gradient"
      />

      {/* Remove member confirmation */}
      <DeleteConfirmationDialog
        open={memberRemoveDialogOpen}
        onClose={() => {
          setMemberRemoveDialogOpen(false);
          setMemberPendingRemoveId(null);
          setMemberPendingRemoveName(null);
        }}
        onConfirm={handleRemoveMemberConfirmed}
        title="Remove Member"
        message={`Are you sure you want to remove ${
          memberPendingRemoveName ?? "this member"
        } from the group?`}
        loading={removingMemberId !== null}
        variant="gradient"
      />

      {/* Applications modal */}
      <Dialog
        open={modalOpen}
        onClose={closeModal}
        maxWidth="md"
        fullWidth
        PaperProps={{ sx: { maxHeight: "80vh" } }}
      >
        <DialogTitle
          sx={{
            m: 0,
            p: 2,
            display: "flex",
            alignItems: "center",
            justifyContent: "space-between",
          }}
        >
          <Typography variant="h6">
            Pending applications for: {selectedListing?.title}
          </Typography>
          <IconButton
            aria-label="close"
            onClick={closeModal}
            sx={{ color: (theme) => theme.palette.grey[500] }}
          >
            <CloseIcon />
          </IconButton>
        </DialogTitle>
        <DialogContent dividers>
          {loadingApplications ? (
            <Box display="flex" justifyContent="center" p={3}>
              <CircularProgress />
            </Box>
          ) : selectedListing && applications[selectedListing.id] ? (
            applications[selectedListing.id].length > 0 ? (
              <List>
                {applications[selectedListing.id]
                  ?.filter((a) => a.status === "Pending")
                  .map((app) => (
                    <ListItem
                      key={app.id}
                      alignItems="flex-start"
                      sx={{
                        px: 0,
                        flexDirection: "column",
                        alignItems: "stretch",
                      }}
                    >
                      <Box display="flex" alignItems="center" width="100%">
                        <ListItemAvatar>
                          <Avatar>
                            {app.applicant?.firstName?.charAt(0)}
                            {app.applicant?.lastName?.charAt(0)}
                          </Avatar>
                        </ListItemAvatar>
                        <ListItemText
                          primary={
                            <Box display="flex" alignItems="center" gap={1}>
                              <Typography
                                variant="subtitle1"
                                fontWeight="bold"
                              >{`${app.applicant?.firstName || ""} ${
                                app.applicant?.lastName || ""
                              }`}</Typography>
                              <Chip
                                label={app.status}
                                size="small"
                                color={
                                  app.status === "Pending"
                                    ? "warning"
                                    : app.status === "Accepted"
                                    ? "success"
                                    : "error"
                                }
                              />
                            </Box>
                          }
                          secondary={
                            <Box mt={1}>
                              <Typography variant="body2" gutterBottom>
                                {app.message}
                              </Typography>
                              {app.applicant?.email && (
                                <Box display="flex" alignItems="center" mt={1}>
                                  <MailOutlineIcon
                                    fontSize="small"
                                    sx={{ mr: 0.5 }}
                                  />
                                  <Typography
                                    variant="body2"
                                    color="text.secondary"
                                  >
                                    {app.applicant.email}
                                  </Typography>
                                </Box>
                              )}
                              <Typography
                                variant="caption"
                                display="block"
                                mt={1}
                              >
                                Applied on:{" "}
                                {new Date(app.createdAt).toLocaleString()}
                              </Typography>
                            </Box>
                          }
                        />
                      </Box>
                      {app.status === "Pending" && (
                        <Box display="flex" gap={1} mt={1} ml={7}>
                          <Button
                            variant="contained"
                            color="success"
                            size="small"
                            onClick={() => handleAccept(app)}
                          >
                            Accept
                          </Button>
                          <Button
                            variant="outlined"
                            color="error"
                            size="small"
                            onClick={() => handleReject(app)}
                          >
                            Reject
                          </Button>
                        </Box>
                      )}
                    </ListItem>
                  ))}
              </List>
            ) : (
              <Box textAlign="center" py={4}>
                <Typography variant="body1" color="text.secondary">
                  No applications for this listing yet.
                </Typography>
              </Box>
            )
          ) : (
            <Box textAlign="center" py={4}>
              <Typography variant="body1" color="text.secondary">
                Loading applications...
              </Typography>
            </Box>
          )}
        </DialogContent>
        <DialogActions>
          <Button onClick={closeModal} variant="outlined">
            Close
          </Button>
        </DialogActions>
      </Dialog>
      <Dialog
        open={rentalsModalOpen}
        onClose={() => setRentalsModalOpen(false)}
        maxWidth="md"
        fullWidth
        PaperProps={{ sx: { maxHeight: "80vh" } }}
      >
        <DialogTitle>
          Rentals for this group
          <IconButton
            aria-label="close"
            onClick={() => setRentalsModalOpen(false)}
            sx={{ position: "absolute", right: 8, top: 8 }}
          >
            <CloseIcon />
          </IconButton>
        </DialogTitle>
        <DialogContent dividers>
          {loadingRentals ? (
            <Box display="flex" justifyContent="center" p={3}>
              <CircularProgress />
            </Box>
          ) : selectedGroupId && groupRentals[selectedGroupId] ? (
            groupRentals[selectedGroupId].length > 0 ? (
              <List>
                {groupRentals[selectedGroupId].map((r) => (
                  <React.Fragment key={r.id}>
                    <ListItem>
                      <ListItemText
                        primary={
                          <Box display="flex" alignItems="center" gap={1}>
                            <Typography variant="subtitle1" fontWeight="bold">
                              {r.room?.name || r.property?.name || "Rental"}
                            </Typography>
                            <Chip
                              size="small"
                              label={r.status}
                              color={
                                r.status.toLowerCase() === "active"
                                  ? "success"
                                  : r.status.toLowerCase() === "pending"
                                  ? "warning"
                                  : "error"
                              }
                            />
                          </Box>
                        }
                        secondary={
                          <>
                            <Typography variant="body2">
                              Monthly Rent: PLN{" "}
                              {Number(r.monthlyRent).toLocaleString()}
                            </Typography>
                            <Typography variant="body2">
                              Period:{" "}
                              {new Date(r.startDate).toLocaleDateString()} →{" "}
                              {r.endDate
                                ? new Date(r.endDate).toLocaleDateString()
                                : "Open-ended"}
                            </Typography>
                            <Typography
                              variant="caption"
                              color="text.secondary"
                            >
                              Created: {new Date(r.createdAt).toLocaleString()}
                            </Typography>
                          </>
                        }
                      />
                    </ListItem>
                    <Divider />
                  </React.Fragment>
                ))}
              </List>
            ) : (
              <Typography>No rentals found for this group.</Typography>
            )
          ) : (
            <Typography>Loading rentals...</Typography>
          )}
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setRentalsModalOpen(false)}>Close</Button>
        </DialogActions>
      </Dialog>
      <Snackbar
        open={snackbar.open}
        autoHideDuration={6000}
        onClose={() => setSnackbar((s) => ({ ...s, open: false }))}
        anchorOrigin={{ vertical: "bottom", horizontal: "center" }}
      >
        <Alert
          onClose={() => setSnackbar((s) => ({ ...s, open: false }))}
          severity={snackbar.severity}
          sx={{ width: "100%" }}
        >
          {snackbar.message}
        </Alert>
      </Snackbar>
    </>
  );
};
