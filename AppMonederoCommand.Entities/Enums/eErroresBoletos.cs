namespace AppMonederoCommand.Entities.Enums
{
    public enum eErroresBoletos
    {
        [Description("No cuentas con saldo suficiente para este servicio")]
        SALDO = 100,

        [Description("Cantidad es requerido")]
        NOCANTIDAD = 101,

        [Description("Se superó el limite de tickets a generar al mismo tiempo")]
        MAXCANTIDAD = 102,

        [Description("No se encontró el paquete")]
        NOPAQUETE = 103,

        [Description("El paquete no es vigente")]
        VIGENCIAPAQUETE = 104

    }
}

