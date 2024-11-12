namespace AppMonederoCommand.Entities.Usuarios.Contrasena
{
    public class EntNuevaContrasena
    {

        /* ========================================================================================
        * Descripción: 
        * Historial de cambios:
        * ---------------------------------------------------------------------------------------
        *    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
        * ---------------------------------------------------------------------------------------
        *      1        | 11/08/2023 | L.I.Oscar Luna        | Creación
        * ---------------------------------------------------------------------------------------
        */

        [JsonProperty("Contrasenia")]
        public string sContrasenia { get; set; }

        [JsonProperty("ConfirmaContrasenia")]
        public string sConfirmaContrasenia { get; set; }


    }
}
