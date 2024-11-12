namespace AppMonederoCommand.Entities.Catalogos
{
    public class EntGenero
    {
        public Guid IdGenero { get; set; }
        public string Genero { get; set; }
        public string Clave { get; set; }
    }

    public class EntGeneroResponse
    {
        public List<EntGenero> generos { get; set; }
    }
}