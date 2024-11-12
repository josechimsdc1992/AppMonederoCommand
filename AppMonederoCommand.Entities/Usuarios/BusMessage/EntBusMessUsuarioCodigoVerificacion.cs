namespace AppMonederoCommand.Entities.Usuarios.BusMessage
{
    public class EntBusMessUsuarioCodigoVerificacion
    {
        /* IMASD S.A.DE C.V
     =========================================================================================
     * Descripción: 
     * Historial de cambios:
     * ---------------------------------------------------------------------------------------
     *    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
     * ---------------------------------------------------------------------------------------
     *      1        | 02/08/2023 | L.I. Oscar Luna        | Creación
     * ---------------------------------------------------------------------------------------
     */

        [JsonProperty("IdUsuario")]
        public Guid uIdUsuario { get; set; }

        [JsonProperty("NumeroTelefono")]
        public string sNumeroTelefono { get; set; }


        [JsonProperty("Mensaje")]
        public string sMensaje { get; set; }
    }
}
