namespace AppMonederoCommand.Entities.Usuarios.BusMessage
{
    public class EntListenerService
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

        [JsonProperty("Telefono")]
        public string sTelefono { get; set; }

        [JsonProperty("Correo")]
        public string sCorreo { get; set; }

        [JsonProperty("CodigoVerificacion")]
        public string sCodigoVerificacion { get; set; }
    }



}
