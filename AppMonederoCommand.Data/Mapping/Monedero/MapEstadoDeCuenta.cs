namespace AppMonederoCommand.Data.Mapping.Monedero
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
    public partial class MapEstadoDeCuenta : IEntityTypeConfiguration<EstadoDeCuenta>
    {
        public void Configure(EntityTypeBuilder<EstadoDeCuenta> builder)
        {
            builder.ToTable("ESTADODECUENTA");

            builder.HasKey(e => e.uIdEstadoDeCuenta).HasName("ESTADODECUENTA_PK");

            builder.Property(e => e.uIdEstadoDeCuenta)
                .HasColumnType("VARCHAR2(50)").HasConversion<string>()
                .HasColumnName("UIDESTADODECUENTA");

            builder.Property(e => e.uIdMonedero)
                .HasColumnType("VARCHAR2(50)").HasConversion<string>()
                .HasColumnName("UIDMONEDERO");

            builder.Property(e => e.uIdTipoTarifa)
                .HasColumnType("VARCHAR2(50)").HasConversion<string>()
                .HasColumnName("UIDTIPOTARIFA");

            builder.Property(e => e.uIdUltimaOperacion)
                .HasColumnType("VARCHAR2(50)")
                .HasColumnName("UIDULTIMAOPERACION");

            builder.Property(e => e.uIdEstatus)
                .HasColumnType("VARCHAR2(50)").HasConversion<string>()
                .HasColumnName("UIDESTATUS");

            builder.Property(e => e.iNumeroMonedero)
                .HasColumnType("NUMBER(16)")
                .HasColumnName("INUMMONEDERO");

            builder.Property(e => e.dSaldo)
                .HasColumnType("NUMBER(18,2)")
                .HasColumnName("DSALDO");

            builder.Property(e => e.sTipoTarifa)
                .HasColumnType("VARCHAR2(20)")
                .HasColumnName("STIPOTARIFA");

            builder.Property(e => e.sTelefono)
                .HasColumnType("VARCHAR2(20)")
                .HasColumnName("STELEFONO");

            builder.Property(e => e.sEstatus)
                .HasColumnType("VARCHAR2(20)")
                .HasColumnName("SESTATUS");

            builder.Property(e => e.bActivo)
                .HasColumnType("NUMBER(1,0)")
                .HasColumnName("BACTIVO").HasConversion<bool>();

            builder.Property(e => e.bBaja)
                .HasColumnType("NUMBER(1,0)")
                .HasColumnName("BBAJA").HasConversion<bool>();

            builder.Property(e => e.dtFechaUltimaOperacion)
                .HasColumnType("DATE")
                .HasColumnName("DTFECHAULTIMAOPERACION");

            builder.Property(e => e.dtFechaUltimoAbono)
                .HasColumnType("DATE")
                .HasColumnName("DTFECHAULTIMOABONO");

            builder.Property(e => e.dtFechaCreacion)
                .HasColumnType("DATE")
                .HasColumnName("DTFECHACREACION");

            builder.Property(e => e.dtFechaBaja)
                .HasColumnType("DATE")
                .HasColumnName("DTFECHABAJA");

            builder.Property(e => e.sNombre)
                .HasColumnType("VARCHAR2(200)")
                .HasColumnName("SNOMBRE");

            builder.Property(e => e.sApellidoPaterno)
                .HasColumnType("VARCHAR2(200)")
                .HasColumnName("SAPELLIDOPATERNO");

            builder.Property(e => e.sApellidoMaterno)
                .HasColumnType("VARCHAR2(200)")
                .HasColumnName("SAPELLIDOMATERNO");

            builder.Property(e => e.sCorreo)
                .HasColumnType("VARCHAR2(200)")
                .HasColumnName("SCORREO");

            builder.Property(e => e.sFechaVigencia)
               .HasColumnType("VARCHAR2(20)")
               .HasColumnName("SFECHAVIGENCIA");

            builder.Property(e => e.dtFechaNacimiento)
               .HasColumnType("DATE")
               .HasColumnName("DTFECHANACIMIENTO");

            builder.Property(e => e.uIdTipoMonedero).HasConversion<string>()
                .HasColumnType("VARCHAR2(50)")
                .HasColumnName("UIDTIPOMONEDERO");

            builder.Property(e => e.sTipoMonedero)
                .HasColumnType("VARCHAR2(50)")
                .HasColumnName("STIPOMONEDERO");

            builder.Property(e => e.iNumTarjeta)
                .HasColumnType("NUMBER(16)")
                .HasColumnName("INUMTARJETA");

            builder.Property(e => e.uIdMotivo).HasConversion<string>()
                .HasColumnType("VARCHAR2(50)")
                .HasColumnName("UIDMOTIVO");

            builder.Property(e => e.sPanHash)
                .HasColumnType("VARCHAR2(100)")
                .HasColumnName("SPANHASH");
        }
    }
}
