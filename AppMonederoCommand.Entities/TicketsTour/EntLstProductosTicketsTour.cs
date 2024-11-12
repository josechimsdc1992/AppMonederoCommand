namespace AppMonederoCommand.Entities.TicketsTour
{
    public class EntLstProductosTicketsTour
    {
        public List<EntProductosTicketsTour>? productos { get; set; }
    }

    public class EntProductosTicketsTour
    {
        public string IdProducto { get; set; }
        public string nombre { get; set; }
        public string descripcion { get; set; }
        public decimal precioUnitario { get; set; }
        public bool app { get; set; }
        public List<EntDescuentosTicketsTour>? descuentos { get; set; }
        public List<EntDescuentosAdicionalesTicketsTour>? descuentosAdicionales { get; set; }
        public List<EntPaquetesTicketsTour>? paquetes { get; set; }
    }

    public class EntDescuentosTicketsTour
    {
        public string tipoAplica { get; set; }
        public string tipoDescuento { get; set; }
        public decimal porcentajeDescuento { get; set; }
    }

    public class EntDescuentosAdicionalesTicketsTour
    {
        public string tipoAplica { get; set; }
        public decimal porcentajeDescuento { get; set; }
        public int MesCompra { get; set; }
        public string Periodo { get; set; }
        public string FechaIniPeriodo { get; set; }
        public string FechaFinPeriodo { get; set; }
    }

    public class EntPaquetesTicketsTour
    {
        public List<EntDescuentosTicketsTour>? descuentos { get; set; }
        public List<EntDescuentosAdicionalesTicketsTour>? descuentosAdicionales { get; set; }
        public string IdPaquete { get; set; }
        public string nombre { get; set; }
        public string idProducto { get; set; }
        public string producto { get; set; }
        public bool aplicaDescuentoAcumulable { get; set; }
        public int vigenciaHoras { get; set; }
        public string fechaVigenciaInicio { get; set; }
        public string fechaVigenciaFin { get; set; }
        public int unidad { get; set; }
    }
}
