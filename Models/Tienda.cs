using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MercadoCampesinoBack.Modelos
{
    public class Tienda
    {
        public int IDTienda { get; set; }
        public string nombre { get; set; }
        public string imagen { get; set; }
        public string telefono { get; set; }
        public string direccion { get; set; }
        public string correo { get; set; }
        public string contrasenia { get; set; }
        public int FK_IDAdministrador { get; set; }
    }
}
