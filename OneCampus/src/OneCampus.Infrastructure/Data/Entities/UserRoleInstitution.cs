using Microsoft.EntityFrameworkCore;

namespace OneCampus.Infrastructure.Data.Entities;

[PrimaryKey(nameof(UserId), nameof(RoleId), nameof(InstitutionId))]
public class UserRoleInstitution
{
    public Guid UserId { get; set; }
    public int RoleId { get; set; }
    public int InstitutionId { get; set; }

    public virtual User User { get; set; } = null!;
    public virtual Role Role { get; set; } = null!;
    public virtual Institution Institution { get; set; } = null!;

}
