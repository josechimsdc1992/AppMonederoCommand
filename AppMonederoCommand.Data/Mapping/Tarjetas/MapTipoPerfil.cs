namespace AppMonederoCommand.Data.Mapping.Tarjetas
{
    /* IMASD S.A.DE C.V
    =========================================================================================
    * Descripción: 
    * Historial de cambios:
    * ---------------------------------------------------------------------------------------
    *    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
    * ---------------------------------------------------------------------------------------
    *      0        | 17/07/2023 | César Cárdenas         | Creación
    * ---------------------------------------------------------------------------------------
    */
    public partial class MapTipoPerfil : IEntityTypeConfiguration<TipoPerfil>
    {
        public void Configure(EntityTypeBuilder<TipoPerfil> builder)
        {
            builder.ToTable("TipoPerfil");

            builder.HasKey(e => e.uIdPerfil).HasName("TipoPerfil_PK");

            builder.Property(e => e.sNombre)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("SNOMBRE");

            builder.Property(e => e.sDescripcion)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("SDESCRIPCION");

            //Auditoria
            builder.Property(e => e.dtFechaCreacion)
                .HasColumnType("DATE")
                .HasColumnName("DTFECHACREACION");

            builder.Property(e => e.uIdUsuarioCreacion)
                .HasColumnType("VARCHAR2(50)").HasConversion<string>()
                .HasColumnName("UIDUSUARIOCREACION");

            builder.Property(e => e.dtFechaModificacion)
                .HasColumnType("DATE")
                .HasColumnName("DTFECHAMODIFICACION");

            builder.Property(e => e.dtFechaBaja)
                .HasColumnType("DATE")
                .HasColumnName("DTFECHABAJA");

            builder.Property(e => e.bActivo).HasColumnName("BACTIVO");

            builder.Property(e => e.bBaja).HasColumnName("BBAJA");

            builder.Property(e => e.uIdUsuarioModificacion)
                .HasColumnType("VARCHAR2(50)").HasConversion<string>()
                .HasColumnName("UIDUSUARIOMODIFICACION");

            builder.Property(e => e.uIdUsuarioBaja)
                .HasColumnType("VARCHAR2(50)").HasConversion<string>()
                .HasColumnName("UIDUSUARIOBAJA");
        }
    }
}
