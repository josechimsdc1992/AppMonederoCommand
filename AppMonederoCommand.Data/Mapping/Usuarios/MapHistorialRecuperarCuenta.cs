namespace AppMonederoCommand.Data.Mapping;

/* IMASD S.A.DE C.V
=========================================================================================
* Descripción: 
* Historial de cambios:
* ---------------------------------------------------------------------------------------
*    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
* ---------------------------------------------------------------------------------------
*      1        | 25/08/2023 | Daniel Ortiz           | Creación
* ---------------------------------------------------------------------------------------
*/
public class MapHistorialRecuperarCuenta : IEntityTypeConfiguration<HistorialRecuperarCuenta>
{
    public void Configure(EntityTypeBuilder<HistorialRecuperarCuenta> builder)
    {
        builder.ToTable("HISTORIALRECUPERARCUENTA");

        builder.HasKey(e => e.uIdHistorialRecuperarCuenta).HasName("HISTORIALRECUPERARCUENTA_PK");

        builder.Property(e => e.uIdHistorialRecuperarCuenta)
               .HasColumnType("VARCHAR2(50)").HasConversion<string>()
               .HasColumnName("UIDHISTORIALRECUPERARCUENTA");

        builder.Property(e => e.sCorreo)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("SCORREO");

        builder.Property(e => e.sToken)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("STOKEN");

        builder.Property(e => e.bActivo)
              .HasColumnName("BACTIVO");

        builder.Property(e => e.dtFechaVencimiento)
              .HasColumnType("TIMESTAMP")
              .HasColumnName("DTFECHAVENCIMIENTO");

        builder.Property(e => e.dtFechaCreacion)
                .HasColumnType("TIMESTAMP")
                .HasColumnName("DTFECHACREACION");

        builder.Property(e => e.dtFechaModificacion)
         .HasColumnType("TIMESTAMP")
         .HasPrecision(0)
         .HasColumnName("DTFECHAMODIFICACION");

        builder.Property(e => e.dtFechaBaja)
            .HasColumnType("TIMESTAMP")
            .HasPrecision(0)
            .HasColumnName("DTFECHABAJA");
    }
}
