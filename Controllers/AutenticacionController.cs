using MC_BackEnd.helpers;
using MercadoCampesinoBack.Models;
using Microsoft.AspNetCore.Mvc;
using MC_BackEnd.Models;
using System.Data;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MercadoCampesinoBack.Modelos;

namespace MercadoCampesinoBack.Controllers
{
    [Route("Autenticar")]
    [ApiController]
    public class AutenticacionController(IConfiguration config) : ControllerBase
    {
        private readonly string secretKey = config.GetSection("settings").GetSection("secretKey").ToString()!;
        private readonly string cadenaSQL = config.GetConnectionString("CadenaSql")!;

        [HttpPost]
        [Route("Cliente")]
        public IActionResult Validar([FromBody] ClienteValidar request)
        {
            string q = $"SELECT * FROM CLIENTE WHERE correo = '{request.correo}' and contrasenia = '{request.contrasenia}'";
            DataTable dt = Methods.GetTableFromQuery(q, new SqlConnection(cadenaSQL));
            if (dt.Rows.Count == 0)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, "El correo o la contraseña son incorrectos.");
            }
                Cliente cliente = new()
                {
                    correo = dt.Rows[0]["correo"].ToString() ?? "unknown data",
                    nombre = dt.Rows[0]["nombre"].ToString() ?? "unknown data",
                    apellido = dt.Rows[0]["apellido"].ToString() ?? "unknown data",
                    telefono = dt.Rows[0]["telefono"].ToString() ?? "unknown data",
                    direccion = dt.Rows[0]["direccion"].ToString() ?? "unknown data",
                    contrasenia = dt.Rows[0]["contrasenia"].ToString() ?? "unknown data",
                    fechaNacimiento = dt.Rows[0]["fechaDeNacimiento"].ToString() ?? "unknown data"
                };
            

            // Si las credenciales so
                var keyBytes = Encoding.ASCII.GetBytes(secretKey);
                List<Claim> claims = [];
                claims.Add(new("correo", cliente.correo));
                claims.Add(new("nombre", cliente.nombre));
                claims.Add(new("apellido", cliente.apellido));
                claims.Add(new("telefono", cliente.telefono));
                claims.Add(new("direccion", cliente.direccion));
                claims.Add(new("contrasenia", cliente.contrasenia));
                claims.Add(new("fechaDeNacimiento", cliente.fechaNacimiento));
                string token = Methods.GenerateToken(claims, secretKey);
                return StatusCode(StatusCodes.Status200OK, new { token });

        }

        [HttpPost]
        [Route("Tienda")]
        public IActionResult Validar([FromBody] TiendaValidar request)
        {
            string q = $"SELECT * FROM TIENDA WHERE correo = '{request.correo}' and contrasenia = '{request.contrasenia}'";
            DataTable dt = Methods.GetTableFromQuery(q, new SqlConnection(cadenaSQL));
            if (dt.Rows.Count == 0)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, "El correo o la contraseña son incorrectos.");
            }
            Tienda tienda = new()
                {
                    IDTienda = Convert.ToInt32(dt.Rows[0]["idTienda"]),
                    imagen = dt.Rows[0]["imagen"].ToString() ?? "defaultImage",
                    correo = dt.Rows[0]["correo"].ToString() ?? "unknown data",
                    nombre = dt.Rows[0]["nombre"].ToString() ?? "unknown data",
                    telefono = dt.Rows[0]["telefono"].ToString() ?? "unknown data",
                    direccion = dt.Rows[0]["direccion"].ToString() ?? "unknown data",
                    contrasenia = dt.Rows[0]["contrasenia"].ToString() ?? "unknown data",
                    FK_IDAdministrador = Convert.ToInt32(dt.Rows[0]["fK_IDAdministrador"])
                    
                };

            // Si las credenciales so
            var keyBytes = Encoding.ASCII.GetBytes(secretKey);
                List<Claim> claims = [];
                claims.Add(new("correo", tienda.correo));
                claims.Add(new("idTienda", tienda.IDTienda));
                claims.Add(new("nombre", tienda.nombre));
                claims.Add(new("telefono", tienda.telefono));
                claims.Add(new("direccion", tienda.direccion));
                claims.Add(new("contrasenia", tienda.contrasenia));
                string token = Methods.GenerateToken(claims, secretKey);
                return StatusCode(StatusCodes.Status200OK, new { token });

        }


    }
}