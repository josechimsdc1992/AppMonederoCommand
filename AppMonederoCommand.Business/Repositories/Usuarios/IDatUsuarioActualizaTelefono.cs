namespace AppMonederoCommand.Business.Repositories;

/* IMASD S.A.DE C.V
=========================================================================================
* Descripción: 
* Historial de cambios:
* ---------------------------------------------------------------------------------------
*    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
* ---------------------------------------------------------------------------------------
*      1        | 18/10/2023 | Daniel Ortiz           | Creación
* ---------------------------------------------------------------------------------------
*/
public interface IDatUsuarioActualizaTelefono
{
    Task<IMDResponse<EntUsuarioActualizaTelefono>> DGetByIdUsuario(Guid uIdUsuario);

    Task<IMDResponse<bool>> DSave(EntUsuarioActualizaTelefonoRequest entUsuario, Guid uIdUsuario);

    Task<IMDResponse<bool>> DUpdate(EntUsuarioActualizaTelefonoRequest entUsuario, Guid uIdUsuario);

    Task<IMDResponse<bool>> DVerificado(Guid uIdUsuario);
}

