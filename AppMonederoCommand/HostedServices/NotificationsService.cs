namespace AppMonederoCommand.Api.HostedServices
{
    public class NotificationsService : BackgroundService
    {
        private readonly ILogger<NotificationsService> _logger;
        private readonly IMDRabbitNotifications _rabbitNotifications;
        private readonly ExchangeConfig _exchangeConfig;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly string ServiceName = Environment.GetEnvironmentVariable("SERVICE_NAME") ?? "SERVICE";
        private readonly string _NotificationService = Environment.GetEnvironmentVariable("NOTIFICATION_SERVICE_ENABLE") ?? "0";
        private IBusParametros _busParametros;
        private IBusTarjetas _busTarjetas;
        private IServiceProvider _serviceProvider;
        public bool IsConnected { get; internal set; }
        private bool hasSubscribed = false;
        public bool IsEnabled { get; internal set; }

        public NotificationsService(ILogger<NotificationsService> logger, 
            IMDRabbitNotifications rabbitNotifications, ExchangeConfig exchangeConfig, 
            IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _rabbitNotifications = rabbitNotifications;
            _exchangeConfig = exchangeConfig;
            _serviceScopeFactory = serviceScopeFactory;

            IsEnabled = _NotificationService == "1" ? true : false;
            IsConnected = _rabbitNotifications != null && _rabbitNotifications.IsConnected;

            if (!IsEnabled)
            {
                _logger.LogWarning($"{ServiceName} is desabled");
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                _rabbitNotifications.OnReconnectEmitter += OnReconnect;

                _ = Task.Run(() =>
                    {
                        while (!stoppingToken.IsCancellationRequested)
                        {
                            if (IsConnected && IsEnabled && !hasSubscribed)
                            {
                                SuscribeQueue();

                                hasSubscribed = true;
                            }
                            else if (IsConnected && !IsEnabled && hasSubscribed)
                            {
                                _logger.LogInformation($"{ServiceName} not listening queues...");
                            }
                        }
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ServiceName} : {ex.Message}");
            }
        }

        private void OnReconnect(object? sender, IMD.Utils.RabbitMQ.ReconnectEventArgs e)
        {
            hasSubscribed = false;
        }

        private void SuscribeQueue()
        {
            try
            {
                _logger.LogInformation($"{ServiceName} is listening queues...");

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    _serviceProvider = scope.ServiceProvider;
                    _busParametros = _serviceProvider.GetRequiredService<IBusParametros>();
                    _busParametros.SetParametros();
                    _busParametros.SetListadosAdicionales();

                }

                _rabbitNotifications.ReceiveAsync<QueueMessage<EntUpdateUsuarioByMonedero>>(RoutingKeys.AppUpdateUsuarioByMonedero.GetDescription(), _exchangeConfig, async x =>
                {
                    _logger.LogInformation($"MessageId: {x.uMessageId}, SourceSystem: {x.sSourceSystem}, Timestamp: {x.dtTimestamp}");
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        IBusUsuariosWeb _busUsuariosWeb = scope.ServiceProvider.GetRequiredService<IBusUsuariosWeb>();
                        var result = await _busUsuariosWeb.BUpdateUsuarioByMonedero(x.Content);
                    }
                });

                #region Monedero
                _rabbitNotifications.ReceiveAsync<QueueMessage<EntCreateReplicaMonederos>>(RoutingKeys.MonederoCreacion.GetDescription(), _exchangeConfig, async x =>
                {
                    _logger.LogInformation($"MessageId: {x.uMessageId}, SourceSystem: {x.sSourceSystem}, Timestamp: {x.dtTimestamp}");
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        IBusMonedero _busMonedero = scope.ServiceProvider.GetRequiredService<IBusMonedero>();

                        _busMonedero = scope.ServiceProvider.GetRequiredService<IBusMonedero>();
                        await _busMonedero.BMonederoCreacion(x.Content);
                    }
                });

                _rabbitNotifications.ReceiveAsync<QueueMessage<EntUpdateReplicaEstatusMonedero>>(RoutingKeys.MonederoEstatus.GetDescription(), _exchangeConfig, async x =>
                {
                    _logger.LogInformation($"MessageId: {x.uMessageId}, SourceSystem: {x.sSourceSystem}, Timestamp: {x.dtTimestamp}, Canal: {RoutingKeys.MonederoEstatus.GetDescription()}");
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        IDatMonedero _datEstadoCuentas = scope.ServiceProvider.GetRequiredService<IDatMonedero>();

                        var list = new List<EntEstadoDeCuenta>();

                        foreach (var item in x.Content.IdMonedero)
                        {
                            var dbmodel = await _datEstadoCuentas.DGetByIdMonedero(item);

                            if (dbmodel.Result != null)
                            {
                                dbmodel.Result.uIdEstatus = x.Content.IdEstatus;
                                dbmodel.Result.sEstatus = x.Content.Estatus;
                                dbmodel.Result.bBaja = x.Content.Baja;
                                dbmodel.Result.bActivo = !x.Content.Baja;
                                dbmodel.Result.uIdMotivo = x.Content.IdMotivo;

                                if (x.Content.Baja)
                                    dbmodel.Result.dtFechaBaja = DateTime.Now;

                                list.Add(dbmodel.Result);
                            }
                            await _datEstadoCuentas.DUpdate(list);
                        }

                    }
                });

                _rabbitNotifications.ReceiveAsync<QueueMessage<EntReplicaUpdateMonederos>>(RoutingKeys.MonederoActualizacion.GetDescription(), _exchangeConfig, async x =>
                {
                    _logger.LogInformation($"MessageId: {x.uMessageId}, SourceSystem: {x.sSourceSystem}, Timestamp: {x.dtTimestamp}, Canal: {RoutingKeys.MonederoActualizacion.GetDescription()}");
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        IDatMonedero _datEstadoCuentas = scope.ServiceProvider.GetRequiredService<IDatMonedero>();
                        var dbmodel = await _datEstadoCuentas.DGetByIdMonedero(x.Content.IdMonedero);

                        dbmodel.Result.uIdTipoTarifa = x.Content.IdTipoTarifa.HasValue ? x.Content.IdTipoTarifa.Value : dbmodel.Result.uIdTipoTarifa;
                        dbmodel.Result.sTipoTarifa = !string.IsNullOrEmpty(x.Content.TipoTarifa) ? x.Content.TipoTarifa : dbmodel.Result.sTipoTarifa;
                      
                        await _datEstadoCuentas.DUpdate(dbmodel.Result);
                    }
                });

                #endregion

                #region Movimientos Monedero

                _rabbitNotifications.ReceiveAsync<QueueMessage<EntAbonoCargoMovimiento>>(RoutingKeys.MonederoAbono.GetDescription(), _exchangeConfig, async x =>
                {
                    _logger.LogInformation($"MessageId: {x.uMessageId}, SourceSystem: {x.sSourceSystem}, Timestamp: {x.dtTimestamp}, Canal: {RoutingKeys.MonederoAbono.GetDescription()}");
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {

                        _logger.LogInformation($"Rabbit Events {x.Content}");
                        IDatMonedero _datEstadoCuentas = scope.ServiceProvider.GetRequiredService<IDatMonedero>();

                        var dbmodelEdoCuenta = await _datEstadoCuentas.DGetByIdMonedero(x.Content.Movimientos.IdMonedero);
                        _logger.LogInformation($"Rabbit Events get {dbmodelEdoCuenta}");

                        dbmodelEdoCuenta.Result.uIdUltimaOperacion = x.Content.EstadoCuenta.IdUltimaOperacion;

                        if (x.Content.EstadoCuenta.Total >= 0)
                            dbmodelEdoCuenta.Result.dtFechaUltimoAbono = x.Content.EstadoCuenta.FechaUltimoAbono;

                        dbmodelEdoCuenta.Result.dtFechaUltimaOperacion = x.Content.EstadoCuenta.FechaUltimaOperacion;
                        decimal SaldoAnt = dbmodelEdoCuenta.Result.dSaldo;
                        dbmodelEdoCuenta.Result.dSaldo += x.Content.EstadoCuenta.Total;
                        dbmodelEdoCuenta.Result.uIdMonedero = x.Content.Movimientos.IdMonedero;

                         await _datEstadoCuentas.DUpdate(dbmodelEdoCuenta.Result);
                     }
                });

                _rabbitNotifications.ReceiveAsync<QueueMessage<EntAbonoCargoMovimiento>>(RoutingKeys.MonederoDebito.GetDescription(), _exchangeConfig, async x =>
                {
                    _logger.LogInformation($"MessageId: {x.uMessageId}, SourceSystem: {x.sSourceSystem}, Timestamp: {x.dtTimestamp}, Canal: {RoutingKeys.MonederoDebito.GetDescription()}");
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        IDatMonedero _datEstadoCuentas = scope.ServiceProvider.GetRequiredService<IDatMonedero>();
                        var dbmodelEdoCuenta = await _datEstadoCuentas.DGetByIdMonedero(x.Content.Movimientos.IdMonedero);

                        dbmodelEdoCuenta.Result.uIdUltimaOperacion = x.Content.EstadoCuenta.IdUltimaOperacion;

                        if (x.Content.EstadoCuenta.Total >= 0)
                            dbmodelEdoCuenta.Result.dtFechaUltimoAbono = x.Content.EstadoCuenta.FechaUltimoAbono;

                        dbmodelEdoCuenta.Result.dtFechaUltimaOperacion = x.Content.EstadoCuenta.FechaUltimaOperacion;
                        dbmodelEdoCuenta.Result.dSaldo += x.Content.EstadoCuenta.Total;
                        dbmodelEdoCuenta.Result.uIdMonedero = x.Content.Movimientos.IdMonedero;

                        await _datEstadoCuentas.DUpdate(dbmodelEdoCuenta.Result);
                    }
                });

                #endregion

                #region TipoTarifas
                _rabbitNotifications.ReceiveAsync<QueueMessage<EntReplicaTipoTarifas>>(RoutingKeys.TipoTarifasCreacion.GetDescription(), _exchangeConfig, async x =>
                {
                    _logger.LogInformation($"MessageId: {x.uMessageId}, SourceSystem: {x.sSourceSystem}, Timestamp: {x.dtTimestamp}");
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        IDatTipoTarifa _datTipoTarifas = scope.ServiceProvider.GetRequiredService<IDatTipoTarifa>();

                        await _datTipoTarifas.DSave(x.Content);
                    };
                });

                _rabbitNotifications.ReceiveAsync<QueueMessage<Guid>>(RoutingKeys.TipoTarifasBorrado.GetDescription(), _exchangeConfig, async x =>
                {
                    _logger.LogInformation($"MessageId: {x.uMessageId}, SourceSystem: {x.sSourceSystem}, Timestamp: {x.dtTimestamp}");
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        IDatTipoTarifa _datTipoTarifas = scope.ServiceProvider.GetRequiredService<IDatTipoTarifa>();
                        await _datTipoTarifas.DDelete(x.Content);
                    };
                });

                _rabbitNotifications.ReceiveAsync<QueueMessage<EntReplicaTipoTarifas>>(RoutingKeys.TipoTarifasActualizacion.GetDescription(), _exchangeConfig, async x =>
                {
                    _logger.LogInformation($"MessageId: {x.uMessageId}, SourceSystem: {x.sSourceSystem}, Timestamp: {x.dtTimestamp}");
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        IDatTipoTarifa _datTipoTarifas = scope.ServiceProvider.GetRequiredService<IDatTipoTarifa>();

                        await _datTipoTarifas.DUpdate(x.Content);
                    };
                });
                #endregion

                #region Motivos
                _rabbitNotifications.ReceiveAsync<QueueMessage<EntMotivo>>(RoutingKeys.MotivosCreacion200.GetDescription(), _exchangeConfig, async x =>
                {
                    _logger.LogInformation($"MessageId: {x.uMessageId}, SourceSystem: {x.sSourceSystem}, Timestamp: {x.dtTimestamp}");
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        IBusMotivos _busMotivos = scope.ServiceProvider.GetRequiredService<IBusMotivos>();
                        _logger.LogInformation(IMDSerializer.Serialize(x.Content));

                        await _busMotivos.BAgregar(x.Content);
                    };
                });

                _rabbitNotifications.ReceiveAsync<QueueMessage<EntMotivo>>(RoutingKeys.MotivosActualizacion200.GetDescription(), _exchangeConfig, async x =>
                {
                    _logger.LogInformation($"MessageId: {x.uMessageId}, SourceSystem: {x.sSourceSystem}, Timestamp: {x.dtTimestamp}");
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        IBusMotivos _busMotivos = scope.ServiceProvider.GetRequiredService<IBusMotivos>();
                        _logger.LogInformation(IMDSerializer.Serialize(x.Content));
                        await _busMotivos.BActualizar(x.Content);
                    };
                });

                _rabbitNotifications.ReceiveAsync<QueueMessage<Guid>>(RoutingKeys.MotivosBorrado200.GetDescription(), _exchangeConfig, async x =>
               {
                   _logger.LogInformation($"MessageId: {x.uMessageId}, SourceSystem: {x.sSourceSystem}, Timestamp: {x.dtTimestamp}");
                   using (var scope = _serviceScopeFactory.CreateScope())
                   {
                       IBusMotivos _busMotivos = scope.ServiceProvider.GetRequiredService<IBusMotivos>();
                       _logger.LogInformation(IMDSerializer.Serialize(x.Content));
                       await _busMotivos.BEliminar(x.Content);
                   };
               });

                _rabbitNotifications.ReceiveAsync<QueueMessage<EntMotivo>>(RoutingKeys.MotivosCreacion500.GetDescription(), _exchangeConfig, async x =>
                 {
                     _logger.LogInformation($"MessageId: {x.uMessageId}, SourceSystem: {x.sSourceSystem}, Timestamp: {x.dtTimestamp}");
                     using (var scope = _serviceScopeFactory.CreateScope())
                     {
                         IBusMotivos _busMotivos = scope.ServiceProvider.GetRequiredService<IBusMotivos>();

                         await _busMotivos.BAgregar(x.Content);
                     };
                 });

                _rabbitNotifications.ReceiveAsync<QueueMessage<EntMotivo>>(RoutingKeys.MotivosActualizacion500.GetDescription(), _exchangeConfig, async x =>
                {
                    _logger.LogInformation($"MessageId: {x.uMessageId}, SourceSystem: {x.sSourceSystem}, Timestamp: {x.dtTimestamp}");
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        IBusMotivos _busMotivos = scope.ServiceProvider.GetRequiredService<IBusMotivos>();

                        await _busMotivos.BActualizar(x.Content);
                    };
                });

                _rabbitNotifications.ReceiveAsync<QueueMessage<Guid>>(RoutingKeys.MotivosBorrado500.GetDescription(), _exchangeConfig, async x =>
               {
                   _logger.LogInformation($"MessageId: {x.uMessageId}, SourceSystem: {x.sSourceSystem}, Timestamp: {x.dtTimestamp}");
                   using (var scope = _serviceScopeFactory.CreateScope())
                   {
                       IBusMotivos _busMotivos = scope.ServiceProvider.GetRequiredService<IBusMotivos>();
                       await _busMotivos.BEliminar(x.Content);
                   };
               });
                #endregion

                #region Tickets
                _rabbitNotifications.ReceiveAsync<QueueMessage<EntReplicaTicket>>(RoutingKeys.TicketCreacion.GetDescription(), _exchangeConfig, async x =>
                {
                    _logger.LogInformation($"MessageId: {x.uMessageId}, SourceSystem: {x.sSourceSystem}, Timestamp: {x.dtTimestamp}");
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        IDatTicket _datTicket = scope.ServiceProvider.GetRequiredService<IDatTicket>();
                        await _datTicket.DSave(x.Content);
                    };
                });

                _rabbitNotifications.ReceiveAsync<QueueMessage<EntReplicaTicket>>(RoutingKeys.TicketModificacion.GetDescription(), _exchangeConfig, async x =>
                {
                    _logger.LogInformation($"MessageId: {x.uMessageId}, SourceSystem: {x.sSourceSystem}, Timestamp: {x.dtTimestamp}");
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        IDatTicket _datTicket = scope.ServiceProvider.GetRequiredService<IDatTicket>();
                        await _datTicket.DUpdateCancelado(x.Content);
                    };
                });

                _rabbitNotifications.ReceiveAsync<QueueMessage<EntReplicaUpdateTicket>>(RoutingKeys.TicketUsadoApp.GetDescription(), _exchangeConfig, async x =>
                {
                    _logger.LogInformation($"MessageId: {x.uMessageId}, SourceSystem: {x.sSourceSystem}, Timestamp: {x.dtTimestamp}");
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        IDatTicket _datTicket = scope.ServiceProvider.GetRequiredService<IDatTicket>();
                        await _datTicket.DUpdateUsado(x.Content);
                    };
                });
                #endregion

                #region TipoOperaciones

                #region TipoOperacionesComercios
                _rabbitNotifications.ReceiveAsync<QueueMessage<EntTipoOperaciones>>(RoutingKeys.TipoOperacionesComercioCreacion.GetDescription(), _exchangeConfig, async x =>
                {
                    _logger.LogInformation($"MessageId: {x.uMessageId}, SourceSystem: {x.sSourceSystem}, Timestamp: {x.dtTimestamp}");
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        IDatTipoOperaciones _datTipoOperaciones = scope.ServiceProvider.GetRequiredService<IDatTipoOperaciones>();

                        await _datTipoOperaciones.DAgregar(x.Content);
                    };
                });

                _rabbitNotifications.ReceiveAsync<QueueMessage<EntTipoOperaciones>>(RoutingKeys.TipoOperacionesComercioActualizacion.GetDescription(), _exchangeConfig, async x =>
                {
                    _logger.LogInformation($"MessageId: {x.uMessageId}, SourceSystem: {x.sSourceSystem}, Timestamp: {x.dtTimestamp}");
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        IDatTipoOperaciones _datTipoOperaciones = scope.ServiceProvider.GetRequiredService<IDatTipoOperaciones>();

                        await _datTipoOperaciones.DActualizar(x.Content);
                    };
                });

                _rabbitNotifications.ReceiveAsync<QueueMessage<Guid>>(RoutingKeys.TipoOperacionesComercioBorrado.GetDescription(), _exchangeConfig, async x =>
                {
                    _logger.LogInformation($"MessageId: {x.uMessageId}, SourceSystem: {x.sSourceSystem}, Timestamp: {x.dtTimestamp}");
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        IDatTipoOperaciones _datTipoOperaciones = scope.ServiceProvider.GetRequiredService<IDatTipoOperaciones>();
                        await _datTipoOperaciones.DEliminar(x.Content);
                    };
                });
                #endregion

                #region TipoOperacionesMonederos
                _rabbitNotifications.ReceiveAsync<QueueMessage<EntTipoOperaciones>>(RoutingKeys.TipoOperacionesMonederosCreacion.GetDescription(), _exchangeConfig, async x =>
                {
                    _logger.LogInformation($"MessageId: {x.uMessageId}, SourceSystem: {x.sSourceSystem}, Timestamp: {x.dtTimestamp}");
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        IDatTipoOperaciones _datTipoOperaciones = scope.ServiceProvider.GetRequiredService<IDatTipoOperaciones>();

                        await _datTipoOperaciones.DAgregar(x.Content);
                    };
                });

                _rabbitNotifications.ReceiveAsync<QueueMessage<EntTipoOperaciones>>(RoutingKeys.TipoOperacionesMonederosActualizacion.GetDescription(), _exchangeConfig, async x =>
                {
                    _logger.LogInformation($"MessageId: {x.uMessageId}, SourceSystem: {x.sSourceSystem}, Timestamp: {x.dtTimestamp}");
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        IDatTipoOperaciones _datTipoOperaciones = scope.ServiceProvider.GetRequiredService<IDatTipoOperaciones>();

                        await _datTipoOperaciones.DActualizar(x.Content);
                    };
                });

                _rabbitNotifications.ReceiveAsync<QueueMessage<Guid>>(RoutingKeys.TipoOperacionesMonederosBorrado.GetDescription(), _exchangeConfig, async x =>
                {
                    _logger.LogInformation($"MessageId: {x.uMessageId}, SourceSystem: {x.sSourceSystem}, Timestamp: {x.dtTimestamp}");
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        IDatTipoOperaciones _datTipoOperaciones = scope.ServiceProvider.GetRequiredService<IDatTipoOperaciones>();
                        await _datTipoOperaciones.DEliminar(x.Content);
                    };
                });
                #endregion

                #region TipoOperacionesApp
                _rabbitNotifications.ReceiveAsync<QueueMessage<EntTipoOperaciones>>(RoutingKeys.TipoOperacionesAppCreacion.GetDescription(), _exchangeConfig, async x =>
                {
                    _logger.LogInformation($"MessageId: {x.uMessageId}, SourceSystem: {x.sSourceSystem}, Timestamp: {x.dtTimestamp}");
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        IDatTipoOperaciones _datTipoOperaciones = scope.ServiceProvider.GetRequiredService<IDatTipoOperaciones>();

                        await _datTipoOperaciones.DAgregar(x.Content);
                    };
                });

                _rabbitNotifications.ReceiveAsync<QueueMessage<EntTipoOperaciones>>(RoutingKeys.TipoOperacionesAppActualizacion.GetDescription(), _exchangeConfig, async x =>
                {
                    _logger.LogInformation($"MessageId: {x.uMessageId}, SourceSystem: {x.sSourceSystem}, Timestamp: {x.dtTimestamp}");
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        IDatTipoOperaciones _datTipoOperaciones = scope.ServiceProvider.GetRequiredService<IDatTipoOperaciones>();

                        await _datTipoOperaciones.DActualizar(x.Content);
                    };
                });

                _rabbitNotifications.ReceiveAsync<QueueMessage<Guid>>(RoutingKeys.TipoOperacionesAppBorrado.GetDescription(), _exchangeConfig, async x =>
                {
                    _logger.LogInformation($"MessageId: {x.uMessageId}, SourceSystem: {x.sSourceSystem}, Timestamp: {x.dtTimestamp}");
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        IDatTipoOperaciones _datTipoOperaciones = scope.ServiceProvider.GetRequiredService<IDatTipoOperaciones>();
                        await _datTipoOperaciones.DEliminar(x.Content);
                    };
                });
                #endregion

                #region TipoOperacionesConfiguracion
                _rabbitNotifications.ReceiveAsync<QueueMessage<EntTipoOperaciones>>(RoutingKeys.TipoOperacionesConfiguracionCreacion.GetDescription(), _exchangeConfig, async x =>
                {
                    _logger.LogInformation($"MessageId: {x.uMessageId}, SourceSystem: {x.sSourceSystem}, Timestamp: {x.dtTimestamp}");
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        IDatTipoOperaciones _datTipoOperaciones = scope.ServiceProvider.GetRequiredService<IDatTipoOperaciones>();

                        await _datTipoOperaciones.DAgregar(x.Content);
                    };
                });

                _rabbitNotifications.ReceiveAsync<QueueMessage<EntTipoOperaciones>>(RoutingKeys.TipoOperacionesConfiguracionActualizacion.GetDescription(), _exchangeConfig, async x =>
                {
                    _logger.LogInformation($"MessageId: {x.uMessageId}, SourceSystem: {x.sSourceSystem}, Timestamp: {x.dtTimestamp}");
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        IDatTipoOperaciones _datTipoOperaciones = scope.ServiceProvider.GetRequiredService<IDatTipoOperaciones>();

                        await _datTipoOperaciones.DActualizar(x.Content);
                    };
                });

                _rabbitNotifications.ReceiveAsync<QueueMessage<Guid>>(RoutingKeys.TipoOperacionesConfiguracionBorrado.GetDescription(), _exchangeConfig, async x =>
                {
                    _logger.LogInformation($"MessageId: {x.uMessageId}, SourceSystem: {x.sSourceSystem}, Timestamp: {x.dtTimestamp}");
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        IDatTipoOperaciones _datTipoOperaciones = scope.ServiceProvider.GetRequiredService<IDatTipoOperaciones>();
                        await _datTipoOperaciones.DEliminar(x.Content);
                    };
                });
                #endregion

                #region TipoOperacionesCredencializacion
                _rabbitNotifications.ReceiveAsync<QueueMessage<EntTipoOperaciones>>(RoutingKeys.TipoOperacionesCredencializacionCreacion.GetDescription(), _exchangeConfig, async x =>
                {
                    _logger.LogInformation($"MessageId: {x.uMessageId}, SourceSystem: {x.sSourceSystem}, Timestamp: {x.dtTimestamp}");
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        IDatTipoOperaciones _datTipoOperaciones = scope.ServiceProvider.GetRequiredService<IDatTipoOperaciones>();

                        await _datTipoOperaciones.DAgregar(x.Content);
                    };
                });

                _rabbitNotifications.ReceiveAsync<QueueMessage<EntTipoOperaciones>>(RoutingKeys.TipoOperacionesCredencializacionActualizacion.GetDescription(), _exchangeConfig, async x =>
                {
                    _logger.LogInformation($"MessageId: {x.uMessageId}, SourceSystem: {x.sSourceSystem}, Timestamp: {x.dtTimestamp}");
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        IDatTipoOperaciones _datTipoOperaciones = scope.ServiceProvider.GetRequiredService<IDatTipoOperaciones>();

                        await _datTipoOperaciones.DActualizar(x.Content);
                    };
                });

                _rabbitNotifications.ReceiveAsync<QueueMessage<Guid>>(RoutingKeys.TipoOperacionesCredencializacionBorrado.GetDescription(), _exchangeConfig, async x =>
                {
                    _logger.LogInformation($"MessageId: {x.uMessageId}, SourceSystem: {x.sSourceSystem}, Timestamp: {x.dtTimestamp}");
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        IDatTipoOperaciones _datTipoOperaciones = scope.ServiceProvider.GetRequiredService<IDatTipoOperaciones>();
                        await _datTipoOperaciones.DEliminar(x.Content);
                    };
                });
                #endregion

                #region TipoOperacionesListaNegra
                _rabbitNotifications.ReceiveAsync<QueueMessage<EntTipoOperaciones>>(RoutingKeys.TipoOperacionesListaNegraCreacion.GetDescription(), _exchangeConfig, async x =>
                {
                    _logger.LogInformation($"MessageId: {x.uMessageId}, SourceSystem: {x.sSourceSystem}, Timestamp: {x.dtTimestamp}");
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        IDatTipoOperaciones _datTipoOperaciones = scope.ServiceProvider.GetRequiredService<IDatTipoOperaciones>();

                        await _datTipoOperaciones.DAgregar(x.Content);
                    };
                });

                _rabbitNotifications.ReceiveAsync<QueueMessage<EntTipoOperaciones>>(RoutingKeys.TipoOperacionesListaNegraActualizacion.GetDescription(), _exchangeConfig, async x =>
                {
                    _logger.LogInformation($"MessageId: {x.uMessageId}, SourceSystem: {x.sSourceSystem}, Timestamp: {x.dtTimestamp}");
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        IDatTipoOperaciones _datTipoOperaciones = scope.ServiceProvider.GetRequiredService<IDatTipoOperaciones>();

                        await _datTipoOperaciones.DActualizar(x.Content);
                    };
                });

                _rabbitNotifications.ReceiveAsync<QueueMessage<Guid>>(RoutingKeys.TipoOperacionesListaNegraBorrado.GetDescription(), _exchangeConfig, async x =>
                {
                    _logger.LogInformation($"MessageId: {x.uMessageId}, SourceSystem: {x.sSourceSystem}, Timestamp: {x.dtTimestamp}");
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        IDatTipoOperaciones _datTipoOperaciones = scope.ServiceProvider.GetRequiredService<IDatTipoOperaciones>();
                        await _datTipoOperaciones.DEliminar(x.Content);
                    };
                });
                #endregion

                #region TipoOperacionesOpenLoop
                _rabbitNotifications.ReceiveAsync<QueueMessage<EntTipoOperaciones>>(RoutingKeys.TipoOperacionesOpenLoopCreacion.GetDescription(), _exchangeConfig, async x =>
                {
                    _logger.LogInformation($"MessageId: {x.uMessageId}, SourceSystem: {x.sSourceSystem}, Timestamp: {x.dtTimestamp}");
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        IDatTipoOperaciones _datTipoOperaciones = scope.ServiceProvider.GetRequiredService<IDatTipoOperaciones>();

                        await _datTipoOperaciones.DAgregar(x.Content);
                    };
                });

                _rabbitNotifications.ReceiveAsync<QueueMessage<EntTipoOperaciones>>(RoutingKeys.TipoOperacionesOpenLoopActualizacion.GetDescription(), _exchangeConfig, async x =>
                {
                    _logger.LogInformation($"MessageId: {x.uMessageId}, SourceSystem: {x.sSourceSystem}, Timestamp: {x.dtTimestamp}");
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        IDatTipoOperaciones _datTipoOperaciones = scope.ServiceProvider.GetRequiredService<IDatTipoOperaciones>();

                        await _datTipoOperaciones.DActualizar(x.Content);
                    };
                });

                _rabbitNotifications.ReceiveAsync<QueueMessage<Guid>>(RoutingKeys.TipoOperacionesOpenLoopBorrado.GetDescription(), _exchangeConfig, async x =>
                {
                    _logger.LogInformation($"MessageId: {x.uMessageId}, SourceSystem: {x.sSourceSystem}, Timestamp: {x.dtTimestamp}");
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        IDatTipoOperaciones _datTipoOperaciones = scope.ServiceProvider.GetRequiredService<IDatTipoOperaciones>();
                        await _datTipoOperaciones.DEliminar(x.Content);
                    };
                });
                #endregion

                #region TipoOperacionesNotificaciones
                _rabbitNotifications.ReceiveAsync<QueueMessage<EntTipoOperaciones>>(RoutingKeys.TipoOperacionesNotificacionesCreacion.GetDescription(), _exchangeConfig, async x =>
                {
                    _logger.LogInformation($"MessageId: {x.uMessageId}, SourceSystem: {x.sSourceSystem}, Timestamp: {x.dtTimestamp}");
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        IDatTipoOperaciones _datTipoOperaciones = scope.ServiceProvider.GetRequiredService<IDatTipoOperaciones>();

                        await _datTipoOperaciones.DAgregar(x.Content);
                    };
                });

                _rabbitNotifications.ReceiveAsync<QueueMessage<EntTipoOperaciones>>(RoutingKeys.TipoOperacionesNotificacionesActualizacion.GetDescription(), _exchangeConfig, async x =>
                {
                    _logger.LogInformation($"MessageId: {x.uMessageId}, SourceSystem: {x.sSourceSystem}, Timestamp: {x.dtTimestamp}");
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        IDatTipoOperaciones _datTipoOperaciones = scope.ServiceProvider.GetRequiredService<IDatTipoOperaciones>();

                        await _datTipoOperaciones.DActualizar(x.Content);
                    };
                });

                _rabbitNotifications.ReceiveAsync<QueueMessage<Guid>>(RoutingKeys.TipoOperacionesNotificacionesBorrado.GetDescription(), _exchangeConfig, async x =>
                {
                    _logger.LogInformation($"MessageId: {x.uMessageId}, SourceSystem: {x.sSourceSystem}, Timestamp: {x.dtTimestamp}");
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        IDatTipoOperaciones _datTipoOperaciones = scope.ServiceProvider.GetRequiredService<IDatTipoOperaciones>();
                        await _datTipoOperaciones.DEliminar(x.Content);
                    };
                });
                #endregion

                #region TipoOperacionesPagos
                _rabbitNotifications.ReceiveAsync<QueueMessage<EntTipoOperaciones>>(RoutingKeys.TipoOperacionesPagosCreacion.GetDescription(), _exchangeConfig, async x =>
                {
                    _logger.LogInformation($"MessageId: {x.uMessageId}, SourceSystem: {x.sSourceSystem}, Timestamp: {x.dtTimestamp}");
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        IDatTipoOperaciones _datTipoOperaciones = scope.ServiceProvider.GetRequiredService<IDatTipoOperaciones>();

                        await _datTipoOperaciones.DAgregar(x.Content);
                    };
                });

                _rabbitNotifications.ReceiveAsync<QueueMessage<EntTipoOperaciones>>(RoutingKeys.TipoOperacionesPagosActualizacion.GetDescription(), _exchangeConfig, async x =>
                {
                    _logger.LogInformation($"MessageId: {x.uMessageId}, SourceSystem: {x.sSourceSystem}, Timestamp: {x.dtTimestamp}");
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        IDatTipoOperaciones _datTipoOperaciones = scope.ServiceProvider.GetRequiredService<IDatTipoOperaciones>();

                        await _datTipoOperaciones.DActualizar(x.Content);
                    };
                });

                _rabbitNotifications.ReceiveAsync<QueueMessage<Guid>>(RoutingKeys.TipoOperacionesPagosBorrado.GetDescription(), _exchangeConfig, async x =>
                {
                    _logger.LogInformation($"MessageId: {x.uMessageId}, SourceSystem: {x.sSourceSystem}, Timestamp: {x.dtTimestamp}");
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        IDatTipoOperaciones _datTipoOperaciones = scope.ServiceProvider.GetRequiredService<IDatTipoOperaciones>();
                        await _datTipoOperaciones.DEliminar(x.Content);
                    };
                });
                #endregion

                #region TipoOperacionesSeguridad
                _rabbitNotifications.ReceiveAsync<QueueMessage<EntTipoOperaciones>>(RoutingKeys.TipoOperacionesSeguridadCreacion.GetDescription(), _exchangeConfig, async x =>
                {
                    _logger.LogInformation($"MessageId: {x.uMessageId}, SourceSystem: {x.sSourceSystem}, Timestamp: {x.dtTimestamp}");
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        IDatTipoOperaciones _datTipoOperaciones = scope.ServiceProvider.GetRequiredService<IDatTipoOperaciones>();

                        await _datTipoOperaciones.DAgregar(x.Content);
                    };
                });

                _rabbitNotifications.ReceiveAsync<QueueMessage<EntTipoOperaciones>>(RoutingKeys.TipoOperacionesSeguridadActualizacion.GetDescription(), _exchangeConfig, async x =>
                {
                    _logger.LogInformation($"MessageId: {x.uMessageId}, SourceSystem: {x.sSourceSystem}, Timestamp: {x.dtTimestamp}");
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        IDatTipoOperaciones _datTipoOperaciones = scope.ServiceProvider.GetRequiredService<IDatTipoOperaciones>();

                        await _datTipoOperaciones.DActualizar(x.Content);
                    };
                });

                _rabbitNotifications.ReceiveAsync<QueueMessage<Guid>>(RoutingKeys.TipoOperacionesSeguridadBorrado.GetDescription(), _exchangeConfig, async x =>
                {
                    _logger.LogInformation($"MessageId: {x.uMessageId}, SourceSystem: {x.sSourceSystem}, Timestamp: {x.dtTimestamp}");
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        IDatTipoOperaciones _datTipoOperaciones = scope.ServiceProvider.GetRequiredService<IDatTipoOperaciones>();
                        await _datTipoOperaciones.DEliminar(x.Content);
                    };
                });
                #endregion

                #region TipoOperacionesSincronizador
                _rabbitNotifications.ReceiveAsync<QueueMessage<EntTipoOperaciones>>(RoutingKeys.TipoOperacionesSincronizadorCreacion.GetDescription(), _exchangeConfig, async x =>
                {
                    _logger.LogInformation($"MessageId: {x.uMessageId}, SourceSystem: {x.sSourceSystem}, Timestamp: {x.dtTimestamp}");
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        IDatTipoOperaciones _datTipoOperaciones = scope.ServiceProvider.GetRequiredService<IDatTipoOperaciones>();

                        await _datTipoOperaciones.DAgregar(x.Content);
                    };
                });

                _rabbitNotifications.ReceiveAsync<QueueMessage<EntTipoOperaciones>>(RoutingKeys.TipoOperacionesSincronizadorActualizacion.GetDescription(), _exchangeConfig, async x =>
                {
                    _logger.LogInformation($"MessageId: {x.uMessageId}, SourceSystem: {x.sSourceSystem}, Timestamp: {x.dtTimestamp}");
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        IDatTipoOperaciones _datTipoOperaciones = scope.ServiceProvider.GetRequiredService<IDatTipoOperaciones>();

                        await _datTipoOperaciones.DActualizar(x.Content);
                    };
                });

                _rabbitNotifications.ReceiveAsync<QueueMessage<Guid>>(RoutingKeys.TipoOperacionesSincronizadorBorrado.GetDescription(), _exchangeConfig, async x =>
                {
                    _logger.LogInformation($"MessageId: {x.uMessageId}, SourceSystem: {x.sSourceSystem}, Timestamp: {x.dtTimestamp}");
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        IDatTipoOperaciones _datTipoOperaciones = scope.ServiceProvider.GetRequiredService<IDatTipoOperaciones>();
                        await _datTipoOperaciones.DEliminar(x.Content);
                    };
                });
                #endregion

                #region TipoOperacionesTarifas
                _rabbitNotifications.ReceiveAsync<QueueMessage<EntTipoOperaciones>>(RoutingKeys.TipoOperacionesTarifasCreacion.GetDescription(), _exchangeConfig, async x =>
                {
                    _logger.LogInformation($"MessageId: {x.uMessageId}, SourceSystem: {x.sSourceSystem}, Timestamp: {x.dtTimestamp}");
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        IDatTipoOperaciones _datTipoOperaciones = scope.ServiceProvider.GetRequiredService<IDatTipoOperaciones>();

                        await _datTipoOperaciones.DAgregar(x.Content);
                    };
                });

                _rabbitNotifications.ReceiveAsync<QueueMessage<EntTipoOperaciones>>(RoutingKeys.TipoOperacionesTarifasActualizacion.GetDescription(), _exchangeConfig, async x =>
                {
                    _logger.LogInformation($"MessageId: {x.uMessageId}, SourceSystem: {x.sSourceSystem}, Timestamp: {x.dtTimestamp}");
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        IDatTipoOperaciones _datTipoOperaciones = scope.ServiceProvider.GetRequiredService<IDatTipoOperaciones>();

                        await _datTipoOperaciones.DActualizar(x.Content);
                    };
                });

                _rabbitNotifications.ReceiveAsync<QueueMessage<Guid>>(RoutingKeys.TipoOperacionesTarifasBorrado.GetDescription(), _exchangeConfig, async x =>
                {
                    _logger.LogInformation($"MessageId: {x.uMessageId}, SourceSystem: {x.sSourceSystem}, Timestamp: {x.dtTimestamp}");
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        IDatTipoOperaciones _datTipoOperaciones = scope.ServiceProvider.GetRequiredService<IDatTipoOperaciones>();
                        await _datTipoOperaciones.DEliminar(x.Content);
                    };
                });
                #endregion

                #region TipoOperacionesTickets
                _rabbitNotifications.ReceiveAsync<QueueMessage<EntTipoOperaciones>>(RoutingKeys.TipoOperacionesTicketsCreacion.GetDescription(), _exchangeConfig, async x =>
                {
                    _logger.LogInformation($"MessageId: {x.uMessageId}, SourceSystem: {x.sSourceSystem}, Timestamp: {x.dtTimestamp}");
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        IDatTipoOperaciones _datTipoOperaciones = scope.ServiceProvider.GetRequiredService<IDatTipoOperaciones>();

                        await _datTipoOperaciones.DAgregar(x.Content);
                    };
                });

                _rabbitNotifications.ReceiveAsync<QueueMessage<EntTipoOperaciones>>(RoutingKeys.TipoOperacionesTicketsActualizacion.GetDescription(), _exchangeConfig, async x =>
                {
                    _logger.LogInformation($"MessageId: {x.uMessageId}, SourceSystem: {x.sSourceSystem}, Timestamp: {x.dtTimestamp}");
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        IDatTipoOperaciones _datTipoOperaciones = scope.ServiceProvider.GetRequiredService<IDatTipoOperaciones>();

                        await _datTipoOperaciones.DActualizar(x.Content);
                    };
                });

                _rabbitNotifications.ReceiveAsync<QueueMessage<Guid>>(RoutingKeys.TipoOperacionesTicketsBorrado.GetDescription(), _exchangeConfig, async x =>
                {
                    _logger.LogInformation($"MessageId: {x.uMessageId}, SourceSystem: {x.sSourceSystem}, Timestamp: {x.dtTimestamp}");
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        IDatTipoOperaciones _datTipoOperaciones = scope.ServiceProvider.GetRequiredService<IDatTipoOperaciones>();
                        await _datTipoOperaciones.DEliminar(x.Content);
                    };
                });
                #endregion

                #region TipoOperacionesUsuarios
                _rabbitNotifications.ReceiveAsync<QueueMessage<EntTipoOperaciones>>(RoutingKeys.TipoOperacionesUsuariosCreacion.GetDescription(), _exchangeConfig, async x =>
                {
                    _logger.LogInformation($"MessageId: {x.uMessageId}, SourceSystem: {x.sSourceSystem}, Timestamp: {x.dtTimestamp}");
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        IDatTipoOperaciones _datTipoOperaciones = scope.ServiceProvider.GetRequiredService<IDatTipoOperaciones>();

                        await _datTipoOperaciones.DAgregar(x.Content);
                    };
                });

                _rabbitNotifications.ReceiveAsync<QueueMessage<EntTipoOperaciones>>(RoutingKeys.TipoOperacionesUsuariosActualizacion.GetDescription(), _exchangeConfig, async x =>
                {
                    _logger.LogInformation($"MessageId: {x.uMessageId}, SourceSystem: {x.sSourceSystem}, Timestamp: {x.dtTimestamp}");
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        IDatTipoOperaciones _datTipoOperaciones = scope.ServiceProvider.GetRequiredService<IDatTipoOperaciones>();

                        await _datTipoOperaciones.DActualizar(x.Content);
                    };
                });

                _rabbitNotifications.ReceiveAsync<QueueMessage<Guid>>(RoutingKeys.TipoOperacionesUsuariosBorrado.GetDescription(), _exchangeConfig, async x =>
                {
                    _logger.LogInformation($"MessageId: {x.uMessageId}, SourceSystem: {x.sSourceSystem}, Timestamp: {x.dtTimestamp}");
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        IDatTipoOperaciones _datTipoOperaciones = scope.ServiceProvider.GetRequiredService<IDatTipoOperaciones>();
                        await _datTipoOperaciones.DEliminar(x.Content);
                    };
                });
                #endregion

                #region TipoOperacionesValidador
                _rabbitNotifications.ReceiveAsync<QueueMessage<EntTipoOperaciones>>(RoutingKeys.TipoOperacionesValidadorCreacion.GetDescription(), _exchangeConfig, async x =>
                {
                    _logger.LogInformation($"MessageId: {x.uMessageId}, SourceSystem: {x.sSourceSystem}, Timestamp: {x.dtTimestamp}");
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        IDatTipoOperaciones _datTipoOperaciones = scope.ServiceProvider.GetRequiredService<IDatTipoOperaciones>();

                        await _datTipoOperaciones.DAgregar(x.Content);
                    };
                });

                _rabbitNotifications.ReceiveAsync<QueueMessage<EntTipoOperaciones>>(RoutingKeys.TipoOperacionesValidadorActualizacion.GetDescription(), _exchangeConfig, async x =>
                {
                    _logger.LogInformation($"MessageId: {x.uMessageId}, SourceSystem: {x.sSourceSystem}, Timestamp: {x.dtTimestamp}");
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        IDatTipoOperaciones _datTipoOperaciones = scope.ServiceProvider.GetRequiredService<IDatTipoOperaciones>();

                        await _datTipoOperaciones.DActualizar(x.Content);
                    };
                });

                _rabbitNotifications.ReceiveAsync<QueueMessage<Guid>>(RoutingKeys.TipoOperacionesValidadorBorrado.GetDescription(), _exchangeConfig, async x =>
                {
                    _logger.LogInformation($"MessageId: {x.uMessageId}, SourceSystem: {x.sSourceSystem}, Timestamp: {x.dtTimestamp}");
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        IDatTipoOperaciones _datTipoOperaciones = scope.ServiceProvider.GetRequiredService<IDatTipoOperaciones>();
                        await _datTipoOperaciones.DEliminar(x.Content);
                    };
                });
                #endregion

                #region TipoOperacionesSolicitudes
                _rabbitNotifications.ReceiveAsync<QueueMessage<EntTipoOperaciones>>(RoutingKeys.TipoOperacionesSolicitudesCreacion.GetDescription(), _exchangeConfig, async x =>
                {
                    _logger.LogInformation($"MessageId: {x.uMessageId}, SourceSystem: {x.sSourceSystem}, Timestamp: {x.dtTimestamp}");
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        IDatTipoOperaciones _datTipoOperaciones = scope.ServiceProvider.GetRequiredService<IDatTipoOperaciones>();

                        await _datTipoOperaciones.DAgregar(x.Content);
                    };
                });

                _rabbitNotifications.ReceiveAsync<QueueMessage<EntTipoOperaciones>>(RoutingKeys.TipoOperacionesSolicitudesActualizacion.GetDescription(), _exchangeConfig, async x =>
                {
                    _logger.LogInformation($"MessageId: {x.uMessageId}, SourceSystem: {x.sSourceSystem}, Timestamp: {x.dtTimestamp}");
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        IDatTipoOperaciones _datTipoOperaciones = scope.ServiceProvider.GetRequiredService<IDatTipoOperaciones>();

                        await _datTipoOperaciones.DActualizar(x.Content);
                    };
                });

                _rabbitNotifications.ReceiveAsync<QueueMessage<Guid>>(RoutingKeys.TipoOperacionesSolicitudesBorrado.GetDescription(), _exchangeConfig, async x =>
                {
                    _logger.LogInformation($"MessageId: {x.uMessageId}, SourceSystem: {x.sSourceSystem}, Timestamp: {x.dtTimestamp}");
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        IDatTipoOperaciones _datTipoOperaciones = scope.ServiceProvider.GetRequiredService<IDatTipoOperaciones>();
                        await _datTipoOperaciones.DEliminar(x.Content);
                    };
                });
                #endregion

                #region TipoOperacionesTarjetas
                _rabbitNotifications.ReceiveAsync<QueueMessage<EntTipoOperaciones>>(RoutingKeys.TipoOperacionesTarjetasCreacion.GetDescription(), _exchangeConfig, async x =>
                {
                    _logger.LogInformation($"MessageId: {x.uMessageId}, SourceSystem: {x.sSourceSystem}, Timestamp: {x.dtTimestamp}");
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        IDatTipoOperaciones _datTipoOperaciones = scope.ServiceProvider.GetRequiredService<IDatTipoOperaciones>();

                        await _datTipoOperaciones.DAgregar(x.Content);
                    };
                });

                _rabbitNotifications.ReceiveAsync<QueueMessage<EntTipoOperaciones>>(RoutingKeys.TipoOperacionesTarjetasActualizacion.GetDescription(), _exchangeConfig, async x =>
                {
                    _logger.LogInformation($"MessageId: {x.uMessageId}, SourceSystem: {x.sSourceSystem}, Timestamp: {x.dtTimestamp}");
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        IDatTipoOperaciones _datTipoOperaciones = scope.ServiceProvider.GetRequiredService<IDatTipoOperaciones>();

                        await _datTipoOperaciones.DActualizar(x.Content);
                    };
                });

                _rabbitNotifications.ReceiveAsync<QueueMessage<Guid>>(RoutingKeys.TipoOperacionesTarjetasBorrado.GetDescription(), _exchangeConfig, async x =>
                {
                    _logger.LogInformation($"MessageId: {x.uMessageId}, SourceSystem: {x.sSourceSystem}, Timestamp: {x.dtTimestamp}");
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        IDatTipoOperaciones _datTipoOperaciones = scope.ServiceProvider.GetRequiredService<IDatTipoOperaciones>();
                        await _datTipoOperaciones.DEliminar(x.Content);
                    };
                });
                #endregion

                #endregion

                #region Usuario
                _rabbitNotifications.ReceiveAsync<QueueMessage<EntUpdateEstatusCuentaByMonedero>>(RoutingKeys.AppUpdateEstatusCuentaByUsuario.GetDescription(), _exchangeConfig, async x =>
                {
                    _logger.LogInformation($"MessageId: {x.uMessageId}, SourceSystem: {x.sSourceSystem}, Timestamp: {x.dtTimestamp}");
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                         
                        IDatUsuario _datUsuario = scope.ServiceProvider.GetRequiredService<IDatUsuario>();
                        var result = await _datUsuario.DUpdateEstatusCuentaByMonedero(x.Content);
                    }
                });
                #endregion

                #region Eventos Parametros

                _rabbitNotifications.ReceiveAsync<QueueMessage<EntReplicaEntParametros>>(RoutingKeys.AppParametrosCreate.GetDescription(), _exchangeConfig, async x =>
                {
                    _logger.LogInformation($"MessageId: {x.uMessageId}, SourceSystem: {x.sSourceSystem}, Timestamp: {x.dtTimestamp}");
                    _logger.LogInformation($"ParametrosCreacion");
                    try
                    {
                        using (var scope = _serviceScopeFactory.CreateScope())
                        {
                            _serviceProvider = scope.ServiceProvider;
                            _busParametros = _serviceProvider.GetRequiredService<IBusParametros>();

                            EntParametros configuracion = new EntParametros();
                            configuracion.uIdParametro = x.Content.uIdParametro;

                            IMDResponse<bool> response = await _busParametros.BAgregar(configuracion);
                        }

                    }
                    catch (Exception ex)
                    {
                        _logger.LogInformation(ex.Message);
                    }
                });

                _rabbitNotifications.ReceiveAsync<QueueMessage<EntReplicaEntParametros>>(RoutingKeys.AppParametrosUpdate.GetDescription(), _exchangeConfig, async x =>
                {
                    _logger.LogInformation($"MessageId: {x.uMessageId}, SourceSystem: {x.sSourceSystem}, Timestamp: {x.dtTimestamp}");
                    try
                    {
                        using (var scope = _serviceScopeFactory.CreateScope())
                        {
                            _serviceProvider = scope.ServiceProvider;
                            _busParametros = _serviceProvider.GetRequiredService<IBusParametros>();

                            EntActualizarParametros configuracion = new EntActualizarParametros();
                            configuracion.uIdParametro = x.Content.uIdParametro.Value;
                            configuracion.sValor = x.Content.sValor;
                            configuracion.bEncriptado= x.Content.bEncriptado;
                            configuracion.sDescripcion = x.Content.sDescripcion;
                            configuracion.bActivo = x.Content.bActivo;
                            configuracion.bBaja=x.Content.bBaja;

                            _busParametros.SetConfiguracionModulo(configuracion.sNombre, configuracion.sValor);

                            IMDResponse<bool> response = await _busParametros.BActualizar(configuracion);
                        }

                    }
                    catch (Exception ex)
                    {
                        _logger.LogInformation(ex.Message);
                    }
                });


                #endregion

                _rabbitNotifications.ReceiveAsync<QueueMessage<List<EntReadTarjetas>>>(RoutingKeys.TarjetasCreacion.GetDescription(), _exchangeConfig, async x =>
                {
                    _logger.LogInformation($"MessageId: {x.uMessageId}, SourceSystem: {x.sSourceSystem}, Timestamp: {x.dtTimestamp}");

                    try
                    {
                        using (var scope = _serviceScopeFactory.CreateScope())
                        {
                            var _serviceProvider = scope.ServiceProvider;
                            _busTarjetas = _serviceProvider.GetRequiredService<IBusTarjetas>();

                            foreach(EntReadTarjetas tarjetas in x.Content)
                            {
                                _busTarjetas.BCreate(tarjetas);
                            }
                            

                        }

                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"{ServiceName} : {ex.Message}");
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ServiceName} : {ex.Message}");
            }
        }
    }
}