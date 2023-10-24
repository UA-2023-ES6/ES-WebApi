using System.ComponentModel.DataAnnotations;

namespace ES6WebApi.Database.Data.Entities;

public class Group
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public DateTime? DeleteDate { get; set; }

    public virtual Class Class { get; set; }
    public virtual List<User> Users { get; set; }
    public virtual List<Event> Events { get; set; }
}
