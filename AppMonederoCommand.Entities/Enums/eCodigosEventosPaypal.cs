namespace AppMonederoCommand.Entities.Enums
{
    public enum eCodigosEventosPaypal
    {
        [Description("PAYMENT.AUTHORIZATION.CREATED")]
        EventoCreado = 1,

        [Description("PAYMENT.AUTHORIZATION.VOIDED")]
        EventoAutorizado = 2,

        [Description("PAYMENT.CAPTURE.DECLINED")]
        EventoDeclinado = 3,

        [Description("f1841732f4a94")]
        EventoCompletado = 4,

        [Description("PAYMENTS.PAYMENT.CREATED")]
        EventoCheckCreado = 5,

        [Description("f554368a5ece0")]
        EventoCheckAprovado = 6,

        [Description("CHECKOUT.CHECKOUT.BUYER-APPROVED")]
        EventoCheckCobrado = 7,

        [Description("e36748f2709c8")]
        EventoCapturadoPendiente = 8
    }
}
