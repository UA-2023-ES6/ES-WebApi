using System.ComponentModel.DataAnnotations;

namespace ES6WebApi.Database.Data.Entities;

public class User
{
    [Key]
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public DateTime? DeleteDate { get; set; }

    public virtual List<UserRoleInstitution> UserRoleInstitution { get; set; }
    public virtual List<Group> Groups { get; set; }
    public virtual List<Class> Classes { get; set; }
}
