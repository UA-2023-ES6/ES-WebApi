using System.ComponentModel.DataAnnotations;

namespace ES6WebApi.Database.Data.Entities;

public class Message
{
    [Key]
    public int Id { get; set; }
    public string Content { get; set; }
    public DateTime CreateDate { get; set; }

    public virtual User User { get; set; }
    public virtual Institution? Institution { get; set; }
    public virtual Class? Class { get; set; }
    public virtual Group? Group { get; set; }
}
