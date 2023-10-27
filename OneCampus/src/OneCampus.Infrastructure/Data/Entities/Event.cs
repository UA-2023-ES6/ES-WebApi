using System.ComponentModel.DataAnnotations;

namespace OneCampus.Infrastructure.Data.Entities;

public class Event
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }

    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public DateTime? DeleteDate { get; set; }

    public virtual Institution? Institution { get; set; }
    public virtual Class? Class { get; set; }
    public virtual Group? Group { get; set; }
}
