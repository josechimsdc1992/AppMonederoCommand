namespace AppMonederoCommand.Data.Mapping.TipoTarifa
{
    /* IMASD S.A.DE C.V
    =========================================================================================
    * Descripción: 
    * Historial de cambios:
    * ---------------------------------------------------------------------------------------
    *    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
    * ---------------------------------------------------------------------------------------
    *      1        | 12/08/2024 | Didier Cauich         | Creación
    * ---------------------------------------------------------------------------------------
    */
    public class MapTiposTarifa : IEntityTypeConfiguration<TiposTarifa>
    {
        public void Configure(EntityTypeBuilder<TiposTarifa> builder)
        {

            builder.ToTable("TIPOSTARIFA");

            builder.HasKey(e => e.uIdTipoTarifa).HasName("TARIFAS_PK");

            builder.Property(e => e.uIdTipoTarifa)
                .HasColumnType("VARCHAR2(50)").HasConversion<string>()
                .HasColumnName("UIDTIPOTARIFA");

            builder.Property(e => e.sTipoTarifa)
                .HasColumnType("VARCHAR2(100)")
                .HasColumnName("STIPOTARIFA");

            builder.Property(e => e.sClaveTipoTarifa)
                .HasColumnType("VARCHAR2(10)")
                .HasColumnName("SCLAVETIPOTARIFA");

            builder.Property(e => e.iTipoTarjeta)
                .HasColumnType("NUMBER()")
                .HasColumnName("ITIPOTARJETA");
        }
    }
}