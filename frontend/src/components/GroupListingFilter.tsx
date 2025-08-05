import React, { useState } from "react";
import {
  Card,
  CardContent,
  Box,
  Typography,
  Button,
  TextField,
  Grid,
  InputAdornment,
  Collapse,
  Stack,
  FormControlLabel,
  Switch,
  IconButton,
} from "@mui/material";
import {
  Search as SearchIcon,
  Clear as ClearIcon,
  ExpandMore as ExpandMoreIcon,
  ExpandLess as ExpandLessIcon,
} from "@mui/icons-material";
import { GroupListingFilters } from "../types/types";

interface Props {
  filters: GroupListingFilters;
  loading: boolean;
  onFilterChange: (key: keyof GroupListingFilters, value: any) => void;
  onSearch: () => void;
  onClear: () => void;
}

export const GroupListingFilter: React.FC<Props> = ({
  filters,
  loading,
  onFilterChange,
  onSearch,
  onClear,
}) => {
  const [expanded, setExpanded] = useState(false);

  return (
    <Card sx={{ mb: 3, borderRadius: 2 }}>
      <CardContent>
        <Box
          sx={{
            display: "flex",
            justifyContent: "space-between",
            alignItems: "center",
          }}
        >
          <Typography
            variant="h6"
            sx={{ display: "flex", alignItems: "center", gap: 1 }}
          >
            <SearchIcon /> Filter Group Listings
          </Typography>
          <Button
            onClick={() => setExpanded((e) => !e)}
            endIcon={expanded ? <ExpandLessIcon /> : <ExpandMoreIcon />}
            sx={{ textTransform: "none" }}
          >
            {expanded ? "Hide Advanced Filters" : "Show Advanced Filters"}
          </Button>
        </Box>
        <Box sx={{ mt: 2 }}>
          <TextField
            fullWidth
            placeholder="Search by title or description…"
            value={filters.searchTerm || ""}
            onChange={(e) => onFilterChange("searchTerm", e.target.value)}
            InputProps={{
              startAdornment: (
                <InputAdornment position="start">
                  <SearchIcon />
                </InputAdornment>
              ),
              endAdornment: filters.searchTerm && (
                <InputAdornment position="end">
                  <IconButton onClick={() => onFilterChange("searchTerm", "")}>
                    <ClearIcon />
                  </IconButton>
                </InputAdornment>
              ),
            }}
          />
        </Box>
        <Collapse in={expanded}>
          <Box sx={{ mt: 3 }}>
            <Grid container spacing={2}>
              <Grid size={{ xs: 12, sm: 6 }}>
                <Typography variant="subtitle2" sx={{ mb: 1, fontWeight: 600 }}>
                  City
                </Typography>
                <Grid size={{ xs: 12, md: 6 }}>
                  <TextField
                    fullWidth
                    label="City"
                    value={filters.preferredCity || ""}
                    onChange={(e) =>
                      onFilterChange("preferredCity", e.target.value)
                    }
                  />
                </Grid>
              </Grid>
              <Grid size={{ xs: 12, md: 6 }}>
                <Typography variant="subtitle2" sx={{ mb: 1, fontWeight: 600 }}>
                  Budget Range (per month)
                </Typography>
                <Stack direction="row" spacing={1}>
                  <TextField
                    label="Min"
                    type="number"
                    fullWidth
                    value={filters.minBudget ?? ""}
                    onChange={(e) =>
                      onFilterChange(
                        "minBudget",
                        e.target.value ? Number(e.target.value) : undefined
                      )
                    }
                    InputProps={{
                      startAdornment: (
                        <InputAdornment position="start">PLN</InputAdornment>
                      ),
                    }}
                  />
                  <TextField
                    label="Max"
                    type="number"
                    fullWidth
                    value={filters.maxBudget ?? ""}
                    onChange={(e) =>
                      onFilterChange(
                        "maxBudget",
                        e.target.value ? Number(e.target.value) : undefined
                      )
                    }
                    InputProps={{
                      startAdornment: (
                        <InputAdornment position="start">PLN</InputAdornment>
                      ),
                    }}
                  />
                </Stack>
              </Grid>
              <Grid size={{ xs: 12, md: 6 }}>
                <Typography variant="subtitle2" sx={{ mb: 1, fontWeight: 600 }}>
                  Roommates Wanted
                </Typography>
                <Stack direction="row" spacing={1}>
                  <TextField
                    label="Min"
                    type="number"
                    fullWidth
                    value={filters.minRoommates ?? ""}
                    onChange={(e) =>
                      onFilterChange(
                        "minRoommates",
                        e.target.value ? Number(e.target.value) : undefined
                      )
                    }
                  />
                  <TextField
                    label="Max"
                    type="number"
                    fullWidth
                    value={filters.maxRoommates ?? ""}
                    onChange={(e) =>
                      onFilterChange(
                        "maxRoommates",
                        e.target.value ? Number(e.target.value) : undefined
                      )
                    }
                  />
                </Stack>
              </Grid>
              <Grid size={{ xs: 12 }}>
                <Stack direction="row" spacing={2}>
                  <FormControlLabel
                    control={
                      <Switch
                        checked={filters.hasProperty === true}
                        onChange={(e) =>
                          onFilterChange(
                            "hasProperty",
                            e.target.checked ? true : undefined
                          )
                        }
                      />
                    }
                    label="Has Property"
                  />
                  <FormControlLabel
                    control={
                      <Switch
                        checked={filters.hasRoom === true}
                        onChange={(e) =>
                          onFilterChange(
                            "hasRoom",
                            e.target.checked ? true : undefined
                          )
                        }
                      />
                    }
                    label="Has Room"
                  />
                  <FormControlLabel
                    control={
                      <Switch
                        checked={filters.noPropertyYet === true}
                        onChange={(e) =>
                          onFilterChange(
                            "noPropertyYet",
                            e.target.checked ? true : undefined
                          )
                        }
                      />
                    }
                    label="Property Not Secured Yet"
                  />
                </Stack>
              </Grid>
            </Grid>
          </Box>
        </Collapse>
        <Box sx={{ display: "flex", gap: 2, mt: 3 }}>
          <Button
            variant="contained"
            onClick={onSearch}
            disabled={loading}
            startIcon={<SearchIcon />}
            sx={{
              borderRadius: 2,
              background: "linear-gradient(45deg, #8E44AD 30%, #AF7AC5 90%)",
              "&:hover": {
                background: "linear-gradient(45deg, #7D3C98 30%, #9B59B6 90%)",
              },
            }}
          >
            {loading ? "Searching…" : "Search"}
          </Button>
          <Button
            variant="outlined"
            onClick={onClear}
            startIcon={<ClearIcon />}
            sx={{ borderRadius: 2 }}
          >
            Clear Filters
          </Button>
        </Box>
      </CardContent>
    </Card>
  );
};
