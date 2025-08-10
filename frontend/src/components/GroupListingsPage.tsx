import React, { useState, useEffect } from "react";
import {
  Box,
  Grid,
  Typography,
  Button,
  Chip,
  Card,
  CardContent,
  CardActions,
  FormControl,
  Select,
  MenuItem,
  Pagination,
  Stack,
  Snackbar,
  Alert,
} from "@mui/material";
import {
  Sort as SortIcon,
  SwapVert as SwapVertIcon,
  FilterList as FilterListIcon,
  Search as SearchIcon,
} from "@mui/icons-material";
import LocationOnIcon from "@mui/icons-material/LocationOn";
import PeopleIcon from "@mui/icons-material/People";
import EuroIcon from "@mui/icons-material/Euro";
import VisibilityIcon from "@mui/icons-material/Visibility";
import CalendarTodayIcon from "@mui/icons-material/CalendarToday";
import { useNavigate } from "react-router-dom";
import { LoadingComponent } from "../components/LoadingComponent";
import { ErrorComponent } from "../components/ErrorComponent";
import { useAuth } from "../components/AuthContext";
import {
  GroupListingDto,
  PagedResult,
  GroupListingFilters,
} from "../types/types";
import { fetchListings } from "../queries/fetchListings";
import { GroupListingFilter } from "../components/GroupListingFilter";

