using System.Text.Json.Serialization;

namespace AppMonederoCommand.Entities;

/* IMASD S.A.DE C.V
=========================================================================================
* Descripción: 
* Historial de cambios:
* ---------------------------------------------------------------------------------------
*    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
* ---------------------------------------------------------------------------------------
*      1        | 03/10/2023 | Daniel Ortiz           | Creación
* ---------------------------------------------------------------------------------------
*/
public class EntKongLoginResponse
{
    [JsonPropertyName("Token")]
    public string? sToken { get; set; }

    [JsonPropertyName("RefreshToken")]
    public string? sRefreshToken { get; set; }

    [JsonPropertyName("Expires")]
    public DateTime dtExpiresToken { get; set; }

    [JsonPropertyName("ExpiresRefresh")]
    public DateTime dtRefreshTokenExpiryTime { get; set; }
}
