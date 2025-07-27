import React from "react";
import { Box, CircularProgress, Typography } from "@mui/material";

export const LoadingComponent: React.FC<{ text: string }> = ({ text }) => {
  return (
    <Box
      display="flex"
      justifyContent="center"
      alignItems="center"
      minHeight="400px"
    >
      <Box textAlign="center">
        <CircularProgress size={60} />
        <Typography variant="h6" sx={{ mt: 2 }}>
          {text}
        </Typography>
      </Box>
    </Box>
  );
};
