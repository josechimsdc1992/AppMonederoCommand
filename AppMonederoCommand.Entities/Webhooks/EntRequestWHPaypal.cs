namespace AppMonederoCommand.Entities.Webhooks
{
    public class EntRequestWHPaypal
    {
        public string id { get; set; }
        public string create_time { get; set; }
        public string resource_type { get; set; }
        public string event_type { get; set; }
        public string summary { get; set; }
        public EntResourcePaypal resource { get; set; }
        public List<EntLinksPaypal> links { get; set; }
        public bool bValidado { get; set; }
        public string sPayload { get; set; }
    }

    public class EntResourcePaypal
    {
        public string id { get; set; }
        public string create_time { get; set; }
        public string update_time { get; set; }
        public string? state { get; set; }
        public string status { get; set; }
        public EntAmountPaypal amount { get; set; }
        public string parent_payment { get; set; }
        public string valid_until { get; set; }
        public List<EntLinksPaypal> links { get; set; }
        public EntSupplementaryData supplementary_data { get; set; }
        public EntSellerReceivableBreakdown seller_receivable_breakdown { get; set; }
    }


    public class EntSellerReceivableBreakdown
    {
        public EntAmountPaypal paypal_fee { get; set; }
        public EntAmountPaypal gross_amount { get; set; }
        public EntAmountPaypal net_amount { get; set; }
    }

    public class EntAmountPaypal
    {
        public string total { get; set; }
        public string currency_code { get; set; }
        public EntDetailsPaypal details { get; set; }
        public string value { get; set; }
    }

    public class EntDetailsPaypal
    {
        public string subtotal { get; set; }
    }

    public class EntLinksPaypal
    {
        public string href { get; set; }
        public string rel { get; set; }
        public string method { get; set; }
    }

    public class EntSupplementaryData
    {
        public EntRelatedIds related_ids { get; set; }
    }

    public class EntRelatedIds
    {
        public string order_id { get; set; }
    }
}

