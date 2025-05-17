using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Application.Services
{
    public class CepService : ICepService
    {
        private readonly ILogger<CepService> _logger;
        private readonly ICepRepository _cepRepository;
        private readonly HttpClient _httpClient;

        public CepService(ILogger<CepService> logger, ICepRepository cepRepository, HttpClient httpClient)
        {
            _logger = logger;
            _cepRepository = cepRepository;
            _httpClient = httpClient;
        }

        public async Task<CepModel> BuscarCepAsync(string cep)
        {
            try
            {
                cep = cep.Replace("-", "").Trim();

                if (!CepValido(cep)) {
                    throw new InvalidOperationException("CEP inválido");
                }

                _logger.LogInformation("Buscando cep na base");

                var cepExistente = await _cepRepository.GetByCepAsync(cep);
                if (cepExistente != null)
                {
                    _logger.LogInformation("CEP existente em nossa base. Retornando para o usuário");
                    return cepExistente;
                }

                var response = await _httpClient.GetAsync($"https://viacep.com.br/ws/{cep}/json/");
                response.EnsureSuccessStatusCode();

                var jsonString = await response.Content.ReadAsStringAsync();
                var json = JObject.Parse(jsonString);

                if (json.ContainsKey("erro"))
                    return null;

                var novoCep = new CepModel
                {
                    cep = ((string)json["cep"])?.Replace("-", ""),
                    Logradouro = (string)json["logradouro"],
                    Complemento = (string)json["complemento"],
                    Bairro = (string)json["bairro"],
                    Localidade = (string)json["localidade"],
                    Uf = (string)json["uf"],
                    Unidade = long.TryParse((string?)json["unidade"], out var unidade) ? unidade : (long?)null,
                    Ibge = int.TryParse((string?)json["ibge"], out var ibge) ? ibge : (int?)null,
                    Gia = (string)json["gia"]
                };

                await _cepRepository.AddAsync(novoCep);
                await _cepRepository.SaveChangesAsync();

                return novoCep;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao consultar cep para salvar na base");
                throw new InvalidOperationException(ex.Message);
            }
        }

        private bool CepValido(string cep)
        {
            if (cep == null)
                throw new ArgumentNullException(nameof(cep), "CEP não pode ser nulo.");

            if (cep.Length == 0)
                throw new InvalidOperationException("CEP não pode ser vazio.");

            if (cep.Length != 8)
                throw new InvalidOperationException("CEP deve conter exatamente 8 caracteres.");

            if (!cep.All(char.IsDigit))
                throw new InvalidOperationException("CEP deve conter somente números.");

            return true;
        }
    }
}
