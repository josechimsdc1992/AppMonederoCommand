using System.Text.Json.Serialization;

namespace AppMonederoCommand.Entities;

public class EntPaqueteResponse
{
    [JsonPropertyName("IdPaquete")]
    public Guid uIdPaquete { get; set; }

    [JsonPropertyName("nombre")]
    public string? sNombre { get; set; }

    [JsonPropertyName("idProducto")]
    public string? iIdProducto { get; set; }

    [JsonPropertyName("producto")]
    public string? sProducto { get; set; }

    [JsonPropertyName("unidad")]
    public decimal iUnidad { get; set; }

    [JsonPropertyName("Importe")]
    public float fImporte { get; set; }

    [JsonPropertyName("PrecioUnitario")]
    public float fPrecioUnitario { get; set; }

    [JsonPropertyName("DescripcionPaquete")]
    public string? sDescripcionPaquete { get; set; }
}

public class EntPaquetesProductosResponse
{
    [JsonPropertyName("paquetes")]
    public List<EntPaqueteResponse> lstPaquetes { get; set; }
}
