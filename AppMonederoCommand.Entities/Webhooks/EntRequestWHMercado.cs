namespace AppMonederoCommand.Entities.Webhooks
{
    public class EntRequestWHMercado
    {
        public string id { get; set; }
        public string live_mode { get; set; }
        public string type { get; set; }
        public string date_created { get; set; }
        public string user_id { get; set; }
        public string api_version { get; set; }
        public string action { get; set; }
        public EntDataWHMercado data { get; set; }
        public string attempts { get; set; }
        public string received { get; set; }
        public string resource { get; set; }
        public string topic { get; set; }
        public string sPayload { get; set; }
        public string? sHeaders { get; set; }
    }

    public class EntDataWHMercado
    {
        public string id { get; set; }
    }
}

