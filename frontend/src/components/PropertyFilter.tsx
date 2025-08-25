import React, { useState } from "react";
import {
  Card,
  CardContent,
  TextField,
  Button,
  Grid,
  Typography,
  Box,
  Collapse,
  InputAdornment,
} from "@mui/material";
import {
  ExpandMore,
  ExpandLess,
  Search,
  Clear,
  Straighten,
  CalendarToday,
} from "@mui/icons-material";
import { PropertyFilterDto } from "../types/types";

interface PropertyFilterProps {
  onFilter: (filters: PropertyFilterDto) => void;
  onClear: () => void;
  loading?: boolean;
}

export const PropertyFilter: React.FC<PropertyFilterProps> = ({
  onFilter,
  onClear,
  loading = false,
}) => {
  const [expanded, setExpanded] = useState(false);
  const [filters, setFilters] = useState<PropertyFilterDto>({
    location: "",
    minPrice: undefined,
    maxPrice: undefined,
    minArea: undefined,
    maxArea: undefined,
    availableSince: "",
    isAvailable: true,
    isEntirePlaceRentable: true,
  });

  const handleInputChange =
    (field: keyof PropertyFilterDto) =>
    (event: React.ChangeEvent<HTMLInputElement>) => {
      const value = event.target.value;
      setFilters((prev) => ({
        ...prev,
        [field]:
          field === "location" || field === "availableSince"
            ? value
            : value === ""
            ? undefined
            : Number(value),
      }));
    };

  const handleSubmit = () => {
    const cleanFilters = Object.fromEntries(
      Object.entries(filters).filter(([key, value]) => {
        if (typeof value === "boolean") return true;

        return value !== "" && value !== undefined && value !== null;
      })
    ) as PropertyFilterDto;

    onFilter(cleanFilters);
  };

  const handleClear = () => {
    setFilters({
      location: "",
      minPrice: undefined,
      maxPrice: undefined,
      minArea: undefined,
      maxArea: undefined,
      availableSince: "",
      isAvailable: true,
      isEntirePlaceRentable: true,
    });
    onClear();
  };

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
            <Search /> Filter Apartments
          </Typography>
          <Button
            onClick={() => setExpanded(!expanded)}
            endIcon={expanded ? <ExpandLess /> : <ExpandMore />}
            sx={{ textTransform: "none" }}
          >
            {expanded ? "Hide Advanced Filters" : "Show Advanced Filters"}
          </Button>
        </Box>
        <Box sx={{ mt: 2 }}>
          <TextField
            fullWidth
            label="Location"
            placeholder="Search by address..."
            value={filters.location}
            onChange={handleInputChange("location")}
            InputProps={{
              startAdornment: (
                <InputAdornment position="start">
                  <Search />
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
                  Price Range (per month)
                </Typography>
                <Box sx={{ display: "flex", gap: 1 }}>
                  <TextField
                    label="Min Price"
                    type="number"
                    value={filters.minPrice || ""}
                    onChange={handleInputChange("minPrice")}
                    InputProps={{
                      startAdornment: (
                        <InputAdornment position="start">PLN</InputAdornment>
                      ),
                    }}
                  />
                  <TextField
                    label="Max Price"
                    type="number"
                    value={filters.maxPrice || ""}
                    onChange={handleInputChange("maxPrice")}
                    InputProps={{
                      startAdornment: (
                        <InputAdornment position="start">PLN</InputAdornment>
                      ),
                    }}
                  />
                </Box>
              </Grid>
              <Grid size={{ xs: 12, sm: 6 }}>
                <Typography variant="subtitle2" sx={{ mb: 1, fontWeight: 600 }}>
                  Area Range (m²)
                </Typography>
                <Box sx={{ display: "flex", gap: 1 }}>
                  <TextField
                    label="Min Area"
                    type="number"
                    value={filters.minArea || ""}
                    onChange={handleInputChange("minArea")}
                    InputProps={{
                      endAdornment: (
                        <InputAdornment position="end">m²</InputAdornment>
                      ),
                      startAdornment: (
                        <InputAdornment position="start">
                          <Straighten />
                        </InputAdornment>
                      ),
                    }}
                  />
                  <TextField
                    label="Max Area"
                    type="number"
                    value={filters.maxArea || ""}
                    onChange={handleInputChange("maxArea")}
                    InputProps={{
                      endAdornment: (
                        <InputAdornment position="end">m²</InputAdornment>
                      ),
                      startAdornment: (
                        <InputAdornment position="start">
                          <Straighten />
                        </InputAdornment>
                      ),
                    }}
                  />
                </Box>
              </Grid>
              <Grid size={{ xs: 12, sm: 6 }}>
                <Typography variant="subtitle2" sx={{ mb: 1, fontWeight: 600 }}>
                  Available From
                </Typography>
                <TextField
                  fullWidth
                  label="Available From"
                  type="date"
                  value={filters.availableSince || ""}
                  onChange={handleInputChange("availableSince")}
                  InputLabelProps={{
                    shrink: true,
                  }}
                  InputProps={{
                    lang: "en",
                    startAdornment: (
                      <InputAdornment position="start">
                        <CalendarToday />
                      </InputAdornment>
                    ),
                  }}
                  helperText="Find properties available from this date onwards"
                />
              </Grid>
            </Grid>
          </Box>
        </Collapse>
        <Box sx={{ display: "flex", gap: 2, mt: 3 }}>
          <Button
            variant="contained"
            onClick={handleSubmit}
            disabled={loading}
            startIcon={<Search />}
            sx={{
              borderRadius: 2,
              background: "linear-gradient(45deg, #8E44AD 30%, #AF7AC5 90%)",
              "&:hover": {
                background: "linear-gradient(45deg, #7D3C98 30%, #9B59B6 90%)",
              },
            }}
          >
            {loading ? "Searching..." : "Search"}
          </Button>
          <Button
            variant="outlined"
            onClick={handleClear}
            startIcon={<Clear />}
            sx={{ borderRadius: 2 }}
          >
            Clear Filters
          </Button>
        </Box>
      </CardContent>
    </Card>
  );
};
