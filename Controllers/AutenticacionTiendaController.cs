using MC_BackEnd.Models;
using MercadoCampesinoBack.Modelos;
using MercadoCampesinoBack.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MC_BackEnd.Controllers
{
    [Route("AuthTienda")]
    [ApiController]
    public class AutenticacionTiendaController : ControllerBase
    {
        private readonly string secretKey;
        private readonly string cadenaSQL;

        public AutenticacionTiendaController(IConfiguration config)
        {
            secretKey = config.GetSection("settings").GetSection("secretKey").ToString();
            cadenaSQL = config.GetConnectionString("CadenaSql");
        }

        [HttpPost]
        [Route("Cliente")]
        public IActionResult Validar([FromBody] Tienda request)
        {
            // Verifica si se proporcionaron el correo y la contraseña.
            if (string.IsNullOrEmpty(request.correo) || string.IsNullOrEmpty(request.contrasenia))
            {
                return StatusCode(StatusCodes.Status400BadRequest, "El correo y la contraseña son obligatorios.");
            }

            // Aquí deberías tener tu lógica de acceso a la base de datos para verificar las credenciales del usuario.
            // Establezco la variable esValido en true para representar que las credenciales son válidas.
            bool esValido = false;
            Tienda tienda;

            // Realiza la conexión a la base de datos y consulta las credenciales del usuario.
            using (var connection = new SqlConnection(cadenaSQL))
            {
                connection.Open();
                string sql = "SELECT * FROM Tienda WHERE correo = @correo AND contrasenia = @contrasenia";

                SqlCommand command = new SqlCommand(sql, connection);
                tienda = Get(request);
            }

            // Si las credenciales son válidas, genera un token JWT.
            if (esValido)
            {
                var keyBytes = Encoding.ASCII.GetBytes(secretKey);
                var claims = new ClaimsIdentity();
                claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, request.correo));
                claims.AddClaim(new Claim("nombre", tienda.nombre));
                claims.AddClaim(new Claim("telefono", tienda.telefono));
                claims.AddClaim(new Claim("direccion", tienda.direccion));
                claims.AddClaim(new Claim("contrasenia", tienda.contrasenia));
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


        public Tienda Get(Tienda request)
        {

            string query = "select nombre, imagen, telefono, direccion, correo, contrasenia nfrom cliente WHERE correo = @correo AND contrasenia = @contrasenia ";
            using (SqlConnection connection = new SqlConnection(cadenaSQL))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();

                command.Parameters.AddWithValue("correo", request.correo);
                command.Parameters.AddWithValue("contrasenia", request.contrasenia);
                SqlDataReader reader = command.ExecuteReader();

                reader.Read();

                string Nombre = reader.GetString(0);
                string Imagen = reader.GetString(1);
                string Telefono = reader.GetString(2);
                string Direccion = reader.GetString(3);
                string Correo = reader.GetString(4);
                string Contrasenia = reader.GetString(5);

                Tienda tienda = new Tienda
                {
                    nombre = Nombre,
                    imagen = Imagen,
                    telefono = Telefono,
                    direccion = Direccion,
                    correo = Correo,
                    contrasenia = Contrasenia,
                }
                    ;


                connection.Close();
                return tienda;
            }

        }


        }

}
