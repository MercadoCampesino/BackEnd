using MC_BackEnd.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using MercadoCampesinoBack.Models;
using Microsoft.Extensions.Primitives;
using MC_BackEnd.helpers;

namespace MC_BackEnd.Controllers
{
    [Route("Favoritos")]
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
            using (var conexion = new SqlConnection(cadenaSQL))
            {
                try
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

                    return StatusCode(StatusCodes.Status200OK, new { mensaje = "ok", response = lista });
                }
                catch (Exception error)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = error.Message, response = lista });
                }
                finally
                {
                    conexion.Close();
                }
            }
        }

        [HttpGet]
        [Route("ListarFavoritosPorPersona")]
        public IActionResult ListaPorPersona([FromQuery] int FK_IDCliente1)
        {
            string query = $"EXECUTE ObtenerProductosFavoritos {FK_IDCliente1}";
            /*
               SELECT p.Nombre, p.Precio, p.Imagen
  FROM Favoritos f
  INNER JOIN Producto p ON f.FK_IDProducto1 = p.IDProducto
  WHERE f.FK_IDCliente1 = ClienteID;
             */
            try
            {

                DataTable dt = Methods.GetTableFromQuery(query, new SqlConnection(cadenaSQL));
                List<Producto> favoritos = [];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    favoritos.Add(new Producto
                    {
                        IDProducto = Convert.ToInt32(dt.Rows[i]["IDProducto"]),
                        nombre = dt.Rows[i]["nombre"].ToString() ?? "unknown",
                        existencia = Convert.ToInt32(dt.Rows[i]["existencia"]),
                        precio = Convert.ToInt32(dt.Rows[i]["precio"]),
                        imagen = dt.Rows[i]["imagen"].ToString() ?? "unknown",
                        FK_IDTienda = Convert.ToInt32(dt.Rows[i]["FK_IDTienda"]),
                        FK_IDCategoria = Convert.ToInt32(dt.Rows[i]["FK_IDCategoria"])
                    });
                }
                
                return StatusCode(StatusCodes.Status200OK, new { mensaje = "ok", response = favoritos });
            } catch (Exception error)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = error.Message });
            }
        }

        [HttpPost]
        //Esta es la ruta de Guardar categoria 
        [Route("GuardarFavoritos")]
        public IActionResult Guardar([FromBody] Favoritos objeto)
        {
            using (var conexion = new SqlConnection(cadenaSQL))
            {
                try
                {
                    conexion.Open();
                    var cmd = new SqlCommand("sp_agregarFavoritos", conexion);
                    cmd.Parameters.AddWithValue("FK_IDCliente1", objeto.FK_IDCliente1);
                    cmd.Parameters.AddWithValue("FK_IDProducto1", objeto.FK_IDProducto1);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();

                    return StatusCode(StatusCodes.Status200OK, new { mensaje = "agregado" });
                }
                catch (Exception error)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = error.Message });
                }
                finally
                {
                    conexion.Close();
                }
            }
        }
        [HttpDelete]
        [Route("EliminarFavoritos/{FK_IDCliente1:int}/{FK_IDProducto1:int}")]
        public IActionResult Eliminar(int FK_IDCliente1, int FK_IDProducto1)
        {
            using (var conexion = new SqlConnection(cadenaSQL))
            {
                try
                {

                    conexion.Open();
                    var cmd = new SqlCommand("sp_eliminarFavoritos", conexion);
                    cmd.Parameters.AddWithValue("FK_IDCliente1", FK_IDCliente1);
                    cmd.Parameters.AddWithValue("FK_IDProducto1", FK_IDProducto1);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();

                    //Retornamos Status200OK si la conexion funciona correctamente
                    return StatusCode(StatusCodes.Status200OK, new { mensaje = "eliminado" });
                }
                catch (Exception error)
                {
                    //retornamos Status500InternalServerError si la conexion no es correcta y mandaamos el mensaje de error 
                    return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = error.Message });
                }
                finally
                {
                    conexion.Close();
                }
            }
        }
    }
}
