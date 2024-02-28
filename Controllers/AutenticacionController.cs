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
            Cliente cliente;
            // Realiza la conexión a la base de datos y consulta las credenciales del usuario.
            using (var connection = new SqlConnection(cadenaSQL))
            {
                connection.Open();
                string sql = "SELECT * FROM Cliente WHERE correo = @correo AND contrasenia = @contrasenia";

                SqlCommand command = new(sql, connection);
                cliente = Get(request);
            }

            // Si las credenciales son válidas, genera un token JWT.
            if (esValido)
            {
                var keyBytes = Encoding.ASCII.GetBytes(secretKey);
                var claims = new ClaimsIdentity();
                claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, request.correo));
                claims.AddClaim(new Claim("nombre", cliente.nombre));
                claims.AddClaim(new Claim("apellido", cliente.apellido));
                claims.AddClaim(new Claim("telefono", cliente.telefono));
                claims.AddClaim(new Claim("direccion", cliente.direccion));
                claims.AddClaim(new Claim("contrasenia", cliente.contrasenia));
                claims.AddClaim(new Claim("fechaDeNacimiento", cliente.fechaNacimiento));
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

        public Cliente Get(ClienteValidar request)
        {

            string query = "select nombre, apellido, telefono, correo, direccion, contrasenia, fechaDeNacimiento from cliente WHERE correo = @correo AND contrasenia = @contrasenia ";
            using (SqlConnection connection = new SqlConnection(cadenaSQL))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();

                command.Parameters.AddWithValue("correo", request.correo);
                command.Parameters.AddWithValue("contrasenia", request.contrasenia);
                SqlDataReader reader = command.ExecuteReader();

                reader.Read();
                string Nombre = reader.GetString(0);
                string Apellido = reader.GetString(1);
                string Telefono = reader.GetString(2);
                string Correo = reader.GetString(3);
                string Direccion = reader.GetString(4);
                string Contrasenia = reader.GetString(5);
                string FechaDeNacimiento = reader.GetString(6);

                Cliente cliente = new Cliente
                {
                    nombre = Nombre,
                    apellido = Apellido,
                    telefono = Telefono,
                    correo = Correo,
                    direccion = Direccion,
                    contrasenia = Contrasenia,
                    fechaNacimiento = FechaDeNacimiento
                }
                    ;


                connection.Close();
                return cliente;
            }

        }
    }
}