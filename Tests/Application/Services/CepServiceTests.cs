using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System.Net;

namespace Application.Services.Tests
{
    public class CepServiceTests
    {
        private readonly Mock<ICepRepository> _cepRepositoryMock;
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private readonly HttpClient _httpClient;
        private readonly Mock<ILogger<CepService>> _loggerMock;
        private readonly CepService _cepService;

        public CepServiceTests()
        {
            _cepRepositoryMock = new Mock<ICepRepository>();
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            _httpClient = new HttpClient(_httpMessageHandlerMock.Object);
            _loggerMock = new Mock<ILogger<CepService>>();

            _cepService = new CepService(
                _loggerMock.Object,
                _cepRepositoryMock.Object,
                _httpClient);
        }

        [Fact]
        public async Task BuscarCepAsync_CepInvalido_ThrowsInvalidOperationException()
        {
            await Assert.ThrowsAsync<InvalidOperationException>(() => _cepService.BuscarCepAsync("123"));
        }

        private void SetupHttpResponse(string jsonResponse, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = statusCode,
                    Content = new StringContent(jsonResponse)
                });
        }

        [Fact]
        public async Task BuscarCepAsync_CepExistenteNoBanco_RetornaCepExistente()
        {
            var cepExistente = new CepModel { cep = "13052693", Bairro = "Teste" };
            _cepRepositoryMock.Setup(r => r.GetByCepAsync("13052693")).ReturnsAsync(cepExistente);

            var result = await _cepService.BuscarCepAsync("13052-693");

            Assert.NotNull(result);
            Assert.Equal("13052693", result.cep);
            _cepRepositoryMock.Verify(r => r.GetByCepAsync("13052693"), Times.Once);
        }

        [Fact]
        public async Task BuscarCepAsync_CepNaoExiste_ConsultaApiAdicionaERetornaNovoCep()
        {
            _cepRepositoryMock.Setup(r => r.GetByCepAsync("13052693")).ReturnsAsync((CepModel)null);

            var jsonResponse = @"{
          ""cep"": ""13052-693"",
          ""logradouro"": ""Avenida Eduardo Alves de Lima"",
          ""complemento"": """",
          ""unidade"": """",
          ""bairro"": ""Vila Abaeté"",
          ""localidade"": ""Campinas"",
          ""uf"": ""SP"",
          ""ibge"": ""3509502"",
          ""gia"": ""2446""
        }";

            SetupHttpResponse(jsonResponse);

            _cepRepositoryMock.Setup(r => r.AddAsync(It.IsAny<CepModel>())).Returns(Task.CompletedTask);
            _cepRepositoryMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            var result = await _cepService.BuscarCepAsync("13052-693");

            Assert.NotNull(result);
            Assert.Equal("13052693", result.cep);
            Assert.Equal("Vila Abaeté", result.Bairro);

            _cepRepositoryMock.Verify(r => r.AddAsync(It.IsAny<CepModel>()), Times.Once);
            _cepRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task BuscarCepAsync_ApiRetornaErro_RetornaNull()
        {
            _cepRepositoryMock.Setup(r => r.GetByCepAsync("13052693")).ReturnsAsync((CepModel)null);

            var jsonResponse = @"{""erro"": true}";
            SetupHttpResponse(jsonResponse);

            var result = await _cepService.BuscarCepAsync("13052693");

            Assert.Null(result);
        }

        [Fact]
        public async Task BuscarCepAsync_ApiRetornaErroHttp_LancaExcecao()
        {
            _cepRepositoryMock.Setup(r => r.GetByCepAsync("13052693")).ReturnsAsync((CepModel)null);

            SetupHttpResponse("Erro", HttpStatusCode.BadRequest);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _cepService.BuscarCepAsync("13052693"));
        }

        [Fact]
        public async Task BuscarCepAsync_ErroGenerico_LogaELancaExcecao()
        {
            _cepRepositoryMock.Setup(r => r.GetByCepAsync("13052693")).ThrowsAsync(new Exception("Erro inesperado"));

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _cepService.BuscarCepAsync("13052693"));

            Assert.Contains("Erro inesperado", ex.Message);
        }
    }
}