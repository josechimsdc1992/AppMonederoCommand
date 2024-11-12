using AppMonederoCommand.Data.Entities.Tarjeta;

namespace AppMonederoCommand.Data;

public partial class TransporteContext : DbContext
{
    public TransporteContext()
    {
    }

    public TransporteContext(DbContextOptions<TransporteContext> options, IConfiguration configuration) : base(options)
    {
    }

    //Lista de dbsets
    public virtual DbSet<Usuario> Usuario { get; set; }

    public virtual DbSet<HistorialRefreshToken> HistorialRefreshToken { get; set; }

    public virtual DbSet<FirebaseToken> FirebaseToken { get; set; }

    public virtual DbSet<Parametros> Parametros { get; set; }

    public virtual DbSet<UbicacionFavorita> UbicacionFavorita { get; set; }

    public virtual DbSet<Sugerencias> Sugerencias { get; set; }

    public virtual DbSet<HistorialRecuperarCuenta> HistorialRecuperarCuenta { get; set; }

    public virtual DbSet<HistorialRecuperacionToken> HistorialRecuperacionToken { get; set; }

    public virtual DbSet<TarjetaUsuario> TarjetaUsuario { get; set; }

    public virtual DbSet<UsuarioActualizaTelefono> UsuarioActualizaTelefono { get; set; }
    public virtual DbSet<EstadoDeCuenta> EstadoDeCuenta { get; set; }
    public virtual DbSet<TiposTarifa> TiposTarifa { get; set; }
    public virtual DbSet<Motivo> Motivo { get; set; }
    public virtual DbSet<TipoOperaciones> TipoOperaciones { get; set; }
    public virtual DbSet<EntTarjetas> Tarjetas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("APP");
        //configuración de tablas
        modelBuilder.ApplyConfiguration(new MapUsuario());
        modelBuilder.ApplyConfiguration(new MapHistorialRefreshToken());
        modelBuilder.ApplyConfiguration(new MapParametros());
        modelBuilder.ApplyConfiguration(new MapUbicacionFavorita());
        modelBuilder.ApplyConfiguration(new MapSugerencia());
        modelBuilder.ApplyConfiguration(new MapFirebaseToken());
        modelBuilder.ApplyConfiguration(new MapHistorialRecuperarCuenta());
        modelBuilder.ApplyConfiguration(new MapHistorialRecuperacionToken());
        modelBuilder.ApplyConfiguration(new MapTarjetaUsuario());
        modelBuilder.ApplyConfiguration(new MapUsuarioActualizaTelefono());
        modelBuilder.ApplyConfiguration(new MapEstadoDeCuenta());
        modelBuilder.ApplyConfiguration(new MapTiposTarifa());
        modelBuilder.ApplyConfiguration(new MapMotivo());
        modelBuilder.ApplyConfiguration(new MapTipoOperaciones());

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}