namespace AppMonederoCommand.Entities.Enums
{
    public enum eEventosPaypal
    {
        [Description("PAYMENT.AUTHORIZATION.CREATED")]
        EventoCreado = 1,

        [Description("PAYMENT.AUTHORIZATION.VOIDED")]
        EventoAutorizado = 2,

        [Description("PAYMENT.CAPTURE.DECLINED")]
        EventoDeclinado = 3,

        [Description("PAYMENT.CAPTURE.COMPLETED")]
        EventoCompletado = 4,

        [Description("PAYMENTS.PAYMENT.CREATED")]
        EventoCheckCreado = 5,

        [Description("CHECKOUT.ORDER.APPROVED")]
        EventoCheckAprovado = 6,

        [Description("CHECKOUT.CHECKOUT.BUYER-APPROVED")]
        EventoCheckCobrado = 7,

        [Description("PAYMENT.CAPTURE.PENDING")]
        EventoCapturadoPendiente = 8

    }
}
