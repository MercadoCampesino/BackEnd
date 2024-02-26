using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using MercadoCampesinoBack.Modelos;

namespace BaackMercadoCampesino.Controllers
{
    [EnableCors("ReglasCors")]          
    [Route("api/[controller]")]
    [ApiController]
    public class TiendaController : ControllerBase
    {
        private readonly string cadenaSQL;
        public TiendaController(IConfiguration config)
        {
            cadenaSQL = config.GetConnectionString("CadenaSQL");

        }
        [HttpGet]
        [Route("Lista")]
        public IActionResult Lista()
        {
            List<Tienda> lista = new List<Tienda>();
            try
            {
                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    conexion.Open();
                    var cmd = new SqlCommand("sp_listarTienda", conexion);
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (var rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            lista.Add(new Tienda
                            {
                                IDTienda = Convert.ToInt32(rd["IDTienda"]),
                                nombre = rd["nombre"].ToString(),
                                imagen = rd["imagen"].ToString(),
                                telefono = rd["telefono"].ToString(),
                                direccion = rd["direccion"].ToString(),
                                correo = rd["correo"].ToString(),
                                contrasenia = rd["contrasenia"].ToString(),
                                FK_IDAdministrador = Convert.ToInt32(rd["FK_IDAdministrador"])
                            });
                        }
                    }
                }
                return StatusCode(StatusCodes.Status200OK, new { mensaje = "ok", response = lista });
            }
            catch (Exception error)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = error.Message, response = lista });
            }
        }
        [HttpGet]
        [Route("ObtenerTienda/{IDCliente:int}")]
        public IActionResult Obtener(int IDTienda)
        {
            List<Tienda> lista = new List<Tienda>();
            Tienda cliente = new Tienda();
            try
            {
                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    conexion.Open();
                    var cmd = new SqlCommand("sp_listarTienda", conexion);
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (var rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            lista.Add(new Tienda
                            {
                                IDTienda = Convert.ToInt32(rd["IDTienda"]),
                                nombre = rd["nombre"].ToString(),
                                imagen = rd["imagen"].ToString(),
                                telefono = rd["telefono"].ToString(),
                                direccion = rd["direccion"].ToString(),
                                correo = rd["correo"].ToString(),
                                contrasenia = rd["contrasenia"].ToString(),
                                FK_IDAdministrador = Convert.ToInt32(rd["FK_IDAdministrador"])
                            });
                        }
                    }
                }
                cliente = lista.Where(item => item.IDTienda == IDTienda).FirstOrDefault();
                return StatusCode(StatusCodes.Status200OK, new { mensaje = "ok", response = cliente });
            }
            catch (Exception error)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = error.Message, response = cliente });
            }
        }
        [HttpPost]
        [Route("GuardarTienda")]
        public IActionResult Guardar([FromBody] Tienda objeto)
        {
            string contraseniaHash = BCrypt.Net.BCrypt.HashPassword(objeto.contrasenia);
            try
            {
                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    conexion.Open();
                    objeto.contrasenia = contraseniaHash;
                    var cmd = new SqlCommand("sp_agregarTienda", conexion);
                    cmd.Parameters.AddWithValue("IDTienda", objeto.IDTienda);
                    cmd.Parameters.AddWithValue("nombre", objeto.nombre);
                    cmd.Parameters.AddWithValue("imagen", objeto.imagen is null ? DBNull.Value : objeto.imagen);
                    cmd.Parameters.AddWithValue("telefono", objeto.telefono);
                    cmd.Parameters.AddWithValue("direccion", objeto.direccion);
                    cmd.Parameters.AddWithValue("correo", objeto.correo);
                    cmd.Parameters.AddWithValue("contrasenia", objeto.contrasenia);
                    cmd.Parameters.AddWithValue("FK_IDAdministrador", objeto.FK_IDAdministrador);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }
                return StatusCode(StatusCodes.Status200OK, new { mensaje = "agregado" });
            }
            catch (Exception error)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = error.Message });
            }
        }
        [HttpPut]
        [Route("EditarTienda")]
        public IActionResult Editar([FromBody] Tienda objeto)
        {
            try
            {
                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    conexion.Open();
                    var cmd = new SqlCommand("sp_editarTienda", conexion);
                    cmd.Parameters.AddWithValue("IDTienda", objeto.IDTienda == 0 ? DBNull.Value : objeto.IDTienda);
                    cmd.Parameters.AddWithValue("nombre", objeto.nombre is null ? DBNull.Value : objeto.nombre);
                    cmd.Parameters.AddWithValue("imagen", objeto.imagen is null ? DBNull.Value : objeto.imagen);
                    cmd.Parameters.AddWithValue("telefono", objeto.telefono is null ? DBNull.Value : objeto.telefono);
                    cmd.Parameters.AddWithValue("direccion", objeto.direccion is null ? DBNull.Value : objeto.direccion);
                    cmd.Parameters.AddWithValue("correo", objeto.correo is null ? DBNull.Value : objeto.correo);
                    cmd.Parameters.AddWithValue("contrasenia", objeto.contrasenia is null ? DBNull.Value : objeto.contrasenia);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }
                return StatusCode(StatusCodes.Status200OK, new { mensaje = "editado" });
            }
            catch (Exception error)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = error.Message });
            }
        }
        [HttpDelete]
        [Route("EliminarTienda/{IDTienda:int}")]
        public IActionResult Eliminar(int IDTienda)
        {
            try
            {
                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    conexion.Open();
                    var cmd = new SqlCommand("sp_eliminarTienda", conexion);
                    cmd.Parameters.AddWithValue("IDTienda", IDTienda);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }
                return StatusCode(StatusCodes.Status200OK, new { mensaje = "eliminado" });
            }
            catch (Exception error)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = error.Message });
            }
        }
    }
}
