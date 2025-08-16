import React from "react";
import { BaseCard } from "./BaseCard";
import { PropertyImageDto } from "../types/types";

interface RoomCardProps {
  name: string;
  description: string;
  image?: string;
  images?: PropertyImageDto[];
  price: number;
  area: number;
  capacity: number;
  address: string;
  onViewDetails: () => void;
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
    />
  );
};
