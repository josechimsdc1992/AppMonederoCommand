namespace AppMonederoCommand.Entities.Usuarios
{
    public class EntUsuarioAppCSV
    {
        /* IMASD S.A.DE C.V
        =========================================================================================
        * Descripción: 
        * Historial de cambios:
        * ---------------------------------------------------------------------------------------
        *    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
        * ---------------------------------------------------------------------------------------
        *      1        | 09/06/2024 | David Realpozo         | Creación
        * ---------------------------------------------------------------------------------------
        */

        [Name("NOMBRE")]
        public string sNombreCompleto { get; set; }

        [Name("CÓDIGO DE PAÍS")]
        public string? sLada { get; set; }

        [Name("TELÉFONO")]
        public string? sTelefono { get; set; }

        [Name("CURP")]
        public string? sCURP { get; set; }

        [Name("CORREO")]
        public string? sCorreo { get; set; }

        [Name("FECHA CREACIÓN")]
        public string? sFechaCreacion { get; set; }

        [Name("MIGRADO")]
        public string sMigrado { get; set; }

        [Name("TIENE MONEDERO")]
        public string sTieneMonedero { get; set; }
    }
}
