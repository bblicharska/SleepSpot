import React, { useState, useEffect } from "react";
import {
  Box,
  Button,
  TextField,
  Typography,
  Select,
  MenuItem,
  FormControl,
  InputLabel,
  Switch,
  FormControlLabel,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Chip,
  Alert,
  CircularProgress,
  Grid,
  Divider,
  InputAdornment,
  Paper,
  IconButton,
  Stack,
} from "@mui/material";
import {
  Add as AddIcon,
  AttachMoney as MoneyIcon,
  Email as EmailIcon,
  Close as CloseIcon,
} from "@mui/icons-material";
import { API_BASE_URL } from "../../types/types";
import { useAuth } from "../AuthContext";
import { fetchUserGroups } from "../../queries/fetchUserGroups";
import { createGroupListing } from "../../queries/createGroupListing";
import {
  GroupDto,
  CreateGroupDto,
  CreateGroupListingDto,
} from "../../types/types";

// Updated createGroup function with enhanced error handling
const createGroup = async (groupData: CreateGroupDto) => {
  const response = await fetch(`${API_BASE_URL}/api/groups`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(groupData),
  });

  if (!response.ok) {
    // Try to extract the error message from the response
    let errorMessage = "Failed to create group";

    try {
      const errorData = await response.json();
      // Handle different error response formats
      if (errorData.message) {
        errorMessage = errorData.message;
      } else if (errorData.title) {
        errorMessage = errorData.title;
      } else if (errorData.errors) {
        // Handle validation errors from ASP.NET Core
        errorMessage = Object.values(errorData.errors).flat().join(", ");
      }
    } catch {
      // If parsing fails, use the default message
    }

    throw new Error(errorMessage);
  }

  return response.json();
};

