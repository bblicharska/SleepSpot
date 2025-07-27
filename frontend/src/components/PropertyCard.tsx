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

interface PropertyCardProps {
  name: string;
  description: string;
  image?: string; // Primary image URL
  images?: PropertyImageDto[]; // All images for carousel
  address: string;
  price: number;
  area: number;
  onViewDetails: () => void;
  canDelete?: boolean;
  onDelete?: () => void;
}

export const PropertyCard: React.FC<PropertyCardProps> = ({
  name,
  description,
  image,
  images = [],
  address,
  price,
  area,
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
      cardType="property"
      onViewDetails={onViewDetails}
      canDelete={canDelete}
      onDelete={onDelete}
    />
  );
};
