namespace AppMonederoCommand.Entities.Replicas
{
    public class EntUpdateReplicaEstatusMonedero
    {
        public List<Guid> IdMonedero { get; set; }
        public Guid IdEstatus { get; set; }
        public string Estatus { get; set; }
        public Guid IdMotivo { get; set; }
        public bool Baja { get; set; }
    }
}
