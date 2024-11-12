namespace AppMonederoCommand.Entities
{
    public class IMDResponseQR<T> : IMDResponse<T>
    {
        public int? TypeCode { get; set; }
        public string? UserMessage { get; set; }
    }
}

