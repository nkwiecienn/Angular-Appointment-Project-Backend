using System.ComponentModel.DataAnnotations;

namespace Backend.DataBase.Data.Models;

public class Question
{
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    public User User { get; set; }

    [Required]
    public required string Content { get; set; }

    [Required]
    public required string Answer { get; set; }

    [Required]
    public required string Topic { get; set; }

    [Required]
    public DateTime CreationDateTime { get; set; }

    [Required]
    public DateTime ModificationDateTime { get; set; }

    [Required]
    public int Weight { get; set; }
}