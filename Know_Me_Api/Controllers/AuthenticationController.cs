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
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authorization;

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
        [AllowAnonymous]
        [Route("PostUserInfo")]
        [HttpPost]
        public async Task<IActionResult> PostUserInfo([FromBody] UserInfo userInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            userInfo.roleId = 3;
            userInfo.isActive = true;
            _context.UserInfo.Add(userInfo);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUserInfo", new { id = userInfo.userId }, userInfo);
        }


        [AllowAnonymous]
        [Route("LoginUser")]
        [HttpPost]
        public IActionResult LoginUser([FromBody]LoginModel login)
        {
            IActionResult response = Unauthorized();
            var user = Authenticate(login);

            if (user.userFound)
            {
                var tokenString = BuildToken(user.userInfo);
                response = Ok(new { token = tokenString });
            }
            else
            {
                response = NotFound(new { message = "User not found" });

            }

            return response;
        }


        #region External Logins
        [AllowAnonymous]
        [Route("GoogleLogin")]
        [HttpPost]
        //[EnableCors("MyCors")]
        public async Task<IActionResult> GoogleLogin([FromBody]string tokenId)
        {
            try
            {
                var payload = GoogleJsonWebSignature.ValidateAsync(tokenId,
                    new GoogleJsonWebSignature.ValidationSettings()
                    {
                        ExpirationTimeClockTolerance = TimeSpan.FromSeconds(20),
                        Clock = new AccurateClock(),
                        IssuedAtClockTolerance = TimeSpan.FromSeconds(20)
                    }).Result;
                var user = await Authenticate(payload);

                var tokenString = BuildToken(user);

                return Ok(new { token = tokenString });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                return NotFound(new { message = "Somethong wen wrong" });
            }
        }


        public async Task<UserInfo> Authenticate(Google.Apis.Auth.GoogleJsonWebSignature.Payload payload)
        {
            await Task.Delay(1);
            return this.FindUserOrAdd(payload);
        }

        private UserInfo FindUserOrAdd(Google.Apis.Auth.GoogleJsonWebSignature.Payload payload)
        {
            var u = _context.UserInfo.Where(x => x.email == payload.Email).FirstOrDefault();
            if (u == null)
            {
                u = new UserInfo()
                {
                    userId = Guid.NewGuid(),
                    lastName = payload.FamilyName,
                    firstName = payload.GivenName,
                    email = payload.Email,
                    IsExternal = true,
                    password = null,
                    isActive = true,
                    userName = payload.Name
                };
                _context.UserInfo.Add(u);
            }
            return u;
        }
        #endregion

        private AuthUser Authenticate(LoginModel login)
        {
            var user = _context.UserInfo.FirstOrDefault(x => x.userName.Equals(login.userName) && x.password.Equals(login.password));

            if (user == null)
            {
                return new AuthUser() { userInfo = new UserInfo(), userFound = false };
            }
            return new AuthUser() { userInfo = user, userFound = true };
        }

        public static DateTime FromUnixTime(long ticks)
        {
            DateTime dateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
            return dateTime.AddSeconds(ticks);
        }

        #region Generate Token

        private string BuildToken(UserInfo user)
        {

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[] {
                //new Claim(JwtRegisteredClaimNames.Sub, user.userName),
                new Claim(JwtRegisteredClaimNames.GivenName, user.firstName),
                new Claim(JwtRegisteredClaimNames.FamilyName, user.lastName),
                new Claim(JwtRegisteredClaimNames.Email, user.email),
                new Claim("Role", user.roleId.ToString())
            };
           

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
               _config["Jwt:Issuer"],
               expires: DateTime.Now.AddMonths(1),
               signingCredentials: creds,
               claims: claims
               );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        #endregion
    }


    #region Model class
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

    public class AuthUser
    {
        public UserInfo userInfo { get; set; }
        public bool userFound { get; set; }
    }

    #endregion
}