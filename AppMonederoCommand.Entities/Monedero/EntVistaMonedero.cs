namespace AppMonederoCommand.Entities.Monedero
{
    public class EntVistaMonedero
    {

        public HttpStatusCode httpCode { get; set; }
        public bool hasError { get; set; }
        public int errorCode { get; set; }
        public string? message { get; set; }
        public EntPaginationVista<EntMonederoRes>? result { get; set; }
    }

    public class EntPaginationVista<T>
    {
        public int iPagina { get; set; }
        public int iNumeroRegistros { get; set; }
        public int iTotalRegistros { get; set; }
        public int iTotalPaginas { get; set; }
        public IEnumerable<T>? datos { get; set; }
        public EntPaginationVista(int iPagina, int iNumeroRegistros, int iTotalRegistros)
        {
            this.iPagina = iPagina;
            this.iNumeroRegistros = iNumeroRegistros;
            this.iTotalRegistros = iTotalRegistros;
            iTotalPaginas = this.iTotalRegistros / this.iNumeroRegistros;
            if (iTotalPaginas <= 0)
            {
                iTotalPaginas = 1;
            }
        }
    }

    public class EntMonederoRes
    {
        public Guid idEstadoCuenta { get; set; }
        public Guid idMonedero { get; set; }
        public long numMonedero { get; set; }
        public decimal saldo { get; set; }
        public Guid idEstatus { get; set; }
        public string? estatus { get; set; }
        public string? telefono { get; set; }
        public DateTime? fechaUltimoAbono { get; set; }
        public DateTime? fechaUltimaOperacion { get; set; }
        public Guid idTipoTarifa { get; set; }
        public string? tarifa { get; set; }
        public bool activo { get; set; }
        public bool baja { get; set; }
        public string? fechaVigencia { get; set; }
        public DateTime fechaCreacion { get; set; }
        public string? nombreUsuario { get; set; }
        public string? apellidoPaterno { get; set; }
        public string? apellidoMaterno { get; set; }
        public string? correoUsuario { get; set; }
        public DateOnly? fechaNacimientoUsuario { get; set; }
        public Motivos? entMotivos { get; set; }
    }
    public class Motivos
    {
        public Guid? IdMotivo { get; set; }
        public string? Nombre { get; set; }
        public bool? PermiteOperaciones { get; set; }
        public bool? PermiteReactivar { get; set; }
    }
}