export const GroupListingsPage: React.FC = () => {
  const [listings, setListings] = useState<GroupListingDto[]>([]);
  const [pagedResult, setPagedResult] =
    useState<PagedResult<GroupListingDto> | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [filters, setFilters] = useState<GroupListingFilters>({
    page: 1,
    pageSize: 12,
    sortBy: "CreatedAt",
    sortOrder: "desc",
  });
  const [pendingFilters, setPendingFilters] =
    useState<GroupListingFilters>(filters);
  const [snackbar, setSnackbar] = useState<{
    open: boolean;
    message: string;
    severity: "success" | "error";
  }>({ open: false, message: "", severity: "success" });

  const navigate = useNavigate();
  const { loading: authLoading } = useAuth();

  const loadListings = async () => {
    try {
      setLoading(true);
      setError(null);
      const { listings, pagedResult } = await fetchListings(filters);
      setListings(listings);
      setPagedResult(pagedResult);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to load");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (!authLoading) {
      loadListings();
    }
  }, [filters, authLoading]);

  const handlePendingChange = (key: keyof GroupListingFilters, value: any) => {
    setPendingFilters((p) => ({ ...p, [key]: value }));
  };
  const applySearch = () => setFilters({ ...pendingFilters, page: 1 });
  const clearAll = () => {
    const base: GroupListingFilters = {
      page: 1,
      pageSize: 12,
      sortBy: "CreatedAt",
      sortOrder: "desc",
    };
    setPendingFilters(base);
    setFilters(base);
  };

  const handlePage = (_: any, v: number) =>
    setFilters((f) => ({ ...f, page: v }));
  const handleViewDetails = (id: string) => navigate(`/group-listings/${id}`);
  const formatDate = (d: string) => new Date(d).toLocaleDateString("en-GB");

  const InlineSortControls = ({ resultCount }: { resultCount: number }) => (
    <Box
      sx={{
        display: "flex",
        alignItems: "center",
        gap: 2,
        py: 2,
        borderBottom: "1px solid #e0e0e0",
        mb: 3,
      }}
    >
      <SortIcon sx={{ color: "#8E44AD" }} />
      <Typography variant="body2" sx={{ color: "#6C3483", fontWeight: 500 }}>
        Sort by:
      </Typography>
      <FormControl size="small" variant="outlined" sx={{ minWidth: 140 }}>
        <Select
          value={filters.sortBy}
          onChange={(e) =>
            setFilters((f) => ({ ...f, sortBy: e.target.value as any }))
          }
          sx={{
            "& .MuiOutlinedInput-notchedOutline": {
              borderColor: "#D2B4DE",
            },
            "&:hover .MuiOutlinedInput-notchedOutline": {
              borderColor: "#8E44AD",
            },
          }}
        >
          <MenuItem value="CreatedAt">Date Created</MenuItem>
          <MenuItem value="Title">Title</MenuItem>
          <MenuItem value="MaxBudgetPerPerson">Budget</MenuItem>
          <MenuItem value="DesiredRoommatesCount">Roommates</MenuItem>
          <MenuItem value="PreferredCity">City</MenuItem>
        </Select>
      </FormControl>
      <Button
        variant="outlined"
        size="small"
        startIcon={<SwapVertIcon />}
        onClick={() =>
          setFilters((f) => ({
            ...f,
            sortOrder: f.sortOrder === "asc" ? "desc" : "asc",
          }))
        }
        sx={{
          textTransform: "none",
          borderColor: "#8E44AD",
          color: "#8E44AD",
          "&:hover": {
            backgroundColor: "rgba(142, 68, 173, 0.1)",
            borderColor: "#8E44AD",
          },
        }}
      >
        {filters.sortOrder === "asc" ? "Ascending" : "Descending"}
      </Button>
      <Chip
        label={`${resultCount} results`}
        size="small"
        sx={{ backgroundColor: "#8E44AD", color: "white" }}
      />
    </Box>
  );

  if (loading) return <LoadingComponent text="Loading…" />;

  if (error && listings.length === 0)
    return (
      <ErrorComponent onClick={loadListings} error={error} text="listings" />
    );

  return (
    <Box>
      <Box mb={4} textAlign="center">
        <Typography variant="h4" sx={{ color: "#8E44AD", fontWeight: 600 }}>
          Group Listings
        </Typography>
        <Typography variant="subtitle1" color="text.secondary">
          Find groups looking for roommates
        </Typography>
      </Box>
      <GroupListingFilter
        filters={pendingFilters}
        loading={loading}
        onFilterChange={handlePendingChange}
        onSearch={applySearch}
        onClear={clearAll}
      />
      <Box display="flex" alignItems="center" gap={2} mb={2}>
        <InlineSortControls resultCount={listings.length} />
        <Box flexGrow={1} />
      </Box>
      {listings.length === 0 ? (
        <Box textAlign="center" mt={4}>
          <Typography variant="h5" color="text.secondary">
            No group listings found
          </Typography>
          <Button onClick={loadListings} startIcon={<SearchIcon />}>
            Refresh
          </Button>
        </Box>
      ) : (
        <>
          <Grid container spacing={3}>
            {listings.map((listing) => (
              <Grid size={{ xs: 12, sm: 6, md: 4 }} key={listing.id}>
                <Card
                  sx={{
                    position: "relative",
                    height: "100%",
                    display: "flex",
                    flexDirection: "column",
                    transition: "transform 0.2s, box-shadow 0.2s",
                    "&:hover": { transform: "translateY(-4px)", boxShadow: 4 },
                  }}
                >
                  <CardContent sx={{ flexGrow: 1, p: 2 }}>
                    <Box
                      display="flex"
                      alignItems="center"
                      justifyContent="space-between"
                      mb={1}
                    >
                      <Typography variant="h6" sx={{ flexGrow: 1, mr: 1 }}>
                        {listing.title}
                      </Typography>
                    </Box>
                    <Stack spacing={1} mb={2}>
                      <Box display="flex" alignItems="center" gap={1}>
                        <Chip
                          label={
                            listing.propertyAlreadyRented
                              ? "Property Secured - Roommates Wanted"
                              : "Seeking Property & Roommates"
                          }
                          color={
                            listing.propertyAlreadyRented ? "error" : "success"
                          }
                          size="small"
                        />
                      </Box>
                      <Box display="flex" alignItems="center" gap={1}>
                        <LocationOnIcon fontSize="small" />
                        <Typography variant="body2">
                          {listing.preferredCity}
                        </Typography>
                      </Box>
                      <Box display="flex" alignItems="center" gap={1}>
                        <PeopleIcon fontSize="small" />
                        <Typography variant="body2">
                          Looking for {listing.desiredRoommatesCount} roommate
                          {listing.desiredRoommatesCount > 1 ? "s" : ""}
                        </Typography>
                      </Box>
                      {listing.maxBudgetPerPerson && (
                        <Box display="flex" alignItems="center" gap={1}>
                          <EuroIcon fontSize="small" />
                          <Typography variant="body2">
                            Max {listing.maxBudgetPerPerson} PLN/person
                          </Typography>
                        </Box>
                      )}
                      <Box display="flex" alignItems="center" gap={1}>
                        <CalendarTodayIcon fontSize="small" />
                        <Typography variant="body2">
                          Posted {formatDate(listing.createdAt)}
                        </Typography>
                      </Box>
                    </Stack>
                  </CardContent>
                  <CardActions sx={{ p: 2, pt: 0 }}>
                    <Button
                      variant="contained"
                      fullWidth
                      onClick={() => handleViewDetails(listing.id)}
                      sx={{
                        textTransform: "none",
                        borderRadius: 2,
                        fontWeight: 600,
                        fontSize: "0.95rem",
                        padding: "12px 24px",
                        background:
                          "linear-gradient(45deg, #8E44AD 30%, #AF7AC5 90%)",
                        boxShadow: "0 3px 5px 2px rgba(142,68,173,0.3)",
                        "&:hover": {
                          background:
                            "linear-gradient(45deg, #6A1B9A 30%, #8E44AD 90%)",
                          boxShadow: "0 6px 10px 4px rgba(142,68,173,0.3)",
                          transform: "translateY(-2px)",
                        },
                        "&:active": { transform: "translateY(0)" },
                      }}
                      startIcon={<VisibilityIcon />}
                    >
                      View Details
                    </Button>
                  </CardActions>
                </Card>
              </Grid>
            ))}
          </Grid>
          {pagedResult && pagedResult.totalPages > 1 && (
            <Box display="flex" justifyContent="center" mt={4}>
              <Pagination
                count={pagedResult.totalPages}
                page={pagedResult.page}
                onChange={handlePage}
              />
            </Box>
          )}
        </>
      )}
      <Snackbar
        open={snackbar.open}
        autoHideDuration={6000}
        onClose={() => setSnackbar((prev) => ({ ...prev, open: false }))}
        anchorOrigin={{ vertical: "bottom", horizontal: "center" }}
      >
        <Alert
          onClose={() => setSnackbar((prev) => ({ ...prev, open: false }))}
          severity={snackbar.severity}
          sx={{ width: "100%" }}
        >
          {snackbar.message}
        </Alert>
      </Snackbar>
    </Box>
  );
};
