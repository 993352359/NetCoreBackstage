using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace tjApi.Controllers
{
    /// <summary>
    /// 测试
    /// </summary>
    [Authorize]
    public class TokenController : ControllerBase
    {
        private IConfiguration _config;

        public TokenController(IConfiguration config)
        {
            this._config = config;
        }

        public IActionResult CreateToken([FromBody]LoginModel login)
        {
            IActionResult response = Unauthorized();
        }

        public string BuildToken(UserModel user)
        {
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("ThisIsASecretKeyForAspNetCoreAPIToken"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken("audience", "audience", expires: DateTime.Now.AddMinutes(30), signingCredentials: creds);
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
