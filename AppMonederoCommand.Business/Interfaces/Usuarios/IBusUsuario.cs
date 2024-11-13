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
        Task<IMDResponse<EntUsuarioResponse>> BCreateUsuario(EntCreateUsuario pCreateModel);
        Task<IMDResponse<dynamic>> BIniciarSesion(EntLogin pUsuario);
        Task<IMDResponse<dynamic>> BIniciaSesionToken(EntRefreshTokenRequest pToken, string sIdAplicacion);
        Task<IMDResponse<dynamic>> BVerificaCodigo(EntCodigoVerificacion codigo);
        Task<IMDResponse<dynamic>> BReenviarCodigo(EntReenviaCodigo solicitud);
        Task<IMDResponse<dynamic>> BCambiaContrasena(Guid uIdUsuario, EntNuevaContrasena password);
        Task<IMDResponse<bool>> BGuardaFireBaseToken(Guid uIdUsuario, string sFcmToken, string sInfoAppOS, string? sIdAplicacion);
        Task<IMDResponse<bool>> BValidaEmail(string pEmail);
        Task<IMDResponse<bool>> BExisteCorreo(string pEmail);
        Task<IMDResponse<bool>> BExisteTelefono(string pTelefono);
        Task<IMDResponse<bool>> BExisteCURP(string pCURP);
        Task<IMDResponse<string>> BGeneraCodigoVerificacion();
        Task<IMDResponse<dynamic>> BHttpMonederoC(EntUsuario pUsuario);
        Task<IMDResponse<bool>> BVerificaContrasena(string contrasena, string confirmaContrasena);
        Task<IMDResponse<string>> BValidaDatosCompletos(EntCreateUsuario pCreateModel);
        Task<IMDResponse<dynamic>> BGeneraTokenRecuperacion();
        Task<IMDResponse<EntUsuario>> BUpdateUsuario(EntUpdateUsuario entUsuario);
        Task<IMDResponse<EntUsuario>> BUpdateUsuario(EntUpdateUsuarioRedSocial creaUsarioRedSocialBaja);
        Task<IMDResponse<EntUsuario>> BUpdateUsuario(EntUsuario nuevoUsuario);
        Task<IMDResponse<EntUsuario>> BUpdateUsuario(EntUpdateUsarioActivo usuarioBaja);
        Task<IMDResponse<bool>> BEliminarUsuario(Guid uIdUsuario);
        Task<IMDResponse<EntUsuario>> BGetByCorreo(string Email);
        Task<IMDResponse<dynamic>> BGuardaImagenPerfil(EntRequestBlobStorage requestImagenPerfil);
        Task<IMDResponse<EntUsuarioTarifa>> BUsuarioTarifa(Guid uIdUsuario, string sToken, string sIdAplicacion);
        Task<IMDResponse<dynamic>> BDescargaImagenPerfil(Guid uIdUsuario);
        Task<IMDResponse<dynamic>> BEliminaImagenPerfil(Guid uIdUsuario);
        Task<IMDResponse<dynamic>> BActualizaDatosMonedero(EntUsuario datosUsuario);
        Task<IMDResponse<EntUsuario>> BGet(Guid uIdUsuario);
        Task<IMDResponse<bool>> EnviarCorreoValidacionAbono(EntBusMessCorreoValidacionAbono entBusMessCorreoValidacionAbono);
        Task<IMDResponse<bool>> EnviarSmsValidacionAbono(EntBusMessSmsValidacionAbono entBusMessSMSValidacionAbono);
        Task<IMDResponse<bool>> EnviarPushValidacionAbono(EntBusMessPushValidacionAbono entBusMessPushValidacionAbono);
        Task<IMDResponse<List<EntFirebaseToken>>> BObtenerFireBaseToken(Guid uIdUsuario, int iTop = 0, string? sIdAplicacion = null);
        Task<IMDResponse<bool>> EnviarMultipleCorreo(EntBusMessCorreoMultiples entBusMessMultipleCorreo);
        Task<IMDResponse<bool>> BNotificarTraspaso(string sMonederoOrigen, string sMonederoDestino, Guid uIdUsuario, TraspasoSaldoRequestModel traspasoSaldoRequestModel);
        Task<IMDResponse<EntUsuario>> BGetByMonedero(Guid uIdMonedero, string? sIdAplicacion = null);
        Task<IMDResponse<EntUsuario>> BValidaMonederoUsuario(Guid uIdUsuario);

        Task<IMDResponse<dynamic>> BDescargaImagenUsuarioApp(Guid uIdUsuario);
        Task<IMDResponse<string>> BGeneraCodigoAlfanumerico(int iLength);
        Task<IMDResponse<bool>> BActualizaDispositivoCuenta(EntDispositivoCuentaUpdate entDispositivoCuentaUpdate);
    }
}
