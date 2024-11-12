namespace AppMonederoCommand.Entities.Sugerencia.Enums
{
    public enum TipoSugerencia
    {
        #region Topos de Sugerencias
        [Description("Autobuses y conductores")]
        suggestionBox_bus = 1,
        [Description("Infraestructura (Puentes, paraderos, banquetas)")]
        suggestionBox_infra = 2,
        [Description("Servicio")]
        suggestionBox_service = 3,
        [Description("App Va y Ven")]
        suggestionBox_app = 4,
        [Description("Otros")]
        suggestionBox_other = 5
        #endregion
    }
}
