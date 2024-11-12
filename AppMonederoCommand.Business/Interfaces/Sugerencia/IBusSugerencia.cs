namespace AppMonederoCommand.Business.Interfaces.Sugerencia;
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
public interface IBusSugerencia
{
    Task<IMDResponse<bool>> BCreate(EntSugerencia sugerenciaJson, Guid uIdUsuario);
}
