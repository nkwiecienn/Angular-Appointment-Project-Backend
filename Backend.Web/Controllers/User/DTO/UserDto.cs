namespace Backend.DTOs;

public class UserDto
{
    public int Id { get; set; }
    public string AccountId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime CreationDateTime { get; set; }
    public int Role { get; set; }
}
