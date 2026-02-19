namespace ApiMoniEquipamentosPesados.Models;

public class Equipamento
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public TipoEquipamento Tipo { get; set; }
    public string Modelo { get; set; } = string.Empty;
    public decimal Horimetro { get; set; }
    public StatusOperacional StatusOperacional { get; set; }
    public DateTime DataAquisicao { get; set; }
    public string? LocalizacaoAtual { get; set; }

    // Relação 1:N com Manutenções
    public List<Manutencao> Manutencoes { get; set; } = new();
}