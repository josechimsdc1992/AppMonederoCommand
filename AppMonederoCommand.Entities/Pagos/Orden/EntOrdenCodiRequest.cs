namespace AppMonederoCommand.Entities.Pagos.Orden;

/* IMASD S.A.DE C.V
=========================================================================================
* Descripción: 
* Historial de cambios:
* ---------------------------------------------------------------------------------------
*    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
* ---------------------------------------------------------------------------------------
*      1        | 11/01/2024 | Daniel Ortiz           | Creación
* ---------------------------------------------------------------------------------------
*/
public class EntOrdenCodiRequest
{
    [JsonProperty("OpcionPago")]
    public int? iOpcionPago { get; set; }

    [JsonProperty("IdUsuario")]
    public Guid? uIdUsuario { get; set; }

    [JsonProperty("Monto")]
    public decimal dMonto { get; set; }

    [JsonProperty("Concepto")]
    public string? sConcepto { get; set; }

    [JsonProperty("IdPaquete")]
    public Guid uIdPaquete { get; set; }

    [JsonProperty("IdMonedero")]
    public Guid uIdMonedero { get; set; }

    [JsonProperty("EmailUsuario")]
    public string? sEmailUsuario { get; set; }

    [JsonProperty("InfoWeb")]
    public EntPagosInfoWebComprador? entPagosWebInfoComprador { get; set; }
}

