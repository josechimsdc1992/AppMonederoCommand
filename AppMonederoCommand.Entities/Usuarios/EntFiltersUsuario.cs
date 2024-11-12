namespace AppMonederoCommand.Entities.Usuarios
{
    public class EntFiltersUsuario
    {
        public string? sNombreCompleto { get; set; }
        public string? sTelefono { get; set; }
        public string? sCorreo { get; set; }
        public string? sCURP { get; set; }
        public string? sNumMonederoTarjeta { get; set; }
        public Guid? uIdMonedero { get; set; }
        public Guid? uIdUsuario { get; set; }
        public int iPage { get; set; }
        public int iRegistros { get; set; }
        public bool bExportar { get; set; }
        public bool? bMonedero { get; set; }
        public bool? bMigrado { get; set; }
    }
}
