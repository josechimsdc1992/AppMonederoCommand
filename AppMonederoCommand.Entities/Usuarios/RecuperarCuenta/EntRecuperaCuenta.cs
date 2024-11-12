namespace AppMonederoCommand.Entities.Usuarios.RecuperarCuenta;

public class EntRecuperaCuenta
{
    [JsonProperty("Correo")]
    public string? sCorreo { get; set; }
}
