namespace OpinionsAnalytics.Persistence.Repositories.Csv
{
    using CsvHelper;
    using CsvHelper.Configuration;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using OpinionsAnalytics.Application.Repositories;
    using OpinionsAnalytics.Domain.Entities.Csv;
    using System.Collections.Generic;
    using System.Threading.Tasks;


    // Pendiente para puntos extras: Crear una abstraccion para el sistema de logeo
    public sealed class CsvEncuestasInternasFileReaderRepository : ICsvEncuestasInternasFileReaderRepository
    {
        private readonly string? _pathFile;
        private IConfiguration _configuration;
        private readonly ILogger<CsvEncuestasInternasFileReaderRepository> _logger;

        public CsvEncuestasInternasFileReaderRepository(IConfiguration configuration, 
                                                        ILogger<CsvEncuestasInternasFileReaderRepository> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }
        public async Task<IEnumerable<EncuestasInternas>> ReadFileAsync(string filePath)
        {
            List<EncuestasInternas>? encuestasInternasData = new List<EncuestasInternas>();

            try 
            {
                using var reader = new StreamReader(filePath);
                using var csv = new CsvReader(reader, System.Globalization.CultureInfo.InvariantCulture);
                
                await foreach (var record in csv.GetRecordsAsync<EncuestasInternas>()) 
                {
                    encuestasInternasData.Add(record);
                }

            } 
            
            catch (Exception) 
            {
                encuestasInternasData = null;
                _logger.LogError("Error reading CSV file at path: {FilePath}", filePath);
            };


            return encuestasInternasData!;
        }
    }
}
