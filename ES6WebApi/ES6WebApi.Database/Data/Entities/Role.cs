using System.ComponentModel.DataAnnotations;

namespace ES6WebApi.Database.Data.Entities;

public class Role
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }

    public DateTime CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public DateTime? DeleteDate { get; set; }

    public virtual List<UserRoleInstitution> UserRoleInstitution { get; set; }
}
