import React, { useEffect, useState, useMemo } from "react";
import {
  Box,
  Grid,
  Typography,
  Button,
  Tabs,
  Tab,
  FormControl,
  Select,
  MenuItem,
  Chip,
} from "@mui/material";
import SortIcon from "@mui/icons-material/Sort";
import SwapVertIcon from "@mui/icons-material/SwapVert";
import { PropertyCard } from "../components/PropertyCard";
import { RoomCard } from "../components/RoomCard";
import { PropertyFilter } from "../components/PropertyFilter";
import { RoomFilter } from "../components/RoomFilter";
import { useNavigate, useLocation } from "react-router-dom";
import {
  Property,
  PropertyImageDto,
  RoomDto,
  PropertyFilterDto,
  RoomSearchFilterDto,
} from "../types/types";
import { LoadingComponent } from "../components/LoadingComponent";
import { ErrorComponent } from "../components/ErrorComponent";
import { fetchProperties } from "../queries/fetchProperties";
import { searchProperties, searchRooms } from "../queries/searchProperties";
import { useAuth } from "../components/AuthContext";

interface ExtendedRoom extends RoomDto {
  propertyId: string;
  propertyName: string;
  propertyAddress: string;
  propertyOwnerId?: string;
}

type PropertySortBy =
  | "name"
  | "pricePerMonth"
  | "areaInSquareMeters"
  | "availableSince"
  | "createdAt";
type RoomSortBy =
  | "name"
  | "pricePerMonth"
  | "areaInSquareMeters"
  | "capacity"
  | "availableSince"
  | "createdAt"
  | "propertyName";
type SortDirection = "asc" | "desc";

const PROPERTY_SORT_OPTIONS = [
  { value: "name", label: "Name" },
  { value: "pricePerMonth", label: "Price" },
  { value: "areaInSquareMeters", label: "Area" },
  { value: "availableSince", label: "Available Since" },
  { value: "createdAt", label: "Date Added" },
] as const;

const ROOM_SORT_OPTIONS = [
  { value: "name", label: "Room Name" },
  { value: "pricePerMonth", label: "Price" },
  { value: "areaInSquareMeters", label: "Area" },
  { value: "capacity", label: "Capacity" },
  { value: "availableSince", label: "Available Since" },
  { value: "propertyName", label: "Property Name" },
  { value: "createdAt", label: "Date Added" },
] as const;

