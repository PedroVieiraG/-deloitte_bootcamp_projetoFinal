using ApiMoniEquipamentosPesados.Controller;
using ApiMoniEquipamentosPesados.Data;
using ApiMoniEquipamentosPesados.Exceptions;
using ApiMoniEquipamentosPesados.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace MoniEquipamentosPesados.Tests;

public class EquipamentosControllerTests
{
    private AppDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) 
            .Options;
            
        return new AppDbContext(options);
    }

    [Fact]
    public async Task AtualizarHorimetro_DeveLancarRegraDeNegocioException_QuandoNovoHorimetroForMenor()
    {
        // 1. Arrange 
        var context = GetDbContext();
        var equipamento = new Equipamento 
        { 
            Id = 1, 
            Codigo = "CAT-001", 
            Modelo = "Teste",
            Horimetro = 1000.0m 
        };
        context.Equipamentos.Add(equipamento);
        await context.SaveChangesAsync();
        var controller = new EquipamentosController(context);
        var exception = await Assert.ThrowsAsync<RegraDeNegocioException>(() => 
            controller.AtualizarHorimetro(1, 900.0m));

        Assert.Equal("O novo horímetro não pode ser menor que o atual.", exception.Message);
    }

    [Fact]
    public async Task AtualizarHorimetro_DeveRetornarOk_QuandoHorimetroForValido()
    {
        
        var context = GetDbContext();
        var equipamento = new Equipamento 
        { 
            Id = 1, 
            Codigo = "CAT-002", 
            Modelo = "Teste",
            Horimetro = 1000.0m 
        };
        context.Equipamentos.Add(equipamento);
        await context.SaveChangesAsync();

        var controller = new EquipamentosController(context);
        var result = await controller.AtualizarHorimetro(1, 1500.0m);
        var okResult = Assert.IsType<OkObjectResult>(result);     
        var equipamentoSalvo = await context.Equipamentos.FindAsync(1);
        Assert.Equal(1500.0m, equipamentoSalvo!.Horimetro);
    }

    [Fact]
    public async Task Delete_DeveLancarRegraDeNegocioException_QuandoHouverManutencaoConcluida()
    {
        var context = GetDbContext();
        
        var equipamento = new Equipamento { Id = 1, Codigo = "ESC-001", Modelo = "Escavadeira X" };
        context.Equipamentos.Add(equipamento);
  
        var manutencao = new Manutencao 
        { 
            Id = 1, 
            EquipamentoId = 1, 
            Descricao = "Troca de óleo", 
            Status = StatusManutencao.Concluida 
        };
        context.Manutencoes.Add(manutencao);
        await context.SaveChangesAsync();

        var controller = new EquipamentosController(context);

        var exception = await Assert.ThrowsAsync<RegraDeNegocioException>(() => 
            controller.Delete(1));

        Assert.Equal("Não é possível remover. Existem manutenções concluídas associadas a este equipamento.", exception.Message);
    }

    [Fact]
    public async Task Create_DeveLancarRegraDeNegocioException_QuandoCodigoJaExistir()
    {
        var context = GetDbContext();
        
        context.Equipamentos.Add(new Equipamento { Id = 1, Codigo = "TRATOR-99", Modelo = "Trator Antigo" });
        await context.SaveChangesAsync();

        var controller = new EquipamentosController(context);

       
        var novoDto = new ApiMoniEquipamentosPesados.DTOs.EquipamentoDto 
        { 
            Codigo = "TRATOR-99", 
            Modelo = "Trator Novo",
            Tipo = TipoEquipamento.Trator
        };

       
        var exception = await Assert.ThrowsAsync<RegraDeNegocioException>(() => 
            controller.Create(novoDto));

        Assert.Equal("Já existe um equipamento cadastrado com este código.", exception.Message);
    }

    [Fact]
    public async Task GetById_DeveLancarNaoEncontradoException_QuandoIdNaoExistir()
    {
        
        var context = GetDbContext();

        var controller = new EquipamentosController(context);
        var exception = await Assert.ThrowsAsync<NaoEncontradoException>(() => 
            controller.GetById(99));

        Assert.Equal("O equipamento com ID 99 não foi encontrado.", exception.Message);
    }
}