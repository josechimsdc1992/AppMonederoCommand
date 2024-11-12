using AppMonederoCommand.Data.Entities.TiposTarifa;

namespace AppMonederoCommand.Data.Mapping;

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
public class DBMappingProfile : Profile
{
    public DBMappingProfile()
    {
        CreateMap<UbicacionFavorita, EntGetAllUbicacionFavorita>();

        CreateMap<UbicacionFavorita, EntUbicacionFavorita>();

        CreateMap<EntAddUbicacionFavorita, UbicacionFavorita>()
            .ForMember(dest => dest.uIdUbicacionFavorita, opt => opt.MapFrom(src => GetGuid()))
            .ForMember(dest => dest.dtFechaCreacion, opt => opt.MapFrom(src => DateTime.UtcNow));

        CreateMap<EntAddSugerencia, Sugerencias>()
            .ForMember(dest => dest.uIdSugerencia, opt => opt.MapFrom(src => GetGuid()))
            .ForMember(dest => dest.dtFechaRegitro, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.dtFechaCreacion, opt => opt.MapFrom(src => DateTime.UtcNow));

        CreateMap<EntSugerencia, EntAddSugerencia>();

        CreateMap<HistorialRecuperarCuenta, EntHistorialRecuperarCuenta>();

        CreateMap<EntHistorialRecuperarCuenta, HistorialRecuperarCuenta>();

        CreateMap<EntUsuarioActualizaTelefonoRequest, UsuarioActualizaTelefono>();

        CreateMap<UsuarioActualizaTelefono, EntUsuarioActualizaTelefono>();

        CreateMap<FirebaseToken, EntFirebaseToken>();

        CreateMap<EntCreateReplicaMonederos, EstadoDeCuenta>()
            .ForMember(dest => dest.dSaldo, opt => opt.MapFrom(src => src.Saldo))
            .ForMember(dest => dest.bActivo, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.bBaja, opt => opt.MapFrom(src => false))
            .ForMember(dest => dest.sEstatus, opt => opt.MapFrom(src => src.Estatus))
            .ForMember(dest => dest.dtFechaCreacion, opt => opt.MapFrom(src => src.FechaCreacion))
            .ForMember(dest => dest.uIdEstadoDeCuenta, opt => opt.MapFrom(src => src.IdEstadoDeCuenta))
            .ForMember(dest => dest.dtFechaUltimaOperacion, opt => opt.MapFrom(src => src.FechaUltimaOperacion))
            .ForMember(dest => dest.dtFechaUltimoAbono, opt => opt.MapFrom(src => src.FechaUltimoAbono))
            .ForMember(dest => dest.sFechaVigencia, opt => opt.MapFrom(src => src.FechaVigencia_))
            .ForMember(dest => dest.dtFechaBaja, opt => opt.MapFrom(src => src.FechaBaja))
            .ForMember(dest => dest.uIdEstatus, opt => opt.MapFrom(src => src.IdEstatusMonedero))
            .ForMember(dest => dest.uIdMonedero, opt => opt.MapFrom(src => src.IdMonedero))
            .ForMember(dest => dest.uIdTipoTarifa, opt => opt.MapFrom(src => src.IdTipoTarifa))
            .ForMember(dest => dest.uIdUltimaOperacion, opt => opt.MapFrom(src => src.IdUltimaOperacion))
            .ForMember(dest => dest.iNumeroMonedero, opt => opt.MapFrom(src => Convert.ToInt64(src.NumeroMonedero)))
            .ForMember(dest => dest.sTelefono, opt => opt.MapFrom(src => src.Telefono))
            .ForMember(dest => dest.sTipoTarifa, opt => opt.MapFrom(src => src.TipoTarifa))
            .ForMember(dest => dest.sNombre, opt => opt.MapFrom(src => src.Nombre))
            .ForMember(dest => dest.sApellidoMaterno, opt => opt.MapFrom(src => src.ApellidoMaterno))
            .ForMember(dest => dest.sApellidoPaterno, opt => opt.MapFrom(src => src.ApellidoPaterno))
            .ForMember(dest => dest.sCorreo, opt => opt.MapFrom(src => src.Correo))
            .ForMember(dest => dest.dtFechaNacimiento, opt => opt.MapFrom(src => src.FechaNacimiento))
            .ForMember(dest => dest.uIdTipoMonedero, opt => opt.MapFrom(src => src.IdTipoMonedero))
            .ForMember(dest => dest.sTipoMonedero, opt => opt.MapFrom(src => src.TipoMonedero));

        CreateMap<EstadoDeCuenta, EntEstadoDeCuenta>();

        CreateMap<EntEstadoDeCuenta, EstadoDeCuenta>();

        CreateMap<EntReplicaTipoTarifas, TiposTarifa>().ReverseMap();

        CreateMap<EntMotivo, Motivo>().ReverseMap();

        CreateMap<EntTipoOperaciones, TipoOperaciones>().ReverseMap();
    }

    private Guid GetGuid()
    {
        return Guid.NewGuid();
    }
}
