namespace AppMonederoCommand.Entities.Boletos.ResponseHTTP.EntResponseHTTP
{
    public class EntTarifa
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
        public Guid uIdTipoTarifa { get; set; }
        public string sTipoTarifa { get; set; }
        public string sClaveTipoTarifa { get; set; }
        public float fTarifa { get; set; }
        public int iNumeroTipoTarifa { get; set; }
    }
}
