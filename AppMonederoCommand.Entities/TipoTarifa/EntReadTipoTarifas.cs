namespace AppMonederoCommand.Entities.TipoTarifa;
public class EntReadTipoTarifas
{
    public string idTipoTarifa { get; set; }
    public string nombreTarifa { get; set; }
    public string claveTarifa { get; set; }
    public int tipoTarjeta { get; set; }
}

public class EntListTipoTarifas
{
    public List<EntReadTipoTarifas> tipostarifa { get; set; }
}
