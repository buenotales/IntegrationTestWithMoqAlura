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

namespace Alura.CoisasAFazer.Testes
{
    public class TarefasControllerEndopintCadastraTarefa
    {
        [Fact]
        public void DadaTarefaComInformacoesValidasDeveRetornar200()
        {
            var mockLogger = new Mock<ILogger<CadastraTarefaHandler>>();

            var options = new DbContextOptionsBuilder<DbTarefasContext>().UseInMemoryDatabase("DbTarefasContext").Options;

            var context = new DbTarefasContext(options);
            context.Categorias.Add(new Core.Models.Categoria(20, "Categoria 20"));
            context.SaveChanges();

            var repo = new RepositorioTarefa(context);

            var controlador = new TarefasController(repo, mockLogger.Object);

            var model = new CadastraTarefaVM()
            {
                IdCategoria = 20,
                Prazo = DateTime.Now,
                Titulo = "Estudar Xunit"
            };

            var retorno = controlador.EndpointCadastraTarefa(model);

            Assert.IsType<OkResult>(retorno);
        }
    }
}
