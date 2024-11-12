namespace AppMonederoCommand.Entities.Usuarios
{
    public class EntUsuarioTarifa
    {
        /* IMASD S.A.DE C.V
          =========================================================================================
          * Descripción: 
          * Historial de cambios:
          * ---------------------------------------------------------------------------------------
          *    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
          * ---------------------------------------------------------------------------------------
          *      1        | 20/10/2023 | David Realpozo         | Creación      
          * ---------------------------------------------------------------------------------------
          */
        [JsonProperty("Usuario")]
        public EntUsuario entUsuario { get; set; }
        [JsonProperty("Tarifa")]
        public EntSaldo entSaldo { get; set; }
        [JsonProperty("Configuraciones")]
        public EntUsuarioConfiguracion entUsuarioConfiguracion { get; set; }
    }
}
