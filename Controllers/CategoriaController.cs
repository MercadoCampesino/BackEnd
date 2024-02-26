using MercadoCampesinoBack.Modelos; 
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Cors;

namespace BaackMercadoCampesino.Controllers
{
    [EnableCors("ReglasCors")]
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriaController : ControllerBase
    {
        private readonly string cadenaSQL;
        public CategoriaController(IConfiguration config)
        {
            cadenaSQL = config.GetConnectionString("CadenaSql");
        }
        [HttpGet]
        [Route("ListaCategoria")]
        public IActionResult lista()
        {
            List<Categoria> lista = new List<Categoria>();
            try
            {
                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    conexion.Open();
                    var cmd = new SqlCommand("sp_listarCategoria", conexion);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    using (var rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            lista.Add(new Categoria
                            {
                                IDCategoria = Convert.ToInt32(rd["IDCategoria"]),
                                tipo = rd["tipo"].ToString()
                            });
                        }
                    }

                    return (StatusCode(StatusCodes.Status200OK, new { mensaje = "ok", response = lista }));
                }
            }
            catch (Exception error)
            {
                return (StatusCode(StatusCodes.Status400BadRequest, new { mensaje = error.Message }));
            }
        }
        [HttpGet]
        [Route("ObtenerCategoria/{IDCategoria:int}")]
        public IActionResult Obtener (int IDCategoria)
        {
            List<Categoria> lista = new List<Categoria>();
            Categoria categoria = new Categoria();
            try
            {
                using(var conexion = new SqlConnection(cadenaSQL))
                {
                    conexion.Open();
                    var cmd = new SqlCommand("sp_listarCategoria", conexion);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    using (var rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            lista.Add(new Categoria
                            {
                                IDCategoria = Convert.ToInt32(rd["IDCategoria"]),
                                tipo= rd["tipo"].ToString()
                            });
                        }
                    }
                }
                categoria = lista.Where(item => item.IDCategoria == IDCategoria).FirstOrDefault();
                return StatusCode(StatusCodes.Status200OK, new { mensaje = "ok", Response = categoria });
            } 
            catch (Exception error)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensage = error.Message });
            }
        }
        [HttpPost]
        [Route("GuardarCategoria")]
        public IActionResult Guardar([FromBody] Categoria objeto)
        {
            try
            {
                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    conexion.Open();
                    var cmd = new SqlCommand("sp_agregarCategoria", conexion);
                    cmd.Parameters.AddWithValue("IDCategoria", objeto.IDCategoria);
                    cmd.Parameters.AddWithValue("tipo", objeto.tipo);
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
        [Route("EditarCategoria")]
        public IActionResult Editar([FromBody] Categoria objeto)
        {
            try
            {
                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    conexion.Open();
                    var cmd = new SqlCommand("sp_editarCategoria", conexion);
                    cmd.Parameters.AddWithValue("IDCategoria", objeto.IDCategoria == 0 ? DBNull.Value : objeto.IDCategoria);
                    cmd.Parameters.AddWithValue("tipo", objeto.tipo is null ? DBNull.Value : objeto.tipo);
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
        [Route("EliminarCategoria/{IDCategoria:int}")]
        public IActionResult Eliminar(int IDCategoria)
        {
            try
            {
                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    conexion.Open();
                    var cmd = new SqlCommand("sp_eliminarCategoria", conexion);
                    cmd.Parameters.AddWithValue("IDCategoria", IDCategoria);
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
