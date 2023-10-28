using System.ComponentModel.DataAnnotations;

namespace OneCampus.Infrastructure.Data.Entities;

public class Institution
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public DateTime CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public DateTime? DeleteDate { get; set; }

    public virtual List<UserRoleInstitution> UserRoleInstitution { get; set; } = new();
    public virtual List<Event> Events { get; set; } = new();
    public virtual List<Class> Classes { get; set; } = new();
    public virtual List<Message> Messages { get; set; } = new();
}
