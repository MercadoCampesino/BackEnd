namespace MercadoCampesinoBack.Models
{
    public class Producto
    {
        public int IDProducto {  get; set; }
        public string nombre { get; set; }
        public int existencia { get; set; }
        public int precio { get; set; }
        public string imagen { get; set; }
        public int FK_IDTienda {  get; set; }
        public int FK_IDCategoria {  get; set; }
    }
}
