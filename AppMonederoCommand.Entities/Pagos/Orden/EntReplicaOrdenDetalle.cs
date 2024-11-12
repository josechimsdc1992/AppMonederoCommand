namespace AppMonederoCommand.Entities.Pagos.Orden
{
    public class EntReplicaOrdenDetalle
    {
        public string sConcepto { get; set; }
        public string sOrdenRef { get; set; }
        public int iOpcionPago { get; set; }
        public string sOpcionPago { get; set; }
        public decimal dComision { get; set; }
        public Guid uIdOperacion { get; set; }
    }
}