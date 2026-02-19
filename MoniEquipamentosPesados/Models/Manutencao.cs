namespace ApiMoniEquipamentosPesados.Models;

public class Manutencao
{
    public int Id { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public DateTime DataInicio { get; set; }
    public StatusManutencao Status { get; set; }

    public int EquipamentoId { get; set; }
    public Equipamento? Equipamento { get; set; } 
}