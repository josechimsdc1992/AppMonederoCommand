namespace AppMonederoCommand.Data.Mapping.Catalogos
{
    public class MapTipoOperaciones : IEntityTypeConfiguration<TipoOperaciones>
    {
        public void Configure(EntityTypeBuilder<TipoOperaciones> builder)
        {
            builder.ToTable("TIPOOPERACIONES");

            builder.HasKey(e => e.uIdTipoOperacion).HasName("TIPOOPERACIONES_PK");

            builder.Property(p => p.uIdTipoOperacion).HasColumnType("VARCHAR2(50)").HasColumnName("UIDTIPOOPERACION");

            builder.Property(p => p.sNombre).HasColumnType("VARCHAR2(100)").HasColumnName("SNOMBRE");

            builder.Property(p => p.sClave).HasColumnType("VARCHAR2(100)").HasColumnName("SCLAVE");

            builder.Property(p => p.iModulo).HasColumnType("NUMBER(10,0)").HasColumnName("IMODULO");

            builder.Property(p => p.bActivo).HasColumnType("NUMBER(1,0)").HasColumnName("BACTIVO");

            builder.Property(p => p.bBaja).HasColumnType("NUMBER(1,0)").HasColumnName("BBAJA");
        }
    }
}
