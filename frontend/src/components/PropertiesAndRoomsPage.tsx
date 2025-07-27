import React, { useEffect, useState } from "react";
import {
  Box,
  Grid,
  Typography,
  Button,
  Tabs,
  Tab,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  IconButton,
} from "@mui/material";
import CloseIcon from "@mui/icons-material/Close";
import { PropertyCard } from "../components/PropertyCard";
import { RoomCard } from "../components/RoomCard";
import { useNavigate, useLocation } from "react-router-dom";
import { Property, PropertyImageDto, RoomDto } from "../types/types";
import { LoadingComponent } from "../components/LoadingComponent";
import { ErrorComponent } from "../components/ErrorComponent";
import { fetchProperties } from "../queries/fetchProperties";
import { useAuth } from "../components/AuthContext";

interface ExtendedRoom extends RoomDto {
  propertyId: string;
  propertyName: string;
  propertyAddress: string;
  propertyOwnerId?: string;
}

export const PropertiesAndRoomsPage: React.FC = () => {
  const [properties, setProperties] = useState<Property[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [deleteTarget, setDeleteTarget] = useState<{
    type: "property" | "room";
    id: string;
  } | null>(null);
  const navigate = useNavigate();
  const location = useLocation();
  const { user } = useAuth();

  const getCurrentTab = () => (location.pathname === "/rooms" ? 1 : 0);
  const [tabValue, setTabValue] = useState(getCurrentTab());

  useEffect(() => {
    setTabValue(getCurrentTab());
  }, [location.pathname]);

  const loadProperties = async () => {
    try {
      setLoading(true);
      setError(null);
      const data = await fetchProperties();
      setProperties(data);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Unexpected error");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadProperties();
  }, []);

  const entirePlaces = properties.filter(
    (p) => p.isEntirePlaceRentable && p.isAvailable
  );
  const allRooms: ExtendedRoom[] = properties
    .filter((p) => !p.isEntirePlaceRentable)
    .flatMap((p) =>
      p.rooms.map((r) => ({
        ...r,
        propertyId: p.id,
        propertyName: p.name,
        propertyAddress: p.address,
        propertyOwnerId: p.ownerId,
      }))
    )
    .filter((r) => r.isAvailable);

  const handleViewPropertyDetails = (id: string) => navigate(`/property/${id}`);

  // pass full room object in state
  const handleViewRoomDetails = (room: ExtendedRoom) => {
    navigate(`/room/${room.id}`, {
      state: {
        propertyId: room.propertyId,
        propertyName: room.propertyName,
        propertyAddress: room.propertyAddress,
      },
    });
  };

  const getPrimaryImage = (images: PropertyImageDto[] = []) => {
    if (!images.length) return "";
    const prim = images.find((i) => i.isPrimary);
    if (prim) return prim.imageUrl;
    return [...images].sort((a, b) => a.displayOrder - b.displayOrder)[0]
      .imageUrl;
  };

  const a11yProps = (index: number) => ({
    id: `simple-tab-${index}`,
    "aria-controls": `simple-tabpanel-${index}`,
  });

  const confirmDelete = (type: "property" | "room", id: string) => {
    setDeleteTarget({ type, id });
  };
  const cancelDelete = () => {
    setDeleteTarget(null);
  };
  const handleDeleteConfirmed = async () => {
    if (!deleteTarget) return;
    try {
      const { type, id } = deleteTarget;
      const url =
        type === "property"
          ? `http://localhost:5000/api/properties/${id}`
          : `http://localhost:5000/api/properties/rooms/${id}`;

      const response = await fetch(url, {
        method: "DELETE",
        headers: {
          "Content-Type": "application/json",
        },
      });

      if (!response.ok) {
        throw new Error("Failed to delete");
      }

      // Reload data after delete
      await loadProperties();
    } catch (err) {
      alert(
        err instanceof Error
          ? err.message
          : "Failed to delete. Please try again later."
      );
    } finally {
      setDeleteTarget(null);
    }
  };

  if (loading) return <LoadingComponent text="Loading…" />;
  if (error)
    return (
      <ErrorComponent
        onClick={loadProperties}
        error={error}
        text="properties and rooms"
      />
    );

  return (
    <Box>
      <Box sx={{ borderBottom: 1, borderColor: "divider" }}>
        <Tabs
          value={tabValue}
          onChange={(e, v) => {
            setTabValue(v);
            navigate(v === 0 ? "/properties" : "/rooms");
          }}
        >
          <Tab
            label={`Apartments (${entirePlaces.length})`}
            {...a11yProps(0)}
          />
          <Tab label={`Rooms (${allRooms.length})`} {...a11yProps(1)} />
        </Tabs>
      </Box>

      {/* Properties Tab */}
      <Box hidden={tabValue !== 0} role="tabpanel">
        {entirePlaces.length === 0 ? (
          <Box textAlign="center" mt={4}>
            <Typography variant="h5">No entire apartments available</Typography>
            <Button onClick={loadProperties}>Refresh</Button>
          </Box>
        ) : (
          <Grid container spacing={2}>
            {entirePlaces.map((p) => (
              <Grid size={{ xs: 12, sm: 4, md: 4 }} key={p.id}>
                <PropertyCard
                  name={p.name}
                  description={p.description}
                  image={getPrimaryImage(p.images)}
                  images={p.images}
                  address={p.address}
                  price={p.pricePerMonth}
                  area={p.areaInSquareMeters}
                  onViewDetails={() => handleViewPropertyDetails(p.id)}
                  canDelete={p.ownerId === user?.userId}
                  onDelete={() => confirmDelete("property", p.id)}
                />
              </Grid>
            ))}
          </Grid>
        )}
      </Box>

      {/* Rooms Tab */}
      <Box hidden={tabValue !== 1} role="tabpanel">
        {allRooms.length === 0 ? (
          <Box textAlign="center" mt={4}>
            <Typography variant="h5">No rooms available</Typography>
            <Button onClick={loadProperties}>Refresh</Button>
          </Box>
        ) : (
          <Grid container spacing={2}>
            {allRooms.map((room) => (
              <Grid size={{ xs: 12, sm: 4, md: 4 }} key={room.id}>
                <RoomCard
                  name={room.name}
                  description={room.description}
                  image={getPrimaryImage(room.images)}
                  images={room.images}
                  price={room.pricePerMonth}
                  area={room.areaInSquareMeters}
                  capacity={room.capacity}
                  address={room.propertyAddress}
                  onViewDetails={() => handleViewRoomDetails(room)}
                  canDelete={room.propertyOwnerId === user?.userId}
                  onDelete={() => confirmDelete("room", room.id)}
                />
              </Grid>
            ))}
          </Grid>
        )}
      </Box>

      {/* Confirmation modal */}
      <Dialog
        open={!!deleteTarget}
        onClose={cancelDelete}
        fullWidth
        maxWidth="sm"
        PaperProps={{
          sx: {
            borderRadius: 3,
            maxHeight: "80vh",
          },
        }}
      >
        <DialogContent sx={{ p: 0 }}>
          {/* Sticky header with gradient */}
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
                Confirm Deletion
              </Typography>
              <IconButton
                onClick={cancelDelete}
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

          {/* Confirmation content */}
          <Box sx={{ p: 4 }}>
            <Typography variant="body1" sx={{ mb: 3 }}>
              Are you sure you want to delete this{" "}
              <strong>
                {deleteTarget?.type === "property" ? "property" : "room"}
              </strong>
              ? This action cannot be undone.
            </Typography>

            <Box sx={{ display: "flex", justifyContent: "flex-end", gap: 2 }}>
              <Button
                onClick={cancelDelete}
                variant="outlined"
                sx={{ borderRadius: 2 }}
              >
                Cancel
              </Button>
              <Button
                onClick={handleDeleteConfirmed}
                variant="contained"
                color="error"
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
                Delete
              </Button>
            </Box>
          </Box>
        </DialogContent>
      </Dialog>
    </Box>
  );
};
