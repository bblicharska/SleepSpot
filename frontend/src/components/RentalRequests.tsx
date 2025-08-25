import React, { useEffect, useState, useCallback } from "react";
import {
  Box,
  Grid,
  Paper,
  Typography,
  CircularProgress,
  Button,
  Snackbar,
  Alert,
  Divider,
  Chip,
} from "@mui/material";
import RefreshIcon from "@mui/icons-material/Refresh";
import OpenInNewIcon from "@mui/icons-material/OpenInNew";
import DoneIcon from "@mui/icons-material/Done";
import CloseIcon from "@mui/icons-material/Close";
import { API_BASE_URL } from "../types/types";
import { useAuth } from "./AuthContext";
import { useNavigate } from "react-router-dom";
import { RentalAgreementDto } from "../types/types";
import { DeleteConfirmationDialog } from "./DeleteConfirmationDialog";

export const RentalRequests: React.FC = () => {
  const { user } = useAuth();
  const navigate = useNavigate();

  const [loading, setLoading] = useState<boolean>(true);
  const [agreements, setAgreements] = useState<RentalAgreementDto[]>([]);
  const [error, setError] = useState<string | null>(null);
  const [processingIds, setProcessingIds] = useState<Set<string>>(new Set());
  const [snackOpen, setSnackOpen] = useState(false);
  const [snackMsg, setSnackMsg] = useState("");
  const [snackSeverity, setSnackSeverity] = useState<
    "success" | "error" | "info" | "warning"
  >("success");
  const [terminateOpen, setTerminateOpen] = useState(false);
  const [terminateId, setTerminateId] = useState<string | null>(null);

  const showSnack = (
    message: string,
    severity: "success" | "error" | "info" | "warning" = "success"
  ) => {
    setSnackMsg(message);
    setSnackSeverity(severity);
    setSnackOpen(true);
  };

  const fetchAll = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const res = await fetch(`${API_BASE_URL}/api/rentals`);
      if (!res.ok) {
        const txt = await res.text();
        throw new Error(txt || "Failed to fetch rental requests");
      }
      const data: RentalAgreementDto[] = await res.json();
      setAgreements(data || []);
    } catch (err: any) {
      console.error("Failed to load rentals", err);
      setError(err?.message || "Failed to load rental requests");
      showSnack(err?.message || "Failed to load rental requests", "error");
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    fetchAll();
  }, [fetchAll]);

  const isOwnerOfAgreement = (a: RentalAgreementDto) => {
    const userId = user?.userId;
    if (!userId) return false;

    const propertyOwnerId =
      (a.status !== "Declined" && a.property?.ownerId) ??
      a.property?.owner?.id ??
      undefined;
    const roomOwnerId = a.room?.ownerId ?? undefined;

    return propertyOwnerId === userId || roomOwnerId === userId;
  };

  const ownerRequests = agreements.filter(isOwnerOfAgreement);

  const formatDate = (d?: string | null) =>
    d ? new Date(d).toLocaleDateString() : "-";

  const openPropertyOrRoom = (a: RentalAgreementDto) => {
    if (a.roomId) navigate(`/room/${a.roomId}`);
    else if (a.propertyId) navigate(`/property/${a.propertyId}`);
    else navigate("/properties");
  };

  const setProcessing = (id: string, processing: boolean) => {
    setProcessingIds((prev) => {
      const copy = new Set(prev);
      if (processing) copy.add(id);
      else copy.delete(id);
      return copy;
    });
  };

  const handleActivate = async (id: string) => {
    if (processingIds.has(id)) return;
    setProcessing(id, true);

    try {
      const res = await fetch(`${API_BASE_URL}/api/rentals/${id}/activate`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
      });
      if (!res.ok) {
        const text = await res.text();
        throw new Error(text || "Failed to accept request");
      }

      setAgreements((prev) =>
        prev.map((a) => (a.id === id ? { ...a, status: "Active" } : a))
      );
      showSnack("Rental request accepted", "success");
    } catch (err: any) {
      console.error("Failed to activate rental:", err);
      showSnack(err?.message || "Failed to accept request", "error");
    } finally {
      setProcessing(id, false);
    }
  };

  const handleDecline = async (id: string) => {
    if (processingIds.has(id)) return;
    setProcessing(id, true);

    try {
      const res = await fetch(`${API_BASE_URL}/api/rentals/${id}/decline`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
      });
      if (!res.ok) {
        const text = await res.text();
        throw new Error(text || "Failed to decline request");
      }

      setAgreements((prev) =>
        prev.map((a) => (a.id === id ? { ...a, status: "Declined" } : a))
      );
      showSnack("Rental request declined", "success");
    } catch (err: any) {
      console.error("Failed to decline rental:", err);
      showSnack(err?.message || "Failed to decline request", "error");
    } finally {
      setProcessing(id, false);
    }
  };

  const handleTerminateConfirm = async () => {
    if (!terminateId) return;
    const id = terminateId;
    setProcessing(id, true);

    try {
      const res = await fetch(`${API_BASE_URL}/api/rentals/${id}/terminate`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
      });
      if (!res.ok) {
        const text = await res.text();
        throw new Error(text || "Failed to terminate rental");
      }

      setAgreements((prev) =>
        prev.map((a) => (a.id === id ? { ...a, status: "Terminated" } : a))
      );
      showSnack("Rental terminated", "success");
    } catch (err: any) {
      console.error("Failed to terminate rental:", err);
      showSnack(err?.message || "Failed to terminate rental", "error");
    } finally {
      setProcessing(id, false);
      setTerminateId(null);
      setTerminateOpen(false);
    }
  };

  return (
    <Box sx={{ p: { xs: 2, md: 4 } }}>
      <Box
        display="flex"
        alignItems="center"
        justifyContent="space-between"
        mb={3}
      >
        <Typography variant="h4" gutterBottom>
          Rental Requests
        </Typography>
        <Box>
          <Button
            startIcon={<RefreshIcon />}
            onClick={fetchAll}
            disabled={loading}
            variant="outlined"
            size="small"
          >
            Refresh
          </Button>
        </Box>
      </Box>
      {loading ? (
        <Box display="flex" justifyContent="center" py={8}>
          <CircularProgress />
        </Box>
      ) : error ? (
        <Paper sx={{ p: 3 }}>
          <Typography color="error">Error: {error}</Typography>
          <Button onClick={fetchAll} sx={{ mt: 2 }} variant="contained">
            Retry
          </Button>
        </Paper>
      ) : ownerRequests.length === 0 ? (
        <Typography variant="body1" color="text.secondary">
          No rental requests for your properties/rooms.
        </Typography>
      ) : (
        <Grid container spacing={2}>
          {ownerRequests.map((a) => {
            const isPending = a.status?.toLowerCase() === "pending";
            const isActive = a.status?.toLowerCase() === "active";
            const processing = processingIds.has(a.id);
            return (
              <Grid size={{ xs: 12, md: 6 }} key={a.id}>
                <Paper sx={{ p: 2 }}>
                  <Box display="flex" justifyContent="space-between" mb={1}>
                    <Box>
                      <Typography variant="subtitle1" sx={{ fontWeight: 700 }}>
                        {a.room?.name ?? a.property?.name ?? "Unnamed"}
                      </Typography>
                      <Typography variant="body2" color="text.secondary">
                        {a.roomId
                          ? `Room • ${a.property?.name ?? ""}`
                          : a.propertyId
                          ? "Property"
                          : ""}
                      </Typography>
                    </Box>

                    <Box textAlign="right">
                      <Chip
                        label={a.status}
                        size="small"
                        sx={{
                          textTransform: "capitalize",
                          fontWeight: 700,
                        }}
                      />
                      <Typography variant="caption" display="block" mt={0.5}>
                        {new Date(a.createdAt).toLocaleString()}
                      </Typography>
                    </Box>
                  </Box>
                  <Divider sx={{ my: 1 }} />
                  <Box
                    display="flex"
                    justifyContent="space-between"
                    gap={2}
                    mb={1}
                  >
                    <Box>
                      <Typography variant="body2" color="text.secondary">
                        Tenant
                      </Typography>
                      <Typography variant="body1" sx={{ fontWeight: 600 }}>
                        {a.group?.name
                          ? `${a.group.name} (group)`
                          : a.user
                          ? `${a.user.firstName ?? ""} ${
                              a.user.lastName ?? ""
                            }`.trim()
                          : a.userId
                          ? `User ${a.userId}`
                          : "—"}
                      </Typography>
                      {a.user?.email && (
                        <Typography variant="caption" color="text.secondary">
                          {a.user.email}
                        </Typography>
                      )}
                    </Box>
                    <Box textAlign="right">
                      <Typography variant="body2" color="text.secondary">
                        Rent
                      </Typography>
                      <Typography variant="body1" sx={{ fontWeight: 700 }}>
                        {typeof a.monthlyRent === "number"
                          ? `PLN ${a.monthlyRent.toFixed(2)} /month`
                          : a.monthlyRent}
                      </Typography>
                    </Box>
                  </Box>
                  <Box display="flex" gap={2} flexWrap="wrap" mb={1}>
                    <Typography variant="body2" color="text.secondary">
                      Start:{" "}
                      <strong style={{ color: "inherit" }}>
                        {formatDate(a.startDate)}
                      </strong>
                    </Typography>
                    <Typography variant="body2" color="text.secondary">
                      End:{" "}
                      <strong style={{ color: "inherit" }}>
                        {formatDate(a.endDate)}
                      </strong>
                    </Typography>
                  </Box>
                  <Box display="flex" justifyContent="space-between" mt={1}>
                    <Box>
                      <Button
                        size="small"
                        startIcon={<OpenInNewIcon />}
                        onClick={() => openPropertyOrRoom(a)}
                      >
                        View
                      </Button>
                    </Box>
                    <Box>
                      {isPending && (
                        <>
                          <Button
                            size="small"
                            startIcon={
                              processing ? (
                                <CircularProgress size={16} />
                              ) : (
                                <DoneIcon />
                              )
                            }
                            onClick={() => handleActivate(a.id)}
                            disabled={processing}
                            sx={{ mr: 1 }}
                            color="success"
                            variant="contained"
                          >
                            Accept
                          </Button>
                          <Button
                            size="small"
                            startIcon={
                              processing ? (
                                <CircularProgress size={16} />
                              ) : (
                                <CloseIcon />
                              )
                            }
                            onClick={() => handleDecline(a.id)}
                            disabled={processing}
                            color="error"
                            variant="outlined"
                          >
                            Decline
                          </Button>
                        </>
                      )}
                      {isActive && (
                        <Button
                          size="small"
                          color="error"
                          variant="outlined"
                          onClick={() => {
                            setTerminateId(a.id);
                            setTerminateOpen(true);
                          }}
                        >
                          Terminate
                        </Button>
                      )}
                    </Box>
                  </Box>
                </Paper>
              </Grid>
            );
          })}
        </Grid>
      )}
      <Snackbar
        open={snackOpen}
        autoHideDuration={6000}
        onClose={() => setSnackOpen(false)}
        anchorOrigin={{ vertical: "bottom", horizontal: "center" }}
      >
        <Alert
          onClose={() => setSnackOpen(false)}
          severity={snackSeverity}
          sx={{ width: "100%" }}
        >
          {snackMsg}
        </Alert>
      </Snackbar>
      <DeleteConfirmationDialog
        open={terminateOpen}
        onClose={() => setTerminateOpen(false)}
        onConfirm={handleTerminateConfirm}
        title="Terminate Rental"
        message="Are you sure you want to terminate this rental? This action cannot be undone."
        loading={terminateId ? processingIds.has(terminateId) : false}
        variant="gradient"
      />
    </Box>
  );
};
