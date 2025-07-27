import React from "react";
import { BaseCard } from "./BaseCard";
import { PropertyImageDto } from "../types/types";

interface Image {
  id: string;
  imageUrl: string;
  originalFileName: string;
  isPrimary: boolean;
  displayOrder: number;
}

interface RoomCardProps {
  name: string;
  description: string;
  image?: string; // Primary image URL
  images?: PropertyImageDto[]; // All images for carousel
  price: number;
  area: number;
  capacity: number;
  address: string;
  onViewDetails: () => void;
  canDelete?: boolean;
  onDelete?: () => void;
}

export const RoomCard: React.FC<RoomCardProps> = ({
  name,
  description,
  image,
  images = [],
  price,
  area,
  capacity,
  address,
  onViewDetails,
  canDelete = false,
  onDelete,
}) => {
  return (
    <BaseCard
      title={name}
      description={description}
      image={image}
      images={images}
      address={address}
      price={price}
      area={area}
      capacity={capacity}
      cardType="room"
      onViewDetails={onViewDetails}
      canDelete={canDelete}
      onDelete={onDelete}
    />
  );
};
