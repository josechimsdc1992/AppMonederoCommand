namespace AppMonederoCommand.Entities.Enums
{
    public enum RoutingKeys
    {
        [Description("PagosQ.EstatusOrden.Creacion.Folio")]
        EstatusOrdenCreacionFolio = 117,
        [Description("notificacion.sms")]
        NotificacionSms = 0,
        [Description("notificacion.email")]
        NotificacionEmail = 1,
        [Description("notificacion.push")]
        NotificacionPush = 2,
        [Description("notificacion.multiplePush")]
        NotificacionMultiplePush = 3,
        [Description("notificacion.multipleEmail")]
        NotificacionMultipleEmail = 4,
        [Description("app.update.usuariobymonedero")]
        AppUpdateUsuarioByMonedero = 5,
        [Description("Monedero.Creacion")]
        MonederoCreacion = 110,
        [Description("Monedero.Actualizacion")]
        MonederoActualizacion = 120,
        [Description("Monedero.Abono")]
        MonederoAbono = 160,
        [Description("Monedero.Debito")]
        MonederoDebito = 170,
        [Description("Monedero.Estatus")]
        MonederoEstatus = 180,
        [Description("Monedero.Credencializacion.Asignacion")]
        MonederoAsignacion = 192,

        [Description("notificacion.pushTopic")]
        NotificacionPushTopic = 20241015,
        [Description("app.update.estatusCuentaByUsuario")]
        AppUpdateEstatusCuentaByUsuario = 20241029,

        #region Tipo Tarifas
        [Description("Catalogos.TipoTarifas.Creacion")]
        TipoTarifasCreacion = 1010,

        [Description("Catalogos.TipoTarifas.Actualizacion")]
        TipoTarifasActualizacion = 1020,

        [Description("Catalogos.TipoTarifas.Borrado")]
        TipoTarifasBorrado = 1030,
        #endregion

        #region Motivos
        [Description("Catalogos.Motivos.Creacion.200")]
        MotivosCreacion200 = 2010,

        [Description("Catalogos.Motivos.Actualizacion.200")]
        MotivosActualizacion200 = 2011,

        [Description("Catalogos.Motivos.Borrado.200")]
        MotivosBorrado200 = 2012,

        [Description("Catalogos.Motivos.Creacion.500")]
        MotivosCreacion500 = 2013,

        [Description("Catalogos.Motivos.Actualizacion.500")]
        MotivosActualizacion500 = 2014,

        [Description("Catalogos.Motivos.Borrado.500")]
        MotivosBorrado500 = 2015,
        #endregion

        #region Tickets
        [Description("Tickets.Creacion")]
        TicketCreacion = 3010,
        [Description("Tickets.Modificacion")]
        TicketModificacion = 3011,
        [Description("Ticket.Usado.App")]
        TicketUsadoApp = 3012,
        #endregion

        #region FirebaseToken
        [Description("FirebaseToken.Creacion")]
        FirebaseTokenCreacion = 4010,
        [Description("FirebaseToken.Modificacion")]
        FirebaseTokenModificacion = 4020,
        #endregion
        
        #region TipoOperaciones
        //NOTA: los numeros al final de cada descripcion deben ser los iIdModulos de cada modulo respectivamente 
        #region Create

        [Description("Catalogos.TipoOperaciones.Creacion.100")]
        TipoOperacionesComercioCreacion = 50110,

        [Description("Catalogos.TipoOperaciones.Creacion.200")]
        TipoOperacionesMonederosCreacion = 50210,

        [Description("Catalogos.TipoOperaciones.Creacion.300")]
        TipoOperacionesAppCreacion = 50310,

        [Description("Catalogos.TipoOperaciones.Creacion.400")]
        TipoOperacionesConfiguracionCreacion = 50410,

        [Description("Catalogos.TipoOperaciones.Creacion.500")]
        TipoOperacionesCredencializacionCreacion = 50510,

        [Description("Catalogos.TipoOperaciones.Creacion.600")]
        TipoOperacionesListaNegraCreacion = 50610,

        [Description("Catalogos.TipoOperaciones.Creacion.700")]
        TipoOperacionesOpenLoopCreacion = 50710,

        [Description("Catalogos.TipoOperaciones.Creacion.800")]
        TipoOperacionesNotificacionesCreacion = 50810,

        [Description("Catalogos.TipoOperaciones.Creacion.900")]
        TipoOperacionesPagosCreacion = 50910,

        [Description("Catalogos.TipoOperaciones.Creacion.1000")]
        TipoOperacionesSeguridadCreacion = 51010,

        [Description("Catalogos.TipoOperaciones.Creacion.1100")]
        TipoOperacionesSincronizadorCreacion = 51110,

        [Description("Catalogos.TipoOperaciones.Creacion.1200")]
        TipoOperacionesTarifasCreacion = 51210,

        [Description("Catalogos.TipoOperaciones.Creacion.1300")]
        TipoOperacionesTicketsCreacion = 51310,

        [Description("Catalogos.TipoOperaciones.Creacion.1400")]
        TipoOperacionesUsuariosCreacion = 51410,

        [Description("Catalogos.TipoOperaciones.Creacion.1500")]
        TipoOperacionesValidadorCreacion = 51510,

        [Description("Catalogos.TipoOperaciones.Creacion.1600")]
        TipoOperacionesSolicitudesCreacion = 51610,

        [Description("Catalogos.TipoOperaciones.Creacion.1700")]
        TipoOperacionesTarjetasCreacion = 51710,


        #endregion

        #region Update

        [Description("Catalogos.TipoOperaciones.Actualizacion.100")]
        TipoOperacionesComercioActualizacion = 50120,

        [Description("Catalogos.TipoOperaciones.Actualizacion.200")]
        TipoOperacionesMonederosActualizacion = 50220,

        [Description("Catalogos.TipoOperaciones.Actualizacion.300")]
        TipoOperacionesAppActualizacion = 50320,

        [Description("Catalogos.TipoOperaciones.Actualizacion.400")]
        TipoOperacionesConfiguracionActualizacion = 50420,

        [Description("Catalogos.TipoOperaciones.Actualizacion.500")]
        TipoOperacionesCredencializacionActualizacion = 50520,

        [Description("Catalogos.TipoOperaciones.Actualizacion.600")]
        TipoOperacionesListaNegraActualizacion = 50620,

        [Description("Catalogos.TipoOperaciones.Actualizacion.700")]
        TipoOperacionesOpenLoopActualizacion = 50720,

        [Description("Catalogos.TipoOperaciones.Actualizacion.800")]
        TipoOperacionesNotificacionesActualizacion = 50820,

        [Description("Catalogos.TipoOperaciones.Actualizacion.900")]
        TipoOperacionesPagosActualizacion = 50920,

        [Description("Catalogos.TipoOperaciones.Actualizacion.1000")]
        TipoOperacionesSeguridadActualizacion = 51020,

        [Description("Catalogos.TipoOperaciones.Actualizacion.1100")]
        TipoOperacionesSincronizadorActualizacion = 51120,

        [Description("Catalogos.TipoOperaciones.Actualizacion.1200")]
        TipoOperacionesTarifasActualizacion = 51220,

        [Description("Catalogos.TipoOperaciones.Actualizacion.1300")]
        TipoOperacionesTicketsActualizacion = 51320,

        [Description("Catalogos.TipoOperaciones.Actualizacion.1400")]
        TipoOperacionesUsuariosActualizacion = 51420,

        [Description("Catalogos.TipoOperaciones.Actualizacion.1500")]
        TipoOperacionesValidadorActualizacion = 51520,

        [Description("Catalogos.TipoOperaciones.Actualizacion.1600")]
        TipoOperacionesSolicitudesActualizacion = 51620,

        [Description("Catalogos.TipoOperaciones.Actualizacion.1700")]
        TipoOperacionesTarjetasActualizacion = 51720,

        #endregion

        #region Delete

        [Description("Catalogos.TipoOperaciones.Borrado.100")]
        TipoOperacionesComercioBorrado = 50130,

        [Description("Catalogos.TipoOperaciones.Borrado.200")]
        TipoOperacionesMonederosBorrado = 50230,

        [Description("Catalogos.TipoOperaciones.Borrado.300")]
        TipoOperacionesAppBorrado = 50330,

        [Description("Catalogos.TipoOperaciones.Borrado.400")]
        TipoOperacionesConfiguracionBorrado = 50430,

        [Description("Catalogos.TipoOperaciones.Borrado.500")]
        TipoOperacionesCredencializacionBorrado = 50530,

        [Description("Catalogos.TipoOperaciones.Borrado.600")]
        TipoOperacionesListaNegraBorrado = 50630,

        [Description("Catalogos.TipoOperaciones.Borrado.700")]
        TipoOperacionesOpenLoopBorrado = 50730,

        [Description("Catalogos.TipoOperaciones.Borrado.800")]
        TipoOperacionesNotificacionesBorrado = 50830,

        [Description("Catalogos.TipoOperaciones.Borrado.900")]
        TipoOperacionesPagosBorrado = 50930,

        [Description("Catalogos.TipoOperaciones.Borrado.1000")]
        TipoOperacionesSeguridadBorrado = 51030,

        [Description("Catalogos.TipoOperaciones.Borrado.1100")]
        TipoOperacionesSincronizadorBorrado = 51130,

        [Description("Catalogos.TipoOperaciones.Borrado.1200")]
        TipoOperacionesTarifasBorrado = 51230,

        [Description("Catalogos.TipoOperaciones.Borrado.1300")]
        TipoOperacionesTicketsBorrado = 51330,

        [Description("Catalogos.TipoOperaciones.Borrado.1400")]
        TipoOperacionesUsuariosBorrado = 51430,

        [Description("Catalogos.TipoOperaciones.Borrado.1500")]
        TipoOperacionesValidadorBorrado = 51530,

        [Description("Catalogos.TipoOperaciones.Borrado.1600")]
        TipoOperacionesSolicitudesBorrado = 51630,

        [Description("Catalogos.TipoOperaciones.Borrado.1700")]
        TipoOperacionesTarjetasBorrado = 51730,

        #endregion

        #endregion

        [Description("app.parametros.create")]
        AppParametrosCreate = 6586,
        [Description("app.parametros.update")]
        AppParametrosUpdate = 6587,
        [Description("app.parametros.delete")]
        AppParametrosDelete = 6588,

        [Description("Credencializacion.Tarjetas.Creada")]
        TarjetasCreacion = 2456,
    }
}
