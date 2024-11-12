namespace AppMonederoCommand.Entities.TipoOperaciones
{
    public class EntTipoOperaciones
    {
        public Guid uIdTipoOperacion { get; set; }
        public string sNombre { get; set; }
        public string sClave { get; set; }
        public int iModulo { get; set; }
        public bool bActivo { get; set; }
        public bool bBaja { get; set; }
    }
}
