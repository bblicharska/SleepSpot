import React from "react";
import { Typography, Paper } from "@mui/material";

interface DetailedDescriptionProps {
  title: string;
  description: string;
}

export const DetailedDescription = ({
  title,
  description,
}: DetailedDescriptionProps) => {
  return (
    <Paper
      elevation={3}
      sx={{
        p: 4,
        mb: 4,
        background:
          "linear-gradient(135deg, rgba(142, 68, 173, 0.05) 0%, rgba(175, 122, 197, 0.05) 100%)",
        border: "1px solid rgba(142, 68, 173, 0.2)",
        borderRadius: 3,
      }}
    >
      <Typography
        variant="h5"
        gutterBottom
        sx={{
          fontWeight: 700,
          mb: 3,
          background: "linear-gradient(45deg, #8E44AD 30%, #AF7AC5 90%)",
          backgroundClip: "text",
          WebkitBackgroundClip: "text",
          WebkitTextFillColor: "transparent",
          position: "relative",
          "&::after": {
            content: '""',
            position: "absolute",
            bottom: -8,
            left: 0,
            width: 60,
            height: 3,
            background: "linear-gradient(45deg, #8E44AD 30%, #AF7AC5 90%)",
            borderRadius: 2,
          },
        }}
      >
        {title}
      </Typography>
      <Typography
        variant="body1"
        sx={{
          whiteSpace: "pre-line",
          lineHeight: 1.8,
          fontSize: "1.1rem",
          color: "text.primary",
          fontWeight: 400,
          "& p": {
            marginBottom: 2,
          },
        }}
      >
        {description}
      </Typography>
    </Paper>
  );
};
