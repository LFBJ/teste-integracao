using Alura.CoisasAFazer.Core.Commands;
using Alura.CoisasAFazer.Core.Models;
using Alura.CoisasAFazer.Infrastructure;
using Alura.CoisasAFazer.Services.Handlers;
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

            var repositorio = new RepositorioFake();
            var handler = new CadastraTarefaHandler(repositorio);

            // Act
            handler.Execute(comando);

            //Assert
            var tarefa = repositorio.ObtemTarefas(t=>t.Titulo == "Fazer alguma coisa").FirstOrDefault();
            Assert.NotNull(tarefa);
        }
    }
}
