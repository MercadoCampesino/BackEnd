using MC_BackEnd.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using MercadoCampesinoBack.Models;

namespace MC_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FavoritosController : ControllerBase
    {
        private readonly string cadenaSQL;

        public FavoritosController(IConfiguration config)
        {
            cadenaSQL = config.GetConnectionString("CadenaSql");
        }
        [HttpGet]
        [Route("ListarFavoritos")]
        public IActionResult Lista()
        {
            List<Favoritos> lista = new List<Favoritos>();
            try
            {
                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    conexion.Open();
                    var cmd = new SqlCommand("sp_listarFavoritos", conexion);
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (var rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            lista.Add(new Favoritos
                            {
                                FK_IDProducto1 = Convert.ToInt32(rd["FK_IDProducto1"]),
                                FK_IDCliente1 = Convert.ToInt32(rd["FK_IDCliente1"])
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

        [HttpPost]
        //Esta es la ruta de Guardar categoria 
        [Route("GuardarFavoritos")]
        public IActionResult Guardar([FromBody] Favoritos objeto)
        {
            try
            {
                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    conexion.Open();
                    var cmd = new SqlCommand("sp_agregarFavoritos", conexion);
                    cmd.Parameters.AddWithValue("FK_IDCliente1", objeto.FK_IDCliente1);
                    cmd.Parameters.AddWithValue("FK_IDProducto1", objeto.FK_IDProducto1);
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
        [HttpDelete]
        [Route("EliminarFavoritos/{FK_IDCliente1:int}/{FK_IDProducto1:int}")]
        public IActionResult Eliminar(int FK_IDCliente1, int FK_IDProducto1)
        {
            try
            {
                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    conexion.Open();
                    var cmd = new SqlCommand("sp_eliminarFavoritos", conexion);
                    cmd.Parameters.AddWithValue("FK_IDCliente1", FK_IDCliente1);
                    cmd.Parameters.AddWithValue("FK_IDProducto1", FK_IDProducto1);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }
                //Retornamos Status200OK si la conexion funciona correctamente
                return StatusCode(StatusCodes.Status200OK, new { mensaje = "eliminado" });
            }
            catch (Exception error)
            {
                //retornamos Status500InternalServerError si la conexion no es correcta y mandaamos el mensaje de error 
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = error.Message });
            }
        }
    }
}
