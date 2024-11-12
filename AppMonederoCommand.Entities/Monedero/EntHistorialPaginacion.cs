namespace AppMonederoCommand.Entities.Monedero
{
    public class EntHistorialPaginacion
    {
        /* IMASD S.A.DE C.V
        =========================================================================================
        * Descripción: 
        * Historial de cambios:
        * ---------------------------------------------------------------------------------------
        *    Revisión   | Fecha      | Desarrollador          | Resumen del cambio
        * ---------------------------------------------------------------------------------------
        *       1       | 19/07/2023 | César Cárdenas         | Creación
        * ---------------------------------------------------------------------------------------
        */
        public List<EntMovimientos> Operaciones { get; set; } = new List<EntMovimientos>();
        public EntPaginacion Paginacion { get; set; } = new EntPaginacion();
    }
}
