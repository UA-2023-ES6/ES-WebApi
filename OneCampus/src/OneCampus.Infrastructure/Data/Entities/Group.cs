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

    public virtual Class Class { get; set; } = null!;
    public virtual List<User> Users { get; set; } = new();
    public virtual List<Event> Events { get; set; } = new();
}
