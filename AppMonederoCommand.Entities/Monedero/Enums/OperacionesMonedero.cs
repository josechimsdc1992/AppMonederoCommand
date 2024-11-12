namespace AppMonederoCommand.Entities.Monedero.Enums
{
    /// <summary>
    /// Identificacion para las operaciones permitidas en la App
    /// </summary>
    public enum OperacionesMonedero
    {

        #region Operaciones de Monedero

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
        GenerarQR,
        [Description("6")]
        VerTarjetas,

        #endregion

    }


    /// <summary>
    /// Se debe omitir el nombre en validaciones ya que se debe tomar la BD en la tabla TIPOOPERACIONES
    /// </summary>
    public enum OperacionesMovimientosMonedero
    {
        [Description("Desconocido")]
        Desconocido = 0,
        [Description("Traspaso")]
        Traspaso,
        [Description("VentQR")]
        VentQR,
        [Description("VentSaldo")]
        VentSaldo,
        [Description("AbordajeNFC")]
        AbordajeNFC,
        [Description("AbordajeQrApp")]
        AbordajeQrApp,
        [Description("VentTj")]
        VentTj,
        [Description("AbordajeQrTuristicoApp")]
        AbordajeQrTuristicoApp,
        [Description("AbordajeTicket")]
        AbordajeTicket
    }

    public enum TipoMovimientos
    {
        [Description("ABONO")]
        ABONO,
        [Description("CARGO")]
        CARGO
    }
}
