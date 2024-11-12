namespace AppMonederoCommand.Entities;

public class EntOrden
{
    public Guid IdOrden { get; set; }
    public string? OrdenRef { get; set; }
    public decimal Monto { get; set; }
    public string? Concepto { get; set; }
    public Guid? IdPaquete { get; set; }
    public DateTime Fecha { get; set; }
    public int OpcionPago { get; set; }
    public bool Activo { get; set; }
    public bool Pagado { get; set; }
    public Guid IdUsuario { get; set; }
    public string? DataEncripted { get; set; }
    public Guid? IdMonedero { get; set; }
    public List<EntOrdenDetalle> Detalle { get; set; }
    public EntPagosInfoWebComprador? InfoWeb { get; set; }
    public EntCoDiQR? CoDiQR { get; set; }
}
