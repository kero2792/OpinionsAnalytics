namespace OpinionsAnalytics.Persistence.Repositories.Api
{
    using Microsoft.Identity.Client;
    using OpinionsAnalytics.Application.Repositories;
    using OpinionsAnalytics.Domain.Entities.Api;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using System.Net.Http.Json;
    using Microsoft.Extensions.Configuration;

    public class ComentariosApiRepository : IComentariosApiRepository
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<ComentariosApiRepository> _logger;
        private readonly IConfiguration _configuration;
        private string baseUrl = string.Empty;

        public ComentariosApiRepository(IHttpClientFactory clientFactory, 
                                        ILogger<ComentariosApiRepository> looger,
                                        IConfiguration configuration
                                        ) 
        {
            _clientFactory = clientFactory;
            _logger = looger;
            _configuration = configuration;
            baseUrl = _configuration.GetValue<string>("ApiConfig:BaseUrl") ?? string.Empty;
        }
        public async Task<IEnumerable<Comentarios>> GetComentariosAsync()
        {
            List<Comentarios> comentarios = new List<Comentarios>();

            try
            {
                using var client = _clientFactory.CreateClient("ComentariosApiRepository");
                client.BaseAddress = new Uri(baseUrl);

                using var response = await client.GetAsync("api/ComentariosApi/GetAllComentarios");

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = await response.Content.ReadFromJsonAsync<IEnumerable<Comentarios>>();
                    if (apiResponse != null)
                    {
                        comentarios = apiResponse.ToList();
                    }
                }
                else
                {
                    _logger.LogError("API request failed with status code: {StatusCode}", response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                comentarios = null;
                _logger.LogError("Error fetching comentarios from API: {ErrorMessage}", ex.Message);
            }
            ;
            return comentarios;
        }
    }
}
