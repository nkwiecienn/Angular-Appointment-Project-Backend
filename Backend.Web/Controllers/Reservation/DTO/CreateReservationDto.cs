namespace Backend.DTOs;

public class CreateReservationDto
{
    public string Date { get; set; }
    public string StartTime { get; set; }
    public string EndTime { get; set; }
    public int Length { get; set; }
    public string Type { get; set; }
    public string Gender { get; set; }
    public int Age { get; set; }
    public string? Details { get; set; }
    public int UserId { get; set; }
    public int DoctorId { get; set; }
}