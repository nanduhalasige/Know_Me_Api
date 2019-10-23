using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Know_Me_Api.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Know_Me_Api.Utility;

namespace Know_Me_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly DBContext _context;
        private IConfiguration _config;

        public AuthenticationController(DBContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // POST: api/Authentication
        [Route("PostUserInfo")]
        [HttpPost]
        public async Task<IActionResult> PostUserInfo([FromBody] UserInfo userInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.UserInfo.Add(userInfo);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUserInfo", new { id = userInfo.userId }, userInfo);
        }


        //[AllowAnonymous]
        [Route("LoginUser")]
        [HttpPost]
        public IActionResult LoginUser([FromBody]LoginModel login)
        {
            IActionResult response = Unauthorized();
            var user = Authenticate(login);

            if (user != null)
            {
                var tokenString = BuildToken(user);
                response = Ok(new { token = tokenString });
            }

            return response;
        }

        private UserInfo Authenticate(LoginModel login)
        {
            var userInfo = _context.UserInfo.FirstOrDefault(x => x.userName.Equals(login.userName) && x.password.Equals(login.password));

            if (userInfo == null)
            {
                return new UserInfo();
            }
            return userInfo;
        }

        private string BuildToken(UserInfo user)
        {

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Sub, user.userName),
                new Claim(JwtRegisteredClaimNames.GivenName, user.firstName),
                new Claim(JwtRegisteredClaimNames.FamilyName, user.lastName),
                new Claim(JwtRegisteredClaimNames.Email, user.email)
            };
            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
               _config["Jwt:Issuer"],
               expires: DateTime.Now.AddMinutes(30),
               signingCredentials: creds,
               claims: claims);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class LoginModel
    {
        public string userName { get; set; }
        public string password { get; set; }
    }

    public class UserModel
    {
        public string userName { get; set; }
        public string password { get; set; }
    }
}