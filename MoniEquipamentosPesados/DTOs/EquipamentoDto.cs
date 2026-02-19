using System.ComponentModel.DataAnnotations;
using ApiMoniEquipamentosPesados.Models;

namespace ApiMoniEquipamentosPesados.DTOs;

public class EquipamentoDto
{
    [Required]
    public string Codigo { get; set; } = string.Empty;
    
    [Required]
    public TipoEquipamento Tipo { get; set; }
    
    [Required]
    public string Modelo { get; set; } = string.Empty;
    
    [Range(0, double.MaxValue, ErrorMessage = "O horímetro não pode ser negativo.")]
    public decimal Horimetro { get; set; }
    
    [Required]
    public StatusOperacional StatusOperacional { get; set; }
    
    public DateTime DataAquisicao { get; set; }
    public string? LocalizacaoAtual { get; set; }
}