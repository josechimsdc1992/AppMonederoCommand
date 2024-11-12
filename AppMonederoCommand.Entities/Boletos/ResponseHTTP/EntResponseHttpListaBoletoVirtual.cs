using AppMonederoCommand.Entities.Boletos.ResponseHTTP.EntResponseHTTP;

namespace AppMonederoCommand.Entities.Boletos.ResponseHTTP
{
    public class EntResponseHttpListaBoletoVirtual
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
        public HttpStatusCode HttpCode { get; set; }
        public bool HasError { get; set; }
        public string? Message { get; set; }
        public long ErrorCode { get; set; }
        public EntResult? Result { get; set; }


    }
}
