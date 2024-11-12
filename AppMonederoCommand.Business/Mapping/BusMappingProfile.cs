namespace AppMonederoCommand.Business.Mapping;

/* IMASD S.A.DE C.V
=========================================================================================
* Descripción: 
* Historial de cambios:
* ---------------------------------------------------------------------------------------
*    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
* ---------------------------------------------------------------------------------------
*      1        | 18/08/2023 | Daniel Ortiz           | Creación
* ---------------------------------------------------------------------------------------
*/
public class BusMappingProfile : Profile
{
    public BusMappingProfile()
    {

        CreateMap<EntPaquetesProductosResponse, List<EntPaquete>>()
              .ConvertUsing(src => src.lstPaquetes.Select(paquete => new EntPaquete
              {
                  uIdPaquete = paquete.uIdPaquete,
                  sNombre = paquete.sNombre,
                  sDescripcion = paquete.sProducto,
                  fPrecio = paquete.fPrecioUnitario,
                  fImporte = paquete.fImporte,
              }).ToList());
    }
}
