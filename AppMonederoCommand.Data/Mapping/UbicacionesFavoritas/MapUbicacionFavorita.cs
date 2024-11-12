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
internal class MapUbicacionFavorita : IEntityTypeConfiguration<UbicacionFavorita>
{
    public void Configure(EntityTypeBuilder<UbicacionFavorita> builder)
    {
        builder.ToTable("UBICACIONESFAVORITAS");

        builder.HasKey(e => e.uIdUbicacionFavorita).HasName("UBICACIONESFAVORITAS_PK");

        builder.Property(e => e.uIdUbicacionFavorita)
            .HasColumnType("VARCHAR2(50)").HasConversion<string>()
            .HasColumnName("UIDUBICACIONFAVORITA");

        builder.Property(e => e.sEtiqueta)
            .HasColumnType("VARCHAR2(100)")
            .HasColumnName("SETIQUETA");

        builder.Property(e => e.sDireccion)
            .HasColumnType("VARCHAR2(250)")
            .HasColumnName("SDIRECCION");

        builder.Property(e => e.fLatitud)
            .HasColumnType("NUMBER(9,6)")
            .HasColumnName("FLATITUD");

        builder.Property(e => e.fLongitud)
            .HasColumnType("NUMBER(9,6)")
            .HasColumnName("FLONGITUD");

        builder.Property(e => e.bActivo)
            .HasColumnType("NUMBER(1,0)")
            .HasColumnName("BACTIVO");

        builder.Property(e => e.bBaja)
            .HasColumnType("NUMBER(1,0)")
            .HasColumnName("BBAJA");

        builder.Property(e => e.dtFechaCreacion)
            .HasColumnType("TIMESTAMP")
            .HasColumnName("DTFECHACREACION");

        builder.Property(e => e.dtFechaModificacion)
            .HasColumnType("TIMESTAMP")
            .HasColumnName("DTFECHAMODIFICACION");

        builder.Property(e => e.dtFechaBaja)
            .HasColumnType("TIMESTAMP")
            .HasColumnName("DTFECHABAJA");

        builder.Property(e => e.uIdUsuarioCreacion)
            .HasColumnType("VARCHAR2(50)").HasConversion<string>()
            .HasColumnName("UIDUSUARIOCREACION");

        builder.Property(e => e.uIdUsuarioModificacion)
            .HasColumnType("VARCHAR2(50)").HasConversion<string>()
            .HasColumnName("UIDUSUARIOMODIFICACION");

        builder.Property(e => e.uIdUsuarioBaja)
            .HasColumnType("VARCHAR2(50)").HasConversion<string>()
            .HasColumnName("UIDUSUARIOBAJA");
    }
}