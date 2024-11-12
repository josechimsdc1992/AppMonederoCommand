using AppMonederoCommand.Data.Entities.TarjetaUsuario;

namespace AppMonederoCommand.Data.Mapping.Tarjetas
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
    public partial class MapTarjetaUsuario : IEntityTypeConfiguration<TarjetaUsuario>
    {
        public void Configure(EntityTypeBuilder<TarjetaUsuario> builder)
        {
            builder.ToTable("TARJETAUSUARIO");

            builder.HasKey(e => e.uIdTarjetaUsuario).HasName("TARJETAUSUARIO_PK");

            builder.Property(e => e.uIdTarjetaUsuario)
                .HasColumnType("VARCHAR2(50)").HasConversion<string>()
                .HasColumnName("UIDTARJETAUSUARIO");

            builder.Property(e => e.uIdTarjeta)
                .HasColumnType("VARCHAR2(50)")
                .HasColumnName("UIDTARJETA")
                .HasConversion(new GuidToStringConverter());

            builder.Property(e => e.uIdMonedero)
                .HasColumnType("VARCHAR2(50)")
                .HasColumnName("UIDMONEDERO")
                .HasConversion(new GuidToStringConverter());

            builder.Property(e => e.uIdUsuario)
                .HasColumnType("VARCHAR2(50)")
                .HasColumnName("UIDUSUARIO")
                .HasConversion(new GuidToStringConverter());

            builder.Property(e => e.dSaldo).HasColumnName("DSALDO");

            builder.Property(e => e.sNumeroTarjeta).HasColumnName("SNUMEROTARJETA");

            builder.Property(e => e.iNoMonedero).HasColumnName("INOMONEDERO");

            builder.Property(e => e.uIdTipoTarifa)
                .HasColumnType("VARCHAR2(50)")
                .HasColumnName("UIDTIPOTARIFA")
                .HasConversion(new GuidToStringConverter());

            builder.Property(e => e.sTipoTarifa).HasColumnName("STIPOTARIFA");

            builder.Property(e => e.sFechaVigencia).HasColumnName("SFECHAVIGENCIA");

            builder.Property(e => e.iActivo).HasColumnName("IACTIVO");

            builder.Property(e => e.sMotivoBaja).HasColumnName("SMOTIVOBAJA");

            builder.Property(e => e.sMotivoBloqueo).HasColumnName("SMOTIVOBLOQUEO");

            builder.Property(e => e.iTipoTarjeta).HasColumnName("ITIPOTARJETA");

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
