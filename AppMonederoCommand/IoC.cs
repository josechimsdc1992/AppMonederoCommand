using AppMonederoCommand.Data.Queries.Tarjetas;

namespace AppMonederoCommand.Api
{
    public static class IoC
    {
        public static IServiceCollection AddRegistration(this IServiceCollection services)
        {
            #region Usuario Service
            services.AddScoped<IBusUsuario, BusUsuario>();
            services.AddScoped<IBusJwToken, BusJwToken>();
            services.AddScoped<IBusParametros, BusParametros>();
            services.AddScoped<IDatHistorialRefreshToken, DatHistorialRefreshToken>();
            services.AddScoped<IDatUsuario, DatUsuario>();
            services.AddScoped<IDatParametros, DatParametros>();
            services.AddScoped<IDatUsuarioActualizaTelefono, DatUsuarioActualizaTelefono>();
            services.AddScoped<IBusUsuariosWeb, BusUsuariosWeb>();
            #endregion

            #region Notificaciones
            services.AddScoped<IBusNotificaciones, BusNotificaciones>();
            #endregion

            #region Monedero
            services.AddScoped<IBusMonedero, BusMonedero>();
            services.AddScoped<IDatMonedero, DatMonedero>();

            services.AddScoped<IDatTarjetas, DatTarjetas>();
            services.AddScoped<IBusTarjetas, BusTarjeta>();
            #endregion

            #region Tarjetas
            services.AddScoped<IBusTarjetaUsuario, BusTarjetaUsuario>();
            services.AddScoped<IDatTarjetaUsuario, DatTarjetaUsuario>();
            #endregion

            #region Ubicaciones Favoritas
            services.AddScoped<IBusUbicacionFavorita, BusUbicacionFavorita>();
            services.AddScoped<IDatUbicacionFavorita, DatUbicacionFavorita>();
            #endregion

            #region Historial recuperar cuenta
            services.AddScoped<IBusHistorialRecuperarCuenta, BusHistorialRecuperarCuenta>();
            services.AddScoped<IDatHistorialRecuperarCuenta, DatHistorialRecuperarCuenta>();
            #endregion

            #region Sugerencia
            services.AddScoped<IBusSugerencia, BusSugerencia>();
            services.AddScoped<IDatSugerencia, DatSugerencia>();
            #endregion

            #region Peticiones HTTP
            services.AddScoped<IServGenerico, ServGenerico>();
            #endregion

            #region Seguridad Kong
            services.AddScoped<IAuthService, AuthService>();
            #endregion

            #region Paquetes
            services.AddScoped<IBusPaquetes, BusPaquetes>();
            #endregion




            #region Folio
            services.AddScoped<IDatFolio, DatFolio>();
            #endregion

            #region Lenguajes
            services.AddScoped<IBusLenguaje, BusLenguaje>();
            #endregion

            #region Azure Blob Storage
            services.AddScoped<IServAzureBlobStorage, ServAzureBlobStorage>();
            #endregion

            #region Catalogos
            services.AddScoped<IBusCatalogos, BusCatalogos>();
            #endregion

            #region TiposTarifa
            services.AddScoped<IDatTipoTarifa, DatTipoTarifas>();
            services.AddScoped<IBusTipoTarifa, BusTipoTarifa>();
            #endregion

            #region Motivo
            services.AddScoped<IBusMotivos, BusMotivos>();
            services.AddScoped<IDatMotivos, DatMotivos>();
            #endregion

            

            #region Tipo Operaciones
            services.AddScoped<IBusTipoOperaciones, BusTipoOperaciones>();
            services.AddScoped<IDatTipoOperaciones, DatTipoOperaciones>();
            #endregion

            return services;
        }
    }
}
