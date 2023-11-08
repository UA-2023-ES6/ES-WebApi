using Database = OneCampus.Infrastructure.Data.Entities;

namespace OneCampus.Domain.Entities.Institutions;

internal static class InstitutionsExtensions
{
    internal static Institution? ToInstitution(this Database.Institution? institution)
    {
        if (institution is null)
        {
            return null;
        }

        return new Institution(institution.Id, institution.Name);
    }
}
