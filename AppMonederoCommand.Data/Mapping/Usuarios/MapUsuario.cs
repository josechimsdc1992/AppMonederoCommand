
namespace AppMonederoCommand.Data.Mapping.Usuarios;

/* IMASD S.A.DE C.V
=========================================================================================
* Descripción: 
* Historial de cambios:
* ---------------------------------------------------------------------------------------
*    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
* ---------------------------------------------------------------------------------------
*      1        | 20/07/2023 | L.I. Oscar Luna    | Creación
* ---------------------------------------------------------------------------------------
*      2        | 21/08/2023 | Daniel Ortiz       | Comentan campos no definidos en modelo 
* ---------------------------------------------------------------------------------------
*      3        | 22/08/2023 | Neftali Rodriguez  | Se corrige el nombre de la propiedad dtFechaNacimiento 
* ---------------------------------------------------------------------------------------
*      4        | 29/08/2023 | César Cárdenas     | Se cambiaron parametros
* ---------------------------------------------------------------------------------------
*     5         | 22/09/2023 |  L.I. Oscar Luna     | Se quita  datos unicos
*/
public partial class MapUsuario : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("USUARIO");

        builder.HasKey(e => e.uIdUsuario).HasName("USUARIO_PK");

        builder.Property(e => e.uIdUsuario)
           .HasColumnType("VARCHAR2(50)").HasConversion<string>()
           .HasColumnName("UIDUSUARIO");

        builder.Property(e => e.sNombre)
            .HasMaxLength(200)
            .IsUnicode(false)
        .HasColumnName("SNOMBRE");

        builder.Property(e => e.sApellidoPaterno)
            .HasMaxLength(200)
            .IsUnicode(false)
            .HasColumnName("SAPELLIDOPATERNO");

        builder.Property(e => e.sApellidoMaterno)
            .HasMaxLength(200)
            .IsUnicode(false)
            .HasColumnName("SAPELLIDOMATERNO");

        builder.Property(e => e.sTelefono)
            .HasMaxLength(10)
            .IsUnicode(false)
            .HasColumnName("STELEFONO");

        builder.Property(e => e.sCorreo)
            .HasMaxLength(250)
            .IsUnicode(false)
            .HasColumnName("SCORREO");

        builder.Property(e => e.sContrasena)
            .HasMaxLength(250)
            .IsUnicode(false)
            .HasColumnName("SCONTRASENA");

        builder.Property(e => e.sCodigoVerificacion)
            .HasMaxLength(8)
            .IsUnicode(false)
            .HasColumnName("SCODIGOVERIFICACION");

        builder.Property(e => e.bCuentaVerificada)
            .HasColumnName("BCUENTAVERIFICADA");

        builder.Property(e => e.dtFechaNacimiento)
            .HasColumnType("datetime2")
            .HasPrecision(0)
            .HasColumnName("DTFECHANACIMIENTO");

        builder.Property(e => e.sCURP)
            .HasMaxLength(18)
            .IsUnicode(false)
            .HasColumnName("SCURP");

        builder.Property(e => e.cGenero)
            .HasMaxLength(20)
            .IsUnicode(false)
            .HasColumnName("CGENERO");

        builder.Property(e => e.uIdRedSocialGoogle)
            .HasMaxLength(100)
            .IsUnicode(false)
            .HasColumnName("UIDREDSOCIALGOOGLE");

        builder.Property(e => e.sRedSocialGoogle)
            .HasMaxLength(20)
            .IsUnicode(false)
            .HasColumnName("SREDSOCIALGOOGLE");

        builder.Property(e => e.uIdRedSocialFaceBook)
            .HasMaxLength(100)
            .IsUnicode(false)
            .HasColumnName("UIDREDSOCIALFACEBOOK");

        builder.Property(e => e.sRedSocialFaceBook)
            .HasMaxLength(20)
            .IsUnicode(false)
            .HasColumnName("SREDSOCIALFACEBOOK");

        builder.Property(e => e.uIdRedSocialApple)
            .HasMaxLength(100)
            .IsUnicode(false)
            .HasColumnName("UIDREDSOCIALAPPLE");

        builder.Property(e => e.sRedSocialApple)
            .HasMaxLength(20)
            .IsUnicode(false)
            .HasColumnName("SREDSOCIALAPPLE");

        builder.Property(e => e.sFotografia)
            .HasColumnType("CLOB")
            .HasColumnName("SFOTOGRAFIA");

        builder.Property(e => e.bMigrado)
           .HasColumnName("BMIGRADO");

        builder.Property(e => e.uIdMonedero)
            .HasColumnType("VARCHAR2(50)").HasConversion<string>()
            .HasColumnName("UIDMONEDERO");

        builder.Property(e => e.dtFechaVencimientoContrasena)
           .HasColumnType("datetime2")
           .HasPrecision(0)
           .HasColumnName("DTFECHAVENCIMIENTOCONTRASENA");

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

        builder.Property(e => e.sLada)
            .HasMaxLength(10)
            .IsUnicode(false)
        .HasColumnName("SLADA");

        builder.Property(e => e.sIdAplicacion)
            .HasColumnType("VARCHAR2(50)")
            .HasMaxLength(50)
            .HasColumnName("SIDAPLICACION");

        builder.Property(cob => cob.iEstatusCuenta)
            .HasColumnType("INT")
        .HasColumnName("IESTATUSCUENTA");
    }
}

