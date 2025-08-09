import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import {
  Box,
  Typography,
  Chip,
  Avatar,
  Paper,
  Stack,
  Divider,
} from "@mui/material";
import {
  Euro as EuroIcon,
  LocationOn as LocationOnIcon,
  CalendarToday as CalendarTodayIcon,
  People as PeopleIcon,
  Hotel as HotelIcon,
} from "@mui/icons-material";
import { LoadingComponent } from "../components/LoadingComponent";
import { PropertyMap } from "../components/PropertyMap";
import { ImageGallery } from "../components/ImageGallery";
import { GroupListingDto } from "../types/types";
import { fetchListingDetails } from "../queries/fetchListingDetails";

export const GroupListingDetailsPage = () => {
  const { id } = useParams();
  const [listing, setListing] = useState<GroupListingDto | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchDetails = async () => {
      try {
        const data = await fetchListingDetails(id!);
        setListing(data);
      } catch (err) {
        console.error(err);
      } finally {
        setLoading(false);
      }
    };
    fetchDetails();
  }, [id]);

  const formatDate = (date: string) =>
    new Date(date).toLocaleDateString("en-GB");

  if (loading) return <LoadingComponent text="Loading listing details..." />;
  if (!listing) return <Typography variant="h6">Listing not found</Typography>;

  const { property, room } = listing;
  const hasProperty = !!property && !room;
  const hasRoom = !!room;

  return (
    <Box sx={{ px: { xs: 2, md: 6 }, py: 4 }}>
      <Typography variant="h4" fontWeight={600} gutterBottom>
        {listing.title}
      </Typography>

      <Stack direction="row" spacing={2} mb={2} flexWrap="wrap">
        <Chip
          label={listing.status}
          color={listing.status === "Active" ? "success" : "default"}
        />
        <Stack direction="row" alignItems="center" spacing={1}>
          <LocationOnIcon fontSize="small" />
          <Typography variant="body2">{listing.preferredCity}</Typography>
        </Stack>
        <Stack direction="row" alignItems="center" spacing={1}>
          <PeopleIcon fontSize="small" />
          <Typography variant="body2">
            {listing.desiredRoommatesCount} roommates wanted
          </Typography>
        </Stack>
        {listing.maxBudgetPerPerson && (
          <Stack direction="row" alignItems="center" spacing={1}>
            <EuroIcon fontSize="small" />
            <Typography variant="body2">
              Max {listing.maxBudgetPerPerson} PLN/person
            </Typography>
          </Stack>
        )}
        <Stack direction="row" alignItems="center" spacing={1}>
          <CalendarTodayIcon fontSize="small" />
          <Typography variant="body2">
            Posted: {formatDate(listing.createdAt)}
          </Typography>
        </Stack>
      </Stack>

      <Typography variant="subtitle1" color="text.secondary" paragraph>
        {listing.description}
      </Typography>

      <Divider sx={{ my: 3 }} />
      {listing.group && (
        <Paper sx={{ p: 3, mb: 4 }}>
          <Typography variant="h6" gutterBottom>
            Group Information
          </Typography>
          <Typography>
            <strong>Name:</strong> {listing.group.name}
          </Typography>
          <Typography>
            <strong>Description:</strong> {listing.group.description}
          </Typography>
          <Typography>
            <strong>Created:</strong> {formatDate(listing.group.createdAt)}
          </Typography>

          <Divider sx={{ my: 2 }} />

          <Typography variant="subtitle1" gutterBottom>
            Members
          </Typography>
          <Stack spacing={1}>
            {listing.group.members.map((member) => (
              <Box key={member.id} display="flex" alignItems="center" gap={2}>
                <Avatar>{member.user?.firstName?.[0] || "?"}</Avatar>
                <Box>
                  <Typography>
                    {member.user?.firstName} {member.user?.lastName}
                  </Typography>
                  <Typography variant="caption" color="text.secondary">
                    {member.role} • Joined {formatDate(member.joinedAt)}
                  </Typography>
                </Box>
              </Box>
            ))}
          </Stack>
        </Paper>
      )}
      {hasRoom && (
        <Paper sx={{ p: 3, mb: 4 }}>
          <Typography variant="h6" gutterBottom>
            <HotelIcon sx={{ mr: 1, verticalAlign: "middle" }} />
            Room Information
          </Typography>
          <Typography>
            <strong>Name:</strong> {room?.name}
          </Typography>
          <Typography>
            <strong>Description:</strong> {room?.description}
          </Typography>
          <Typography>
            <strong>Price:</strong> {room?.pricePerMonth} PLN/month
          </Typography>
          <Typography>
            <strong>Size:</strong> {room?.areaInSquareMeters} m²
          </Typography>
          <Typography>
            <strong>Capacity:</strong> {room?.capacity}{" "}
            {room?.capacity === 1 ? "person" : "people"}
          </Typography>
          {room?.detailedDescription && (
            <Typography sx={{ mt: 1 }}>
              <strong>Details:</strong> {room?.detailedDescription}
            </Typography>
          )}
          {room?.images.length > 0 && (
            <Box mt={3}>
              <ImageGallery images={room?.images} title={room?.name} />
            </Box>
          )}

          <Box sx={{ my: 4 }}>
            <PropertyMap address={room.propertyAddress} />
          </Box>
        </Paper>
      )}
      {hasProperty && (
        <Paper sx={{ p: 3, mb: 4 }}>
          <Typography variant="h6" gutterBottom>
            Property Information
          </Typography>
          <Typography>
            <strong>Name:</strong> {property.name}
          </Typography>
          <Typography>
            <strong>Address:</strong> {property.address}
          </Typography>
          <Typography>
            <strong>Description:</strong> {property.description}
          </Typography>
          <Typography>
            <strong>Size:</strong> {property.areaInSquareMeters} m²
          </Typography>
          <Typography>
            <strong>Type:</strong>{" "}
            {property.isEntirePlaceRentable ? "Entire Place" : "Shared Space"}
          </Typography>
          {property.detailedDescription && (
            <Typography sx={{ mt: 1 }}>
              <strong>Details:</strong> {property.detailedDescription}
            </Typography>
          )}
          {property.images.length > 0 && (
            <Box mt={3}>
              <ImageGallery images={property.images} title={property.name} />
            </Box>
          )}
          <Box sx={{ my: 4 }}>
            <PropertyMap address={property.address} />
          </Box>
        </Paper>
      )}
    </Box>
  );
};
