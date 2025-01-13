using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Backend.Configuration;
using Backend.Controllers.Auth.DTO;
using Backend.DataBase.Data;
using Backend.DataBase.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Backend.Controllers.Auth;


[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly DataContext _dataContext;
    private readonly IConfiguration _configuration;
    private readonly IOptions<JwtSettings> _jwtSettings;
    public AuthController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, DataContext dataContext, IConfiguration configuration, IOptions<JwtSettings> jwtSettings)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _dataContext = dataContext;
        _configuration = configuration;
        _jwtSettings = jwtSettings;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = new IdentityUser { UserName = model.Email, Email = model.Email };
        var result = await _userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
            return BadRequest("Invalid registration attempt");

        var newUser = new User()
        {
            AccountId = user.Id,
            FirstName = model.FirstName,
            LastName = model.LastName,
            CreationDateTime = DateTime.UtcNow,
            Role = model.Role
        };

        _dataContext.Users.Add(newUser);
        _dataContext.SaveChanges();
        return Ok();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);

        if (!result.Succeeded)
        {
            return Unauthorized();
        }
        var user = _dataContext.Users.SingleOrDefault(u => u.AccountId == _userManager.FindByEmailAsync(model.Email).Result.Id);
        var userEntity = await _userManager.FindByEmailAsync(model.Email);

        var jwtToken = GenerateAccessToken($"{user.FirstName} {user.LastName}", model.Email);
        var refreshToken = Guid.NewGuid().ToString();

        await _userManager.RemoveAuthenticationTokenAsync(userEntity, "Default", "RefreshToken");
        await _userManager.SetAuthenticationTokenAsync(userEntity, "Default", "RefreshToken", refreshToken);

        return Ok(new
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken),
            RefreshToken = refreshToken
        });
    }

    private JwtSecurityToken GenerateAccessToken(string userName, string email)
    {
        var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.Email, email)
            };

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Value.Issuer,
            audience: _jwtSettings.Value.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.Value.RefreshTokenLifetimeDays),
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Value.SecretKey)),
                SecurityAlgorithms.HmacSha256)
        );
        return token;
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(RefreshTokenDTO request)
    {
        if (request == null)
        {
            return BadRequest("Invalid client request");
        }

        var userEntity = await _userManager.FindByEmailAsync(request.Email);
        var refreshToken = await _userManager.GetAuthenticationTokenAsync(userEntity, "Default", "RefreshToken");

        if (!refreshToken.Equals(request.RefreshToken))
        {
            return BadRequest("Invalid token");
        }

        var user = _dataContext.Users.SingleOrDefault(u => u.AccountId == userEntity.Id);

        var jwtToken = GenerateAccessToken($"{user.FirstName} {user.LastName}", request.Email);
        var newRefreshToken = Guid.NewGuid().ToString();

        await _userManager.RemoveAuthenticationTokenAsync(userEntity, "Default", "RefreshToken");
        await _userManager.SetAuthenticationTokenAsync(userEntity, "Default", "RefreshToken", newRefreshToken);

        return Ok(new
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken),
            RefreshToken = newRefreshToken
        });
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        var jwtToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        var decodedToken = new JwtSecurityTokenHandler().ReadJwtToken(jwtToken);
        var email = decodedToken.Claims.Where(c => c.Type == ClaimTypes.Email).FirstOrDefault().Value;
        var userEntity = await _userManager.FindByEmailAsync(email);

        await _userManager.RemoveAuthenticationTokenAsync(userEntity, "Default", "RefreshToken");
        await _signInManager.SignOutAsync();
        return Ok();
    }
}