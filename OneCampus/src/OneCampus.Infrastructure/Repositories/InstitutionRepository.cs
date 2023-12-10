using Microsoft.EntityFrameworkCore;
using OneCampus.Domain.Entities.Institutions;
using OneCampus.Domain.Repositories;
using OneCampus.Infrastructure.Data;

namespace OneCampus.Infrastructure.Repositories;

public class InstitutionRepository : IInstitutionRepository
{
    private readonly IDbContextFactory<OneCampusDbContext> _oneCampusDbContextFactory;

    public InstitutionRepository(IDbContextFactory<OneCampusDbContext> oneCampusDbContextFactory)
    {
        _oneCampusDbContextFactory = oneCampusDbContextFactory.ThrowIfNull().Value;
    }

    public async Task<Institution> FindAsync(int id)
    {
        using (var context = await _oneCampusDbContextFactory.CreateDbContextAsync())
        {
            var institution = await context.Institutions
                .AsNoTracking()
                .FirstOrDefaultAsync(item => item.DeleteDate == null && item.Id == id);

            return institution.ToInstitution()!;
        }
    }

    public async Task<IEnumerable<Institution>> GetAsync(Guid userId)
    {
        using (var context = await _oneCampusDbContextFactory.CreateDbContextAsync())
        {
            var institutions = await context.Institutions
                .AsNoTracking()
                .Where(item => item.DeleteDate == null && item.Group.Users.Any(item => item.Id == userId))
                .ToListAsync();

            return institutions.Select(item => item.ToInstitution()!);
        }
    }
}
