namespace AppMonederoCommand.Business.Repositories.Sugerencia;
/* IMASD S.A.DE C.V
=========================================================================================
* Descripción: 
* Historial de cambios:
* ---------------------------------------------------------------------------------------
*    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
* ---------------------------------------------------------------------------------------
*      1        | 23/08/2023 | Ing. César Cárdenas    | Creación
* ---------------------------------------------------------------------------------------
*/

public interface IDatSugerencia
{
    Task<IMDResponse<bool>> DSave(EntAddSugerencia entSugerencia);
}
