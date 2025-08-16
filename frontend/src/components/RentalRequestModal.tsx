import React, { useState } from "react";
import {
  Box,
  Button,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
} from "@mui/material";
import { API_BASE_URL } from "../types/types";

export const RentalRequestModal = ({
  open,
  onClose,
  propertyId,
}: {
  open: boolean;
  onClose: () => void;
  propertyId: string;
}) => {
  const [startDate, setStartDate] = useState("");
  const [monthlyRent, setMonthlyRent] = useState("");

  const handleSubmit = async () => {
    try {
      const payload = {
        propertyId,
        roomId: null,
        startDate,
        monthlyRent: parseFloat(monthlyRent),
        status: "Pending",
      };

      const res = await fetch(`${API_BASE_URL}/api/rentals`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(payload),
      });

      if (!res.ok) throw new Error("Failed to create rental");

      alert("Rental request sent!");
      onClose();
    } catch (err) {
      console.error(err);
      alert("Error sending rental request");
    }
  };

  return (
    <Dialog open={open} onClose={onClose}>
      <DialogTitle>Send Rental Request</DialogTitle>
      <DialogContent>
        <Box sx={{ display: "flex", flexDirection: "column", gap: 2, mt: 1 }}>
          <TextField
            type="date"
            label="Start Date"
            InputLabelProps={{ shrink: true }}
            value={startDate}
            onChange={(e) => setStartDate(e.target.value)}
          />
          <TextField
            type="number"
            label="Monthly Rent"
            value={monthlyRent}
            onChange={(e) => setMonthlyRent(e.target.value)}
          />
        </Box>
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose} color="secondary">
          Cancel
        </Button>
        <Button onClick={handleSubmit} variant="contained" color="primary">
          Send
        </Button>
      </DialogActions>
    </Dialog>
  );
};
