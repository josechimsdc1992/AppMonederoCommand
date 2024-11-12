namespace AppMonederoCommand.Business.Repositories.Boletos
{
  /* IMASD S.A.DE C.V
=========================================================================================
* Descripción: 
* Historial de cambios:
* ---------------------------------------------------------------------------------------
*    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
* ---------------------------------------------------------------------------------------
*      1        | 13/09/2023 | L.I. Oscar Luna        | Creación
* ---------------------------------------------------------------------------------------
*/
  public interface IDatHistorialBoletoVirtual
  {
    Task<IMDResponse<List<EntHistorialBoletosVirtuales>>> DGetListaBoleto(Guid iKey);
    Task<IMDResponse<List<EntHistorialBoletosVirtuales>>> DSave(List<EntHistorialBoletosVirtuales> rangeItems);
  }
}
