using AppMonederoCommand.Data.Queries.Tarjetas;

namespace AppMonederoCommand.Api
{
    public static class IoC
    {
        public static IServiceCollection AddRegistration(this IServiceCollection services)
        {
            #region Usuario Service
            services.AddScoped<IBusUsuario, BusUsuario>();
            services.AddScoped<IBusParametros, BusParametros>();
            services.AddScoped<IDatUsuario, DatUsuario>();
            services.AddScoped<IDatParametros, DatParametros>();
            services.AddScoped<IBusUsuariosWeb, BusUsuariosWeb>();
            #endregion


            #region Monedero
            services.AddScoped<IBusMonedero, BusMonedero>();
            services.AddScoped<IDatMonedero, DatMonedero>();

            services.AddScoped<IDatTarjetas, DatTarjetas>();
            services.AddScoped<IBusTarjetas, BusTarjeta>();
            #endregion



            #region Peticiones HTTP
            services.AddScoped<IServGenerico, ServGenerico>();
            #endregion

            #region Seguridad Kong
            services.AddScoped<IAuthService, AuthService>();
            #endregion

            #region Folio
            services.AddScoped<IDatFolio, DatFolio>();
            #endregion
            #region Lenguajes
            services.AddScoped<IBusLenguaje, BusLenguaje>();
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
