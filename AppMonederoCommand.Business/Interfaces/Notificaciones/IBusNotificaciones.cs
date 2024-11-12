namespace AppMonederoCommand.Business.Interfaces;

public interface IBusNotificaciones
{
    Task<IMDResponse<List<EntNotificaciones>>> BGetNotificaciones(string sToken, Guid uIdUsuario, string? sIdAplicacion = null);
    Task<IMDResponse<bool>> BBorrarNotificacion(Guid uIdNotificacion);
    Task<IMDResponse<bool>> BNotificacionLeida(Guid uIdNotificacion);
}
