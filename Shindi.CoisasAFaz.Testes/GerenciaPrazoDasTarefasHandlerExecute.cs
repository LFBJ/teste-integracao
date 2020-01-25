using Alura.CoisasAFazer.Core.Commands;
using Alura.CoisasAFazer.Core.Models;
using Alura.CoisasAFazer.Infrastructure;
using Alura.CoisasAFazer.Services.Handlers;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Shindi.CoisasAFaz.Testes
{
    public class GerenciaPrazoDasTarefasHandlerExecute
    {
        [Fact]
        public void QuandoTarefasEstiveremAtrasadasDeveMudarSeuStatus()
        {
            //arrange
            var compCateg = new Categoria(300, "Compras");
            var casaCateg = new Categoria(302, "Casa");
            var trabCateg = new Categoria(303, "Trabalho");
            var saudCateg = new Categoria(304, "Saúde");
            var higiCateg = new Categoria(305, "Higiene");

            var tarefas = new List<Tarefa>
            {
                //atrasadas a partir de 1/1/2019
                new Tarefa(401, "Tirar lixo", casaCateg, new DateTime(2018,12,31), null, StatusTarefa.Criada),
                new Tarefa(404, "Fazer o almoço", casaCateg, new DateTime(2017,12,1), null, StatusTarefa.Criada),
                new Tarefa(409, "Ir à academia", saudCateg, new DateTime(2018,12,31), null, StatusTarefa.Criada),
                new Tarefa(407, "Concluir o relatório", trabCateg, new DateTime(2018,5,7), null, StatusTarefa.Pendente),
                new Tarefa(410, "Beber água", saudCateg, new DateTime(2018,12,31), null, StatusTarefa.Criada),
                //dentro do prazo em 1/1/2019
                new Tarefa(408, "Comparecer à reunião", trabCateg, new DateTime(2018,11,12), new DateTime(2018,11,30), StatusTarefa.Concluida),
                new Tarefa(402, "Arrumar a cama", casaCateg, new DateTime(2019,4,5), null, StatusTarefa.Criada),
                new Tarefa(403, "Escovar os dentes", higiCateg, new DateTime(2019,1,2), null, StatusTarefa.Criada),
                new Tarefa(405, "Comprar presente pro João", compCateg, new DateTime(2019,10,8), null, StatusTarefa.Criada),
                new Tarefa(406, "Comprar ração", compCateg, new DateTime(2019,11,20), null, StatusTarefa.Criada),
            };

            var options = new DbContextOptionsBuilder<DbTarefasContext>()
                .UseInMemoryDatabase("DbTarefasContext")
                .Options;
            var contexto = new DbTarefasContext(options);
            var repo = new RepositorioTarefa(contexto);

            repo.IncluirTarefas(tarefas.ToArray());

            var comando = new GerenciaPrazoDasTarefas(new DateTime(2019, 1, 1));
            var handler = new GerenciaPrazoDasTarefasHandler(repo);

            //act
            handler.Execute(comando);

            //assert
            var tarefasEmAtraso = repo.ObtemTarefas(t => t.Status == StatusTarefa.EmAtraso);
            Assert.Equal(5, tarefasEmAtraso.Count());
        }

        // Foi criado esse teste para verificar se a atualização é feita com apenas um request
        [Fact]
        public void QuandoInvocadoDeveChamarAtualizarTarefasSomenteUmaVez()
        {
            // Arrange
            var categoria = new Categoria("Categoria teste");
            var tarefas = new List<Tarefa>
            {
                new Tarefa()
                { 
                    Categoria = categoria, 
                    Titulo="ToDo", 
                    ConcluidaEm= DateTime.Now,
                    Prazo=new DateTime(2019,9,21),
                    Id = 200
                },
                new Tarefa()
                {
                    Categoria = categoria,
                    Titulo="Do",
                    ConcluidaEm= DateTime.Now,
                    Prazo=new DateTime(2019,7,21),
                    Id = 201
                }
            };

            var mock = new Mock<IRepositorioTarefas>();
            mock.Setup(r => r.ObtemTarefas(It.IsAny<Func<Tarefa, bool>>())).Returns(tarefas);

            var repo = mock.Object;

            var comando = new GerenciaPrazoDasTarefas(new DateTime(2019,1,1));
            var handler = new GerenciaPrazoDasTarefasHandler(repo);

            //act
            handler.Execute(comando);

            //assert
            // Pode ser feito dessa forma também
            //mock.Verify(r => r.AtualizarTarefas(It.IsAny<Tarefa[]>()), Times.Exactly(1));
            mock.Verify(r => r.AtualizarTarefas(It.IsAny<Tarefa[]>()), Times.Once());

        }
    }
}
