using Microsoft.EntityFrameworkCore;
using PropertyService.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyService.Infrastructure
{
    public class PropertyUnitOfWork : IPropertyUnitOfWork
    {
        private readonly PropertyDbContext _context;

        public IPropertyRepository PropertyRepository { get; }
        public IPropertyImageRepository PropertyImageRepository { get; }
        public IRoomRepository RoomRepository { get; }


        public PropertyUnitOfWork(PropertyDbContext context, IPropertyRepository propertyRepository, IPropertyImageRepository propertyImageRepository, IRoomRepository roomRepository)
        {
            _context = context;
            PropertyRepository = propertyRepository;
            PropertyImageRepository = propertyImageRepository;
            RoomRepository = roomRepository;
        }

        public void Commit()
        {
            _context.SaveChanges();
        }

        public async Task<int> CommitAsync()  // Teraz zwraca int
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }

}
