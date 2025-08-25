import React, { useEffect, useState } from "react";
import {
  Card,
  CardContent,
  Typography,
  CircularProgress,
  Alert,
  Tabs,
  Tab,
  Box,
  List,
  ListItem,
  Divider,
  Chip,
  Stack,
  IconButton,
} from "@mui/material";
import DeleteIcon from "@mui/icons-material/Delete";
import {
  API_BASE_URL,
  RentalAgreementDto,
  RoomApplicationDto,
  GroupListingDto,
} from "../types/types";
import { useAuth } from "./AuthContext";

const statusColor = (status: string) => {
  if (!status) return "default";
  switch (status.toLowerCase()) {
    case "active":
      return "success";
    case "pending":
      return "warning";
    case "declined":
    case "rejected":
      return "error";
    default:
      return "default";
  }
};

const prettyStatus = (status: string) =>
  status ? status.charAt(0).toUpperCase() + status.slice(1).toLowerCase() : "";

export const MyRequests: React.FC = () => {
  const { user } = useAuth();
  const [rentals, setRentals] = useState<RentalAgreementDto[]>([]);
  const [applications, setApplications] = useState<RoomApplicationDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [tab, setTab] = useState(0);
  const [listingDetails, setListingDetails] = useState<
    Record<string, GroupListingDto>
  >({});

  const fetchData = async () => {
    setLoading(true);
    setError(null);
    try {
      const [rentalsRes, appsRes] = await Promise.all([
        fetch(`${API_BASE_URL}/api/rentals/user/${user?.userId}`),
        fetch(
          `${API_BASE_URL}/api/groups/applications/applicant/${user?.userId}`
        ),
      ]);

      if (!rentalsRes.ok || !appsRes.ok) {
        throw new Error("Failed to fetch user requests");
      }

      const rentalsData: RentalAgreementDto[] = await rentalsRes.json();
      const appsData: RoomApplicationDto[] = await appsRes.json();

      setRentals(rentalsData);
      setApplications(appsData);

      const listingResponses = await Promise.all(
        appsData.map((app) =>
          fetch(`${API_BASE_URL}/api/groups/listings/${app.listingId}`)
        )
      );

      const listingsData: GroupListingDto[] = await Promise.all(
        listingResponses.map((res) => res.json())
      );

      const listingsMap: Record<string, GroupListingDto> = {};
      listingsData.forEach((listing) => {
        listingsMap[listing.id] = listing;
      });

      setListingDetails(listingsMap);
    } catch (err: any) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (user?.userId) fetchData();
  }, [user]);

  const handleTabChange = (e: React.SyntheticEvent, newValue: number) =>
    setTab(newValue);

  const handleDeleteRental = async (id: string) => {
    try {
      await fetch(`${API_BASE_URL}/api/rentals/${id}`, { method: "DELETE" });
      setRentals((prev) => prev.filter((r) => r.id !== id));
    } catch (err) {
      console.error("Failed to delete rental", err);
    }
  };

  const handleDeleteApplication = async (applicationId: string) => {
    try {
      await fetch(`${API_BASE_URL}/api/groups/applications/${applicationId}`, {
        method: "DELETE",
      });
      setApplications((prev) => prev.filter((a) => a.id !== applicationId));
    } catch (err) {
      console.error("Failed to delete application", err);
    }
  };

  if (loading) return <CircularProgress sx={{ display: "block", m: 2 }} />;
  if (error) return <Alert severity="error">{error}</Alert>;

  return (
    <Card sx={{ maxWidth: 900, mx: "auto", mt: 4 }}>
      <CardContent>
        <Typography variant="h5" gutterBottom>
          My Requests
        </Typography>

        <Tabs value={tab} onChange={handleTabChange}>
          <Tab label="Rental Requests" />
          <Tab label="Group Applications" />
        </Tabs>
        <Box sx={{ mt: 2 }}>
          {tab === 0 && (
            <List>
              {rentals.length > 0 ? (
                rentals.map((r) => (
                  <React.Fragment key={r.id}>
                    <ListItem disableGutters sx={{ py: 1.5 }}>
                      <Box
                        sx={{
                          display: "grid",
                          gridTemplateColumns: "1fr auto",
                          columnGap: 1.5,
                          rowGap: 0.75,
                          width: "100%",
                          alignItems: "center",
                        }}
                      >
                        {/* Header (main info + status chip) */}
                        <Stack direction="row" spacing={1} alignItems="center">
                          <Typography
                            variant="subtitle1"
                            fontWeight={700}
                            noWrap
                          >
                            {r.room?.name ||
                              r.property?.name ||
                              r.group?.name ||
                              "N/A"}
                          </Typography>
                          <Chip
                            size="small"
                            label={prettyStatus(r.status)}
                            color={statusColor(r.status)}
                          />
                        </Stack>
                        {r.status?.toLowerCase() === "pending" ? (
                          <IconButton
                            color="error"
                            onClick={() => handleDeleteRental(r.id)}
                            sx={{ justifySelf: "end" }}
                            aria-label="Delete pending rental"
                          >
                            <DeleteIcon />
                          </IconButton>
                        ) : (
                          <Box />
                        )}
                        <Stack spacing={0.5} sx={{ gridColumn: "1 / -1" }}>
                          {r.room?.name && (
                            <Typography variant="body2">
                              Room: {r.room.name}
                            </Typography>
                          )}
                          {r.property?.name && (
                            <Typography variant="body2">
                              Property: {r.property.name}
                            </Typography>
                          )}
                          {r.group?.name && (
                            <Typography variant="body2">
                              Group: {r.group.name}
                            </Typography>
                          )}
                          <Typography variant="body2">
                            Monthly Rent: PLN{" "}
                            {Number(r.monthlyRent).toLocaleString()}
                          </Typography>
                          <Typography
                            variant="body2"
                            fontWeight={600}
                            color="primary"
                          >
                            Period: {new Date(r.startDate).toLocaleDateString()}{" "}
                            →{" "}
                            {r.endDate
                              ? new Date(r.endDate).toLocaleDateString()
                              : "Open-ended"}
                          </Typography>
                          <Typography
                            variant="caption"
                            color="secondary"
                            fontWeight={500}
                          >
                            Created: {new Date(r.createdAt).toLocaleString()}
                          </Typography>
                        </Stack>
                      </Box>
                    </ListItem>
                    <Divider />
                  </React.Fragment>
                ))
              ) : (
                <Typography>No rental agreements found.</Typography>
              )}
            </List>
          )}
          {tab === 1 && (
            <List>
              {applications.length > 0 ? (
                applications.map((app) => (
                  <React.Fragment key={app.id}>
                    <ListItem disableGutters sx={{ py: 1.5 }}>
                      <Box
                        sx={{
                          display: "grid",
                          gridTemplateColumns: "1fr auto",
                          columnGap: 1.5,
                          rowGap: 0.75,
                          width: "100%",
                          alignItems: "center",
                        }}
                      >
                        <Stack direction="row" spacing={1} alignItems="center">
                          <Typography
                            variant="subtitle1"
                            fontWeight={700}
                            noWrap
                          >
                            {listingDetails[app.listingId]?.group?.name ||
                              "Listing"}
                          </Typography>
                          <Chip
                            size="small"
                            label={prettyStatus(app.status)}
                            color={statusColor(app.status)}
                          />
                        </Stack>
                        {app.status?.toLowerCase() === "pending" ? (
                          <IconButton
                            color="error"
                            onClick={() => handleDeleteApplication(app.id)}
                            sx={{ justifySelf: "end" }}
                            aria-label="Delete pending application"
                          >
                            <DeleteIcon />
                          </IconButton>
                        ) : (
                          <Box />
                        )}
                        <Stack spacing={0.5} sx={{ gridColumn: "1 / -1" }}>
                          {app.message && (
                            <Typography variant="body2">
                              Message: {app.message}
                            </Typography>
                          )}
                          <Typography
                            variant="caption"
                            color="secondary"
                            fontWeight={500}
                          >
                            Submitted:{" "}
                            {new Date(app.createdAt).toLocaleString()}
                          </Typography>
                        </Stack>
                      </Box>
                    </ListItem>
                    <Divider />
                  </React.Fragment>
                ))
              ) : (
                <Typography>No room applications found.</Typography>
              )}
            </List>
          )}
        </Box>
      </CardContent>
    </Card>
  );
};
