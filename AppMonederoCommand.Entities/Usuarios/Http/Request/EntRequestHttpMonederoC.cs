using System.Text.Json.Serialization;

namespace AppMonederoCommand.Entities.Usuarios.Http.Request
{
    public class EntRequestHttpMonederoC
    {

        /* IMASD S.A.DE C.V
     =========================================================================================
     * Descripción: 
     * Historial de cambios:
     * ---------------------------------------------------------------------------------------
     *    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
     * ---------------------------------------------------------------------------------------
     *      1        | 11/10/2023 | L.I. Oscar Luna        | Creación
     *      2        | 16/10/2023 | L.I. Oscar Luna        | Modificación de propiedades
     * ---------------------------------------------------------------------------------------
     */

        [JsonPropertyName("cantidad")]
        public int Cantidad { get; set; }

        [JsonPropertyName("idTipo")]
        public Guid uIdTipo { get; set; }

        [JsonPropertyName("idTarifa")]
        public Guid uIdTarifa { get; set; }

        [JsonPropertyName("numTelefono")]
        public string? sTelefono { get; set; }

        [JsonPropertyName("nombre")]
        public string sNombre { get; set; }

        [JsonPropertyName("apellidoPaterno")]
        public string sApellidoPaterno { get; set; }

        [JsonPropertyName("apellidoMaterno")]
        public string? sApellidoMaterno { get; set; }

        [JsonPropertyName("correo")]
        public string? sCorreo { get; set; }

        [JsonPropertyName("fechaNacimiento")]
        public DateTime? dtFechaNacimiento { get; set; }


    }
}
