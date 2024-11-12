namespace AppMonederoCommand.Data.Entities.FirebaseTokens
{
    /* IMASD S.A.DE C.V
 =========================================================================================
 * Descripción: 
 * Historial de cambios:
 * ---------------------------------------------------------------------------------------
 *    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
 * ---------------------------------------------------------------------------------------
 *      1        | 18/08/2023 | L.I. Oscar Luna        | Creación
 * ---------------------------------------------------------------------------------------
 */
    public class FirebaseToken
    {


        public Guid uIdFirebaseToken { get; set; }
        public Guid uIdUsuario { get; set; }
        public string sFcmToken { get; set; }
        public string? sInfoAppOS { get; set; }
        public string? sIdAplicacion { get; set; }

        #region Auditoria
        public DateTime dtFechaCreacion { get; set; }
        public DateTime? dtFechaModificacion { get; set; }
        public DateTime? dtFechaBaja { get; set; }
        public bool bActivo { get; set; }
        public bool? bBaja { get; set; }
        public Guid? uIdUsuarioCreacion { get; set; }
        public Guid? uIdUsuarioModificacion { get; set; }
        public Guid? uIdUsuarioBaja { get; set; }
        #endregion
    }
}
