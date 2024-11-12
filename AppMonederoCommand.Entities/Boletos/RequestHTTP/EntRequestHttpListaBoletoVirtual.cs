namespace AppMonederoCommand.Entities.Boletos.RequestHTTP
{
    public class EntRequestHttpListaBoletoVirtual
    {
        /* IMASD S.A.DE C.V
    =========================================================================================
    * Descripción: 
    * Historial de cambios:
    * ---------------------------------------------------------------------------------------
    *    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
    * ---------------------------------------------------------------------------------------
    *      1        | 03/10/2023 | L.I. Oscar Luna        | Creación
    * ---------------------------------------------------------------------------------------
    */
        public Guid uIdMonedero { get; set; }

        public int iUsado { get; set; }

        public int iVigente { get; set; }

        public string? sClaveApp { get; set; }
        public Guid? uIdSolicitud { get; set; }
    }
}
