using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace tjApi
{
    /// <summary>
    /// token提供类
    /// </summary>
    public class TokenProvider
    {
        readonly TokenProviderOptions _options;

        public TokenProvider(TokenProviderOptions options)
        {
            this._options = options;
        }

        /// <summary>
        /// 生成令牌
        /// </summary>
        /// <param name="context">http上下文</param>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="role">角色</param>
        /// <returns></returns>
        public async Task<TokenEntity> GenerateToken(HttpContext context,string username,string password,string role)
        {
            var identity = await Getidentity(username);
            if (identity == null)
            {
                return null;
            }
            var now = DateTime.UtcNow;
            //声明
            var claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub,username),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat,ToUnixEpchDate(now).ToString(),ClaimValueTypes.Integer64),
                new Claim(ClaimTypes.Role,role),
                new Claim(ClaimTypes.Name,username)
            };
            //jwt安全令牌
            var jwt = new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                claims: claims,
                notBefore: now,
                expires: now.Add(_options.Expiration),
                signingCredentials:_options.SigningCredentials
            );
            //生成令牌字符串
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            var response = new TokenEntity
            {
                access_token = encodedJwt,
                expires_in = (int)_options.Expiration.TotalSeconds
            };
            return response;
        }

        /// <summary>
        /// 查看令牌是否存在
        /// </summary>
        /// <param name="username">用户名</param>
        /// <returns></returns>
        public Task<ClaimsIdentity> Getidentity(string username)
        {
            return Task.FromResult(
                new ClaimsIdentity(new System.Security.Principal.GenericIdentity(username,"token"),
                new Claim[]
                {
                    new Claim(ClaimTypes.Name,username)
                }));
        }

        private static long ToUnixEpchDate(DateTime date)
        {
            return (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970,1,1,0,0,0,TimeSpan.Zero)).TotalSeconds);
        }

    }
}
