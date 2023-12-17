using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
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

        //Refrfesh tojes 

        private readonly TokenValidationParameters _tokenValidationParameters;

        public AuthenticationController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, AppDbContext context, IConfiguration configuration, TokenValidationParameters tokenValidationParameters)
        {
            _userManager=userManager;
            _roleManager=roleManager;
            _context=context;
            _configuration=configuration;
            _tokenValidationParameters=tokenValidationParameters;
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
                var tokenValue = await GenerateJwtTokenAsync(user," "); 
                return Ok (tokenValue);  

            }  

            return Unauthorized();
        }
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRequestVM playload)
        {
            try
            {
                var reslt = await VerifyAndGenerateTokenAsync(playload);
                if (reslt == null) return BadRequest("Invalis tokens"); 

                return Ok(reslt);   
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private async Task<AuthResultVm> VerifyAndGenerateTokenAsync(TokenRequestVM payload)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            try
            {

                //cheack 1 : cheack JWT token format 
                var tokenInVerification = jwtTokenHandler.ValidateToken(payload.Token, _tokenValidationParameters, out var validatedToken);

                //cheack 2 : Encryption algorithm 

                if (validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);
                    if (result == false) return null;
                }

                //cheack 3 - validate expiry date 
                var utcExpiryDate = long.Parse(tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

                var expiryDate = UnixTimeStampDateTimeinUTC(utcExpiryDate);

                if (expiryDate > DateTime.UtcNow) throw new Exception("Token has not expired yet ");

                //Cheack 4 Refresh token exists in the DB 

                var dbRefreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(n => n.Token == payload.RefreshToken);

                if (dbRefreshToken == null) throw new Exception($"Refresh token does not exists in our DB");
                else
                {
                    //cheack 5 - validate Id 
                    var jti = tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

                    if (dbRefreshToken.JwtId != jti) throw new Exception("Token doesn't match  "); // не соответствует

                    // cheack 6 Refresh token expiration 
                    if (dbRefreshToken.DateExpired <= DateTime.UtcNow) throw new Exception("Your refresh token has expired , please re-athenticate ");

                    //cheack 7 refresh token is revoked  отозван

                    if (dbRefreshToken.IsRevoked) throw new Exception("Refresh token is revoked ");

                    // Generate new token (with existing refresh token ) 

                    var dbUserData = await _userManager.FindByIdAsync(dbRefreshToken.UserId);

                    var newTokenResponse = GenerateJwtTokenAsync(dbUserData, payload.RefreshToken);

                    return await newTokenResponse;
                }
            }
            catch (SecurityTokenExpiredException)
            {
                // Generate new token (with existing refresh token ) 
                var dbRefreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(n => n.Token == payload.RefreshToken);

                var dbUserData = await _userManager.FindByIdAsync(dbRefreshToken.UserId);

                var newTokenResponse = GenerateJwtTokenAsync(dbUserData, payload.RefreshToken);

                return await newTokenResponse;
            }
            catch (Exception ex) 
            {
                throw new Exception(ex.Message);
            }

        }
        public async Task<AuthResultVm> GenerateJwtTokenAsync(ApplicationUser user, string existinfRefreshToken)
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
            var refreshToken = new RefreshToken();

            if (string.IsNullOrEmpty(existinfRefreshToken)) 
            {
                 refreshToken = new RefreshToken()
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
            }
           

            var response = new AuthResultVm()
            {
                Token = jwtToken,
                RefreshToken = (string.IsNullOrEmpty(existinfRefreshToken))? refreshToken.Token : existinfRefreshToken ,
                ExpirestAt = token.ValidTo
            };

            return response;
        }
       
        private DateTime UnixTimeStampDateTimeinUTC(long unixTimeStamp)
        {
            var dateTimeVal = new DateTime(1970,1,1,0,0,0,0, DateTimeKind.Utc);
            dateTimeVal = dateTimeVal.AddSeconds(unixTimeStamp);
            return dateTimeVal;
        }
        
        public IActionResult Index()
        {
            return View();
        }
    }

}
