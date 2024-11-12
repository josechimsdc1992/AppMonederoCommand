using AppMonederoCommand.Data.Entities.Boletos;

namespace AppMonederoCommand.Data.Mapping.Boletos
{
    public partial class MapHistorialBoletoVirtual : IEntityTypeConfiguration<HistorialBoletoVirtual>
    {
        /* IMASD S.A.DE C.V
        =========================================================================================
        * Descripción: 
        * Historial de cambios:
        * ---------------------------------------------------------------------------------------
        *    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
        * ---------------------------------------------------------------------------------------
        *      1        | 13/09/2023 | L.I. Oscar Luna    | Creación
        * ---------------------------------------------------------------------------------------
        */
        public void Configure(EntityTypeBuilder<HistorialBoletoVirtual> builder)
        {
            builder.ToTable("HISTORIALBOLETOSVIRTUALES");

            builder.HasKey(e => e.uIdHistorialBoletoVirtual).HasName("UIDHISTORIALBOLETOVIRTUAL_PK");

            builder.Property(e => e.uIdHistorialBoletoVirtual)
              .HasColumnType("VARCHAR2(50)").HasConversion<string>()
              .HasColumnName("UIDHISTORIALBOLETOVIRTUAL");

            builder.Property(e => e.uIdUsuario)
              .HasColumnType("VARCHAR2(50)").HasConversion<string>()
              .HasColumnName("UIDUSUARIO");

            builder.Property(e => e.uIdTicket)
              .HasColumnType("VARCHAR2(50)").HasConversion<string>()
              .HasColumnName("UIDTICKET");

            builder.Property(e => e.sBoleto)
                .HasColumnType("CLOB")
                .HasColumnName("SBOLETO");

            builder.Property(e => e.sTipoTarifa)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("STIPOTARIFA");

            builder.Property(e => e.dtFechaOperacion)
                .HasColumnType("datetime2")
                .HasPrecision(0)
                .HasColumnName("DTFECHAOPERACION");

            builder.Property(e => e.dtFechaVencimiento)
                .HasColumnType("datetime2")
                .HasPrecision(0)
                .HasColumnName("DTFECHAVENCIMIENTO");

            #region Auditoria

            builder.Property(e => e.dtFechaCreacion)
                .HasColumnType("datetime2")
                .HasPrecision(0)
                .HasColumnName("DTFECHACREACION");

            builder.Property(e => e.dtFechaModificacion)
                .HasColumnType("datetime2")
                .HasPrecision(0)
                .HasColumnName("DTFECHAMODIFICACION");

            builder.Property(e => e.dtFechaBaja)
                .HasColumnType("datetime2")
                .HasPrecision(0)
                .HasColumnName("DTFECHABAJA");

            builder.Property(e => e.bActivo)
                .HasColumnName("BACTIVO");

            builder.Property(e => e.bBaja)
                .HasColumnName("BBAJA");

            builder.Property(e => e.uIdUsuarioCreacion)
                .HasColumnType("VARCHAR2(50)").HasConversion<string>()
                .HasColumnName("UIDUSUARIOCREACION");

            builder.Property(e => e.uIdUsuarioModificacion)
                .HasColumnType("VARCHAR2(50)").HasConversion<string>()
                .HasColumnName("UIDUSUARIOMODIFICACION");

            builder.Property(e => e.uIdUsuarioBaja)
                .HasColumnType("VARCHAR2(50)").HasConversion<string>()
                .HasColumnName("UIDUSUARIOBAJA");

            #endregion
        }
    }
}
