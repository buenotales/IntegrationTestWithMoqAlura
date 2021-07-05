using Alura.CoisasAFazer.Core.Commands;
using Alura.CoisasAFazer.Core.Models;
using Alura.CoisasAFazer.Infrastructure;
using Alura.CoisasAFazer.Services.Handlers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using Xunit;

namespace Alura.CoisasAFazer.Testes
{
    public class CadastraTarefaHandleExecute
    {
        delegate void CapturaMensagemLog(LogLevel level, EventId eventId, object state, Exception exception, Func<object, Exception, string> func);

        [Fact]
        public void DadaTarefaComInfoValidasDeveIncluirNoDB()
        {
            var comando = new CadastraTarefa("Estudar Xunit", new Categoria("Estudo"), DateTime.Now);

            LogLevel levelCapturado = LogLevel.Error;
            string mensagemCapturada = string.Empty;

            CapturaMensagemLog captura = (level, eventId, state, exception, func) =>
            {
                levelCapturado = level;
                mensagemCapturada = func(state, exception);
            };

            var mock = new Mock<ILogger<CadastraTarefaHandler>>();
            mock.Setup(l => l.Log(It.IsAny<LogLevel>(), It.IsAny<EventId>(), It.IsAny<object>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>())).Callback(captura);

            var logger = mock.Object;

            var options = new DbContextOptionsBuilder<DbTarefasContext>().UseInMemoryDatabase("DbTarefasContext").Options;

            var context = new DbTarefasContext(options);

            var repo = new RepositorioTarefa(context);

            var handle = new CadastraTarefaHandler(repo, logger);

            CommandResult resultado = handle.Execute(comando);

            Assert.True(resultado.IsSuccess);

            Assert.Equal(LogLevel.Debug, levelCapturado);
            Assert.Contains("Estudar Xunit", mensagemCapturada);
        }

        [Fact]
        public void QuandoExceptionForlancadaResultadoIsSuccessDedeSerFalse()
        {
            var comando = new CadastraTarefa("Estudar Xunit", new Categoria("Estudo"), DateTime.Now);

            var mockRepo = new Mock<IRepositorioTarefas>();
            var mockLogger = new Mock<ILogger<CadastraTarefaHandler>>();

            mockRepo.Setup(r => r.IncluirTarefas(It.IsAny<Tarefa[]>())).Throws(new Exception("Houve um erro na inclusão da tarefa"));

            var repo = mockRepo.Object;

            var logger = mockLogger.Object;

            var handle = new CadastraTarefaHandler(repo, logger);

            CommandResult resultado = handle.Execute(comando);

            Assert.False(resultado.IsSuccess);
        }

        [Fact]
        public void QuandoExceptionForLancadaDeveLogarAMensagemDaExcecao()
        {
            var mensagemDeErroEsperada = "Houve um erro na inclusão da tarefa";
            var excecaoexperada = new Exception(mensagemDeErroEsperada);

            var comando = new CadastraTarefa("Estudar Xunit", new Categoria("Estudo"), DateTime.Now);

            var mockLogger = new Mock<ILogger<CadastraTarefaHandler>>();
            var mockRepo = new Mock<IRepositorioTarefas>();

            mockRepo.Setup(r => r.IncluirTarefas(It.IsAny<Tarefa[]>())).Throws(excecaoexperada);

            var repo = mockRepo.Object;
            var logger = mockLogger.Object;

            var handle = new CadastraTarefaHandler(repo, logger);

            handle.Execute(comando);

            mockLogger.Verify(l => l.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<object>(), excecaoexperada, It.IsAny<Func<object, Exception, string>>()), Times.Once);
        }
    }
}
