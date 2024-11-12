namespace AppMonederoCommand.Entities.Pagos.Orden;

public class EntOrdenCodi
{
    public DateTime Fecha { get; set; }
    public int OpcionPago { get; set; }
    public bool Activo { get; set; }
    public bool Pagado { get; set; }
    public Guid IdUsuario { get; set; }
    public string? DataEncripted { get; set; }
    public Guid IdMonedero { get; set; }
    public EntCoDiQR? CoDiQR { get; set; }
    public Guid? IdOrden { get; set; } = Guid.Empty;
    public decimal? Monto { get; set; } = 0;
    public string? Concepto { get; set; } = string.Empty;
    public Guid? IdPaquete { get; set; } = Guid.Empty;
    public List<EntOrdenDetalle> Detalle { get; set; }
    public string? OrdenRef { get; set; }
    public EntPagosInfoWebComprador? InfoWeb { get; set; }
}

public class EntCoDiQR
{
    public string ImagenQR { get; set; }
    public DateTime Vigencia { get; set; }
}
