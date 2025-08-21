import React, { useEffect, useMemo, useState } from "react";
import {
  Box,
  Button,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
  Radio,
  RadioGroup,
  FormControlLabel,
  FormControl,
  FormLabel,
  FormHelperText,
  Select,
  MenuItem,
  InputLabel,
  CircularProgress,
  Typography,
  Snackbar,
  Alert,
} from "@mui/material";
import { API_BASE_URL } from "../types/types";
import { fetchUserGroups } from "../queries/fetchUserGroups";

export const RentalRequestModal = ({
  open,
  onClose,
  propertyId,
  roomId,
  currentUserId,
  availableSince,
}: {
  open: boolean;
  onClose: () => void;
  propertyId?: string | null;
  roomId?: string | null;
  currentUserId?: string | null;
  availableSince?: string | null;
}) => {
  const [isGroup, setIsGroup] = useState(false);
  const [startDate, setStartDate] = useState("");
  const [endDate, setEndDate] = useState("");
  const [monthlyRent, setMonthlyRent] = useState("");
  const [groupId, setGroupId] = useState("");
  const [groups, setGroups] = useState<{ id: string; name: string }[]>([]);
  const [groupsLoading, setGroupsLoading] = useState(false);
  const [groupsError, setGroupsError] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);
  const [errors, setErrors] = useState<{ [k: string]: string }>({});
  const [snackbarOpen, setSnackbarOpen] = useState(false);
  const [snackbarMessage, setSnackbarMessage] = useState("");
  const [snackbarSeverity, setSnackbarSeverity] = useState<
    "success" | "error" | "info" | "warning"
  >("success");

  useEffect(() => {
    if (!open) {
      setIsGroup(false);
      setStartDate("");
      setEndDate("");
      setMonthlyRent("");
      setGroupId("");
      setErrors({});
      setGroups([]);
      setGroupsError(null);
      setGroupsLoading(false);
    }
  }, [open]);

  useEffect(() => {
    let cancelled = false;
    const loadGroups = async () => {
      if (!isGroup) return;
      if (!currentUserId) {
        setGroups([]);
        setGroupsError("Log in to see your groups.");
        return;
      }
      setGroupsLoading(true);
      setGroupsError(null);
      try {
        const data = await fetchUserGroups(currentUserId);
        if (cancelled) return;
        setGroups(Array.isArray(data) ? data : []);
        if (Array.isArray(data) && data.length === 1) setGroupId(data[0].id);
      } catch (err: any) {
        if (cancelled) return;
        console.error("Failed to fetch groups:", err);
        setGroups([]);
        setGroupsError(err?.message || "Failed to load groups");
      } finally {
        if (!cancelled) setGroupsLoading(false);
      }
    };

    loadGroups();
    return () => {
      cancelled = true;
    };
  }, [isGroup, currentUserId]);

  const addMonthsSafe = (date: Date, months: number) => {
    const year = date.getFullYear();
    const targetMonth = date.getMonth() + months;
    const target = new Date(year, targetMonth, date.getDate());
    const expectedMonth = ((targetMonth % 12) + 12) % 12;
    if (target.getMonth() !== expectedMonth) {
      return new Date(year, targetMonth + 1, 0);
    }
    return target;
  };

  const formatToInputDate = (d: Date) => d.toISOString().slice(0, 10);

  const minEndDateIso = useMemo(() => {
    if (!startDate) return "";
    const s = new Date(startDate);
    s.setHours(0, 0, 0, 0);
    const min = addMonthsSafe(s, 1);
    return formatToInputDate(min);
  }, [startDate]);

  const isStartBeforeAvailable = (start: string, available?: string | null) => {
    if (!start || !available) return false;
    const s = new Date(start);
    const a = new Date(available);
    s.setHours(0, 0, 0, 0);
    a.setHours(0, 0, 0, 0);
    return s.getTime() < a.getTime();
  };

  const formattedAvailableSince = (available?: string | null) => {
    if (!available) return "";
    const d = new Date(available);
    return d.toLocaleDateString("en-US", {
      year: "numeric",
      month: "short",
      day: "numeric",
    });
  };

  const validate = () => {
    const e: { [k: string]: string } = {};
    if (!startDate) e.startDate = "Start date is required";
    if (!monthlyRent) e.monthlyRent = "Monthly rent is required";
    else if (isNaN(Number(monthlyRent)) || Number(monthlyRent) <= 0)
      e.monthlyRent = "Enter a valid positive rent";

    if (
      availableSince &&
      startDate &&
      isStartBeforeAvailable(startDate, availableSince)
    ) {
      e.startDate = `Start date cannot be earlier than ${formattedAvailableSince(
        availableSince
      )}`;
    }

    if (endDate && startDate) {
      const s = new Date(startDate);
      s.setHours(0, 0, 0, 0);
      const eDate = new Date(endDate);
      eDate.setHours(0, 0, 0, 0);
      const minAllowed = addMonthsSafe(s, 1);
      if (eDate.getTime() < minAllowed.getTime()) {
        e.endDate = `End date must be at least 1 month after start date (>= ${minAllowed.toLocaleDateString()})`;
      }
    }

    if (isGroup) {
      if (!groupId) e.groupId = "Please select a group for a group rental";
    } else {
      if (!currentUserId)
        e.general = "You must be logged in to create an individual rental";
    }

    setErrors(e);
    return Object.keys(e).length === 0;
  };

  const showSnackbar = (
    message: string,
    severity: "success" | "error" | "info" | "warning" = "success"
  ) => {
    setSnackbarMessage(message);
    setSnackbarSeverity(severity);
    setSnackbarOpen(true);
  };

  const handleSubmit = async () => {
    if (!validate()) return;
    setSubmitting(true);

    try {
      const payload: any = {
        startDate: new Date(startDate).toISOString(),
        monthlyRent: parseFloat(monthlyRent),
        status: "Pending",
      };

      if (endDate) payload.endDate = new Date(endDate).toISOString();
      if (propertyId) payload.propertyId = propertyId;
      if (roomId) payload.roomId = roomId;

      if (isGroup) payload.groupId = groupId;
      else payload.userId = currentUserId;

      const res = await fetch(`${API_BASE_URL}/api/rentals`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(payload),
      });

      if (!res.ok) {
        const text = await res.text();
        throw new Error(text || "Failed to create rental");
      }

      showSnackbar("Rental request sent!", "success");

      setTimeout(() => {
        setSubmitting(false);
        onClose();
      }, 900);
    } catch (err) {
      console.error(err);
      const message =
        err && (err as any).message
          ? (err as any).message
          : "Error sending rental request";
      showSnackbar(message, "error");
      setSubmitting(false);
    }
  };

  const startTooEarly = !!(
    startDate &&
    availableSince &&
    isStartBeforeAvailable(startDate, availableSince)
  );

  const endTooShort = !!(
    endDate &&
    startDate &&
    minEndDateIso &&
    endDate < minEndDateIso
  );

  return (
    <>
      <Dialog open={open} onClose={onClose} fullWidth maxWidth="sm">
        <DialogTitle>Send Rental Request</DialogTitle>
        <DialogContent>
          <Box sx={{ display: "flex", flexDirection: "column", gap: 2, mt: 1 }}>
            <FormControl component="fieldset">
              <FormLabel component="legend">Rental Type</FormLabel>
              <RadioGroup
                row
                value={isGroup ? "group" : "individual"}
                onChange={(e) => setIsGroup(e.target.value === "group")}
              >
                <FormControlLabel
                  value="individual"
                  control={<Radio />}
                  label="Individual"
                />
                <FormControlLabel
                  value="group"
                  control={<Radio />}
                  label="Group"
                />
              </RadioGroup>
            </FormControl>
            {isGroup ? (
              <FormControl fullWidth error={!!errors.groupId || !!groupsError}>
                <InputLabel id="group-select-label">Select Group</InputLabel>
                <Select
                  labelId="group-select-label"
                  value={groupId}
                  label="Select Group"
                  onChange={(e) => setGroupId(e.target.value as string)}
                  disabled={groupsLoading || !currentUserId}
                  renderValue={(selected) => {
                    const found = groups.find((g) => g.id === selected);
                    return found ? found.name : "";
                  }}
                >
                  {groupsLoading ? (
                    <MenuItem disabled>
                      <Box
                        sx={{ display: "flex", alignItems: "center", gap: 1 }}
                      >
                        <CircularProgress size={18} />
                        <Typography variant="body2">
                          Loading groups...
                        </Typography>
                      </Box>
                    </MenuItem>
                  ) : groups.length > 0 ? (
                    groups.map((g) => (
                      <MenuItem key={g.id} value={g.id}>
                        {g.name}
                      </MenuItem>
                    ))
                  ) : (
                    <MenuItem disabled>
                      {groupsError ? groupsError : "No groups found"}
                    </MenuItem>
                  )}
                </Select>
                <FormHelperText>
                  {errors.groupId
                    ? errors.groupId
                    : currentUserId
                    ? "Choose one of your groups"
                    : "Log in to choose a group"}
                </FormHelperText>
              </FormControl>
            ) : (
              <FormControl>
                <FormHelperText sx={{ mb: 0.5 }}>
                  {errors.general
                    ? errors.general
                    : currentUserId
                    ? "Your account will be used for this request"
                    : "You must be logged in to create an individual rental"}
                </FormHelperText>
              </FormControl>
            )}
            <TextField
              type="date"
              label="Start Date"
              InputLabelProps={{ shrink: true }}
              value={startDate}
              onChange={(e) => {
                setStartDate(e.target.value);
                setErrors((prev) => {
                  const copy = { ...prev };
                  delete copy.endDate;
                  return copy;
                });
              }}
              error={!!errors.startDate}
              helperText={
                errors.startDate
                  ? errors.startDate
                  : availableSince
                  ? `Earliest allowed start: ${formattedAvailableSince(
                      availableSince
                    )}`
                  : ""
              }
            />
            <TextField
              type="date"
              label="End Date (optional)"
              InputLabelProps={{ shrink: true }}
              value={endDate}
              onChange={(e) => setEndDate(e.target.value)}
              error={!!errors.endDate || endTooShort}
              helperText={
                errors.endDate
                  ? errors.endDate
                  : minEndDateIso
                  ? `Minimum end date: ${new Date(
                      minEndDateIso
                    ).toLocaleDateString()}`
                  : ""
              }
              inputProps={minEndDateIso ? { min: minEndDateIso } : undefined}
            />
            <TextField
              type="number"
              label="Monthly Rent"
              value={monthlyRent}
              onChange={(e) => setMonthlyRent(e.target.value)}
              error={!!errors.monthlyRent}
              helperText={errors.monthlyRent}
            />
            <FormControl>
              <FormHelperText>
                {propertyId
                  ? "This request will be associated with the provided propertyId."
                  : roomId
                  ? "This request will be associated with the provided roomId."
                  : "No propertyId or roomId provided — pass one from the route (property/:id or room/:id)."}
              </FormHelperText>
            </FormControl>
          </Box>
        </DialogContent>
        <DialogActions>
          <Button onClick={onClose} color="secondary" disabled={submitting}>
            Cancel
          </Button>
          <Button
            onClick={handleSubmit}
            variant="contained"
            color="primary"
            disabled={
              submitting ||
              (isGroup && (groupsLoading || (!groupId && groups.length > 0))) ||
              (!isGroup && !currentUserId) ||
              startTooEarly ||
              endTooShort
            }
          >
            {submitting ? "Sending..." : "Send"}
          </Button>
        </DialogActions>
      </Dialog>
      <Snackbar
        open={snackbarOpen}
        autoHideDuration={6000}
        onClose={() => setSnackbarOpen(false)}
        anchorOrigin={{ vertical: "bottom", horizontal: "left" }}
      >
        <Alert
          onClose={() => setSnackbarOpen(false)}
          severity={snackbarSeverity}
          sx={{ width: "100%" }}
        >
          {snackbarMessage}
        </Alert>
      </Snackbar>
    </>
  );
};
