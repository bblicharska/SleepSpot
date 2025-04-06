using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PropertyService.Application.Dto;
using PropertyService.Domain.Contracts;
using PropertyService.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyService.Application.Services
{
    public class PropertyManager : IPropertyService
    {
        private readonly IPropertyUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PropertyManager(IPropertyUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // Pobieranie nieruchomości po ID
        public async Task<PropertyDto> GetPropertyByIdAsync(Guid id)
        {
            var property = await _unitOfWork.PropertyRepository.GetByIdAsync(id);
            if (property == null)
            {
                throw new KeyNotFoundException($"Property with ID {id} not found.");
            }

            return _mapper.Map<PropertyDto>(property);
        }

        // Pobieranie wszystkich nieruchomości
        public async Task<IEnumerable<PropertyDto>> GetAllPropertiesAsync()
        {
            var properties = await _unitOfWork.PropertyRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<PropertyDto>>(properties);
        }

        // Pobieranie nieruchomości według właściciela
        public async Task<IEnumerable<PropertyDto>> GetPropertiesByOwnerIdAsync(Guid ownerId)
        {
            var properties = await _unitOfWork.PropertyRepository.GetByOwnerIdAsync(ownerId);
            return _mapper.Map<IEnumerable<PropertyDto>>(properties);
        }

        // Wyszukiwanie nieruchomości
        public async Task<IEnumerable<PropertyDto>> SearchPropertiesAsync(string location, decimal? minPrice, decimal? maxPrice)
        {
            var properties = await _unitOfWork.PropertyRepository.SearchAsync(location, minPrice, maxPrice);
            return _mapper.Map<IEnumerable<PropertyDto>>(properties);
        }

        // Tworzenie nowej nieruchomości
        public async Task<PropertyDto> CreatePropertyAsync(CreatePropertyDto createPropertyDto)
        {
            using (_unitOfWork)  // Transakcyjność
            {
                var property = _mapper.Map<Property>(createPropertyDto);
                await _unitOfWork.PropertyRepository.AddAsync(property);
                _unitOfWork.Commit(); // Zapis zmian

                return _mapper.Map<PropertyDto>(property);
            }
        }

        // Aktualizacja nieruchomości
        public async Task<PropertyDto> UpdatePropertyAsync(Guid id, UpdatePropertyDto updatePropertyDto)
        {
            // Step 1: Update the property entity (without images)
            var property = await _unitOfWork.PropertyRepository.GetByIdAsync(id);
            if (property == null)
            {
                throw new KeyNotFoundException($"Property with ID {id} not found.");
            }

            // Update basic property fields (without touching the Images collection)
            property.Name = updatePropertyDto.Name;
            property.Description = updatePropertyDto.Description;
            property.Address = updatePropertyDto.Address;
            property.PricePerNight = updatePropertyDto.PricePerNight;
            property.Capacity = updatePropertyDto.Capacity;

            // Save property changes first
            _unitOfWork.PropertyRepository.Update(property);
            await _unitOfWork.CommitAsync();

            // Step 2: Handle images in a separate transaction
            // First, delete all existing images for this property
            var existingImages = await _unitOfWork.PropertyImageRepository.GetImagesByPropertyIdAsync(id);
            _unitOfWork.PropertyImageRepository.RemoveRange(existingImages);
            await _unitOfWork.CommitAsync();

            // Step 3: Add new images
            if (updatePropertyDto.Images != null && updatePropertyDto.Images.Any())
            {
                var newImages = updatePropertyDto.Images.Select(url => new PropertyImage
                {
                    Id = Guid.NewGuid(),
                    PropertyId = id,
                    ImageUrl = url
                }).ToList();

                await _unitOfWork.PropertyImageRepository.AddRangeAsync(newImages);
                await _unitOfWork.CommitAsync();
            }

            // Step 4: Get the updated property with images for the return DTO
            var updatedProperty = await _unitOfWork.PropertyRepository.GetByIdWithImagesAsync(id);
            return _mapper.Map<PropertyDto>(updatedProperty);
        }


        // Usunięcie nieruchomości
        public async Task DeletePropertyAsync(Guid id)
        {
            using (_unitOfWork)  // Transakcyjność
            {
                var property = await _unitOfWork.PropertyRepository.GetByIdAsync(id);
                if (property == null)
                {
                    throw new KeyNotFoundException($"Property with ID {id} not found.");
                }

                _unitOfWork.PropertyRepository.Delete(property);
                _unitOfWork.Commit(); // Zapis zmian
            }
        }
    }

}
