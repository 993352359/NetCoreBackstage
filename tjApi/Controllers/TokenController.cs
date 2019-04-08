using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace tjApi.Controllers
{
    /// <summary>
    /// 测试
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private IConfiguration _config;

        public TokenController(IConfiguration config)
        {
            this._config = config;
        }

        [AllowAnonymous]
        [HttpPost]
        //public IActionResult CreateToken([FromBody]LoginModel login)
        public JsonResult CreateToken(LoginModel login)
        {
            var tokenString = "";
            var user = Authenticate(login);
            if (user != null)
            {
                tokenString = BuildToken(user);
            }
            var reseult = new JsonResult(new { token = tokenString });
            return reseult;
        }

        [AllowAnonymous]
        [HttpGet]
        private UserModel Authenticate(LoginModel login)
        {
            UserModel user = null;
            if (login.Username == "mario" && login.Password == "secret")
            {
                user = new UserModel { Name = "Mario Rossi", Email = "mario.rossi@domain.com" };
            }
            return user;
        }

        [AllowAnonymous]
        [HttpGet]
        public string BuildToken(UserModel user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Name),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Birthdate, user.Birthdate.ToString("yyyy-MM-dd")),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(_config["Jwt:Issuer"], _config["Jwt:Issuer"], claims:claims,expires: DateTime.Now.AddMinutes(30), signingCredentials: creds);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public class LoginModel
        {
            public string Username { get; set; }

            public string Password { get; set; }
        }

        public class UserModel
        {
            public string Name { get; set; }

            public string Email { get; set; }

            public DateTime Birthdate { get; set; }
        }

    }
}
