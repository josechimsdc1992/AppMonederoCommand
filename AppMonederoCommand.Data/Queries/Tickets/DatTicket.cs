namespace AppMonederoCommand.Data.Queries.Tickets
{
    public class DatTicket : IDatTicket
    {
        private readonly TransporteContext _dbContext;
        private readonly ILogger<DatTicket> _logger;
        private readonly IMapper _mapper;
        public DatTicket(TransporteContext dbContext, ILogger<DatTicket> logger,IMapper mapper)
        {
            _dbContext = dbContext;
            _logger = logger;
            _mapper = mapper;
        }

        [IMDMetodo(67823465976049, 67823465976826)]
        public async Task<IMDResponse<EntReplicaTicket>> DSave(EntReplicaTicket entReplicaTicket)
        {
            IMDResponse<EntReplicaTicket> response = new IMDResponse<EntReplicaTicket>();
            IMDMetodo metodo = MethodBase.GetCurrentMethod().GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo.sNombre}{metodo.sParametros}"));
            try
            {
                var mapTicket = _mapper.Map<Ticket>(entReplicaTicket);

                _dbContext.Ticket.Add(mapTicket);
                var exec = await _dbContext.SaveChangesAsync();

                if (exec > 0)
                {
                    response.SetSuccess(entReplicaTicket, Menssages.DatAddSuccessInfo);
                }
                else
                {
                    response.ErrorCode = metodo.iCodigoError;
                    response.SetError(Menssages.DatNoAddInfo);
                    _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError,
                        $"Error en {metodo.sNombre}({metodo.sParametros}): {Menssages.DatNoAddInfo}", entReplicaTicket, response));
                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = 500;
                response.SetError(ex);
                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo.sNombre}{metodo.sParametros}: {ex.Message}", entReplicaTicket, ex, response));
            }
            return response;
        }

        [IMDMetodo(67823465977603, 67823465978380)]
        public async Task<IMDResponse<bool>> DUpdateCancelado(EntReplicaTicket entReplicaTicket)
        {
            IMDResponse<bool> response = new IMDResponse<bool>();

            IMDMetodo metodo = MethodBase.GetCurrentMethod().GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo.sNombre}{metodo.sParametros}"));
            try
            {
                var dbmodel = await _dbContext.Ticket.FirstOrDefaultAsync(i => i.uIdTicket == entReplicaTicket.uIdTicket);

                if (dbmodel != null) {
                    dbmodel.bCancelada = entReplicaTicket.bCancelada;

                    _dbContext.Attach(dbmodel);
                    _dbContext.Entry(dbmodel).Property(x => x.bCancelada).IsModified = true;

                    var exec = await _dbContext.SaveChangesAsync();

                    if (exec > 0)
                    {
                        response.SetSuccess(true, Menssages.DatUpdateSucces);
                    }
                    else
                    {
                        response.ErrorCode = metodo.iCodigoError;
                        response.SetError(Menssages.DatUpdateFailed);
                        _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError,
                            $"Error en {metodo.sNombre}({metodo.sParametros}): {Menssages.DatUpdateSucces}", entReplicaTicket, response));
                    }
                }
                else
                {
                    response.SetNotFound(false,Menssages.DatNoGetRegister);
                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = 500;
                response.SetError(ex);
                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo.sNombre}{metodo.sParametros}: {ex.Message}", entReplicaTicket, ex, response));
            }
            return response;
        }

        [IMDMetodo(67823465982265, 67823465983042)]
        public async Task<IMDResponse<bool>> DUpdateUsado(EntReplicaUpdateTicket entReplicaUpdateTicket)
        {
            IMDResponse<bool> response = new IMDResponse<bool>();

            IMDMetodo metodo = MethodBase.GetCurrentMethod().GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo.sNombre}{metodo.sParametros}"));
            try
            {
                var dbmodel = await _dbContext.Ticket.FirstOrDefaultAsync(i => i.uIdTicket == entReplicaUpdateTicket.uIdTicket 
                && i.uIdMonedero == entReplicaUpdateTicket.uIdMonedero && i.claveApp == entReplicaUpdateTicket.claveApp);

                if (dbmodel != null)
                {
                    dbmodel.bUsado = true;

                    _dbContext.Attach(dbmodel);
                    _dbContext.Entry(dbmodel).Property(x => x.bUsado).IsModified = true;

                    var exec = await _dbContext.SaveChangesAsync();

                    if (exec > 0)
                    {
                        response.SetSuccess(true, Menssages.DatUpdateSucces);
                    }
                    else
                    {
                        response.ErrorCode = metodo.iCodigoError;
                        response.SetError(Menssages.DatUpdateFailed);
                        _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError,
                            $"Error en {metodo.sNombre}({metodo.sParametros}): {Menssages.DatUpdateSucces}", entReplicaUpdateTicket, response));
                    }
                }
                else
                {
                    response.SetNotFound(false, Menssages.DatNoGetRegister);
                }
            }
            catch (Exception ex)
            {
                response.ErrorCode = 500;
                response.SetError(ex);
                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo.sNombre}{metodo.sParametros}: {ex.Message}", entReplicaUpdateTicket, ex, response));
            }
            return response;
        }

        [IMDMetodo(67823465988481, 67823465989258)]
        public async Task<IMDResponse<List<EntTicketResponse>>> DGetListadoTickets(EntConsultaTickets entConsultaTickets)
        {
            IMDResponse<List<EntTicketResponse>> response = new IMDResponse<List<EntTicketResponse>>();
            IMDMetodo metodo = MethodBase.GetCurrentMethod().GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo.sNombre}{metodo.sParametros}"));
            try
            {
                var query = _dbContext.Ticket.AsQueryable();

                if (entConsultaTickets.sOpcion == eOpcionesTicket.Listar)
                {
                    _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Opción Listar"));
                    query = query.Where(w => w.uIdMonedero == entConsultaTickets.uIdMonedero && w.claveApp == entConsultaTickets.claveApp
                                    && w.uIdSolicitud == entConsultaTickets.uIdSolicitud && w.bUsado == entConsultaTickets.bUsado && w.bVigente == entConsultaTickets.bVigente);
                }
                else if (entConsultaTickets.sOpcion == eOpcionesTicket.Regenerar)
                {
                    _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Opción Regenerar"));
                    query = query.Where(w => w.uIdMonedero == entConsultaTickets.uIdMonedero && w.claveApp == entConsultaTickets.claveApp
                                    && w.uIdSolicitud == entConsultaTickets.uIdSolicitud && w.bUsado == entConsultaTickets.bUsado);
                }

                query = query.Skip(entConsultaTickets.skip).Take(entConsultaTickets.take).OrderBy(x => x.uIdTicket);

                List<Ticket> lstTickets = new List<Ticket>();
                lstTickets = query.ToList();

                _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Total de registros: {lstTickets.Count}"));

                List<EntTicketResponse> lstTicketsResponse = new List<EntTicketResponse>();
                lstTicketsResponse = map(lstTickets);

                response.SetSuccess(lstTicketsResponse, Menssages.DatGetSuccessInfo);
            }
            catch (Exception ex)
            {
                response.ErrorCode = 500;
                response.SetError(ex);
                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo.sNombre}{metodo.sParametros}: {ex.Message}", entConsultaTickets, ex, response));
            }
            return response;
        }

        [IMDMetodo(67823466143881, 67823466144658)]
        public async Task<IMDResponse<int>> DGetCountTickets(EntConsultaTickets entConsultaTickets)
        {
            IMDResponse<int> response = new IMDResponse<int>();
            IMDMetodo metodo = MethodBase.GetCurrentMethod().GetIMDMetodo();
            _logger.LogInformation(IMDSerializer.Serialize(metodo.iCodigoInformacion, $"Inicia {metodo.sNombre}{metodo.sParametros}", entConsultaTickets));
            try
            {
                int count = await _dbContext.Ticket.CountAsync(w => w.uIdMonedero == entConsultaTickets.uIdMonedero && w.claveApp == entConsultaTickets.claveApp
                                && w.uIdSolicitud == entConsultaTickets.uIdSolicitud && w.bUsado == entConsultaTickets.bUsado);

                response.SetSuccess(count, Menssages.DatGetSuccessInfo);
            }
            catch (Exception ex)
            {
                response.ErrorCode = 500;
                response.SetError(ex);
                _logger.LogError(IMDSerializer.Serialize(metodo.iCodigoError, $"Error en {metodo.sNombre}{metodo.sParametros}: {ex.Message}", entConsultaTickets, ex, response));
            }
            return response;
        }

        private List<EntTicketResponse> map(List<Ticket> tickets)
        {
            List<EntTicketResponse> lstEntTicketResponse = new List<EntTicketResponse>();
            foreach (var ticket in tickets)
            {
                EntTicketResponse entTicketResponse = _mapper.Map<EntTicketResponse>(ticket);
                try
                {
                    entTicketResponse.qr = JsonConvert.DeserializeObject<HSMQRDynamicResponse>(IMDParser.Base64Decode(entTicketResponse.FirmaHSM));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString());
                }
                lstEntTicketResponse.Add(entTicketResponse);
            }
            return lstEntTicketResponse;
        }
    }
}
