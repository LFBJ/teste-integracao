using Alura.CoisasAFazer.Core.Models;
using Alura.CoisasAFazer.Infrastructure;
using Alura.CoisasAFazer.Services.Handlers;
using Alura.CoisasAFazer.WebApp.Controllers;
using Alura.CoisasAFazer.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using Xunit;

namespace Shindi.CoisasAFaz.Testes
{
    
    public class TarefasControllerEndPointCadastraTarefa
    {
        [Fact]
        public void DadaTarefaComInformacoesValidasDeveCadastrarERetornarOk()
        {

            // arrange
            var mockLogger = new Mock<ILogger<CadastraTarefaHandler>>();
            var options = new DbContextOptionsBuilder<DbTarefasContext>()
            .UseInMemoryDatabase("DbTarefasContext")
            .Options;

            var contexto = new DbTarefasContext(options);

            contexto.Categorias.Add(new Categoria(20, "Categoria teste"));
            contexto.SaveChanges();

            var repo = new RepositorioTarefa(contexto);

            TarefasController controlador = new TarefasController(repo, mockLogger.Object);
            CadastraTarefaVM modelo = new CadastraTarefaVM();
            modelo.Titulo = "Fazer alguma coisa";
            modelo.IdCategoria = 20;
            modelo.Prazo = DateTime.Now;

            // act
            var retorno = controlador.EndpointCadastraTarefa(modelo);

            // assert
            Assert.IsType<OkResult>(retorno);
        }

        [Fact]
        public void QuandoExcecaoForRetornadoAoCadastrarTarefaDeveRetornar500()
        {

            // arrange
            var mockLogger = new Mock<ILogger<CadastraTarefaHandler>>();

            var mock = new Mock<IRepositorioTarefas>();
            mock.Setup(r => r.ObtemCategoriaPorId(20)).Returns(new Categoria(20, "Estudo"));
            mock.Setup(r => r.IncluirTarefas(It.IsAny<Tarefa[]>())).Throws(new Exception("Houve um erro"));
            var repo = mock.Object;

            TarefasController controlador = new TarefasController(repo, mockLogger.Object);
            CadastraTarefaVM modelo = new CadastraTarefaVM();
            modelo.Titulo = "Fazer alguma coisa";
            modelo.IdCategoria = 20;
            modelo.Prazo = DateTime.Now;

            // act
            var retorno = controlador.EndpointCadastraTarefa(modelo);

            // assert
            var statusCodeEsperado = 500;
            Assert.IsType<StatusCodeResult>(retorno);
            var statusCodeRetornado = (retorno as StatusCodeResult).StatusCode;
            Assert.Equal(statusCodeEsperado, statusCodeRetornado);
        }

    }
}