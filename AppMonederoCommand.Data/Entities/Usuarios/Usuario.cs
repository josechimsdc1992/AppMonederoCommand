namespace AppMonederoCommand.Data.Entities.Usuarios
{
    /* IMASD S.A.DE C.V
    =========================================================================================
    * Descripción: 
    * Historial de cambios:
    * ---------------------------------------------------------------------------------------
    *    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
    * ---------------------------------------------------------------------------------------
    *      1        | 20/07/2023 | L.I. Oscar Luna        | Creación
    * ---------------------------------------------------------------------------------------
    *      2        | 22/08/2023 | Neftali Rodriguez  | Se corrige el nombre de la propiedad dtFechaNacimiento
    * ---------------------------------------------------------------------------------------
    *      3        | 22/09/2023 |  L.I. Oscar Luna  | sCorreo acepete null
    */
    public class Usuario
    {
        public Guid uIdUsuario { get; set; }
        public string sNombre { get; set; }
        public string sApellidoPaterno { get; set; }
        public string? sApellidoMaterno { get; set; }
        public string? sTelefono { get; set; }
        public string? sCorreo { get; set; }
        public string? sContrasena { get; set; }
        public string? sCodigoVerificacion { get; set; }
        public bool bCuentaVerificada { get; set; }
        public DateTime? dtFechaNacimiento { get; set; }
        public string? sCURP { get; set; }
        public string cGenero { get; set; }
        public string? uIdRedSocialGoogle { get; set; }
        public string? sRedSocialGoogle { get; set; }
        public string? uIdRedSocialFaceBook { get; set; }
        public string? sRedSocialFaceBook { get; set; }
        public string? uIdRedSocialApple { get; set; }
        public string? sRedSocialApple { get; set; }
        public string? sFotografia { get; set; }
        public bool? bMigrado { get; set; }
        public DateTime? dtFechaVencimientoContrasena { get; set; }
        public DateTime dtFechaCreacion { get; set; }
        public DateTime? dtFechaModificacion { get; set; }
        public DateTime? dtFechaBaja { get; set; }
        public Guid? uIdMonedero { get; set; }
        public bool bActivo { get; set; }
        public bool? bBaja { get; set; }
        public Guid? uIdUsuarioCreacion { get; set; }
        public Guid? uIdUsuarioModificacion { get; set; }
        public Guid? uIdUsuarioBaja { get; set; }
        public string? sLada { get; set; }
        public string? sIdAplicacion { get; set; }
        public int iEstatusCuenta { get; set; }
    }
}
