﻿using System.ComponentModel.DataAnnotations;

namespace OneCampus.Infrastructure.Data.Entities;

public class Message
{
    [Key]
    public int Id { get; set; }
    public string Content { get; set; } = null!;
    public DateTime CreateDate { get; set; }

    public virtual User User { get; set; } = null!;
    public virtual Institution? Institution { get; set; }
    public virtual Class? Class { get; set; } 
    public virtual Group? Group { get; set; }
}
