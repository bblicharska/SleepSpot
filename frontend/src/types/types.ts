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

export interface RoomDetailsResponseDto extends RoomDto {
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
  reviews?: ReviewDto[];
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
  location?: string;       
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

export interface GroupListingDto {
  id: string;
  groupId: string;
  group?: GroupDto;
  title: string;
  description: string;
  desiredRoommatesCount: number;
  status: "Active" | "Closed";
  propertyId?: string;
  property?: Property;
  preferredCity: string;
  maxBudgetPerPerson?: number;
  createdAt: string;
  applications?: RoomApplicationDto[];
  propertyAlreadyRented?: boolean;
  roomId?: string;
  room?: RoomDetailsResponseDto;
}

export interface GroupDto {
  id: string;
  name: string;
  description: string;
  createdByUserId: string;
  createdAt: string;
  members: GroupMemberDto[];
  listings?: GroupListingDto[];
}

export interface GroupMemberDto {
  id: string;
  groupId: string;
  userId: string;
  role: "Member" | "Admin";
  joinedAt: string;
  user?: UserDto;
}

export interface CreateGroupListingDto {
  groupId: string;
  title: string;
  description: string;
  desiredRoommatesCount: number;
  propertyId?: string;
  roomId?: string;
  propertyAlreadyRented: boolean;
  preferredCity: string;
  maxBudgetPerPerson?: number;
}

export interface CreateGroupDto {
  name: string;
  description: string;
  createdByUserId?: string;
  memberEmails: string[];
}

export interface AddMemberByEmailRequest {
  email: string;
  role: string;
  groupId?: string;
}

export interface RoomApplicationDto {
  id: string;
  listingId: string;
  applicantUserId: string;
  message: string;
  status: string;
  createdAt: string;
  applicant?: UserDto;
}

export interface GroupListingFilters {
  page: number;
  pageSize: number;
  preferredCity?: string;
  minBudget?: number;
  maxBudget?: number;
  minRoommates?: number;
  maxRoommates?: number;
  hasProperty?: boolean;
  hasRoom?: boolean;
  noPropertyYet?: boolean;
  searchTerm?: string;
  sortBy: string;
  sortOrder: "asc" | "desc";
}

export interface RentalAgreementDto {
  id: string;
  propertyId: string;
  roomId: string | null;
  property?: Property;
  room?:RoomDto;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
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