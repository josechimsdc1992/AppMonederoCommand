namespace AppMonederoCommand.Data.Mapping.FirebaseTokens
{

    public partial class MapFirebaseToken : IEntityTypeConfiguration<FirebaseToken>
    {
        /* IMASD S.A.DE C.V
       =========================================================================================
       * Descripción: 
       * Historial de cambios:
       * ---------------------------------------------------------------------------------------
       *    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
       * ---------------------------------------------------------------------------------------
       *      1        | 21/08/2023 | L.I. Oscar Luna  | Creación
       * ---------------------------------------------------------------------------------------
       */


        public void Configure(EntityTypeBuilder<FirebaseToken> builder)
        {
            builder.ToTable("FIREBASETOKEN");

            builder.HasKey(e => e.uIdFirebaseToken).HasName("FIREBASETOKEN_PK");

            builder.Property(e => e.uIdFirebaseToken)
              .HasColumnType("VARCHAR2(50)").HasConversion<string>()
              .HasColumnName("UIDFIREBASETOKEN");

            builder.Property(e => e.uIdUsuario)
               .HasColumnType("VARCHAR2(50)").HasConversion<string>()
               .HasColumnName("UIDUSUARIO");

            builder.Property(e => e.sInfoAppOS)
                .HasColumnType("VARCHAR2(700)")
                .HasMaxLength(700)
                .HasColumnName("SINFOAPPOS");

            builder.Property(e => e.sFcmToken)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("SFCMTOKEN");

            builder.Property(e => e.sIdAplicacion)
                .HasColumnType("VARCHAR2(50)")
                .HasMaxLength(50)
                .HasColumnName("SIDAPLICACION");

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
