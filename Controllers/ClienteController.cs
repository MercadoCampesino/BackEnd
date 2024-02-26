using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using Microsoft.AspNetCore.Cors;
using MercadoCampesinoBack.Models;
using System;
using NodaTime;


namespace BaackMercadoCampesino.Controllers
{
    [Route("Cliente")]
    [ApiController]
    public class ClienteController : ControllerBase
    {
        private readonly string cadenaSQL;
        public ClienteController(IConfiguration config)
        {
            cadenaSQL = config.GetConnectionString("CadenaSQL");
        }
        [HttpGet]
        [Route("ListaCliente")]
        public IActionResult Lista()
        {
            List<Cliente> lista = new List<Cliente>();
            try
            {
                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    conexion.Open();
                    var cmd = new SqlCommand("sp_listarCliente", conexion);
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (var rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            lista.Add(new Cliente
                            {
                                IDCliente = Convert.ToInt32(rd["IDCliente"]),
                                nombre = rd["nombre"].ToString(),
                                apellido = rd["apellido"].ToString(),
                                telefono = rd["telefono"].ToString(),
                                correo = rd["correo"].ToString(),
                                direccion = rd["direccion"].ToString(),
                                contrasenia = rd["contrasenia"].ToString(),
                                fechaNacimiento = rd["fechaDeNacimiento"].ToString(),
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
        [Route("ObtenerCliente/{IDCliente:int}")]
        public IActionResult Obtener(int IDCliente)
        {
            List<Cliente> lista = new List<Cliente>();
            Cliente cliente = new Cliente();
            try
            {
                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    conexion.Open();
                    var cmd = new SqlCommand("sp_listaCliente", conexion);
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (var rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            lista.Add(new Cliente
                            {
                                IDCliente = Convert.ToInt32(rd["IDCliente"]),
                                nombre = rd["nombre"].ToString(),
                                apellido = rd["apellido"].ToString(),
                                fechaNacimiento = (string)rd["fechaDeNacimiento"],
                                telefono = rd["telefono"].ToString(),
                                correo = rd["correo"].ToString(),
                                contrasenia = rd["contrasenia"].ToString(),
                                direccion = rd["direccion"].ToString(),
                                FK_IDAdministrador = Convert.ToInt32(rd["FK_IDAdministrador"])
                            });
                        }
                    }
                }
                cliente = lista.Where(item => item.IDCliente == IDCliente).FirstOrDefault();
                return StatusCode(StatusCodes.Status200OK, new { mensaje = "ok", response = cliente });
            }
            catch (Exception error)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = error.Message, response = cliente });
            }
        }
        [HttpPost]
        [Route("GuardarCliente")]
        public IActionResult Guardar([FromBody] Cliente objeto)
        {
            string contraseniaHash = objeto.contrasenia;//BCrypt.Net.BCrypt.HashPassword(objeto.contrasenia);
            try
            {
                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    conexion.Open();
                    objeto.contrasenia = contraseniaHash;
                    var cmd = new SqlCommand("sp_agregarCliente", conexion);
                    cmd.Parameters.AddWithValue("IDCliente", objeto.IDCliente);
                    cmd.Parameters.AddWithValue("nombre", objeto.nombre);
                    cmd.Parameters.AddWithValue("apellido", objeto.apellido);
                    cmd.Parameters.AddWithValue("fechaDeNacimiento", objeto.fechaNacimiento);
                    cmd.Parameters.AddWithValue("telefono", objeto.telefono);
                    cmd.Parameters.AddWithValue("correo", objeto.correo);
                    cmd.Parameters.AddWithValue("contrasenia", objeto.contrasenia);
                    cmd.Parameters.AddWithValue("direccion", objeto.direccion);
                    cmd.Parameters.AddWithValue("FK_IDAdministrador", objeto.FK_IDAdministrador = 1094880982);
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
        [Route("EditarCliente")]
        public IActionResult Editar([FromBody] Cliente objeto)
        {
            try
            {
                string contraseniaHash = BCrypt.Net.BCrypt.HashPassword(objeto.contrasenia);
                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    conexion.Open();
                    objeto.contrasenia = contraseniaHash;
                    var cmd = new SqlCommand("sp_editarCliente", conexion);
                    cmd.Parameters.AddWithValue("IDCliente", objeto.IDCliente ==  0 ? DBNull.Value : objeto.IDCliente);
                    cmd.Parameters.AddWithValue("nombre", objeto.nombre is null ? DBNull.Value : objeto.nombre);
                    cmd.Parameters.AddWithValue("apellido", objeto.apellido is null ? DBNull.Value : objeto.apellido);
                    cmd.Parameters.AddWithValue("fechaDeNacimiento", objeto.fechaNacimiento == default ? DBNull.Value : objeto.fechaNacimiento);
                    cmd.Parameters.AddWithValue("telefono", objeto.telefono is null ? DBNull.Value : objeto.telefono);
                    cmd.Parameters.AddWithValue("correo", objeto.correo is not null ? DBNull.Value : objeto.correo);
                    cmd.Parameters.AddWithValue("contrasenia", objeto.contrasenia is not null ? DBNull.Value : objeto.contrasenia);
                    cmd.Parameters.AddWithValue("direccion", objeto.direccion is null ? DBNull.Value : objeto.direccion);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }
                return StatusCode(StatusCodes.Status200OK, new { mensaje = "El usuario ha sido correctamente editado"});
            }
            catch (Exception error)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = error.Message });
            }
        }
        [HttpDelete]
        [Route("EliminarCliente/{IDCliente:int}")]
        public IActionResult Eliminar(int IDCliente )
        {
            try
            {
                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    conexion.Open();
                    var cmd = new SqlCommand("sp_eliminarCliente", conexion);
                    cmd.Parameters.AddWithValue("IDCliente", IDCliente);
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
