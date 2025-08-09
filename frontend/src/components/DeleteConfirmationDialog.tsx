import React from "react";
import {
  Dialog,
  DialogContent,
  DialogActions,
  DialogTitle,
  DialogContentText,
  Typography,
  Button,
  Box,
  IconButton,
} from "@mui/material";
import CloseIcon from "@mui/icons-material/Close";

interface DeleteConfirmationDialogProps {
  open: boolean;
  onClose: () => void;
  onConfirm: () => void;
  title: string;
  message: string;
  loading?: boolean;
  variant?: "standard" | "gradient";
}

export const DeleteConfirmationDialog: React.FC<
  DeleteConfirmationDialogProps
> = ({
  open,
  onClose,
  onConfirm,
  title,
  message,
  loading = false,
  variant = "standard",
}) => {
  const isGradientVariant = variant === "gradient";

  return (
    <Dialog
      open={open}
      onClose={onClose}
      fullWidth
      maxWidth="sm"
      PaperProps={{
        sx: isGradientVariant
          ? {
              borderRadius: 3,
              maxHeight: "80vh",
            }
          : undefined,
      }}
    >
      <>
        <DialogContent sx={{ p: 0 }}>
          <Box
            sx={{
              p: 4,
              background: "linear-gradient(45deg, #8E44AD 30%, #AF7AC5 90%)",
              color: "white",
              position: "sticky",
              top: 0,
              zIndex: 1,
            }}
          >
            <Box
              sx={{
                display: "flex",
                justifyContent: "space-between",
                alignItems: "center",
              }}
            >
              <Typography variant="h5" fontWeight={600}>
                {title}
              </Typography>
              <IconButton
                onClick={onClose}
                disabled={loading}
                sx={{
                  backgroundColor: "rgba(255, 255, 255, 0.2)",
                  color: "white",
                  "&:hover": { backgroundColor: "rgba(255, 255, 255, 0.3)" },
                }}
              >
                <CloseIcon />
              </IconButton>
            </Box>
          </Box>
          <Box sx={{ p: 4 }}>
            <Typography variant="body1" sx={{ mb: 3 }}>
              {message}
            </Typography>
            <Box sx={{ display: "flex", justifyContent: "flex-end", gap: 2 }}>
              <Button
                onClick={onClose}
                variant="outlined"
                disabled={loading}
                sx={{ borderRadius: 2 }}
              >
                Cancel
              </Button>
              <Button
                onClick={onConfirm}
                variant="contained"
                color="error"
                disabled={loading}
                sx={{
                  borderRadius: 2,
                  background:
                    "linear-gradient(45deg, #C0392B 30%, #E74C3C 90%)",
                  "&:hover": {
                    background:
                      "linear-gradient(45deg, #A93226 30%, #C0392B 90%)",
                  },
                }}
              >
                {loading ? "Deleting..." : "Delete"}
              </Button>
            </Box>
          </Box>
        </DialogContent>
      </>
    </Dialog>
  );
};
