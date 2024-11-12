namespace AppMonederoCommand.Business.Interfaces.Webhooks
{
    public interface IBusWebhooks
    {
        Task<IMDResponse<bool>> BNotificarMercadoPago(Dictionary<string, string> headers, object body);
        Task<IMDResponse<bool>> BNotificarPayPal(Dictionary<string, string> headers, object body);
    }
}

