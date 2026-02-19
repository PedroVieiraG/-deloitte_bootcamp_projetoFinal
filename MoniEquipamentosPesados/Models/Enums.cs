namespace ApiMoniEquipamentosPesados.Models;

public enum TipoEquipamento
{
    Caminhao, Escavadeira, Perfuratriz, Carregadeira, Trator, Guindaste
}

public enum StatusOperacional
{
    Operacional, EmManutencao, Parado
}

public enum StatusManutencao
{
    Agendada, EmAndamento, Concluida, Cancelada
}