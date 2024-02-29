using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MC_BackEnd.helpers
{
    internal class Methods()
    {
        //dependency injections to appsettings.json


        public static string GenerateToken(List<Claim> claims, string secretKey)

        {
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey)), SecurityAlgorithms.HmacSha256Signature)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }


        public static DataTable GetTableFromQuery   (string query, SqlConnection _conn)
        {
            SqlCommand command = new (query, _conn);
            SqlDataAdapter da = new (command);
            DataTable dt = new ();
            da.Fill(dt);
            return dt;
        }


    }
}
