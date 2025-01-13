using System.ComponentModel.DataAnnotations;

namespace Backend.Controllers.Auth.DTO;

public class RegisterModel
{
    [Required(ErrorMessage = "First Name is required.")]
    [DataType(DataType.Text)]
    public required string FirstName { get; set; }

    [Required(ErrorMessage = "Last Name is required.")]
    [DataType(DataType.Text)]
    public required string LastName { get; set; }

    [Required(ErrorMessage = "Email is required.")]
    [DataType(DataType.EmailAddress)]
    public required string Email { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    [DataType(DataType.Password)]
    public required string Password { get; set; }
    
    [Required(ErrorMessage = "Role is required.")]
    public required int Role { get; set; }
}