using OneCampus.Domain.Entities.Institutions;

namespace OneCampus.Domain.Repositories;

public interface IInstitutionRepository
{
    Task<Institution> FindAsync(int id);

    Task<IEnumerable<Institution>> GetAsync(Guid userId);
}
