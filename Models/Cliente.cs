using NodaTime;

namespace MercadoCampesinoBack.Models
{
    public class Cliente
    {

        public int IDCliente { get; set; }
        public string nombre { get; set; }
        public string apellido { get; set; }
        public string fechaNacimiento { get; set; }
        public string telefono { get; set; }
        public string correo { get; set; }
        public string contrasenia { get; set; }
        public string direccion { get; set; }
        public int FK_IDAdministrador { get; set; }

    }
}
