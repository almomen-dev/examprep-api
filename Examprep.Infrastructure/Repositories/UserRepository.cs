using Examprep.Application.Repositories;
using Examprep.Domain.Model;
using Examprep.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Examprep.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
        }

        public async Task<User> AddAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User?> GetByRefreshTokenAsync(string refreshToken)
    => await _context.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task<(List<Question> items, int totalCount)> QueryAsync(
    string? search, string? sortBy, string? sortDir, int page, int pageSize)
        {
            var query = _context.Questions.AsQueryable();

            // 1. Filter
            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(q => q.text.Contains(search));

            // 2. Sort
            query = sortBy?.ToLower() switch
            {
                "text" => sortDir == "desc"
                    ? query.OrderByDescending(q => q.text)
                    : query.OrderBy(q => q.text),
                _ => sortDir == "desc"
                    ? query.OrderByDescending(q => q.id)
                    : query.OrderBy(q => q.id),
            };

            // 3. Paginate
            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

    }
}