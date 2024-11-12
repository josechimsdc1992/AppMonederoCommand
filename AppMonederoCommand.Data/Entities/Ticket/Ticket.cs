﻿namespace AppMonederoCommand.Data.Entities.Ticket
{
    public class Ticket
    {
        public Guid uIdTicket { get; set; }
        public bool bUsado { get; set; }
        public bool bVigente { get; set; }
        public DateTime dtFechaUsado { get; set; }
        public Guid uIdTipoTicket { get; set; }
        public Guid? uIdMonedero { get; set; }
        public Guid uIdTarifa { get; set; }
        public DateTime dtFechaCreacion { get; set; }
        public DateTime dtFechaVigencia { get; set; }
        public int iNumSequencial { get; set; }
        public bool bCancelada { get; set; }
        public string? FirmaHSM { get; set; }
        public string? claveApp { get; set; }
        public Guid? uIdSolicitud { get; set; }
    }
}