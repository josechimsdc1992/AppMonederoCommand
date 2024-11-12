namespace AppMonederoCommand.Entities.TarjetaUsuario.Enums
{
    public enum OperacionesTarjeta
    {

        #region Operaciones de tarjetas

        [Description("0")]
        TodasOperaciones,
        [Description("1")]
        Detalles,
        [Description("2")]
        Movimientos,
        [Description("3")]
        Recarga,
        [Description("4")]
        Traspasos,
        [Description("5")]
        Vincular,
        [Description("6")]
        Visualizar,
        [Description("7")]
        GenerarQR

        #endregion

    }
}
