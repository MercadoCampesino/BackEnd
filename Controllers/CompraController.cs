using MercadoCampesinoBack.Modelos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using MC_BackEnd.Models;
using MercadoCampesinoBack.Models;

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
                    cmd.Parameters.AddWithValue("FK_IDProducto", objeto.FK_IDProducto);
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
        [HttpGet]
        [Route("ObtenerCompra/{FK_IDCliente:int}/{FK_IDProducto:int}")]
        public IActionResult ObtenerCompra(int FK_IDCliente, int FK_IDProducto)
        {
            try
            {
                Cliente cliente = null;
                Producto producto = null;
                Compra compra = null;

                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    conexion.Open();

                    // Obtener información del cliente
                    var cmdCliente = new SqlCommand("SELECT * FROM Cliente WHERE IDCliente = @IDCliente", conexion);
                    cmdCliente.Parameters.AddWithValue("@IDCliente", FK_IDCliente);
                    using (var readerCliente = cmdCliente.ExecuteReader())
                    {
                        if (readerCliente.Read())
                        {
                            cliente = new Cliente
                            {
                                IDCliente = Convert.ToInt32(readerCliente["IDCliente"]),
                                nombre = Convert.ToString(readerCliente["Nombre"]),
                                apellido = Convert.ToString(readerCliente["apellido"]),
                                telefono = Convert.ToString(readerCliente["telefono"]),
                                correo = Convert.ToString(readerCliente["correo"]),
                                direccion = Convert.ToString(readerCliente["direccion"]),
                                // Añadir más propiedades según la estructura de tu tabla Cliente
                            };
                        }
                    }

                    // Obtener información del producto
                    var cmdProducto = new SqlCommand("SELECT * FROM Producto WHERE IDProducto = @IDProducto", conexion);
                    cmdProducto.Parameters.AddWithValue("@IDProducto", FK_IDProducto);
                    using (var readerProducto = cmdProducto.ExecuteReader())
                    {
                        if (readerProducto.Read())
                        {
                            producto = new Producto
                            {
                                IDProducto = Convert.ToInt32(readerProducto["IDProducto"]),
                                nombre = Convert.ToString(readerProducto["Nombre"]),
                                existencia = Convert.ToInt32(readerProducto["existencia"]),
                                precio = Convert.ToInt32(readerProducto["precio"]),
                                imagen = Convert.ToString(readerProducto["imagen"]),
                                FK_IDCategoria = Convert.ToInt32(readerProducto["FK_IDCategoria"]),
                                FK_IDTienda = Convert.ToInt32(readerProducto["FK_IDTienda"]),
                                // Añadir más propiedades según la estructura de tu tabla producto
                            };
                        }
                    }

                    // Obtener información de la compra
                    var cmdCompra = new SqlCommand("sp_listarCompra", conexion);
                    cmdCompra.CommandType = CommandType.StoredProcedure;
                    using (var rd = cmdCompra.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            var compraActual = new Compra
                            {
                                FK_IDProducto = Convert.ToInt32(rd["FK_IDProducto"]),
                                FK_IDCliente = Convert.ToInt32(rd["FK_IDCliente"])
                            };

                            if (compraActual.FK_IDCliente == FK_IDCliente && compraActual.FK_IDProducto == FK_IDProducto)
                            {
                                compra = compraActual;
                                break;
                            }
                        }
                    }
                }

                if (cliente != null && producto != null && compra != null)
                    return Ok(new { mensaje = "ok", Cliente = cliente, Tienda = producto, Compra = compra });
                else
                    return NotFound();
            }
            catch (Exception error)
            {
                return StatusCode(500, new { mensaje = error.Message });
            }
        }
        [HttpGet]
        [Route("ListarCompra")]
        public IActionResult ListarCompra()
        {
            try
            {
                Cliente cliente = null;
                Producto producto = null;
                List<Compra> compras = new List<Compra>(); // Lista de compras

                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    conexion.Open();

                    // Obtener información del cliente
                    var cmdCliente = new SqlCommand("sp_listarCliente", conexion);
                    using (var readerCliente = cmdCliente.ExecuteReader())
                    {
                        if (readerCliente.Read())
                        {
                            cliente = new Cliente
                            {
                                IDCliente = Convert.ToInt32(readerCliente["IDCliente"]),
                                nombre = Convert.ToString(readerCliente["Nombre"]),
                                apellido = Convert.ToString(readerCliente["apellido"]), // Corregir aquí
                                telefono = Convert.ToString(readerCliente["telefono"]),
                                correo = Convert.ToString(readerCliente["correo"]),
                                direccion = Convert.ToString(readerCliente["direccion"]),
                                // Añadir más propiedades según la estructura de tu tabla Cliente
                            };
                        }
                    }

                    // Obtener información del producto
                    var cmdProducto = new SqlCommand("sp_listarProductos", conexion);
                    using (var readerProducto = cmdProducto.ExecuteReader())
                    {
                        if (readerProducto.Read())
                        {
                            producto = new Producto
                            {
                                IDProducto = Convert.ToInt32(readerProducto["IDProducto"]),
                                nombre = Convert.ToString(readerProducto["Nombre"]),
                                existencia = Convert.ToInt32(readerProducto["existencia"]),
                                precio = Convert.ToInt32(readerProducto["precio"]),
                                imagen = Convert.ToString(readerProducto["imagen"]),
                                FK_IDCategoria = Convert.ToInt32(readerProducto["FK_IDCategoria"]),
                                FK_IDTienda = Convert.ToInt32(readerProducto["FK_IDTienda"]),
                                // Añadir más propiedades según la estructura de tu tabla producto
                            };
                        }
                    }

                    // Obtener información de la compra
                    var cmdCompra = new SqlCommand("sp_listarCompra", conexion);
                    cmdCompra.CommandType = CommandType.StoredProcedure;
                    using (var rd = cmdCompra.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            var compraActual = new Compra
                            {
                                FK_IDProducto = Convert.ToInt32(rd["FK_IDProducto"]),
                                FK_IDCliente = Convert.ToInt32(rd["FK_IDCliente"])
                            };

                            compras.Add(compraActual); // Agregar la compra actual a la lista de compras
                        }
                    }
                }

                // Verificar si se encontraron cliente, producto y compras
                if (cliente != null && producto != null && compras.Count > 0)
                    return Ok(new { mensaje = "ok", Cliente = cliente, Producto = producto, Compras = compras });
                else
                    return NotFound();
            }
            catch (Exception error)
            {
                return StatusCode(500, new { mensaje = error.Message });
            }
        }

        [HttpGet]
        [Route("ListarPorCliente")]
        public IActionResult ListarPorCliente([FromQuery] int IdCliente)
        {
            /*
             CREATE PROCEDURE sp_ListarProductosComprados
            @idCliente INT
            AS
            BEGIN
                SELECT P.existencia, P.imagen, P.nombre, P.precio, P.FK_IDCategoria, P.FK_IDCategoria
                FROM producto P  INNER JOIN compra C ON P.IDProducto = C.FK_IDProducto
                WHERE C.FK_IDCliente = @idCliente
            END;
             */

            string q= "sp_ListarProductosComprados";
            try
            {
                List<Producto> productos = new List<Producto>();
                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    conexion.Open();
                    SqlCommand cmd = new(q, conexion)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.AddWithValue("@idCliente", IdCliente);
                    using (var rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            var producto = new Producto
                            {
                                existencia = Convert.ToInt32(rd["existencia"]),
                                imagen = Convert.ToString(rd["imagen"]),
                                nombre = Convert.ToString(rd["nombre"]),
                                precio = Convert.ToInt32(rd["precio"]),
                                FK_IDCategoria = Convert.ToInt32(rd["FK_IDCategoria"]),
                                FK_IDTienda = Convert.ToInt32(rd["FK_IDTienda"])
                            };
                            productos.Add(producto);
                        }
                    }
                }
                return Ok(new { mensaje = "ok", Productos = productos });
            }
            catch (Exception error)
            {
                return StatusCode(500, new { mensaje = error.Message });
            }

        }
    }
}
