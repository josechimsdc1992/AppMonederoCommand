namespace AppMonederoCommand.Business.Repositories
{
    public interface IBusLenguaje
    {
        bool SetLenguaje(string? pEntity);
        string BusSetLanguajeVentaSaldo();
        string BusSetLanguajeBienvenido();
        string BusSetLanguajeDelCuenta();
        string BusSetLanguajeRecuperarPass();
        string BusSetLanguajeSugerencias();
        string BusSetLanguajeTemporalPass();
        string BusSetLanguajeVerificationCode();
        string BusSetLanguajeTraspasoSaldo();
        string BusSetLanguajeEliminaCuentaCode();
        string BusSetLanguajeVerificationCodeVigencia();
    }
}
