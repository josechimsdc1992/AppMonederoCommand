namespace AppMonederoCommand.Entities.Replicas
{
    public class EntReplicaUpdateTicket
    {
        public Guid uIdTicket { get; set; }
        public Guid uIdMonedero { get; set; }
        public string claveApp { get; set; } = "";
    }
}
