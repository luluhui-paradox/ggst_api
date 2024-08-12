using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ggst_api.utils
{
    public class TokenGenUtil
    {
        private readonly IConfiguration _configuration;

        private const string SecretKey = "your-very-long-secret-key-that-is-probably-over-32-bytes"; // 用于签名的密钥
        private const string Issuer = "luluhui"; // 颁发者
        private const string Audience = "your_audience"; // 受众

        public TokenGenUtil(IConfiguration configuration) { 
            _configuration = configuration;
        }

        public string genToken(string username) {
            var claims = new[]
             {
                    new Claim(JwtRegisteredClaimNames.Sub, username),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
             };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your-very-long-secret-key-that-is-probably-over-32-bytes"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // 设置过期时间
            var expires = DateTime.UtcNow.AddHours(1); // 例如，设置为1小时后过期

            var token = new JwtSecurityToken(
                issuer: "luluhui",
                audience: "your-audience",
                claims: claims,
                expires: expires, // 设置过期时间
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public bool ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(SecretKey);
            if (!tokenHandler.CanReadToken(token))
            {
                Console.WriteLine($"------------Token is not in a valid format.------------------");
                return false;
            }
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = Issuer,
                    ValidateAudience = false,
                    ValidAudience = Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero // 不允许时钟偏差
                }, out SecurityToken validatedToken);

                return true; // Token 是有效的
            }
            catch (SecurityTokenExpiredException)
            {
                Console.WriteLine("Token has expired.");
                return false; // Token 已过期
            }
            catch (SecurityTokenInvalidSignatureException)
            {
                Console.WriteLine("Token has an invalid signature.");
                return false; // Token 签名无效
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Token validation failed: {ex.Message}");
                return false; // 其他验证失败
            }
        }
    }
}
