namespace AppMonederoCommand.Business.Interfaces.Catalogos
{
    public interface IBusTipoOperaciones
    {
        Task<IMDResponse<List<EntTipoOperaciones>>> BObtenerTipoOperaciones();
    }
}
