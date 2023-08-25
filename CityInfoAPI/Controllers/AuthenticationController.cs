using Microsoft.AspNetCore.Mvc; // ControllerBase
using Microsoft.IdentityModel.Tokens; // SymmetricSecurityKey
using System.IdentityModel.Tokens.Jwt; // JwtSecurityToken
using System.Security.Claims; // Claim
using System.Text; // Encoding

namespace CityInfo.API.Controllers
{
    [Route("api/authentication")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public class AuthenticationRequestBody
        {
            public string? UserName { get; set; }
            public string? Password { get; set; }
        }

        public class CityInfoUser
        {
            public int UserId { get; set; }
            public string UserName { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string City { get; set; }
            public CityInfoUser(int userId, string userName, string firstName, string lastName, string city)
            {
                UserId = userId;    
                UserName = userName;
                FirstName = firstName;
                LastName = lastName;
                City = city;
            }
        }

        public AuthenticationController(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        [HttpPost("authenticate")]
        public ActionResult<string> Authenticate(AuthenticationRequestBody authenticationRequestBody)
        {
            // Step1: validate the username/password     
            var user = ValidateUserCredentials(
                authenticationRequestBody.UserName,
                authenticationRequestBody.Password);

            if (user is null) return Unauthorized();

            // Step2: create a token
            var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(
                _configuration["Authentication:SecretForKey"] ?? string.Empty));
            var signingCredentials = new SigningCredentials(
                securityKey, SecurityAlgorithms.HmacSha256);

            // The Claims that
            var claimsForToken = new List<Claim>();
            claimsForToken.Add(new Claim("sub", user.UserId.ToString()));
            claimsForToken.Add(new Claim("givan-name", user.FirstName));
            claimsForToken.Add(new Claim("family-name", user.LastName));
            claimsForToken.Add(new Claim("city", user.City));

            var jwtSecurityToken = new JwtSecurityToken(
                    _configuration["Authentication:Issuer"],
                    _configuration["Authentication:Audience"],
                    claimsForToken,
                    DateTime.UtcNow,
                    DateTime.UtcNow.AddHours(1),
                    signingCredentials
                );

            var tokenToReturn = new JwtSecurityTokenHandler()
                .WriteToken(jwtSecurityToken);
            
            return tokenToReturn;
        }

        private CityInfoUser ValidateUserCredentials(string? userName, string? password)
        {
            // for this demo
            return new CityInfoUser(
                    1,
                    userName ?? "",
                    "Talaat",
                    "Ramadan",
                    "Antwerp"
                );
        }
    }
}
