using MercadoCampesinoBack.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace MercadoCampesinoBack.Controllers
{
    [Route("Producto")]
    [ApiController]
    public class ProductoController : ControllerBase
    {
        private readonly string cadenaSQL;
        public ProductoController(IConfiguration config)
        {
            cadenaSQL = config.GetConnectionString("CadenaSQL");

        }

        [HttpGet]
        [Route("ListaProducto")]
        public IActionResult Lista()
        {
            List<Producto> lista = new List<Producto>();
            try
            {
                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    conexion.Open();
                    var cmd = new SqlCommand("sp_listarProductos", conexion);
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (var rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            lista.Add(new Producto
                            {
                                IDProducto = Convert.ToInt32(rd["IdProducto"]),
                                nombre = rd["Nombre"].ToString(),
                                existencia = Convert.ToInt32(rd["existencia"]),
                                imagen = rd["imagen"].ToString(),
                                precio = Convert.ToInt32(rd["precio"]),
                                FK_IDTienda = Convert.ToInt32(rd["FK_IDTienda"]),
                                FK_IDCategoria = Convert.ToInt32(rd["FK_IDCategoria"])
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
        [Route("ObtenerProducto/{idProducto:int}")]
        public IActionResult Obtener(int idProducto)
        {
            List<Producto> lista = new List<Producto>();
            Producto producto = new Producto();
            try
            {
                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    conexion.Open();
                    var cmd = new SqlCommand("sp_lista_productos", conexion);
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (var rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            lista.Add(new Producto
                            {
                                IDProducto = Convert.ToInt32(rd["IdProducto"]),
                                nombre = rd["Nombre"].ToString(),
                                existencia = Convert.ToInt32(rd["existencia"]),
                                precio = Convert.ToInt32(rd["precio"]),
                                imagen = rd["imagen"].ToString(),
                                FK_IDTienda = Convert.ToInt32(rd["FK_IDTienda"]),
                                FK_IDCategoria = Convert.ToInt32(rd["FK_IDCategoria"])
                            });
                        }
                    }
                }
                producto = lista.Where(item => item.IDProducto == idProducto).FirstOrDefault();
                return StatusCode(StatusCodes.Status200OK, new { mensaje = "ok", response = producto });
            }
            catch (Exception error)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = error.Message, response = producto });
            }
        }
        [HttpPost]
        [Route("GuardarProducto")]
        public IActionResult Guardar([FromBody] Producto objeto)
        {
            try
            {
                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    conexion.Open();
                    var cmd = new SqlCommand("sp_agregarProducto", conexion);
                    cmd.Parameters.AddWithValue("IDProducto", objeto.IDProducto);
                    cmd.Parameters.AddWithValue("nombre", objeto.nombre);
                    cmd.Parameters.AddWithValue("existencia", objeto.existencia);
                    cmd.Parameters.AddWithValue("imagen", objeto.imagen);
                    cmd.Parameters.AddWithValue("precio", objeto.precio);
                    cmd.Parameters.AddWithValue("FK_IDTienda", objeto.FK_IDTienda);
                    cmd.Parameters.AddWithValue("FK_IDCategoria", objeto.FK_IDCategoria);
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
        [Route("EditarProducto")]
        public IActionResult Editar([FromBody] Producto objeto)
        {
            try
            {
                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    conexion.Open();
                    var cmd = new SqlCommand("sp_editarProducto", conexion);
                    cmd.Parameters.AddWithValue("IDProducto", objeto.IDProducto == 0 ? DBNull.Value : objeto.IDProducto);
                    cmd.Parameters.AddWithValue("nombre", objeto.nombre is null ? DBNull.Value : objeto.nombre);
                    cmd.Parameters.AddWithValue("existencia", objeto.existencia ==0 ? DBNull.Value : objeto.existencia);
                    cmd.Parameters.AddWithValue("precio", objeto.precio == 0 ? DBNull.Value : objeto.precio);
                    cmd.Parameters.AddWithValue("imagen", objeto.imagen is null ? DBNull.Value : objeto.imagen);
                    cmd.Parameters.AddWithValue("FK_IDTienda", objeto.FK_IDTienda == 0 ? DBNull.Value : objeto.FK_IDTienda);
                    cmd.Parameters.AddWithValue("FK_IDCategoria", objeto.FK_IDCategoria == 0 ? DBNull.Value : objeto.FK_IDCategoria);
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
        [Route("EliminarProducto/{IDProducto:int}")]
        public IActionResult Eliminar(int IDProducto)
        {
            try
            {
                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    conexion.Open();
                    var cmd = new SqlCommand("sp_eliminarProducto", conexion);
                    cmd.Parameters.AddWithValue("IDProducto", IDProducto);
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
