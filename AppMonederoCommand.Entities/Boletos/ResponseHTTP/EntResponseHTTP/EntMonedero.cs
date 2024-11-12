namespace AppMonederoCommand.Entities.Boletos.ResponseHTTP.EntResponseHTTP
{
    public class EntMonedero
    {
        /* IMASD S.A.DE C.V
    =========================================================================================
    * Descripción: 
    * Historial de cambios:
    * ---------------------------------------------------------------------------------------
    *    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
    * ---------------------------------------------------------------------------------------
    *      01        | 05/10/2023 | L.I. Oscar Luna        | Creación
    * ---------------------------------------------------------------------------------------
    */
        public Guid uIdMonedero { get; set; }
        public Guid uIdMonederoOrigen { get; set; }
        public long iNumeroMonedero { get; set; }
        public string sTokenTarjeta { get; set; }
        public string sTipoTarifa { get; set; }
        public Guid uIdTipoTarifa { get; set; }
        public string sNumeroTarjeta { get; set; }
        public int iNumSequencial { get; set; }
        public DateTime dtFechaCreacion { get; set; }
        public DateTime dtFechaUltMod { get; set; }
        public bool bActivo { get; set; }
        public bool bBaja { get; set; }
        public Guid idUsuarioCreacion { get; set; }
        public Guid idUsuarioUltMod { get; set; }
        public Guid idUsuarioBaja { get; set; }



    }
}
