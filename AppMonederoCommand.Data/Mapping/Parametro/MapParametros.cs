namespace AppMonederoCommand.Data.Mapping.Parametro
{
    public class MapParametros : IEntityTypeConfiguration<Parametros>
    {
        public void Configure(EntityTypeBuilder<Parametros> builder)
        {
            builder.ToTable("PARAMETROS");

            builder.HasKey(e => e.uIdParametro).HasName("PARAMETROS_PK");

            builder.Property(e => e.uIdParametro)
                .HasColumnType("VARCHAR2(50)").HasConversion<string>()
                .HasColumnName("UIDPARAMETRO");

            builder.Property(e => e.sNombre)
                .HasColumnType("VARCHAR2(100)")
                .HasColumnName("SNOMBRE");

            builder.Property(e => e.sValor)
                .HasColumnType("VARCHAR2(150)")
                .HasColumnName("SVALOR");

            builder.Property(e => e.dtFechaCreacion)
                .HasColumnType("DATE")
                .HasColumnName("DTFECHACREACION");

            builder.Property(e => e.dtFechaModificacion)
                .HasColumnType("DATE")
                .HasColumnName("DTFECHAMODIFICACION");

            builder.Property(e => e.dtFechaBaja)
                .HasColumnType("DATE")
                .HasColumnName("DTFECHABAJA");

            builder.Property(e => e.bBaja)
                .HasColumnType("NUMBER(1,0)")
                .HasColumnName("BBAJA");

            builder.Property(e => e.bActivo)
                .HasColumnType("NUMBER(1,0)")
                .HasColumnName("BACTIVO");

            builder.Property(e => e.uIdUsuarioCreacion)
                .HasColumnType("VARCHAR2(50)").HasConversion<string>()
                .HasColumnName("UIDUSUARIOCREACION");

            builder.Property(e => e.uIdUsuarioModificacion)
                .HasColumnType("VARCHAR2(50)").HasConversion<string>()
                .HasColumnName("UIDUSUARIOMODIFICACION");

            builder.Property(e => e.uIdUsuarioBaja)
                .HasColumnType("VARCHAR2(50)").HasConversion<string>()
                .HasColumnName("UIDUSUARIOBAJA");

            builder.Property(e => e.bEncriptado)
            .HasColumnType("NUMBER(1,0)")
            .HasColumnName("BENCRIPTADO");

            builder.Property(e => e.sDescripcion)
            .HasColumnType("VARCHAR2(300)")
            .HasColumnName("SDESCRIPCION");
        }
    }
}
