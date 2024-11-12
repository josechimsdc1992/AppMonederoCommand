namespace AppMonederoCommand.Entities.Usuarios.CodigoVerificacion
{
    public class EntCodigoVerificacion
    {
        /* IMASD S.A.DE C.V
       =========================================================================================
       * Descripción: 
       * Historial de cambios:
       * ---------------------------------------------------------------------------------------
       *    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
       * ---------------------------------------------------------------------------------------
       *      1        | 08/08/2023 | L.I. Oscar Luna        | Creación      
       * ---------------------------------------------------------------------------------------
       */

        [JsonProperty("Correo")]
        public string? sCorreo { get; set; }

        [JsonProperty("ClaveVerificacion")]
        public string sClaveVerificacion { get; set; }

        [JsonProperty("IdAplicacion")]
        [JsonIgnore]
        public string? sIdAplicacion { get; set; }
    }
}
