namespace AppMonederoCommand.Data
{
    public class DBContextExtensions
    {
        private readonly TransporteContext _dbContext;
        public DBContextExtensions(TransporteContext dbContext)
        {
            _dbContext = dbContext;
        }

        public string GetDbVersion()
        {
            return _dbContext.Model.GetProductVersion() ?? "undefined";
        }
        public bool CanConnect()
        {
            if (_dbContext.Database.CanConnect())
            {
                return true;
            }

            return false;
        }
    }
}
