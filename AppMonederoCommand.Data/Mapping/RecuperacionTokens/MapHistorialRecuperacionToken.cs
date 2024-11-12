namespace AppMonederoCommand.Data.Mapping.RecuperacionTokens
{
    public partial class MapHistorialRecuperacionToken : IEntityTypeConfiguration<HistorialRecuperacionToken>
    {
        public void Configure(EntityTypeBuilder<HistorialRecuperacionToken> builder)
        {
            builder.ToTable("HISTORIALRECUPERACIONTOKEN");

            builder.HasKey(e => e.uIdHistorialRecuperacionToken).HasName("HISTORIALRECUPERACIONTOKEN_PK");

            builder.Property(e => e.uIdHistorialRecuperacionToken)
              .HasColumnType("VARCHAR2(50)").HasConversion<string>()
              .HasColumnName("UIDHISTORIALRECUPERACIONTOKEN");

            builder.Property(e => e.uIdUsuario)
               .HasColumnType("VARCHAR2(50)").HasConversion<string>()
               .HasColumnName("UIDUSUARIO");

            builder.Property(e => e.sCorreo)
            .HasMaxLength(250)
            .IsUnicode(false)
            .HasColumnName("SCORREO");

            builder.Property(e => e.sToken)
                .HasMaxLength(250)
                .IsUnicode(false)
            .HasColumnName("STOKEN");

            builder.Property(e => e.sNombre)
            .HasMaxLength(250)
            .IsUnicode(false)
            .HasColumnName("SNOMBRE");

            builder.Property(e => e.dtFechaVencimiento)
            .HasColumnType("datetime2")
            .HasPrecision(0)
            .HasColumnName("DTFECHAVENCIMIENTO");


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