export const GroupListingCreationPage: React.FC = () => {
  const { user } = useAuth();

  const [formData, setFormData] = useState<CreateGroupListingDto>({
    groupId: "",
    title: "",
    description: "",
    desiredRoommatesCount: 1,
    propertyId: undefined,
    roomId: undefined,
    propertyAlreadyRented: false,
    preferredCity: "",
    maxBudgetPerPerson: undefined,
  });
  const [groups, setGroups] = useState<GroupDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [groupsLoading, setGroupsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);
  const [createGroupOpen, setCreateGroupOpen] = useState(false);
  const [newGroup, setNewGroup] = useState<CreateGroupDto>({
    name: "",
    description: "",
    createdByUserId: user?.userId || "",
    memberEmails: [],
  });
  const [newMemberEmail, setNewMemberEmail] = useState("");
  const [invalidEmails, setInvalidEmails] = useState<string[]>([]);

  useEffect(() => {
    loadUserGroups();
  }, []);

  const loadUserGroups = async () => {
    try {
      setGroupsLoading(true);
      setError(null);
      const userGroups = await fetchUserGroups(user?.userId);
      setGroups(userGroups);
    } catch (err) {
      setError("Failed to load groups. Please try again.");
      console.error("Error loading groups:", err);
    } finally {
      setGroupsLoading(false);
    }
  };

  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

  const handleInputChange = (
    field: keyof CreateGroupListingDto,
    value: any
  ) => {
    setFormData((prev) => ({
      ...prev,
      [field]: value,
    }));
    setError(null);
  };

  const handleAddMemberEmail = () => {
    const email = newMemberEmail.trim().toLowerCase();
    if (!email) return;

    if (!emailRegex.test(email)) {
      setError("Invalid email format");
      return;
    }

    if (newGroup.memberEmails.includes(email)) {
      setError("Email already added to the group");
      return;
    }

    setNewGroup((prev) => ({
      ...prev,
      memberEmails: [...prev.memberEmails, email],
    }));

    setNewMemberEmail("");
    setError(null);
    // Clear invalid email status when user adds a new email
    setInvalidEmails((prev) =>
      prev.filter((invalidEmail) => invalidEmail !== email)
    );
  };

  const handleRemoveMemberEmail = (email: string) => {
    setNewGroup((prev) => ({
      ...prev,
      memberEmails: prev.memberEmails.filter((e) => e !== email),
    }));
    // Clear invalid status when email is removed
    setInvalidEmails((prev) =>
      prev.filter((invalidEmail) => invalidEmail !== email)
    );
  };

  const handleCreateGroup = async () => {
    if (!newGroup.name.trim()) {
      setError("Group name is required");
      return;
    }

    try {
      setLoading(true);
      setError(null);
      setInvalidEmails([]); // Clear previous invalid emails

      const createdGroupId = await createGroup(newGroup);

      await loadUserGroups();

      setFormData((prev) => ({ ...prev, groupId: createdGroupId }));

      setCreateGroupOpen(false);

      setSuccess(`Group "${newGroup.name}" successfully created.`);
      setTimeout(() => setSuccess(null), 5000);

      setNewGroup({
        name: "",
        description: "",
        createdByUserId: user?.userId,
        memberEmails: [],
      });
    } catch (err: any) {
      const errorMessage =
        err.message || "Failed to create group. Please try again.";
      setError(errorMessage);

      // Extract invalid emails from error message for visual feedback
      if (errorMessage.includes("were not found:")) {
        const emailPart = errorMessage.split("were not found:")[1];
        if (emailPart) {
          const emails = emailPart
            .split(",")
            .map((email: string) => email.trim());
          setInvalidEmails(emails);
        }
      }

      console.error("Error when creating group:", err);
    } finally {
      setLoading(false);
    }
  };

  const handleSubmit = async () => {
    if (!formData.groupId || !formData.title.trim()) {
      setError("Please fill in all required fields");
      return;
    }

    if (!formData.preferredCity.trim()) {
      setError("Preferred city is required");
      return;
    }

    try {
      setLoading(true);
      setError(null);

      await createGroupListing(formData);

      setSuccess("Listing created successfully! 🎉");

      setTimeout(() => {
        setSuccess(null);
        setFormData({
          groupId: "",
          title: "",
          description: "",
          desiredRoommatesCount: 1,
          propertyId: undefined,
          roomId: undefined,
          propertyAlreadyRented: false,
          preferredCity: "",
          maxBudgetPerPerson: undefined,
        });
      }, 3000);
    } catch (err) {
      setError("Failed to create listing. Please try again.");
      console.error("Error creating listing:", err);
    } finally {
      setLoading(false);
    }
  };

  const handleCloseDialog = () => {
    setCreateGroupOpen(false);
    setNewGroup({
      name: "",
      description: "",
      createdByUserId: user?.userId,
      memberEmails: [],
    });
    setNewMemberEmail("");
    setError(null);
    setInvalidEmails([]);
  };

  return (
    <Box sx={{ maxWidth: 900, mx: "auto", p: 3 }}>
      <Typography
        variant="h4"
        gutterBottom
        sx={{ display: "flex", alignItems: "center", gap: 1, mb: 3 }}
      >
        Create Group Listing
      </Typography>
      {error && (
        <Alert severity="error" sx={{ mb: 3 }}>
          {error}
        </Alert>
      )}
      {success && (
        <Alert severity="success" sx={{ mb: 3 }}>
          {success}
        </Alert>
      )}
      <Paper elevation={3} sx={{ p: 4 }}>
        <Grid container spacing={4}>
          <Grid size={{ xs: 12 }}>
            <Typography
              variant="h6"
              gutterBottom
              sx={{ display: "flex", alignItems: "center", gap: 1 }}
            >
              Select or Create Group
            </Typography>
            <Box sx={{ display: "flex", gap: 2, alignItems: "flex-start" }}>
              <FormControl fullWidth required>
                <InputLabel>Select Group</InputLabel>
                <Select
                  value={formData.groupId}
                  onChange={(e) => handleInputChange("groupId", e.target.value)}
                  disabled={groupsLoading}
                  label="Select Group"
                >
                  {groups.map((group) => (
                    <MenuItem key={group.id} value={group.id}>
                      <Box sx={{ width: "100%" }}>
                        <Typography variant="body1">{group.name}</Typography>
                        <Typography variant="caption" color="text.secondary">
                          {group.members?.length || 0} members
                        </Typography>
                        {group.members && group.members.length > 1 && (
                          <Box sx={{ mt: 0.5 }}>
                            <Typography
                              variant="caption"
                              color="text.secondary"
                              sx={{ fontSize: "0.7rem" }}
                            >
                              {group.members
                                .filter((m) => m.user?.email)
                                .slice(0, 2)
                                .map((m) => m.user?.email)
                                .join(", ")}
                              {group.members.length > 3 && " +more"}
                            </Typography>
                          </Box>
                        )}
                      </Box>
                    </MenuItem>
                  ))}
                </Select>
                {groupsLoading && (
                  <Box
                    sx={{ display: "flex", justifyContent: "center", mt: 1 }}
                  >
                    <CircularProgress size={20} />
                  </Box>
                )}
              </FormControl>
              <Button
                variant="outlined"
                startIcon={<AddIcon />}
                onClick={() => setCreateGroupOpen(true)}
                sx={{ minWidth: "fit-content", height: 56 }}
              >
                New Group
              </Button>
            </Box>
          </Grid>
          <Grid size={{ xs: 12 }}>
            <Divider />
          </Grid>
          <Grid size={{ xs: 12 }}>
            <TextField
              fullWidth
              label="Listing Title"
              value={formData.title}
              onChange={(e) => handleInputChange("title", e.target.value)}
              required
            />
          </Grid>
          <Grid size={{ xs: 12 }}>
            <TextField
              fullWidth
              label="Description"
              multiline
              minRows={3}
              value={formData.description}
              onChange={(e) => handleInputChange("description", e.target.value)}
            />
          </Grid>
          <Grid size={{ xs: 12, sm: 6 }}>
            <TextField
              type="number"
              label="Desired Roommates Count"
              value={formData.desiredRoommatesCount}
              onChange={(e) =>
                handleInputChange(
                  "desiredRoommatesCount",
                  Number(e.target.value)
                )
              }
              inputProps={{ min: 1 }}
              fullWidth
            />
          </Grid>
          <Grid size={{ xs: 12, sm: 6 }}>
            <TextField
              label="Preferred City"
              value={formData.preferredCity}
              onChange={(e) =>
                handleInputChange("preferredCity", e.target.value)
              }
              fullWidth
              required
            />
          </Grid>
          <Grid size={{ xs: 12, sm: 6 }}>
            <TextField
              type="number"
              label="Max Budget Per Person"
              value={formData.maxBudgetPerPerson || ""}
              onChange={(e) =>
                handleInputChange("maxBudgetPerPerson", Number(e.target.value))
              }
              InputProps={{
                startAdornment: (
                  <InputAdornment position="start">
                    <MoneyIcon />
                  </InputAdornment>
                ),
              }}
              inputProps={{
                inputMode: "numeric",
                pattern: "[0-9]*",
              }}
              fullWidth
            />
          </Grid>
          <Grid size={{ xs: 12 }}>
            <FormControlLabel
              control={
                <Switch
                  checked={formData.propertyAlreadyRented}
                  onChange={(e) =>
                    handleInputChange("propertyAlreadyRented", e.target.checked)
                  }
                />
              }
              label="Property Already Rented"
            />
          </Grid>
          <Grid size={{ xs: 12 }}>
            <Button
              variant="contained"
              onClick={handleSubmit}
              disabled={loading}
              fullWidth
              size="large"
            >
              {loading ? <CircularProgress size={24} /> : "Create Listing"}
            </Button>
          </Grid>
        </Grid>
      </Paper>

      {/* Create Group Dialog */}
      <Dialog
        open={createGroupOpen}
        onClose={handleCloseDialog}
        maxWidth="sm"
        fullWidth
      >
        <DialogTitle>Create New Group</DialogTitle>
        <DialogContent>
          <TextField
            label="Group Name"
            value={newGroup.name}
            onChange={(e) =>
              setNewGroup((prev) => ({ ...prev, name: e.target.value }))
            }
            fullWidth
            required
            sx={{ mb: 2 }}
          />
          <TextField
            label="Description"
            value={newGroup.description}
            onChange={(e) =>
              setNewGroup((prev) => ({ ...prev, description: e.target.value }))
            }
            fullWidth
            multiline
            minRows={3}
            sx={{ mb: 2 }}
          />

          <Typography variant="subtitle1" gutterBottom>
            Add Members by Email
          </Typography>
          <Box sx={{ display: "flex", gap: 1, mb: 2 }}>
            <TextField
              label="Email"
              value={newMemberEmail}
              onChange={(e) => setNewMemberEmail(e.target.value)}
              fullWidth
              InputProps={{
                startAdornment: (
                  <InputAdornment position="start">
                    <EmailIcon />
                  </InputAdornment>
                ),
              }}
              onKeyDown={(e) => {
                if (e.key === "Enter") {
                  e.preventDefault();
                  handleAddMemberEmail();
                }
              }}
            />
            <Button variant="contained" onClick={handleAddMemberEmail}>
              Add
            </Button>
          </Box>

          <Stack direction="row" spacing={1} flexWrap="wrap">
            {newGroup.memberEmails.map((email) => (
              <Chip
                key={email}
                label={email}
                onDelete={() => handleRemoveMemberEmail(email)}
                color={invalidEmails.includes(email) ? "error" : "primary"}
                sx={{
                  mb: 1,
                  ...(invalidEmails.includes(email) && {
                    backgroundColor: "error.light",
                    "& .MuiChip-deleteIcon": {
                      color: "error.main",
                    },
                  }),
                }}
              />
            ))}
          </Stack>
        </DialogContent>
        <DialogActions>
          <Button onClick={handleCloseDialog} disabled={loading}>
            Cancel
          </Button>
          <Button
            onClick={handleCreateGroup}
            variant="contained"
            disabled={loading}
          >
            {loading ? <CircularProgress size={24} /> : "Create Group"}
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
};
