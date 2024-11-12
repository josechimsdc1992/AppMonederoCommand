namespace AppMonederoCommand.Business.Interfaces.Tickets
{
    public interface IBusTicket
    {
        Task<IMDResponse<List<EntTicketResponse>>> BGetListadoTickets(EntConsultaTickets entConsultaTickets);
        Task<IMDResponse<int>> BGetCountTickets(EntConsultaTickets entConsultaTickets);
    }
}
