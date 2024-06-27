


using ChatApp.Data.Repository.Users;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ChatApp.Models
{
    public static class FunctionHelper
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        public static string? GenarateToken(IConfiguration configuration, int id)
        {
            var key = Encoding.ASCII.GetBytes(configuration.GetValue<string>("JWTSecret"));
            var tokenHandler = new JwtSecurityTokenHandler();
            var expiresAt = DateTime.Now.AddMinutes(10);
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                //payload
                Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim("userId", id.ToString()),
                        new Claim("role", "User"),
                    }),
                Expires = expiresAt,
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenGenarated = tokenHandler.WriteToken(token);
            return tokenGenarated;
        }
        public static string GenerateRefreshToken()
        {
            StringBuilder result = new StringBuilder(30);
            Random random = new Random();
            for (int i = 0; i < 30; i++)
            {
                result.Append(chars[random.Next(chars.Length)]);
            }
            return result.ToString();
        }
        public static async Task<string> GetNameRoomByTwoIds(int id1, int id2,IUserRepository _userRepository)
        {
            var fromUser = await _userRepository.GetItemByQuery(x => x.Id == id1);
            var toUser = await _userRepository.GetItemByQuery(x => x.Id == id2);
            var stringCompare = string.CompareOrdinal(fromUser?.Name, toUser?.Name) < 0;
            return stringCompare ? $"{fromUser?.Name} - {toUser?.Name}" : $"{toUser?.Name} - {fromUser?.Name}";
        }
    }

}
