namespace AppMonederoCommand.Business.Interfaces.Parametro
{
    public interface IBusParametros
    {
        Task<IMDResponse<bool>> BAgregar(EntParametros entParametro);
        Task<IMDResponse<List<EntParametros>>> BObtener();
        Task<IMDResponse<EntParametros>> BObtener(string sNombre);
        Task<IMDResponse<bool>> BActualizar(EntActualizarParametros entParametros);
        Task<IMDResponse<List<EntParametros>>> BObtenerByClaves(List<string> claves);
        public void SetParametros();
        public void SetListadosAdicionales();
        public void SetConfiguracionModulo(string sClave, string sValor);
    }
}
