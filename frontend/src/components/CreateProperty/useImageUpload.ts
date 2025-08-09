import {  useRef } from 'react';
import { PropertyImageDto } from '../../types/types';

export const useImageUpload = () => {
  const fileInputRef = useRef<HTMLInputElement>(null);

  const createImageFromFile = (file: File, index: number, existingImagesCount: number): PropertyImageDto => ({
    id: `${Date.now()}-${index}`,
    file,
    imageUrl: URL.createObjectURL(file),
    isPrimary: false,
    displayOrder: existingImagesCount + index,
    originalFileName: file.name,
  });

  const handleImageUpload = (
    event: React.ChangeEvent<HTMLInputElement>,
    existingImages: PropertyImageDto[]
  ): PropertyImageDto[] => {
    const files = event.target.files;
    if (!files) return [];

    return Array.from(files).map((file, index) =>
      createImageFromFile(file, index, existingImages.length)
    );
  };

  const removeImage = (images: PropertyImageDto[], imageId: string): PropertyImageDto[] =>
    images.filter((img) => img.id !== imageId);

  const setPrimaryImage = (images: PropertyImageDto[], imageId: string): PropertyImageDto[] =>
    images.map((img) => ({
      ...img,
      isPrimary: img.id === imageId,
    }));

  return {
    fileInputRef,
    handleImageUpload,
    removeImage,
    setPrimaryImage,
  };
};