export const PropertiesAndRoomsPage: React.FC = () => {
  const [properties, setProperties] = useState<Property[]>([]);
  const [rooms, setRooms] = useState<ExtendedRoom[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [isFiltering, setIsFiltering] = useState(false);
  const [activeFilters, setActiveFilters] = useState<{
    properties?: PropertyFilterDto;
    rooms?: RoomSearchFilterDto;
  }>({});

  const [propertySortBy, setPropertySortBy] =
    useState<PropertySortBy>("createdAt");
  const [propertySortDirection, setPropertySortDirection] =
    useState<SortDirection>("desc");
  const [roomSortBy, setRoomSortBy] = useState<RoomSortBy>("createdAt");
  const [roomSortDirection, setRoomSortDirection] =
    useState<SortDirection>("desc");

  const navigate = useNavigate();
  const location = useLocation();
  const { user } = useAuth();

  const getCurrentTab = () => (location.pathname === "/rooms" ? 1 : 0);
  const [tabValue, setTabValue] = useState(getCurrentTab());

  useEffect(() => {
    setTabValue(getCurrentTab());
  }, [location.pathname]);

  const sortProperties = (
    props: Property[],
    sortBy: PropertySortBy,
    direction: SortDirection
  ) => {
    return [...props].sort((a, b) => {
      let aValue: any;
      let bValue: any;

      switch (sortBy) {
        case "name":
          aValue = a.name?.toLowerCase() || "";
          bValue = b.name?.toLowerCase() || "";
          break;
        case "pricePerMonth":
          aValue = a.pricePerMonth || 0;
          bValue = b.pricePerMonth || 0;
          break;
        case "areaInSquareMeters":
          aValue = a.areaInSquareMeters || 0;
          bValue = b.areaInSquareMeters || 0;
          break;
        case "availableSince":
          aValue = a.availableSince ? new Date(a.availableSince).getTime() : 0;
          bValue = b.availableSince ? new Date(b.availableSince).getTime() : 0;
          break;
        case "createdAt":
          aValue = new Date(a.createdAt || 0).getTime();
          bValue = new Date(b.createdAt || 0).getTime();
          break;
        default:
          return 0;
      }

      if (aValue < bValue) return direction === "asc" ? -1 : 1;
      if (aValue > bValue) return direction === "asc" ? 1 : -1;
      return 0;
    });
  };

  const sortRooms = (
    rooms: ExtendedRoom[],
    sortBy: RoomSortBy,
    direction: SortDirection
  ) => {
    return [...rooms].sort((a, b) => {
      let aValue: any;
      let bValue: any;

      switch (sortBy) {
        case "name":
          aValue = a.name?.toLowerCase() || "";
          bValue = b.name?.toLowerCase() || "";
          break;
        case "pricePerMonth":
          aValue = a.pricePerMonth || 0;
          bValue = b.pricePerMonth || 0;
          break;
        case "areaInSquareMeters":
          aValue = a.areaInSquareMeters || 0;
          bValue = b.areaInSquareMeters || 0;
          break;
        case "capacity":
          aValue = a.capacity || 0;
          bValue = b.capacity || 0;
          break;
        case "availableSince":
          aValue = a.availableSince ? new Date(a.availableSince).getTime() : 0;
          bValue = b.availableSince ? new Date(b.availableSince).getTime() : 0;
          break;
        case "propertyName":
          aValue = a.propertyName?.toLowerCase() || "";
          bValue = b.propertyName?.toLowerCase() || "";
          break;
        case "createdAt":
          aValue = new Date(a.createdAt || 0).getTime();
          bValue = new Date(b.createdAt || 0).getTime();
          break;
        default:
          return 0;
      }

      if (aValue < bValue) return direction === "asc" ? -1 : 1;
      if (aValue > bValue) return direction === "asc" ? 1 : -1;
      return 0;
    });
  };

  const loadProperties = async () => {
    try {
      setLoading(true);
      setError(null);
      const data = await fetchProperties();
      setProperties(data);

      const allRooms: ExtendedRoom[] = data
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

      setRooms(allRooms);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Unexpected error");
    } finally {
      setLoading(false);
    }
  };

  const handlePropertyFilter = async (filters: PropertyFilterDto) => {
    try {
      setIsFiltering(true);
      setError(null);
      setActiveFilters((prev) => ({ ...prev, properties: filters }));

      const data = await searchProperties(filters);
      setProperties(data);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Search failed");
    } finally {
      setIsFiltering(false);
    }
  };

  const handleRoomFilter = async (filters: RoomSearchFilterDto) => {
    try {
      setIsFiltering(true);
      setError(null);
      setActiveFilters((prev) => ({ ...prev, rooms: filters }));

      const roomData = await searchRooms(filters);

      const extendedRooms: ExtendedRoom[] = roomData
        .map((room) => ({
          id: room.id!,
          name: room.name!,
          description: room.description!,
          pricePerMonth: room.pricePerMonth!,
          areaInSquareMeters: room.areaInSquareMeters!,
          capacity: room.capacity!,
          isAvailable: room.isAvailable!,
          availableSince: room.availableSince,
          images: room.images || [],
          propertyId: room.propertyId!,
          propertyName: room.propertyName!,
          propertyAddress: room.propertyAddress!,
          propertyOwnerId: room.propertyOwnerId,
        }))
        .filter((r) => r.isAvailable);

      setRooms(extendedRooms);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Search failed");
    } finally {
      setIsFiltering(false);
    }
  };

  const handleClearPropertyFilters = () => {
    setActiveFilters((prev) => ({ ...prev, properties: undefined }));
    loadProperties();
  };

  const handleClearRoomFilters = () => {
    setActiveFilters((prev) => ({ ...prev, rooms: undefined }));
    loadProperties();
  };

  useEffect(() => {
    loadProperties();
  }, []);

  // Memoized filtered and sorted data
  const sortedEntirePlaces = useMemo(() => {
    const filtered = properties.filter(
      (p) => p.isEntirePlaceRentable && p.isAvailable
    );
    return sortProperties(filtered, propertySortBy, propertySortDirection);
  }, [properties, propertySortBy, propertySortDirection]);

  const sortedAvailableRooms = useMemo(() => {
    const filtered = rooms.filter((r) => r.isAvailable);
    return sortRooms(filtered, roomSortBy, roomSortDirection);
  }, [rooms, roomSortBy, roomSortDirection]);

  const handleViewPropertyDetails = (id: string) => navigate(`/property/${id}`);

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

  const InlineSortControls = ({
    isRoom = false,
    resultCount,
  }: {
    isRoom?: boolean;
    resultCount: number;
  }) => (
    <Box
      sx={{
        display: "flex",
        alignItems: "center",
        gap: 2,
        py: 2,
        borderBottom: "1px solid #e0e0e0",
        mb: 3,
      }}
    >
      <SortIcon sx={{ color: "#8E44AD" }} />
      <Typography variant="body2" sx={{ color: "#6C3483", fontWeight: 500 }}>
        Sort by:
      </Typography>

      <FormControl size="small" variant="outlined" sx={{ minWidth: 120 }}>
        <Select
          value={isRoom ? roomSortBy : propertySortBy}
          onChange={(e) => {
            if (isRoom) {
              setRoomSortBy(e.target.value as RoomSortBy);
            } else {
              setPropertySortBy(e.target.value as PropertySortBy);
            }
          }}
          sx={{
            "& .MuiOutlinedInput-notchedOutline": {
              borderColor: "#D2B4DE",
            },
            "&:hover .MuiOutlinedInput-notchedOutline": {
              borderColor: "#8E44AD",
            },
          }}
        >
          {(isRoom ? ROOM_SORT_OPTIONS : PROPERTY_SORT_OPTIONS).map(
            (option) => (
              <MenuItem key={option.value} value={option.value}>
                {option.label}
              </MenuItem>
            )
          )}
        </Select>
      </FormControl>

      <Button
        variant="outlined"
        size="small"
        startIcon={<SwapVertIcon />}
        onClick={() => {
          if (isRoom) {
            setRoomSortDirection(roomSortDirection === "asc" ? "desc" : "asc");
          } else {
            setPropertySortDirection(
              propertySortDirection === "asc" ? "desc" : "asc"
            );
          }
        }}
        sx={{
          textTransform: "none",
          borderColor: "#8E44AD",
          color: "#8E44AD",
          "&:hover": {
            backgroundColor: "rgba(142, 68, 173, 0.1)",
            borderColor: "#8E44AD",
          },
        }}
      >
        {(isRoom ? roomSortDirection : propertySortDirection) === "asc"
          ? "Ascending"
          : "Descending"}
      </Button>

      <Chip
        label={`${resultCount} results`}
        size="small"
        sx={{
          backgroundColor: "#8E44AD",
          color: "white",
          fontWeight: 500,
        }}
      />
    </Box>
  );

  if (loading) return <LoadingComponent text="Loading…" />;

  if (error && !activeFilters.properties && !activeFilters.rooms) {
    return (
      <ErrorComponent
        onClick={loadProperties}
        error={error}
        text="properties and rooms"
      />
    );
  }

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
            label={`Apartments (${sortedEntirePlaces.length})`}
            {...a11yProps(0)}
          />
          <Tab
            label={`Rooms (${sortedAvailableRooms.length})`}
            {...a11yProps(1)}
          />
        </Tabs>
      </Box>

      {/* Properties Tab */}
      <Box hidden={tabValue !== 0} role="tabpanel">
        <PropertyFilter
          onFilter={handlePropertyFilter}
          onClear={handleClearPropertyFilters}
          loading={isFiltering}
        />

        <InlineSortControls resultCount={sortedEntirePlaces.length} />

        {error && (
          <Box sx={{ mb: 2, p: 2, bgcolor: "error.light", borderRadius: 1 }}>
            <Typography color="error">{error}</Typography>
          </Box>
        )}

        {sortedEntirePlaces.length === 0 ? (
          <Box textAlign="center" mt={4}>
            <Typography variant="h5">
              {activeFilters.properties
                ? "No apartments match your search criteria"
                : "No entire apartments available"}
            </Typography>
            <Button onClick={loadProperties} sx={{ mt: 2 }}>
              Refresh
            </Button>
          </Box>
        ) : (
          <Grid container spacing={2}>
            {sortedEntirePlaces.map((p) => (
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
                />
              </Grid>
            ))}
          </Grid>
        )}
      </Box>

      {/* Rooms Tab */}
      <Box hidden={tabValue !== 1} role="tabpanel">
        <RoomFilter
          onFilter={handleRoomFilter}
          onClear={handleClearRoomFilters}
          loading={isFiltering}
        />

        <InlineSortControls
          isRoom={true}
          resultCount={sortedAvailableRooms.length}
        />

        {error && (
          <Box sx={{ mb: 2, p: 2, bgcolor: "error.light", borderRadius: 1 }}>
            <Typography color="error">{error}</Typography>
          </Box>
        )}

        {sortedAvailableRooms.length === 0 ? (
          <Box textAlign="center" mt={4}>
            <Typography variant="h5">
              {activeFilters.rooms
                ? "No rooms match your search criteria"
                : "No rooms available"}
            </Typography>
            <Button onClick={loadProperties} sx={{ mt: 2 }}>
              Refresh
            </Button>
          </Box>
        ) : (
          <Grid container spacing={2}>
            {sortedAvailableRooms.map((room) => (
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
                />
              </Grid>
            ))}
          </Grid>
        )}
      </Box>
    </Box>
  );
};
