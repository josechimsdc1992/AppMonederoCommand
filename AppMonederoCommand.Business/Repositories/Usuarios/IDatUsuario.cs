namespace AppMonederoCommand.Business.Repositories.Usuarios
{

    /* IMASD S.A.DE C.V
    =========================================================================================
    * Descripción: 
    * Historial de cambios:
    * ---------------------------------------------------------------------------------------
    *    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
    * ---------------------------------------------------------------------------------------
    *      1        | 21/07/2023 | L.I. Oscar Luna        | Creación
    *      2        | 22/08/2023 | Neftalí Rodríguez     | Update y Delete usuario
    * ---------------------------------------------------------------------------------------
    */
    public interface IDatUsuario
    {
        Task<IMDResponse<EntUsuario>> DSave(EntUsuario nuevoUsuario);
        Task<IMDResponse<bool>> DSaveFirebaseToken(EntFirebaseToken firebaseToken, EntUsuario entUsuario);
        Task<IMDResponse<EntUsuario>> DGet(Guid uIdUsuario);
        Task<IMDResponse<EntUsuario>> DGetCorreo(string pEmail);
        Task<IMDResponse<EntUsuario>> DGetByRedSocial(string sIdRedSocial, string sRedSocial);
        Task<IMDResponse<EntUsuario>> DGetTelefono(string pEmail);
        Task<IMDResponse<EntUsuario>> DGetCURP(string pCURP);
        Task<IMDResponse<EntUsuario>> DGetUsuario(EntLogin pUsuario, bool pTipoLogin, bool? bCuentaVerificada = null, bool? bActivo = true, bool? bBaja = false);
        Task<IMDResponse<EntUsuario>> DGetUsuarioMigrado(EntLogin pUsuario);
        Task<IMDResponse<EntUsuario>> DGetByMonedero(Guid uIdMonedero);
        Task<IMDResponse<bool>> DGetValidaUsuario(EntCodigoVerificacion codigo);
        Task<IMDResponse<bool>> DGetCodigoValido(EntCodigoVerificacion codigo);
        Task<IMDResponse<EntUsuario>> DUpdateUsuarioVerificado(EntCodigoVerificacion codigo);
        Task<IMDResponse<bool>> DUpdateCodigoVerificacion(EntReenviaCodigo solicitud, string codgioEncrypt);
        Task<IMDResponse<bool>> DUpdateContrasena(Guid uIdUsuario, EntNuevaContrasena contrasena);
        Task<IMDResponse<bool>> DUpdateContrasena(Guid uIdUsuario, EntNuevaContrasena contrasena, string token);
        Task<IMDResponse<bool>> DUpdateMonederoUsuario(Guid uIdUsuario, Guid uIdMonedero);
        Task<IMDResponse<bool>> DUpdateUsuario(EntUpdateUsuario entUsuario);
        Task<IMDResponse<bool>> DUpdateUsuario(EntUpdateUsuarioRedSocial entUsuario);
        Task<IMDResponse<bool>> DUpdateUsuario(EntUsuario nuevoUsuario);
        Task<IMDResponse<bool>> DUpdateUsuario(EntUpdateUsarioActivo usuarioActivo);
        Task<IMDResponse<EntUsuario>> DGetUsuario(Guid uIdUsuario);
        Task<IMDResponse<EntUsuario>> DGetExisteCuenta(string correo);
        Task<IMDResponse<EntUsuario>> DGetExisteCuenta(string correo, bool isSocialNetwork);
        Task<IMDResponse<bool>> DEliminarUsuario(EntEliminarUsuario entUsuario);
        Task<IMDResponse<bool>> DUpdateContrasenaTemporal(Guid uIdUsuario, string contrasenaAleatoria);

        Task<IMDResponse<EntUsuario>> DGetByAppleId(string sAppleId);

        Task<IMDResponse<EntUsuario>> DUpdateUsuarioDatosAdicionales(Guid uIdUsuario, string sTelefono, string sCURP);
        Task<IMDResponse<bool>> DUpdateFotografia(Guid uIdUsuario, string sFotografia);
        Task<IMDResponse<List<EntFirebaseToken>>> DGetFirebaseToken(Guid uIdUsuario, int iTop = 0, string? sIdAplicacion = null);

        Task<IMDResponse<EntPagination<EntUsuarioApp>>> DGetUsuariosApp(EntFiltersUsuario pEntity);
        Task<IMDResponse<List<EntUsuarioAppInfo>>> DGetUsuarioAppInfo(Guid uIdUsuario);
        Task<IMDResponse<bool>> DUpdateUsuarioByMonedero(EntUpdateUsuarioByMonedero entUpdateUsuarioByMonedero);
        Task<IMDResponse<bool>> DActualizaEstatusCuenta(EntEstatusCuentaUpdate entEstatusCuentaUpdate);
        Task<IMDResponse<bool>> DActualizaDispositivoCuenta(EntDispositivoCuentaUpdate entDispositivoCuentaUpdate);
        Task<IMDResponse<List<EntFirebaseToken>>> DGetListFirebaseToken(Guid uIdUsuario, string sIdAplicacion);
        Task<IMDResponse<bool>> DUpdateEstatusCuentaByMonedero(EntUpdateEstatusCuentaByMonedero entUpdateEstatusCuentaByMonedero);
        Task<IMDResponse<bool>> DActualizarEstatusCuenta(EntActualizarEstatusCuenta entActualizarEstatusCuenta);
    }
}
