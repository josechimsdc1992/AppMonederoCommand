namespace AppMonederoCommand.Business.Repositories.Ticket
{
    public interface IDatTicket
    {
        Task<IMDResponse<EntReplicaTicket>> DSave(EntReplicaTicket entReplicaTicket);
        Task<IMDResponse<bool>> DUpdateCancelado(EntReplicaTicket entReplicaTicket);
        Task<IMDResponse<bool>> DUpdateUsado(EntReplicaUpdateTicket entReplicaUpdateTicket);
        Task<IMDResponse<List<EntTicketResponse>>> DGetListadoTickets(EntConsultaTickets entConsultaTickets);
        Task<IMDResponse<int>> DGetCountTickets(EntConsultaTickets entConsultaTickets);
    }
}
