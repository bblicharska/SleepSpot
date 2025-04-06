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


        public PropertyUnitOfWork(PropertyDbContext context, IPropertyRepository propertyRepository, IPropertyImageRepository propertyImageRepository)
        {
            _context = context;
            PropertyRepository = propertyRepository;
            PropertyImageRepository = propertyImageRepository;
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
