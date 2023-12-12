using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using my_books_api.Data;
using my_books_api.Data.Models;
using my_books_api.Data.ViewModels.Authentication;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace my_books_api.Controllers
{
    public class AuthenticationController : Controller
    {

        private readonly UserManager<ApplicationUser> _userManager; 
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration; 

        public AuthenticationController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, AppDbContext context, IConfiguration configuration)
        {
            _userManager=userManager;
            _roleManager=roleManager;
            _context=context;
            _configuration=configuration;
        }

        [HttpPost("register-user")]
        public async Task<IActionResult> Register([FromBody] RegisterVM  payload)
        {
            var userExists = await _userManager.FindByEmailAsync(payload.UserName);  

            if(userExists != null)
            {
                return BadRequest($"User {payload.Email} already exists");
            }

            ApplicationUser newUser = new ApplicationUser()
            {
                Email = payload.Email,
                UserName = payload.UserName,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var result = await  _userManager.CreateAsync(newUser, payload.Password);

            if(!result.Succeeded)
            {
                return BadRequest("User could not be created");
            }

            return Created(nameof(Register), $"User {payload.Email} already exists"); 
        }

        [HttpPost("login-user")]
        public async Task <IActionResult> Login([FromBody]LoginVm payload)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Please, provide all required fields"); 

            }

            var user = await _userManager.FindByEmailAsync(payload.Email);
            if(user != null && await _userManager.CheckPasswordAsync(user, payload.Password))
            {
                var tokenValue = await GenerateJwtToken(user); 
                return Ok (tokenValue);  

            }  

            return Unauthorized();
        }

        public async Task<AuthResultVm> GenerateJwtToken(ApplicationUser user)
        {
            var authClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var authSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration.GetSection("JWT:Secret").Value!));

            var token = new JwtSecurityToken(
                issuer: _configuration.GetSection("JWT:Issuer").Value, 
                audience: _configuration.GetSection("JWT:Audience").Value,
                expires: DateTime.Now.AddDays(1), // 5 - 10mins
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha384Signature)
                );

            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

            var refreshToken = new RefreshToken()
            {
                JwtId = token.Id,
                IsRevoked = false,
                UserId = user.Id,
                DateAdded = DateTime.UtcNow,
                DateExpired = DateTime.UtcNow.AddMonths(6),
                Token = Guid.NewGuid().ToString() + "-" + Guid.NewGuid().ToString()
            };
            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();

            var response = new AuthResultVm()
            {
                Token = jwtToken,
                RefreshToken = refreshToken.Token,
                ExpirestAt = token.ValidTo
            };

            return response;
        }
       
        
        public IActionResult Index()
        {
            return View();
        }
    }

}
