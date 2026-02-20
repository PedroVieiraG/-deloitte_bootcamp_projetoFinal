using ApiMoniEquipamentosPesados.Data;
using ApiMoniEquipamentosPesados.DTOs;
using ApiMoniEquipamentosPesados.Models;
using ApiMoniEquipamentosPesados.Exceptions; // 👈 Necessário para os novos throws
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiMoniEquipamentosPesados.Controller;

[ApiController]
[Route("api/[controller]")]
public class EquipamentosController(AppDbContext context) : ControllerBase
{
    // DASHBOARD 
    [HttpGet("dashboard")]
    public async Task<IActionResult> ObterResumoDashboard()
    {
        var total = await context.Equipamentos.CountAsync();
        var operacionais = await context.Equipamentos.CountAsync(e => e.StatusOperacional == StatusOperacional.Operacional);
        var emManutencao = await context.Equipamentos.CountAsync(e => e.StatusOperacional == StatusOperacional.EmManutencao);
        var parados = await context.Equipamentos.CountAsync(e => e.StatusOperacional == StatusOperacional.Parado);

        return Ok(new
        {
            TotalFrota = total,
            Operacionais = operacionais,
            EmManutencao = emManutencao,
            Parados = parados,
            TaxaDisponibilidade = total > 0 ? $"{(operacionais * 100.0) / total:F2}%" : "0%"
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] EquipamentoDto dto)
    {
        var codigoTrimmed = dto.Codigo.Trim();

        if (await context.Equipamentos.AnyAsync(e => e.Codigo == codigoTrimmed))
            throw new RegraDeNegocioException("Já existe um equipamento cadastrado com este código.");

        var equipamento = new Equipamento
        {
            Codigo = codigoTrimmed,
            Tipo = dto.Tipo,
            Modelo = dto.Modelo,
            Horimetro = dto.Horimetro,
            StatusOperacional = dto.StatusOperacional,
            DataAquisicao = dto.DataAquisicao.ToUniversalTime(),
            LocalizacaoAtual = dto.LocalizacaoAtual
        };

        context.Equipamentos.Add(equipamento);
        await context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = equipamento.Id }, equipamento);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 10,
        [FromQuery] TipoEquipamento? tipo = null, [FromQuery] StatusOperacional? status = null, [FromQuery] string? codigo = null)
    {
        var query = context.Equipamentos.AsQueryable();

        if (tipo.HasValue) query = query.Where(e => e.Tipo == tipo);
        if (status.HasValue) query = query.Where(e => e.StatusOperacional == status);
        if (!string.IsNullOrEmpty(codigo)) query = query.Where(e => e.Codigo.Contains(codigo));

        var totalItems = await query.CountAsync();
        var itens = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        return Ok(new { TotalItems = totalItems, Page = page, PageSize = pageSize, Data = itens });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var equipamento = await context.Equipamentos.FindAsync(id);
        
        if (equipamento == null) 
            throw new NaoEncontradoException($"O equipamento com ID {id} não foi encontrado.");
            
        return Ok(equipamento);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] EquipamentoDto dto)
    {
        var equipamento = await context.Equipamentos.FindAsync(id);
        
        if (equipamento == null) 
            throw new NaoEncontradoException($"O equipamento com ID {id} não foi encontrado.");

        var codigoTrimmed = dto.Codigo.Trim();
        if (equipamento.Codigo != codigoTrimmed && await context.Equipamentos.AnyAsync(e => e.Codigo == codigoTrimmed))
            throw new RegraDeNegocioException("Já existe outro equipamento cadastrado com este código.");

        equipamento.Codigo = codigoTrimmed;
        equipamento.Tipo = dto.Tipo;
        equipamento.Modelo = dto.Modelo;
        equipamento.Horimetro = dto.Horimetro;
        equipamento.StatusOperacional = dto.StatusOperacional;
        equipamento.DataAquisicao = dto.DataAquisicao.ToUniversalTime();
        equipamento.LocalizacaoAtual = dto.LocalizacaoAtual;

        await context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var equipamento = await context.Equipamentos.FindAsync(id);
        
        if (equipamento == null) 
            throw new NaoEncontradoException($"O equipamento com ID {id} não foi encontrado.");

        bool temManutencaoConcluida = await context.Manutencoes
            .AnyAsync(m => m.EquipamentoId == id && m.Status == StatusManutencao.Concluida);
        
        if (temManutencaoConcluida)
            throw new RegraDeNegocioException("Não é possível remover. Existem manutenções concluídas associadas a este equipamento.");

        context.Equipamentos.Remove(equipamento);
        await context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPatch("{id}/horimetro")]
    public async Task<IActionResult> AtualizarHorimetro(int id, [FromBody] decimal novoHorimetro)
    {
        var equipamento = await context.Equipamentos.FindAsync(id);
        
        if (equipamento == null) 
            throw new NaoEncontradoException($"O equipamento com ID {id} não foi encontrado.");

        if (novoHorimetro < equipamento.Horimetro)
            throw new RegraDeNegocioException("O novo horímetro não pode ser menor que o atual.");

        equipamento.Horimetro = novoHorimetro;
        await context.SaveChangesAsync();

        return Ok(new { Mensagem = "Horímetro atualizado com sucesso.", NovoHorimetro = equipamento.Horimetro });
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> AlterarStatus(int id, [FromQuery] StatusOperacional novoStatus)
    {
        var equipamento = await context.Equipamentos.FindAsync(id);
        
        if (equipamento == null) 
            throw new NaoEncontradoException($"O equipamento com ID {id} não foi encontrado.");

        if (equipamento.StatusOperacional == novoStatus)
            throw new RegraDeNegocioException($"O equipamento já se encontra no status {novoStatus}.");

        equipamento.StatusOperacional = novoStatus;
        await context.SaveChangesAsync();
        
        return Ok(new { Mensagem = $"Status alterado para {novoStatus}." });
    }

    [HttpGet("{id}/manutencoes")]
    public async Task<IActionResult> GetManutencoesDoEquipamento(int id)
    {
        var existe = await context.Equipamentos.AnyAsync(e => e.Id == id);
        
        if (!existe) 
            throw new NaoEncontradoException($"O equipamento com ID {id} não foi encontrado.");

        var manutencoes = await context.Manutencoes
            .Where(m => m.EquipamentoId == id)
            .ToListAsync();
            
        return Ok(manutencoes);
    }
}