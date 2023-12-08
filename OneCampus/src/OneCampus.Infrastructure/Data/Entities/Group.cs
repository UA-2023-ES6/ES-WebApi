using System.ComponentModel.DataAnnotations;

namespace OneCampus.Infrastructure.Data.Entities;

public class Group
{
    [Key]
    public int Id { get; set; }

    public string Name { get; set; } = null!;
    public DateTime CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public DateTime? DeleteDate { get; set; }

    public virtual List<UserGroup> UserGroups { get; set; } = new();
    public virtual List<Event> Events { get; set; } = new();
    public virtual List<Message> Messages { get; set; } = new();
    public virtual List<Group> Groups { get; set; } = new();

    public int? ParentId { get; set; }
    public virtual Institution? Institution { get; set; }
    public virtual Group? Parent { get; set; }
}
