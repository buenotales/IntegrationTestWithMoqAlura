using Alura.CoisasAFazer.Core.Commands;
using Alura.CoisasAFazer.Core.Models;
using Alura.CoisasAFazer.Infrastructure;
using Alura.CoisasAFazer.Services.Handlers;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Alura.CoisasAFazer.Testes
{
    public class GerenciaPrazoDasTarefasHandleexecute
    {
        [Fact]
        public void QuandoInvocadoDeveChamarAtualizarTarefasNaQtdeVezesDoTotalDeTarefasAtrasadas()
        {
            var categ = new Categoria("Dummy");

            var tarefas = new List<Tarefa>
            {
                new Tarefa(100, "Tirar lixo", categ, new DateTime(2018, 12, 31), null, StatusTarefa.Criada),
                new Tarefa(104, "Fazer o almoço", categ, new DateTime(2017, 12, 1), null, StatusTarefa.Criada),
                new Tarefa(109, "Ir à academia", categ, new DateTime(2018, 12, 31), null, StatusTarefa.Criada)
            };

            var mock = new Mock<IRepositorioTarefas>();
            mock.Setup(r => r.ObtemTarefas(It.IsAny<Func<Tarefa, bool>>())).Returns(tarefas);

            var repo = mock.Object;

            var comando = new GerenciaPrazoDasTarefas(new DateTime(2019, 1, 1));
            var handle = new GerenciaPrazoDasTarefasHandler(repo);

            handle.Execute(comando);

            mock.Verify(r => r.AtualizarTarefas(It.IsAny<Tarefa[]>()), Times.Once());
        }
    }
}
