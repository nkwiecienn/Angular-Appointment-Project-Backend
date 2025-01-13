namespace Backend.DTOs;

public class AbsenceDto
{
    public int Id { get; set; }
    public string Day { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
}