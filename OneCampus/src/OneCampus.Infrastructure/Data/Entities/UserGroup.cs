using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace OneCampus.Infrastructure.Data.Entities;

[Index(nameof(UserId), nameof(GroupId), IsUnique = true)]
public class UserGroup
{
    [Key]
    public int Id { get; set; }

    public Guid UserId { get; set; }

    public int GroupId { get; set; }

    public User User { get; set; } = null!;
    public Group Group { get; set; } = null!;
}
