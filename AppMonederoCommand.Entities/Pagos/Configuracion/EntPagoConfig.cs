namespace AppMonederoCommand.Entities;

/* IMASD S.A.DE C.V
=========================================================================================
* Descripción: 
* Historial de cambios:
* ---------------------------------------------------------------------------------------
*    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
* ---------------------------------------------------------------------------------------
*      1        | 06/10/2023 | Daniel Ortiz           | Creación
* ---------------------------------------------------------------------------------------
*/
public class EntPagoConfig
{
    public string? ClientId { get; set; }
    public string? ClientSecret { get; set; }
    public string? BaseUrl { get; set; }

    public string? Currency { get; set; }

    public string? AccessTokenServer { get; set; }

    public string? AccessTokenClient { get; set; }

    public string? LinkSuccess { get; set; }

    public string? LinkFailure { get; set; }

}
