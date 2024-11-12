namespace AppMonederoCommand.Entities.Enums
{
    public enum eEstatusCuenta
    {
        [Description("DESBLOQUEADO")]
        DESBLOQUEADO = 1,
        [Description("BLOQUEADO")]
        BLOQUEADO = 2,
        [Description("EN CAMBIO")]
        ENCAMBIO = 3,
        [Description("REPORTADO")]
        REPORTADO = 4
    }
}
