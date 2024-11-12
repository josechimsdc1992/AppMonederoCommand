namespace AppMonederoCommand.Data.Mapping;

/* IMASD S.A.DE C.V
=========================================================================================
* Descripción: 
* Historial de cambios:
* ---------------------------------------------------------------------------------------
*    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
* ---------------------------------------------------------------------------------------
*      1        | 18/10/2023 | Daniel Ortiz           | Creación
* ---------------------------------------------------------------------------------------
*/
public class MapUsuarioActualizaTelefono : IEntityTypeConfiguration<UsuarioActualizaTelefono>
{
    public void Configure(EntityTypeBuilder<UsuarioActualizaTelefono> builder)
    {
        builder.ToTable("USUARIOVERICACIONTELEFONO");

        builder.HasKey(e => e.uIdUsuario).HasName("USUARIOVERICACIONTELEFONO_PK");

        builder.Property(e => e.uIdUsuario)
           .HasColumnType("VARCHAR2(50)").HasConversion<string>()
           .HasColumnName("UIDUSUARIO");

        builder.Property(e => e.sTelefono)
            .HasMaxLength(10)
            .IsUnicode(false)
            .HasColumnName("STELEFONO");

        builder.Property(e => e.sCorreo)
            .HasMaxLength(250)
            .IsUnicode(false)
            .HasColumnName("SCORREO");

        builder.Property(e => e.sCURP)
            .HasMaxLength(18)
            .IsUnicode(false)
            .HasColumnName("SCURP");

        builder.Property(e => e.sCodigoVerificacion)
            .HasMaxLength(8)
            .IsUnicode(false)
            .HasColumnName("SCODIGOVERIFICACION");

        builder.Property(e => e.BVerificado)
            .HasColumnName("BVERIFICADO");

        builder.Property(e => e.dtFechaCreacion)
            .HasColumnType("TIMESTAMP")
            .HasPrecision(0)
            .HasColumnName("DTFECHACREACION");


        builder.Property(e => e.dtFechaModificacion)
           .HasColumnType("TIMESTAMP")
           .HasPrecision(0)
           .HasColumnName("DTFECHAMODIFICACION");
    }
}
