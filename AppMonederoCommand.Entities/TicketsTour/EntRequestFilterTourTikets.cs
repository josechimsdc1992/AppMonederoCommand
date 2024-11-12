namespace AppMonederoCommand.Entities.TicketsTour
{
    public class EntRequestFilterTourTikets
    {
        public bool bDia { get; set; }

        public bool bPrecio { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int NumPag { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int NumReg { get; set; }

        public EntRequestFilterTourTikets()
        {
            bDia = false;
            bPrecio = false;
            NumPag = 1;
            NumReg = 10;
        }
    }
}
