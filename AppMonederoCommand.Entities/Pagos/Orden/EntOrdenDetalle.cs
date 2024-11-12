namespace AppMonederoCommand.Entities.Pagos.Orden;

public class EntOrdenDetalle
{
    public Guid IdOrdenDetalle { get; set; }
    public Guid IdOrden { get; set; }
    public string Concepto { get; set; }
    public decimal Monto { get; set; }
    public bool Comision { get; set; }
    public decimal Orden { get; set; }
}