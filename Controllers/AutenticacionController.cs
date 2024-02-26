using MercadoCampesinoBack.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MercadoCampesinoBack.Controllers
{
    [Route("Autenticar")]
    [ApiController]
    public class AutenticacionController : ControllerBase
    {
        private readonly string secretKey;
        private readonly string cadenaSQL;

        public AutenticacionController(IConfiguration config)
        {
            secretKey = config.GetSection("settings").GetSection("secretKey").ToString();
            cadenaSQL = config.GetConnectionString("CadenaSql");
        }

        [HttpPost]
        [Route("Cliente")]
        public IActionResult Validar([FromBody] ClienteValidar request)
        {
            // Verifica si se proporcionaron el correo y la contraseña.
            if (string.IsNullOrEmpty(request.correo) || string.IsNullOrEmpty(request.contrasenia))
            {
                return StatusCode(StatusCodes.Status400BadRequest, "El correo y la contraseña son obligatorios.");
            }

            // Aquí deberías tener tu lógica de acceso a la base de datos para verificar las credenciales del usuario.
            // Establezco la variable esValido en true para representar que las credenciales son válidas.
            bool esValido = false;

            // Realiza la conexión a la base de datos y consulta las credenciales del usuario.
            using (var connection = new SqlConnection(cadenaSQL))
            {
                connection.Open();
                string sql = "SELECT COUNT(*) FROM Cliente WHERE correo = @correo AND contrasenia = @contrasenia";

                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("correo", request.correo);
                command.Parameters.AddWithValue("contrasenia", request.contrasenia);
                int count = Convert.ToInt32(command.ExecuteScalar());
                esValido = count == 1;
            }

            // Si las credenciales son válidas, genera un token JWT.
            if (esValido)
            {
                var keyBytes = Encoding.ASCII.GetBytes(secretKey);
                var claims = new ClaimsIdentity();
                claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, request.correo));
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = claims,
                    Expires = DateTime.UtcNow.AddMinutes(5),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature)
                };
                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenConfig = tokenHandler.CreateToken(tokenDescriptor);
                string tokencreado = tokenHandler.WriteToken(tokenConfig);
                return StatusCode(StatusCodes.Status200OK, new { token = tokencreado });
            }
            else
            {
                return StatusCode(StatusCodes.Status401Unauthorized, new { token = "" });
            }
        }

    }
}