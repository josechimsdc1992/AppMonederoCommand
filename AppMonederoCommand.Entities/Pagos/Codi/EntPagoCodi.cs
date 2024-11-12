
namespace AppMonederoCommand.Entities;

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
public class EntPagoCodi
{
    [JsonProperty("reference")]
    public string? sReference { get; set; }

    [JsonProperty("Identificador")]
    public string? sIdentificador { get; set; }

    [JsonProperty("bankID")]
    public string? sBankID { get; set; }

    [JsonProperty("date")]
    public DateTime dtDate { get; set; }

    [JsonProperty("cadena")]
    public string? sCadena { get; set; }

    [JsonProperty("user")]
    public string? sUser { get; set; }

    [JsonProperty("password")]
    public string? sPassword { get; set; }

    [JsonProperty("amount")]
    public decimal dAmount { get; set; }

    [JsonProperty("PaymentType")]
    public string? sPaymentType { get; set; }

    [JsonProperty("clientInformation")]
    public string? sClientInformation { get; set; }

    [JsonProperty("branch")]
    public string? sBranch { get; set; }

    [JsonProperty("EstatusCoDi")]
    public string? sEstatusCoDi { get; set; }

    [JsonProperty("DescripcionCoDi")]
    public string? sDescripcionCoDi { get; set; }

    [JsonProperty("FolioCoDi")]
    public string? sFolioCoDi { get; set; }
}
