namespace AppMonederoCommand.Entities.Pagos.Orden
{
    public enum OpcionesPago
    {
        #region Opciones de Pago
        [Description("PAYPAL")]
        PayPal = 1,
        [Description("MERCADO_PAGO")]
        MercadoPago = 2,
        [Description("BANORTE")]
        Banorte = 3,
        [Description("CODI")]
        CoDi = 4
        #endregion
    }
}

