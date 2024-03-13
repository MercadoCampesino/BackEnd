using MercadoCampesinoBack.Modelos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using MC_BackEnd.Models;

namespace MC_BackEnd.Controllers
{
    [Route("api/Compra")]
    [ApiController]
    public class CompraController : ControllerBase
    {
        private readonly string cadenaSQL;
        public CompraController(IConfiguration config)
        {
            cadenaSQL = config.GetConnectionString("CadenaSql");
        }
        //Este es el metodo de peticion para traer los datos
        [HttpGet]
        //Esta es la ruta de la lista de las compra ingresadas en el sistema
        [Route("ListaCompra")]
        public IActionResult lista()
        {
            //lista generica de Categoria
            List<Compra> lista = new List<Compra>();
            //Hacemos un try catch para verificar que la conexion a la base de datos es correcta o no
            try
            {
                //Usamos la conexion de la base de datos 
                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    //Abrimos la conexion de la base de datos 
                    conexion.Open();
                    //Creamos una variable por la cual llamamos el procedimiento almacenado de listar compra que esta almacenado en la base de datos 
                    //cada que lo requerimos
                    var cmd = new SqlCommand("sp_listarCompra", conexion);
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (var rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            lista.Add(new Compra
                            {
                                FK_IDTienda = Convert.ToInt32(rd["FK_IDTienda"]),
                                FK_IDCliente = Convert.ToInt32(rd["FK_IDCliente"]),
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
        [Route("ObtenerCompra/{FK_IDCliente:int}/{FK_IDTienda:int}")]
        public IActionResult Obtener(int FK_IDCliente, int FK_IDTienda)
        {
            //Lista generica de categoria que el resultado que desea traer y observar 
            List<Compra> lista = new List<Compra>();
            Compra compra = new Compra();
            //Hacemos un try catch para verificar que la conexion a la base de datos es correcta o no
            try
            {
                //Se crea una variable para usar la conexion de la base de datos cada que le hagamos la petición
                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    //Abrimos la conexion de la base de datos 
                    conexion.Open();
                    //Traemos el procedimiento almacenado corespondiente
                    var cmd = new SqlCommand("sp_listarCompra", conexion);
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (var rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            //creamos una nueva conexion, para cada vez que necesitemos los datos reuqeridos, que lo traiga en una lista
                            lista.Add(new Compra
                            {
                                FK_IDTienda = Convert.ToInt32(rd["FK_IDTienda"]),
                                FK_IDCliente = Convert.ToInt32(rd["FK_IDCliente"]),
                            });
                        }
                    }
                }
                compra = lista.Where(item => item.FK_IDCliente == FK_IDCliente && item.FK_IDTienda == FK_IDTienda).FirstOrDefault();
                return StatusCode(StatusCodes.Status200OK, new { mensaje = "ok", Response = compra });
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
        [Route("GuardarCompra")]
        public IActionResult Guardar([FromBody] Compra objeto)
        {
            try
            {
                //usamos una nueva conexion de la base de datos 
                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    //abrimos la conexion de la base de datos
                    conexion.Open();
                    var cmd = new SqlCommand("sp_agregarCompra", conexion);
                    //con la variable de la conexion llamamos los parametros y agregamos por medio de addWhithValue los datos
                    cmd.Parameters.AddWithValue("FK_IDTienda", objeto.FK_IDTienda);
                    cmd.Parameters.AddWithValue("FK_IDCliente", objeto.FK_IDCliente);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }
                //Retornamos Status200OK si la conexion funciona correctamente
                return StatusCode(StatusCodes.Status200OK, new { mensaje = "agregado" });
            }
            catch (Exception error)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = error.Message });
            }
        }
        [HttpDelete]
        [Route("EliminarCompra/{FK_IDCliente:int}/{FK_IDTienda:int}")]
        public IActionResult Eliminar(int FK_IDTienda, int FK_IDCliente)
        {
            try
            {
                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    conexion.Open();
                    var cmd = new SqlCommand("sp_eliminarCompra", conexion);
                    cmd.Parameters.AddWithValue("FK_IDTienda", FK_IDTienda);
                    cmd.Parameters.AddWithValue("FK_IDCliente", FK_IDCliente);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }
                //Retornamos Status200OK si la conexion funciona correctamente
                return StatusCode(StatusCodes.Status200OK, new { mensaje = "eliminado" });
            }
            catch (Exception error)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = error.Message });
            }
        }
    }
}
