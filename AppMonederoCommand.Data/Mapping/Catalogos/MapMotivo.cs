namespace AppMonederoCommand.Data.Mapping.Catalogos
{
    public class MapMotivo : IEntityTypeConfiguration<Motivo>
    {
        public void Configure(EntityTypeBuilder<Motivo> builder) {
            builder.ToTable("MOTIVOS");

            builder.HasKey(e => e.uIdMotivo).HasName("MOTIVOS_PK");

            builder.Property(p => p.uIdMotivo).HasColumnType("VARCHAR2(50)").HasColumnName("UIDMOTIVO");

            builder.Property(p => p.sMotivo).HasColumnType("VARCHAR2(100)").HasColumnName("SMOTIVO");

            builder.Property(p => p.bActivo).HasColumnType("NUMBER(1,0)").HasColumnName("BACTIVO");
            
            builder.Property(p => p.bBaja).HasColumnType("NUMBER(1,0)").HasColumnName("BBAJA");

            builder.Property(p => p.sDescripcion).HasColumnType("VARCHAR2(100)").HasColumnName("SDESCRIPCION");

            builder.Property(p => p.iTipo).HasColumnType("NUMBER(10,0)").HasColumnName("ITIPO");

            builder.Property(p => p.bPermitirOperaciones).HasColumnType("NUMBER(1,0)").HasColumnName("BPERMITIROPERACIONES");

            builder.Property(p => p.bPermitirReactivar).HasColumnType("NUMBER(1,0)").HasColumnName("BPERMITIRREACTIVAR");

            builder.Property(p => p.bPermitirEditar).HasColumnType("NUMBER(1,0)").HasColumnName("BPERMITIREDITAR");
        }
    }
}
