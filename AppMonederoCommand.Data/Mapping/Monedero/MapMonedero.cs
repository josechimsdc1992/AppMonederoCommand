namespace AppMonederoCommand.Data.Mapping.Monedero
{
    /* IMASD S.A.DE C.V
    =========================================================================================
    * Descripción: 
    * Historial de cambios:
    * ---------------------------------------------------------------------------------------
    *    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
    * ---------------------------------------------------------------------------------------
    *      1        | 17/07/2023 | César Cárdenas         | Creación
    * ---------------------------------------------------------------------------------------
    */
    public partial class MapMonedero : IEntityTypeConfiguration<Monederos>
    {
        public void Configure(EntityTypeBuilder<Monederos> builder)
        {
            builder.ToTable("Monedero");

            builder.HasKey(e => e.uIdMonedero).HasName("Monedero");

            builder.Property(e => e.uIdMonedero)
                .HasColumnType("VARCHAR2(50)").HasConversion<string>()
                .HasColumnName("UIDTARJETA");

            builder.Property(e => e.dSaldo).HasColumnName("DSALDO");

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
