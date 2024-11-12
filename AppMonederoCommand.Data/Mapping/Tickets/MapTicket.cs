namespace AppMonederoCommand.Data.Mapping.Tickets
{
    public class MapTicket : IEntityTypeConfiguration<Ticket>
    {
        public void Configure(EntityTypeBuilder<Ticket> builder)
        {
            builder.ToTable("TICKET");

            builder.HasKey(e => e.uIdTicket).HasName("TICKET_PK");

            builder.Property(p => p.uIdTicket).HasColumnType("VARCHAR2(50)").HasColumnName("UIDTICKET");

            builder.Property(p => p.bUsado).HasColumnType("NUMBER(1,0)").HasColumnName("BUSADO");

            builder.Property(e => e.dtFechaUsado).HasColumnType("DATE").HasColumnName("DTFECHAUSADO");

            builder.Property(p => p.uIdTipoTicket).HasColumnType("VARCHAR2(50)").HasColumnName("UIDTIPOTICKET");

            builder.Property(p => p.uIdMonedero).HasColumnType("VARCHAR2(50)").HasColumnName("UIDMONEDERO");

            builder.Property(p => p.uIdTarifa).HasColumnType("VARCHAR2(50)").HasColumnName("UIDTARIFA");

            builder.Property(e => e.dtFechaCreacion).HasColumnType("DATE").HasColumnName("DTFECHACREACION");

            builder.Property(e => e.dtFechaVigencia).HasColumnType("DATE").HasColumnName("DTFECHAVIGENCIA");

            builder.Property(e => e.iNumSequencial).HasColumnType("INTEGER").HasColumnName("INUMSEQUENCIAL");

            builder.Property(p => p.bCancelada).HasColumnType("NUMBER(1,0)").HasColumnName("BCANCELADA");

            builder.Property(e => e.FirmaHSM).HasColumnType("CLOB").HasConversion<string>().HasColumnName("FIRMAHSM");

            builder.Property(p => p.claveApp).HasColumnType("VARCHAR2(50)").HasColumnName("CLAVEAPP");

            builder.Property(p => p.uIdSolicitud).HasColumnType("VARCHAR2(50)").HasColumnName("UIDSOLICITUD");

            builder.Property(p => p.bVigente).HasColumnType("NUMBER(1,0)").HasColumnName("BVIGENTE");
        }
    }
}
