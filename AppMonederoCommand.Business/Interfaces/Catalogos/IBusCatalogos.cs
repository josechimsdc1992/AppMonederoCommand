namespace AppMonederoCommand.Business.Interfaces.Catalogos
{
    public interface IBusCatalogos
    {
        Task<IMDResponse<List<EntGenero>>> BObtenerGeneros();
    }
}