import React, { useState } from "react";
import {
  Box,
  Typography,
  Paper,
  Button,
  Stepper,
  Step,
  StepLabel,
} from "@mui/material";
import {
  API_BASE_URL,
  PropertyFormData,
  RoomDto,
  initialPropertyData,
  initialRoomData,
  steps,
} from "../../types/types";
import { useAuth } from "../AuthContext";
import { useImageUpload } from "./useImageUpload";
import { PropertyDetailsStep } from "./PropertyDetailsStep";
import { RoomsStep } from "./RoomsStep";
import { PropertyImagesStep } from "./PropertyImagesStep";
import { RoomDialog } from "./RoomDialog";

export const PropertyCreationPage: React.FC = () => {
  const { user } = useAuth();
  const [activeStep, setActiveStep] = useState(0);
  const [propertyData, setPropertyData] =
    useState<PropertyFormData>(initialPropertyData);
  const [currentRoom, setCurrentRoom] = useState<RoomDto>(initialRoomData);
  const [editingRoomIndex, setEditingRoomIndex] = useState<number | null>(null);
  const [roomDialogOpen, setRoomDialogOpen] = useState(false);
  const [loading, setLoading] = useState(false);
  const [errors, setErrors] = useState<Record<string, string>>({});
  const [submitStatus, setSubmitStatus] = useState<
    "idle" | "success" | "error"
  >("idle");
  console.log(user);
  const propertyImageUpload: ReturnType<typeof useImageUpload> =
    useImageUpload();
  const roomImageUpload: ReturnType<typeof useImageUpload> = useImageUpload();

  const handlePropertyChange = (field: keyof PropertyFormData, value: any) => {
    setPropertyData((prev) => ({ ...prev, [field]: value }));
    if (errors[field]) {
      setErrors((prev) => ({ ...prev, [field]: "" }));
    }
  };

  const handlePropertyImageUpload = (
    event: React.ChangeEvent<HTMLInputElement>
  ) => {
    const newImages = propertyImageUpload.handleImageUpload(
      event,
      propertyData.images
    );
    setPropertyData((prev) => ({
      ...prev,
      images: [...prev.images, ...newImages],
    }));
  };

  const handlePropertyImageRemove = (imageId: string) => {
    setPropertyData((prev) => ({
      ...prev,
      images: propertyImageUpload.removeImage(prev.images, imageId),
    }));
  };

  const handlePropertyImageSetPrimary = (imageId: string) => {
    setPropertyData((prev) => ({
      ...prev,
      images: propertyImageUpload.setPrimaryImage(prev.images, imageId),
    }));
  };

  const handleRoomChange = (field: keyof RoomDto, value: any) => {
    setCurrentRoom((prev) => ({ ...prev, [field]: value }));
  };

  const handleAddRoom = () => {
    setCurrentRoom({ ...initialRoomData, id: `room-${Date.now()}` });
    setEditingRoomIndex(null);
    setRoomDialogOpen(true);
  };

  const handleEditRoom = (index: number) => {
    setCurrentRoom(propertyData.rooms[index]);
    setEditingRoomIndex(index);
    setRoomDialogOpen(true);
  };

  const handleSaveRoom = () => {
    const updatedRoom = { ...currentRoom };

    if (editingRoomIndex !== null) {
      setPropertyData((prev) => ({
        ...prev,
        rooms: prev.rooms.map((room, index) =>
          index === editingRoomIndex ? updatedRoom : room
        ),
      }));
    } else {
      setPropertyData((prev) => ({
        ...prev,
        rooms: [...prev.rooms, updatedRoom],
      }));
    }

    setRoomDialogOpen(false);
    setCurrentRoom(initialRoomData);
    setEditingRoomIndex(null);
  };

  const handleDeleteRoom = (index: number) => {
    setPropertyData((prev) => ({
      ...prev,
      rooms: prev.rooms.filter((_, i) => i !== index),
    }));
  };

  const handleRoomImageUpload = (
    event: React.ChangeEvent<HTMLInputElement>
  ) => {
    const newImages = roomImageUpload.handleImageUpload(
      event,
      currentRoom.images
    );
    setCurrentRoom((prev) => ({
      ...prev,
      images: [...prev.images, ...newImages],
    }));
  };

  const handleRoomImageRemove = (imageId: string) => {
    setCurrentRoom((prev) => ({
      ...prev,
      images: roomImageUpload.removeImage(prev.images, imageId),
    }));
  };

  const handleRoomImageSetPrimary = (imageId: string) => {
    setCurrentRoom((prev) => ({
      ...prev,
      images: roomImageUpload.setPrimaryImage(prev.images, imageId),
    }));
  };

  const visibleSteps = React.useMemo(() => {
    if (propertyData.isEntirePlaceRentable) {
      return steps;
    } else {
      return steps.filter((step) => step !== "Property Images");
    }
  }, [propertyData.isEntirePlaceRentable]);

  const validateStep = (step: number): boolean => {
    const actualStep = visibleSteps[step];
    const newErrors: Record<string, string> = {};

    if (actualStep === "Property Details") {
      if (!propertyData.name.trim())
        newErrors.name = "Property name is required";
      if (!propertyData.description.trim())
        newErrors.description = "Description is required";
      if (!propertyData.address.trim())
        newErrors.address = "Address is required";
      if (propertyData.pricePerMonth <= 0)
        newErrors.pricePerMonth = "Price must be greater than 0";
      if (propertyData.areaInSquareMeters <= 0)
        newErrors.areaInSquareMeters = "Area must be greater than 0";
    }

    if (actualStep === "Property Images") {
      if (propertyData.images.length === 0)
        newErrors.images = "At least one image is required";
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleNext = () => {
    if (validateStep(activeStep)) {
      setActiveStep((prev) => prev + 1);
    }
  };

  const handleBack = () => {
    setActiveStep((prev) => prev - 1);
  };

  const handleSubmit = async () => {
    if (!validateStep(activeStep)) return;

    setLoading(true);
    try {
      if (!user?.userId) throw new Error("User not authenticated");

      const roomIdMapping = new Map<string, string>();

      const sanitizedRooms = propertyData.rooms.map(({ images, ...room }) => ({
        ...room,
        id: undefined,
      }));

      const response = await fetch(`${API_BASE_URL}/api/properties`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          name: propertyData.name,
          description: propertyData.description,
          detailedDescription: propertyData.detailedDescription || "",
          address: propertyData.address,
          pricePerMonth: propertyData.pricePerMonth,
          areaInSquareMeters: propertyData.areaInSquareMeters,
          isEntirePlaceRentable: propertyData.isEntirePlaceRentable,
          isAvailable: propertyData.isAvailable,
          availableSince: propertyData.availableSince,
          ownerId: user.userId,
          rooms: sanitizedRooms,
          images: [],
        }),
      });

      if (!response.ok) throw new Error(await response.text());

      const createdProperty = await response.json();

      if (createdProperty.rooms && createdProperty.rooms.length > 0) {
        propertyData.rooms.forEach((originalRoom, index) => {
          if (createdProperty.rooms[index]) {
            roomIdMapping.set(originalRoom.id, createdProperty.rooms[index].id);
          }
        });
      }

      if (
        propertyData.isEntirePlaceRentable &&
        propertyData.images.length > 0
      ) {
        const imageFormData = new FormData();
        propertyData.images.forEach((image, index) => {
          if (image.file) {
            imageFormData.append("files", image.file);
            imageFormData.append(
              "metadata",
              JSON.stringify({
                id: image.id,
                originalFileName: image.originalFileName,
                isPrimary: image.isPrimary,
                displayOrder: image.displayOrder ?? index,
              })
            );
          }
        });

        const imageUploadResponse = await fetch(
          `${API_BASE_URL}/api/properties/${createdProperty.id}/images`,
          {
            method: "POST",
            body: imageFormData,
          }
        );

        if (!imageUploadResponse.ok)
          throw new Error(await imageUploadResponse.text());
      }

      for (const room of propertyData.rooms) {
        if (!room.images || room.images.length === 0) continue;

        const serverRoomId = roomIdMapping.get(room.id);
        if (!serverRoomId) {
          console.error(`Could not find server ID for room ${room.id}`);
          continue;
        }

        const roomFormData = new FormData();
        room.images.forEach((image, index) => {
          if (image.file) {
            roomFormData.append("files", image.file);
            roomFormData.append(
              "metadata",
              JSON.stringify({
                id: image.id,
                originalFileName: image.originalFileName,
                isPrimary: image.isPrimary,
                displayOrder: image.displayOrder ?? index,
              })
            );
          }
        });

        const uploadRoomImagesResponse = await fetch(
          `${API_BASE_URL}/api/properties/rooms/${serverRoomId}/images`,
          {
            method: "POST",
            body: roomFormData,
          }
        );

        if (!uploadRoomImagesResponse.ok) {
          const errorText = await uploadRoomImagesResponse.text();
          console.error(
            `Failed to upload images for room ${serverRoomId}:`,
            errorText
          );
          throw new Error(`Failed to upload images for room: ${errorText}`);
        }
      }

      setSubmitStatus("success");
      console.log("✅ Property and all images created successfully");
    } catch (err) {
      console.error("Error creating property:", err);
      setSubmitStatus("error");
    } finally {
      setLoading(false);
    }
  };

  const renderStepContent = () => {
    const actualStep = visibleSteps[activeStep];

    switch (actualStep) {
      case "Property Details":
        return (
          <PropertyDetailsStep
            propertyData={propertyData}
            errors={errors}
            onPropertyChange={handlePropertyChange}
          />
        );
      case "Property Images":
        return (
          <PropertyImagesStep
            images={propertyData.images}
            errors={errors}
            fileInputRef={propertyImageUpload.fileInputRef}
            onImageUpload={handlePropertyImageUpload}
            onRemoveImage={handlePropertyImageRemove}
            onSetPrimaryImage={handlePropertyImageSetPrimary}
          />
        );
      case "Rooms":
        return (
          <RoomsStep
            rooms={propertyData.rooms}
            isEntirePlaceRentable={propertyData.isEntirePlaceRentable}
            onAddRoom={handleAddRoom}
            onEditRoom={handleEditRoom}
            onDeleteRoom={handleDeleteRoom}
          />
        );
      default:
        return null;
    }
  };

  return (
    <Box sx={{ maxWidth: 800, mx: "auto", p: 3 }}>
      <Typography variant="h4" gutterBottom>
        Create New Property
      </Typography>
      <Stepper activeStep={activeStep} sx={{ mb: 4 }}>
        {visibleSteps.map((label) => (
          <Step key={label}>
            <StepLabel>{label}</StepLabel>
          </Step>
        ))}
      </Stepper>
      <Paper sx={{ p: 3 }}>
        {renderStepContent()}
        {submitStatus === "success" && (
          <Box sx={{ mt: 3 }}>
            <Typography color="success.main" variant="body1">
              Property created successfully!
            </Typography>
          </Box>
        )}
        {submitStatus === "error" && (
          <Box sx={{ mt: 3 }}>
            <Typography color="error.main" variant="body1">
              Failed to create property. Please try again.
            </Typography>
          </Box>
        )}
        <Box sx={{ display: "flex", justifyContent: "space-between", mt: 4 }}>
          <Button onClick={handleBack} disabled={activeStep === 0}>
            Back
          </Button>
          <Box>
            {activeStep < visibleSteps.length - 1 ? (
              <Button variant="contained" onClick={handleNext} sx={{ ml: 1 }}>
                Next
              </Button>
            ) : (
              <Button
                variant="contained"
                onClick={handleSubmit}
                disabled={loading}
                sx={{ ml: 1 }}
              >
                {loading ? "Creating..." : "Create Property"}
              </Button>
            )}
          </Box>
        </Box>
      </Paper>
      <RoomDialog
        open={roomDialogOpen}
        room={currentRoom}
        isEditing={editingRoomIndex !== null}
        fileInputRef={roomImageUpload.fileInputRef}
        onClose={() => setRoomDialogOpen(false)}
        onSave={handleSaveRoom}
        onRoomChange={handleRoomChange}
        onImageUpload={handleRoomImageUpload}
        onRemoveImage={handleRoomImageRemove}
        onSetPrimaryImage={handleRoomImageSetPrimary}
      />
    </Box>
  );
};
