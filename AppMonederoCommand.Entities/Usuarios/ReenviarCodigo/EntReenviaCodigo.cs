namespace AppMonederoCommand.Entities.Usuarios.ReenviarCodigo
{
    public class EntReenviaCodigo
    {
        /* IMASD S.A.DE C.V
       =========================================================================================
       * Descripción: 
       * Historial de cambios:
       * ---------------------------------------------------------------------------------------
       *    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
       * ---------------------------------------------------------------------------------------
       *      1        | 11/08/2023 | L.I. Oscar Luna        | Creación
       * ---------------------------------------------------------------------------------------
       */

        [JsonProperty("Correo")]
        public string sCorreo { get; set; }
    }
}
