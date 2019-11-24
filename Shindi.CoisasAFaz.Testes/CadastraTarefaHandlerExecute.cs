using Alura.CoisasAFazer.Core.Commands;
using Alura.CoisasAFazer.Core.Models;
using Alura.CoisasAFazer.Infrastructure;
using Alura.CoisasAFazer.Services.Handlers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Xunit;

namespace Shindi.CoisasAFaz.Testes
{
    public class CadastraTarefaHandlerExecute
    {
        [Fact]
        public void DadaTarefaComInformacoesValidasDeveIncluirNoBD()
        {
            // Arrange
            var comando = new CadastraTarefa("Fazer alguma coisa", new Categoria("Estudo"), new DateTime(2019, 07, 21));

            var options = new DbContextOptionsBuilder<DbTarefasContext>()
                .UseInMemoryDatabase("DbTarefaContext")
                .Options;

            var contexto = new DbTarefasContext(options);
            var repositorio = new RepositorioTarefa(contexto);
            var handler = new CadastraTarefaHandler(repositorio);

            // Act
            handler.Execute(comando);

            //Assert
            var tarefa = repositorio.ObtemTarefas(t=>t.Titulo == "Fazer alguma coisa").FirstOrDefault();
            Assert.NotNull(tarefa);
        }
    }
}
