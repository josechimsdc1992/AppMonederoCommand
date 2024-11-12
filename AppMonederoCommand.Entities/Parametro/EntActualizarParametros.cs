namespace AppMonederoCommand.Entities.Parametro
{
    public class EntActualizarParametros
    {
        public Guid uIdParametro { get; set; }
        public string sNombre { get; set; }
        public string sValor { get; set; }
        public bool bActivo { get; set; }
        public bool bBaja { get; set; }
        public Guid? uIdUsuarioModificacion { get; set; }
        public DateTime? dtFechaModificacion { get; set; }
        public bool? bEncriptado { get; set; }
        public string? sDescripcion { get; set; }
    }
}
