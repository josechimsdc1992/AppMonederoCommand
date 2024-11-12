namespace AppMonederoCommand.Data.Mapping.Sugerencia;
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

public class MapSugerencia : IEntityTypeConfiguration<Sugerencias>
{
    public void Configure(EntityTypeBuilder<Sugerencias> builder)
    {
        builder.ToTable("SUGERENCIAS");

        builder.HasKey(e => e.uIdSugerencia).HasName("SUGERENCIAS_PK");

        builder.Property(e => e.uIdSugerencia)
            .HasColumnType("VARCHAR2(50)").HasConversion<string>()
            .HasColumnName("UIDSUGERENCIA");

        builder.Property(e => e.uIdUsuario)
            .HasColumnType("VARCHAR2(50)").HasConversion<string>()
            .HasColumnName("UIDUSUARIO");

        builder.Property(e => e.dtFechaRegitro)
            .HasColumnType("TIMESTAMP(6)")
            .HasColumnName("DTFECHAREGISTRO");

        builder.Property(e => e.iTipo)
            .HasColumnType("INT")
            .HasColumnName("ITIPO");

        builder.Property(e => e.sComentario)
            .HasColumnType("VARCHAR2(500)")
            .HasColumnName("SCOMENTARIO");

        builder.Property(e => e.sEmail)
            .HasColumnType("VARCHAR2(50)")
            .HasColumnName("SEMAIL");

        builder.Property(e => e.sNombre)
            .HasColumnType("VARCHAR2(100)")
            .HasColumnName("SNOMBRE");

        builder.Property(e => e.dtFecha)
            .HasColumnType("TIMESTAMP(6)")
            .HasColumnName("DTFECHA");

        builder.Property(e => e.sUnidad)
            .HasColumnType("VARCHAR2(50)")
            .HasColumnName("SUNIDAD");

        builder.Property(e => e.sInfraTipo)
            .HasColumnType("VARCHAR2(50)")
            .HasColumnName("SINFRATIPO");

        builder.Property(e => e.sInfraUbicacion)
            .HasColumnType("VARCHAR2(50)")
            .HasColumnName("SINFRAUBICACION");

        builder.Property(e => e.sRuta)
            .HasColumnType("VARCHAR2(50)")
            .HasColumnName("SRUTA");

        builder.Property(e => e.iIdRuta)
            .HasColumnType("INT")
            .HasColumnName("IIDRUTA");

        builder.Property(e => e.dtFechaCreacion)
            .HasColumnType("TIMESTAMP(6)")
            .HasColumnName("DTFECHACREACION");

        builder.Property(e => e.dtFechaActualizacion)
            .HasColumnType("TIMESTAMP(6)")
            .HasColumnName("DTFECHAACTUALIZACION");

        builder.Property(e => e.dtFechaEliminacion)
            .HasColumnType("TIMESTAMP(6)")
            .HasColumnName("DTFECHAELIMINACION");

        builder.Property(e => e.bActivo)
            .HasColumnType("NUMBER(1,0)")
            .HasColumnName("BACTIVO");

        builder.Property(e => e.uIdCreadoPor)
            .HasColumnType("VARCHAR2(50)").HasConversion<string>()
            .HasColumnName("UIDCREADOPOR");

        builder.Property(e => e.uIdActualizadoPor)
            .HasColumnType("VARCHAR2(50)").HasConversion<string>()
            .HasColumnName("UIDACTUALIZADOPOR");

        builder.Property(e => e.uIdEliminadoPor)
            .HasColumnType("VARCHAR2(50)").HasConversion<string>()
            .HasColumnName("UIDELIMINADOPOR");
    }
    public class GuidToStringConverter : ValueConverter<Guid, string>
    {
        public GuidToStringConverter(ConverterMappingHints? mappingHints = null)
            : base(
                guid => guid.ToString(), // Función de conversión de Guid a string
                str => Guid.Parse(str),  // Función de conversión de string a Guid
                mappingHints)
        {
        }
    }
}
