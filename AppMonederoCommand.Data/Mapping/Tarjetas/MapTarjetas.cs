using AppMonederoCommand.Data.Entities.Tarjeta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppMonederoCommand.Data.Mapping.Tarjetas
{
    public partial class MapTarjetas : IEntityTypeConfiguration<EntTarjetas>
    {
        public void Configure(EntityTypeBuilder<EntTarjetas> builder)
        {
            // table
            builder.ToTable("TARJETAS");

            // key
            builder.HasKey(e => e.uIdTarjeta).HasName("TARJETAS_PK");

            // properties
            builder.Property(e => e.uIdTarjeta)
                .HasColumnType("VARCHAR2(50)")
                .HasColumnName("UIDTARJETA")
                //.HasDefaultValueSql("SYS_GUID()")
                .HasConversion(new GuidToStringConverter());

            builder.Property(e => e.iNumeroTarjeta)
                .HasColumnType("NUMBER(16)")
                .HasColumnName("INUMEROTARJETA");

            builder.Property(e => e.iNumeroMonedero)
                .HasColumnType("NUMBER(16)")
                .HasColumnName("INUMEROMONEDERO");

            builder.Property(e => e.sNombreProveedor)
                .HasColumnType("VARCHAR2(100)")
                .HasColumnName("SNOMBREPROVEEDOR");

            builder.Property(e => e.sTelefono)
                .HasColumnType("VARCHAR2(50)")
                .HasColumnName("STELEFONO");

            builder.Property(e => e.dtFechaFabricacion)
                .HasColumnType("DATE")
                .HasColumnName("DTFECHAFABRICACION");

            builder.Property(e => e.sVigencia)
                .HasColumnType("VARCHAR2(20)")
                .HasColumnName("SVIGENCIA");

            builder.Property(e => e.bVendida)
                .HasColumnType("NUMBER(1,0)")
                .HasColumnName("BVENDIDA");

            builder.Property(e => e.bAsociada)
                .HasColumnType("NUMBER(1,0)")
                .HasColumnName("BASOCIADA");

            builder.Property(e => e.bInicializada)
                .HasColumnType("NUMBER(1,0)")
                .HasColumnName("BINICIALIZADA");

            builder.Property(e => e.sCCV)
                .HasColumnType("VARCHAR(3)")
                .HasColumnName("SCCV");

            builder.Property(e => e.uIdMonedero)
                .HasColumnType("VARCHAR2(50)")
                .HasColumnName("UIDMONEDERO")
                .HasConversion(new GuidToStringConverter());

            builder.Property(e => e.sFolio)
                .HasColumnType("VARCHAR2(16)")
                .HasColumnName("SFOLIO");

            builder.Property(e => e.iNumeroProveedor)
                .HasColumnName("NUMBER(10)")
                .HasColumnName("INUMEROPROVEEDOR");

            builder.Property(e => e.bActivo)
                .HasColumnType("NUMBER(1,0)")
                .HasColumnName("BACTIVO").HasConversion<bool>();

            builder.Property(e => e.bBaja)
                .HasColumnType("NUMBER(1,0)")
                .HasColumnName("BBAJA").HasConversion<bool>();

            builder.Property(e => e.uIdUsuarioCreacion)
                .HasColumnType("VARCHAR2(50)")
                .HasColumnName("UIDUSUARIOCREACION")
                .HasConversion(new GuidToStringConverter());

            builder.Property(e => e.uIdUsuarioModificacion)
                .HasColumnType("VARCHAR2(50)")
                .HasColumnName("UIDUSUARIOMODIFICACION")
                .HasConversion(new GuidToStringConverter());

            builder.Property(e => e.uIdUsuarioBaja)
                .HasColumnType("VARCHAR2(50)")
                .HasColumnName("UIDUSUARIOBAJA")
                .HasConversion(new GuidToStringConverter());

            builder.Property(e => e.dtFechaCreacion)
                .HasColumnType("DATE")
                .HasColumnName("DTFECHACREACION");

            builder.Property(e => e.dtFechaModificacion)
                .HasColumnType("DATE")
                .HasColumnName("DTFECHAMODIFICACION");

            builder.Property(e => e.dtFechaBaja)
                .HasColumnType("DATE")
                .HasColumnName("DTFECHABAJA");

            builder.Property(e => e.uIdEstatusTarjeta)
                .HasColumnType("VARCHAR2(50)")
                .HasColumnName("UIDESTATUSTARJETA")
                .HasConversion(new GuidToStringConverter());

            builder.Property(e => e.uIdSolicitud)
                .HasColumnType("VARCHAR2(50)")
                .HasColumnName("UIDSOLICITUD")
                .HasConversion(new GuidToStringConverter());

            builder.Property(e => e.uIdTipoTarifa)
                .HasColumnType("VARCHAR2(50)")
                .HasColumnName("UIDTIPOTARIFA")
                .HasConversion(new GuidToStringConverter());

            builder.Property(e => e.uIdMotivo)
                .HasColumnType("VARCHAR2(50)")
                .HasColumnName("UIDMOTIVO")
                .HasConversion(new GuidToStringConverter());

            builder.Property(e => e.uIdUsuarioTarjeta)
                .HasColumnType("VARCHAR2(50)")
                .HasConversion<string>()
                .HasColumnName("UIDUSUARIOTARJETA");

            builder.Property(e => e.uIdDetalleSolicitud)
              .HasColumnType("VARCHAR2(50)")
              .HasColumnName("UIDDETALLESOLICITUD")
              .HasConversion(new GuidToStringConverter());

            builder.Property(e => e.sSKU)
                .HasColumnType("VARCHAR2(16)")
                .HasColumnName("SSKU");

            builder.Property(e => e.sPanHash)
                .HasColumnType("VARCHAR2(100)")
                .HasColumnName("SPANHASH");

            builder.Property(e => e.dtFechaValidez)
                .HasColumnType("DATE")
                .HasColumnName("DTFECHAVALIDEZ");

            

            builder.HasOne(c => c.entTipoTarifa)
                .WithMany(e => e.lstTarjetas)
                .HasForeignKey(c => c.uIdTipoTarifa)
                .HasConstraintName("TARJETAS_TIPOTARIFA_FK");

            builder.HasOne(c => c.entMotivos)
                .WithMany(e => e.lstTarjetas)
                .HasForeignKey(c => c.uIdMotivo)
                .HasConstraintName("TARJETAS_MOTIVOS_FK");

            
        }
    }
}
