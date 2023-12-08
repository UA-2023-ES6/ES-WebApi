using System.ComponentModel.DataAnnotations;

namespace OneCampus.Infrastructure.Data.Entities;

public class Permission
{
    [Key]
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public DateTime CreateDate { get; set; }

    public virtual List<UserGroupPermissions> UserGroupPermissions { get; set; } = new();
}
