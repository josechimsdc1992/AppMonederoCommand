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
    public partial class MapTransferencia : IEntityTypeConfiguration<Transferencia>
    {
        public void Configure(EntityTypeBuilder<Transferencia> builder)
        {
            builder.ToTable("TRANSFERENCIA");

            builder.HasKey(e => e.uIdTransferencia).HasName("Transferencia_PK");

            builder.Property(e => e.uIdTransferencia)
                .HasColumnType("VARCHAR2(50)").HasConversion<string>()
                .HasColumnName("UIDTRANSFERENCIA");

            builder.Property(e => e.uIdMonederoOrigen)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("UIDMONEDEROORIGEN");

            builder.Property(e => e.uIdMonederoDestino)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("UIDMONEDERODESTINO");

            builder.Property(e => e.dImporte).HasColumnName("DIMPORTE");

            builder.Property(e => e.fLatitud).HasColumnName("FLATITUD");

            builder.Property(e => e.fLongitud).HasColumnName("FLONGITUD");

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
