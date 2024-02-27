using MercadoCampesinoBack.Modelos; 
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Cors;

namespace BaackMercadoCampesino.Controllers
{
    [EnableCors("ReglasCors")]
    [Route("Categoria")]
    [ApiController]
    public class CategoriaController : ControllerBase
    {
        private readonly string cadenaSQL;
        public CategoriaController(IConfiguration config)
        {
            cadenaSQL = config.GetConnectionString("CadenaSql");
        }
        //Este es el metodo de peticion para traer los datos
        [HttpGet]
        //Esta es la ruta de la lista de las categorias ingresadas en el sistema
        [Route("ListaCategoria")]
        public IActionResult lista()
        {            
            //lista generica de Categoria
            List<Categoria> lista = new List<Categoria>();
            //Hacemos un try catch para verificar que la conexion a la base de datos es correcta o no
            try
            {
                //Usamos la conexion de la base de datos 
                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    //Abrimos la conexion de la base de datos 
                    conexion.Open();
                    //Creamos una variable por la cual llamamos el procedimiento almacenado de listar categoria que esta almacenado en la base de datos 
                    //cada que lo requeramos
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
                    //Retornamos Status200OK si la conexion funciona correctamente
                    return (StatusCode(StatusCodes.Status200OK, new { mensaje = "ok", response = lista }));
                }
            }
            catch (Exception error)
            {
                //Retornamos Status500InternalServerError si la conexion no funciona correctamente
                return (StatusCode(StatusCodes.Status400BadRequest, new { mensaje = error.Message }));
            }
        }
        //Este es el metodo de peticion para traer los datos
        [HttpGet]
        //Esta es la ruta de obtenr la categoria que desea buscar
        [Route("ObtenerCategoria/{IDCategoria:int}")]
        public IActionResult Obtener (int IDCategoria)
        {
            //Lista generica de categoria que el resultado que desea traer y onbservar 
            List<Categoria> lista = new List<Categoria>();
            Categoria categoria = new Categoria();
            //Hacemos un try catch para verificar que la conexion a la base de datos es correcta o no
            try
            {
                //Se crea una variable para usar la conexion de la base de datos cada que le hagamos la petición
                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    //Abrimos la conexion de la base de datos 
                    conexion.Open();
                    //Traemos el procedimiento almacenado corespondiente
                    var cmd = new SqlCommand("sp_listarCategoria", conexion);
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (var rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            //creamos una nueva conexion, para cada vez que necesitemos los datos reuqeridos, que lo traiga en una lista
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
                //retornamos Status500InternalServerError si la conexion no es correcta y mandaamos el mensaje de error 
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensage = error.Message });
            }
        }
        //Este es el metodo de peticion para ingresar datos 
        [HttpPost]
        //Esta es la ruta de Guardar categoria 
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
