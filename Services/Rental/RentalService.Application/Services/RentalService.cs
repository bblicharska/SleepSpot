using AutoMapper;
using RentalService.Application.Dto;
using RentalService.Application.Interfaces;
using RentalService.Domain.Contracts;
using RentalService.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentalService.Application.Services
{
    public class RentalService : IRentalService
    {
        private readonly IRentalUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserClient _userClient;
        private readonly IGroupClient _groupClient;
        private readonly IPropertyClient _propertyClient;

        public RentalService(IRentalUnitOfWork unitOfWork, IMapper mapper, IUserClient userClient, IPropertyClient propertyClient, IGroupClient groupClient)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userClient = userClient;
            _groupClient = groupClient;
            _propertyClient = propertyClient;
        }

        public async Task<IEnumerable<RentalAgreementDto>> GetAllAsync()
        {
            var agreements = await _unitOfWork.RentalAgreementRepository.GetAllAsync();
            var agreementDtos = _mapper.Map<List<RentalAgreementDto>>(agreements);

            var tasks = agreementDtos.Select(async dto =>
            {
                // Property
                dto.Property = await _propertyClient.GetPropertyByIdAsync(dto.PropertyId);

                // Room (optional)
                if (dto.RoomId.HasValue)
                {
                    dto.Room = await _propertyClient.GetRoomByIdAsync(dto.RoomId.Value);
                }

                // Group (optional)
                if (dto.GroupId.HasValue)
                {
                    dto.Group = await _groupClient.GetGroupByIdAsync(dto.GroupId.Value);
                }

                // User (optional)
                if (dto.UserId.HasValue)
                {
                    dto.User = await _userClient.GetUserByIdAsync(dto.UserId.Value);
                }

                return dto;
            });

            return await Task.WhenAll(tasks);
        }


        public async Task<RentalAgreementDto> GetByIdAsync(Guid id)
        {
            var agreement = await _unitOfWork.RentalAgreementRepository.GetByIdAsync(id);
            if (agreement == null) return null;

            var dto = _mapper.Map<RentalAgreementDto>(agreement);

            // Pobierz szczegóły Property
            dto.Property = await _propertyClient.GetPropertyByIdAsync(agreement.PropertyId);

            // Pobierz szczegóły Room, jeśli jest RoomId
            if (agreement.RoomId.HasValue)
            {
                dto.Room = await _propertyClient.GetRoomByIdAsync(agreement.RoomId.Value);
            }

            // Pobierz szczegóły Group, jeśli jest GroupId
            if (dto.GroupId.HasValue)
            {
                dto.Group = await _groupClient.GetGroupByIdAsync(dto.GroupId.Value);
            }

            // Pobierz szczegóły User, jeśli jest UserId
            if (agreement.UserId.HasValue)
            {
                dto.User = await _userClient.GetUserByIdAsync(agreement.UserId.Value);
            }

            return dto;
        }

        public async Task<RentalAgreementDto> CreateAsync(CreateRentalAgreementDto dto)
        {
            var agreement = _mapper.Map<RentalAgreement>(dto);
            await _unitOfWork.RentalAgreementRepository.AddAsync(agreement);
            await _unitOfWork.CommitAsync();
            return _mapper.Map<RentalAgreementDto>(agreement);
        }

        public async Task<RentalAgreementDto> UpdateAsync(Guid id, UpdateRentalAgreementDto dto)
        {
            var agreement = await _unitOfWork.RentalAgreementRepository.GetByIdAsync(id);
            if (agreement == null) return null;

            _mapper.Map(dto, agreement);
            _unitOfWork.RentalAgreementRepository.Update(agreement);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<RentalAgreementDto>(agreement);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var agreement = await _unitOfWork.RentalAgreementRepository.GetByIdAsync(id);
            if (agreement == null) return false;

            _unitOfWork.RentalAgreementRepository.Remove(agreement);
            await _unitOfWork.CommitAsync();
            return true;
        }
    }
}
