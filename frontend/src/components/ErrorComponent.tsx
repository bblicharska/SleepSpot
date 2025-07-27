import React from "react";
import { Box, Alert, Typography, Button } from "@mui/material";

export const ErrorComponent: React.FC<{
  onClick?: React.MouseEventHandler<HTMLButtonElement>;
  text: string;
  error: string;
}> = ({ onClick, text, error }) => {
  return (
    <Box sx={{ maxWidth: 600, mx: "auto", mt: 4 }}>
      <Alert
        severity="error"
        action={
          <Button color="inherit" size="small" onClick={onClick}>
            Retry
          </Button>
        }
      >
        <Typography variant="h6" gutterBottom>
          {`Failed to load ${text}`}
        </Typography>
        <Typography variant="body2">{error}</Typography>
      </Alert>
    </Box>
  );
};
