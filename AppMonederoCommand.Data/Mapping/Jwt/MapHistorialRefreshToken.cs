namespace AppMonederoCommand.Data.Mapping.Jwt
{

    public partial class MapHistorialRefreshToken : IEntityTypeConfiguration<HistorialRefreshToken>
    {
        /* IMASD S.A.DE C.V
        =========================================================================================
        * Descripción: 
        * Historial de cambios:
        * ---------------------------------------------------------------------------------------
        *    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
        * ---------------------------------------------------------------------------------------
        *      1        | 04/08/2023 | L.I. Oscar Luna  | Creación
        * ---------------------------------------------------------------------------------------
        */

        public void Configure(EntityTypeBuilder<HistorialRefreshToken> builder)
        {
            builder.ToTable("HISTORIALREFRESHTOKEN");

            builder.HasKey(e => e.uIdHistorialToken).HasName("HISTORIALREFRESHTOKEN_PK");

            builder.Property(e => e.uIdHistorialToken)
              .HasColumnType("VARCHAR2(50)").HasConversion<string>()
              .HasColumnName("UIDHISTORIALTOKEN");

            builder.Property(e => e.uIdUsuario)
               .HasColumnType("VARCHAR2(50)").HasConversion<string>()
               .HasColumnName("UIDUSUARIO");

            builder.Property(e => e.sToken)
                .HasMaxLength(600)
                .IsUnicode(false)
            .HasColumnName("STOKEN");

            builder.Property(e => e.sRefreshToken)
                .HasMaxLength(300)
                .IsUnicode(false)
            .HasColumnName("SREFRESHTOKEN");

            builder.Property(e => e.dtFechaExpiracion)
                .HasColumnType("datetime2")
                .HasPrecision(0)
                .HasColumnName("DTFECHAEXPIRACION");

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


        }
    }
}
