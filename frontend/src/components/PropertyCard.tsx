import React from "react";
import { BaseCard } from "./BaseCard";
import { PropertyImageDto } from "../types/types";

interface PropertyCardProps {
  name: string;
  description: string;
  image?: string;
  images?: PropertyImageDto[];
  address: string;
  price: number;
  area: number;
  onViewDetails: () => void;
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
    />
  );
};
