using System.ComponentModel.DataAnnotations;

namespace Backend.Controllers.Auth.DTO;

public class LoginModel
{
    [Required(ErrorMessage = "Email is required.")]
    [DataType(DataType.EmailAddress)]
    public required string Email { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    [DataType(DataType.Password)]
    public required string Password { get; set; }
}