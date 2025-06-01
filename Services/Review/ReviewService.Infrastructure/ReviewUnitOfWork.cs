using ReviewService.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviewService.Infrastructure
{
    public class ReviewUnitOfWork : IReviewUnitOfWork
    {
        private readonly ReviewDbContext _context;

        public IReviewRepository ReviewRepository { get; }

        public ReviewUnitOfWork(ReviewDbContext context, IReviewRepository reviewRepository)
        {
            _context = context;
            ReviewRepository = reviewRepository;
        }

        public void Commit()
        {
            _context.SaveChanges();
        }

        public async Task<int> CommitAsync()  
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
