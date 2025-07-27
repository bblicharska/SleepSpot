import { useNavigate } from "react-router-dom";
import { Box, Typography, Paper, Grid, Chip } from "@mui/material";
import PersonIcon from "@mui/icons-material/Person";
import CheckCircleIcon from "@mui/icons-material/CheckCircle";
import CancelIcon from "@mui/icons-material/Cancel";
import { RoomSummaryDto } from "./RoomDetailsPage";

interface OtherRoomsSectionProps {
  rooms: RoomSummaryDto[];
  propertyName: string;
  currentRoomId?: string;
}

export const OtherRoomsSection = ({
  rooms,
  propertyName,
  currentRoomId,
}: OtherRoomsSectionProps) => {
  const navigate = useNavigate();

  const handleRoomNavigation = (roomId: string) => {
    navigate(`/room/${roomId}`);
  };

  // Filter out current room if provided
  const filteredRooms = currentRoomId
    ? rooms.filter((room) => room.id !== currentRoomId)
    : rooms;

  if (filteredRooms.length === 0) {
    return null;
  }

  return (
    <Box sx={{ mt: 4 }}>
      <Typography variant="h5" fontWeight={600} gutterBottom>
        Other Rooms in This Property
      </Typography>
      <Typography variant="body2" color="text.secondary" sx={{ mb: 3 }}>
        Explore other rooms in {propertyName}
      </Typography>

      <Grid container spacing={3}>
        {filteredRooms.map((room) => (
          <Grid size={{ xs: 12, sm: 6, md: 6 }} key={room.id}>
            <Paper
              elevation={room.isAvailable ? 2 : 1}
              sx={{
                p: 3,
                borderRadius: 2,
                transition: "all 0.3s ease",
                opacity: room.isAvailable ? 1 : 0.6,
                filter: room.isAvailable ? "none" : "grayscale(30%)",
                backgroundColor: room.isAvailable
                  ? "background.paper"
                  : "grey.50",
                "&:hover": room.isAvailable
                  ? {
                      elevation: 4,
                      transform: "translateY(-2px)",
                      borderColor: "primary.main",
                    }
                  : {},
                cursor: room.isAvailable ? "pointer" : "default",
                border: `1px solid ${
                  room.isAvailable
                    ? "rgba(142, 68, 173, 0.1)"
                    : "rgba(0, 0, 0, 0.12)"
                }`,
                position: "relative",
              }}
              onClick={() => room.isAvailable && handleRoomNavigation(room.id)}
            >
              {/* Room Image */}
              {room.mainImage && (
                <Box
                  sx={{
                    width: "100%",
                    height: 120,
                    backgroundImage: `url(${room.mainImage})`,
                    backgroundSize: "cover",
                    backgroundPosition: "center",
                    borderRadius: 1,
                    mb: 2,
                    position: "relative",
                    "&::after": !room.isAvailable
                      ? {
                          content: '""',
                          position: "absolute",
                          top: 0,
                          left: 0,
                          right: 0,
                          bottom: 0,
                          backgroundColor: "rgba(0, 0, 0, 0.4)",
                          borderRadius: 1,
                        }
                      : {},
                  }}
                >
                  {!room.isAvailable && (
                    <Box
                      sx={{
                        position: "absolute",
                        top: "50%",
                        left: "50%",
                        transform: "translate(-50%, -50%)",
                        color: "white",
                        fontWeight: "bold",
                        textAlign: "center",
                        zIndex: 1,
                      }}
                    >
                      <Typography variant="body2" fontWeight={700}>
                        NOT AVAILABLE
                      </Typography>
                    </Box>
                  )}
                </Box>
              )}

              {/* Room Name */}
              <Typography
                variant="h6"
                fontWeight={600}
                gutterBottom
                sx={{
                  color: room.isAvailable ? "text.primary" : "text.disabled",
                }}
              >
                {room.name}
              </Typography>

              {/* Room Description */}
              <Typography
                variant="body2"
                color={room.isAvailable ? "text.secondary" : "text.disabled"}
                paragraph
                sx={{
                  display: "-webkit-box",
                  WebkitLineClamp: 2,
                  WebkitBoxOrient: "vertical",
                  overflow: "hidden",
                }}
              >
                {room.description}
              </Typography>

              {/* Price and Availability */}
              <Box
                sx={{
                  display: "flex",
                  justifyContent: "space-between",
                  alignItems: "center",
                  mb: 1,
                }}
              >
                <Typography
                  variant="h6"
                  color={room.isAvailable ? "primary" : "text.disabled"}
                  fontWeight={600}
                >
                  {room.pricePerMonth} PLN/month
                </Typography>
                <Chip
                  icon={room.isAvailable ? <CheckCircleIcon /> : <CancelIcon />}
                  label={room.isAvailable ? "Available" : "Not Available"}
                  size="small"
                  color={room.isAvailable ? "success" : "error"}
                />
              </Box>

              {/* Room Details Chips */}
              <Box sx={{ display: "flex", gap: 1, flexWrap: "wrap" }}>
                <Chip
                  label={`${room.areaInSquareMeters} m²`}
                  size="small"
                  variant="outlined"
                  sx={{
                    opacity: room.isAvailable ? 1 : 0.6,
                    color: room.isAvailable ? "inherit" : "text.disabled",
                  }}
                />
                <Chip
                  icon={<PersonIcon />}
                  label={`${room.capacity} ${
                    room.capacity === 1 ? "person" : "people"
                  }`}
                  size="small"
                  variant="outlined"
                  sx={{
                    opacity: room.isAvailable ? 1 : 0.6,
                    color: room.isAvailable ? "inherit" : "text.disabled",
                  }}
                />
              </Box>
            </Paper>
          </Grid>
        ))}
      </Grid>
    </Box>
  );
};
