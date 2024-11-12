namespace AppMonederoCommand.Entities.Monedero.RequestHTTP
{
    public class EntRequestHTTPActualizaMonedero
    {
        [JsonProperty("IdMonedero")]
        public Guid uIdMonedero { get; set; }

        [JsonProperty("IdTipoTarifa")]
        public Guid? uIdTipoTarifa { get; set; }

        [JsonProperty("NumTelefono")]
        public string? sNumTelefono { get; set; }

        [JsonProperty("Nombre")]
        public string? sNombre { get; set; }

        [JsonProperty("ApellidoPaterno")]
        public string? sApellidoPaterno { get; set; }

        [JsonProperty("ApellidoMaterno")]
        public string? sApellidoMaterno { get; set; }

        [JsonProperty("Correo")]
        public string? sCorreo { get; set; }

        [JsonProperty("FechaNacimiento")]
        public DateTime? dtFechaNacimiento { get; set; }

        [JsonProperty("FechaVigencia")]
        public string? sFechaVigencia { get; set; }
    }
}
