export const API_BASE_URL = 'http://localhost:5000';

export interface PropertyImageDto {
  id: string;
  imageUrl?: string;
  file?: File;
  originalFileName: string;
  isPrimary: boolean;
  displayOrder: number;
}

export interface RoomDto {
  id: string;
  name: string;
  description: string;
  pricePerMonth: number;
  areaInSquareMeters: number;
  capacity: number;
  isAvailable: boolean;
  createdAt?: string;
  images: PropertyImageDto[];
  detailedDescription?: string;
  reviews?: ReviewDto[];
}

export interface RoomDetailsResponseDto {
  room: RoomDto;
  propertyAddress: string;
  propertyName: string;
  otherRoomsInProperty: RoomSummaryDto[];
}

export interface RoomSummaryDto {
  id: string;
  name: string;
  description: string;
  pricePerMonth: number;
  areaInSquareMeters: number;
  capacity: number;
  isAvailable: boolean;
  mainImage: string;
}

export interface UserDto {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  role: string;
}

export interface ReviewDto {
  id: string;
  reviewerId: string;
  propertyId?: string;
  roomId?: string;
  rating: number;
  comment: string;
  createdAt: string;
  reviewer?: UserDto;
}

export interface Property {
  id: string;
  name: string;
  description: string;
  detailedDescription?: string;
  address: string;
  pricePerMonth: number;
  areaInSquareMeters: number;
  isEntirePlaceRentable: boolean;
  images: PropertyImageDto[];
  rooms: RoomDto[];
  ownerId: string;
  createdAt: string;
  isAvailable: boolean;
  owner?: UserDto;
  reviews?: ReviewDto[];
}

export interface ReviewFormData {
  rating: number;
  comment: string;
}

export interface PropertyFormData {
  name: string;
  description: string;
  detailedDescription?: string;
  address: string;
  pricePerMonth: number;
  areaInSquareMeters: number;
  isEntirePlaceRentable: boolean;
  isAvailable: boolean;
  images: PropertyImageDto[];
  rooms: RoomDto[];
}

export interface PropertyFilterDto {
  location?: string;
  minPrice?: number;
  maxPrice?: number;
  minArea?: number;
  maxArea?: number;
  isAvailable?: boolean;
  isEntirePlaceRentable?: boolean;
}

export interface RoomSearchFilterDto {
  location?: string;        // Add location filtering
  minPrice?: number;
  maxPrice?: number;
  minArea?: number;
  maxArea?: number;
  minCapacity?: number;
  isAvailable?: boolean;
}

export interface RoomFilterDto {
  id: string;
  name: string;
  description: string;
  pricePerMonth: number;
  areaInSquareMeters: number;
  capacity: number;
  isAvailable: boolean;
  images: any[];
  propertyId: string;
  propertyName: string;
  propertyAddress: string;
  propertyOwnerId?: string;
}

export const initialPropertyData: PropertyFormData = {
  name: "",
  description: "",
  detailedDescription: "",
  address: "",
  pricePerMonth: 0,
  areaInSquareMeters: 0,
  isEntirePlaceRentable: true,
  isAvailable: true,
  images: [],
  rooms: [],
};

export const initialRoomData: RoomDto = {
  id: "",
  name: "",
  description: "",
  detailedDescription: "",
  pricePerMonth: 0,
  areaInSquareMeters: 0,
  capacity: 1,
  isAvailable: true,
  images: [],
};

export const steps = [
  "Property Details",
  "Property Images",
  "Rooms",
  "Review & Submit",
];