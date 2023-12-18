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

    public int GroupId { get; set; }

    public virtual Group Group { get; set; } = null!;
}
