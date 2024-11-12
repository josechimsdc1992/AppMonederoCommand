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
    public partial class MapMovimiento : IEntityTypeConfiguration<Entities.Monedero.Movimiento>
    {
        public void Configure(EntityTypeBuilder<Entities.Monedero.Movimiento> builder)
        {
            builder.ToTable("Tranferencia");

            builder.HasKey(e => e.uId).HasName("Transferencia_PK");

            builder.Property(e => e.uId)
                .HasColumnType("VARCHAR2(50)").HasConversion<string>()
                .HasColumnName("UID");

            builder.Property(e => e.uIdMonedero)
                .HasColumnType("VARCHAR2(50)").HasConversion<string>()
                .HasColumnName("UIDMONEDERO");

            builder.Property(e => e.uIdTipoMovimiento)
                .HasColumnType("VARCHAR2(50)").HasConversion<string>()
                .HasColumnName("UIDMONEDERODESTINO");

            builder.Property(e => e.sTipoMovimiento)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("STIPOMOVIMIENTO");

            builder.Property(e => e.dSaldoAnterior).HasColumnName("DIMPORTE");

            builder.Property(e => e.dImporte).HasColumnName("FLATITUD");

            builder.Property(e => e.dSaldoActual).HasColumnName("FLONGITUD");

            builder.Property(e => e.dtFechaOperacion)
                .HasColumnType("DATETIME")
                .HasColumnName("DTFECHAOPERACION");

            builder.Property(e => e.uIdReferencia)
                .HasColumnType("VARCHAR2(50)").HasConversion<string>()
                .HasColumnName("UIDREFERENCIA");

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
