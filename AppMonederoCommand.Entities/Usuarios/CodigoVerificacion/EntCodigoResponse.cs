namespace AppMonederoCommand.Entities.Usuarios.CodigoVerificacion
{
    public class EntCodigoResponse
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
        [JsonProperty("Usuario")]
        public EntUsuarioCodigoVerificacion? entUsuario { get; set; }

        [JsonProperty("Favoritos")]
        public List<EntGetAllUbicacionFavorita>? lisFavoritos { get; set; }

        [JsonProperty("Token")]
        public EntTokenCodigoVerificacion entToken { get; set; }

    }
}
