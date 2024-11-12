namespace AppMonederoCommand.Business.Tickets
{
    public class BusTicket : IBusTicket
    {
        private readonly ILogger<BusTicket> _logger;
        private readonly IDatTicket _datTickets;
        public BusTicket(ILogger<BusTicket> logger, IDatTicket datTickets)
        {
            _logger = logger;
            _datTickets = datTickets;
        }

        [IMDMetodo(67823465990035, 67823465990812)]
        public async Task<IMDResponse<List<EntTicketResponse>>> BGetListadoTickets(EntConsultaTickets entConsultaTickets)
        {
            IMDResponse<List<EntTicketResponse>> response = new IMDResponse<List<EntTicketResponse>>();
            IMDMetodo metodo = MethodBase.GetCurrentMethod().GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo.sNombre}{metodo.sParametros}"));
            try
            {
                response = await _datTickets.DGetListadoTickets(entConsultaTickets);
            }
            catch (Exception ex)
            {
                response.ErrorCode = 500;
                response.SetError(ex);
                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo.sNombre}{metodo.sParametros}: {ex.Message}", entConsultaTickets, ex, response));
            }
            return response;
        }
        
        [IMDMetodo(67823466145435, 67823466146212)]
        public async Task<IMDResponse<int>> BGetCountTickets(EntConsultaTickets entConsultaTickets)
        {
            IMDResponse<int> response = new IMDResponse<int>();
            IMDMetodo metodo = MethodBase.GetCurrentMethod().GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo.sNombre}{metodo.sParametros}", entConsultaTickets));
            try
            {
                response = await _datTickets.DGetCountTickets(entConsultaTickets);
            }
            catch (Exception ex)
            {
                response.ErrorCode = 500;
                response.SetError(ex);
                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo.sNombre}{metodo.sParametros}: {ex.Message}", entConsultaTickets, ex, response));
            }
            return response;
        }

    }
}
