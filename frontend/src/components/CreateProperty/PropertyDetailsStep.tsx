import React from "react";
import {
  Box,
  Grid,
  TextField,
  FormControlLabel,
  Switch,
  InputAdornment,
} from "@mui/material";
import {
  Home as HomeIcon,
  LocationOn as LocationIcon,
  PriceCheck as PriceCheckIcon,
  Square as SquareIcon,
} from "@mui/icons-material";
import { PropertyFormData } from "../../types/types";

interface PropertyDetailsStepProps {
  propertyData: PropertyFormData;
  errors: Record<string, string>;
  onPropertyChange: (field: keyof PropertyFormData, value: any) => void;
}

export const PropertyDetailsStep: React.FC<PropertyDetailsStepProps> = ({
  propertyData,
  errors,
  onPropertyChange,
}) => {
  return (
    <Box sx={{ mt: 3 }}>
      <Grid container spacing={3}>
        <Grid size={{ xs: 12 }}>
          <TextField
            fullWidth
            label="Property Name"
            value={propertyData.name}
            onChange={(e) => onPropertyChange("name", e.target.value)}
            error={!!errors.name}
            helperText={errors.name}
            InputProps={{
              startAdornment: (
                <InputAdornment position="start">
                  <HomeIcon />
                </InputAdornment>
              ),
            }}
          />
        </Grid>

        <Grid size={{ xs: 12 }}>
          <TextField
            fullWidth
            label="Address"
            value={propertyData.address}
            onChange={(e) => onPropertyChange("address", e.target.value)}
            error={!!errors.address}
            helperText={errors.address}
            InputProps={{
              startAdornment: (
                <InputAdornment position="start">
                  <LocationIcon />
                </InputAdornment>
              ),
            }}
          />
        </Grid>

        <Grid size={{ xs: 12 }}>
          <TextField
            fullWidth
            multiline
            rows={3}
            label="Description"
            value={propertyData.description}
            onChange={(e) => onPropertyChange("description", e.target.value)}
            error={!!errors.description}
            helperText={errors.description}
          />
        </Grid>

        <Grid size={{ xs: 12 }}>
          <TextField
            fullWidth
            multiline
            rows={4}
            label="Detailed Description (Optional)"
            value={propertyData.detailedDescription}
            onChange={(e) =>
              onPropertyChange("detailedDescription", e.target.value)
            }
            helperText="Provide additional details about the property"
          />
        </Grid>

        <Grid size={{ xs: 12, sm: 6 }}>
          <TextField
            fullWidth
            type="number"
            label="Price Per Month (PLN)"
            value={propertyData.pricePerMonth}
            onChange={(e) =>
              onPropertyChange("pricePerMonth", parseFloat(e.target.value) || 0)
            }
            error={!!errors.pricePerMonth}
            helperText={errors.pricePerMonth}
            InputProps={{
              startAdornment: (
                <InputAdornment position="start">
                  <PriceCheckIcon />
                </InputAdornment>
              ),
            }}
          />
        </Grid>

        <Grid size={{ xs: 12, sm: 6 }}>
          <TextField
            fullWidth
            type="number"
            label="Area (m²)"
            value={propertyData.areaInSquareMeters}
            onChange={(e) =>
              onPropertyChange(
                "areaInSquareMeters",
                parseFloat(e.target.value) || 0
              )
            }
            error={!!errors.areaInSquareMeters}
            helperText={errors.areaInSquareMeters}
            InputProps={{
              startAdornment: (
                <InputAdornment position="start">
                  <SquareIcon />
                </InputAdornment>
              ),
            }}
          />
        </Grid>

        <Grid size={{ xs: 12 }}>
          <FormControlLabel
            control={
              <Switch
                checked={propertyData.isEntirePlaceRentable}
                onChange={(e) =>
                  onPropertyChange("isEntirePlaceRentable", e.target.checked)
                }
                color="primary"
              />
            }
            label="Entire place rentable"
          />
        </Grid>
      </Grid>
    </Box>
  );
};
