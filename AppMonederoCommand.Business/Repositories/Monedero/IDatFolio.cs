namespace AppMonederoCommand.Business.Repositories.Monedero
{
    /* IMASD S.A.DE C.V
    =========================================================================================
    * Descripción: 
    * Historial de cambios:
    * ---------------------------------------------------------------------------------------
    *    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
    * ---------------------------------------------------------------------------------------
    *      1        | 10/11/2023 | Oscar Luna        | Creación
    * ---------------------------------------------------------------------------------------
    */
    public interface IDatFolio
    {
        Task<IMDResponse<long>> DGetFolio(OperacionesMovimientosMonedero sOperacion);
    }
}
