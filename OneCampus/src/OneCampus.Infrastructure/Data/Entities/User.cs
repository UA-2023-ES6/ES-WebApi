using System.ComponentModel.DataAnnotations;

namespace OneCampus.Infrastructure.Data.Entities;

public class User
{
    [Key]
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public DateTime CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public DateTime? DeleteDate { get; set; }

    public virtual List<UserRoleInstitution> UserRoleInstitution { get; set; } = new();
    public virtual List<Group> Groups { get; set; } = new();
    public virtual List<Class> Classes { get; set; } = new();
}
