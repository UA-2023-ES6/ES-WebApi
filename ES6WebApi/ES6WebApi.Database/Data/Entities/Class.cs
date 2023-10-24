using System.ComponentModel.DataAnnotations;

namespace ES6WebApi.Database.Data.Entities;

public class Class
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public DateTime? DeleteDate { get; set; }

    public virtual Institution Institution { get; set; }
    public virtual List<Message> Messages { get; set; }
    public virtual List<Event> Events { get; set; }
    public virtual List<Group> Groups { get; set; }
}
