namespace AppMonederoCommand.Business.Interfaces.Usuarios
{
    /* IMASD S.A.DE C.V
  =========================================================================================
  * Descripción: 
  * Historial de cambios:
  * ---------------------------------------------------------------------------------------
  *    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
  * ---------------------------------------------------------------------------------------
  *      1        | 24/07/2023 | L.I. Oscar Luna   | Creación
  *      2        | 22/08/2023 | Neftalí Rodríguez     | Update y Delete usuario
  * ---------------------------------------------------------------------------------------
  */
    public interface IBusUsuario
    {
        
        Task<IMDResponse<EntUsuario>> BGet(Guid uIdUsuario);
       
        Task<IMDResponse<List<EntFirebaseToken>>> BObtenerFireBaseToken(Guid uIdUsuario, int iTop = 0, string? sIdAplicacion = null);
        
        Task<IMDResponse<bool>> BNotificarTraspaso(string sMonederoOrigen, string sMonederoDestino, Guid uIdUsuario, TraspasoSaldoRequestModel traspasoSaldoRequestModel);
        
        
    }
}
