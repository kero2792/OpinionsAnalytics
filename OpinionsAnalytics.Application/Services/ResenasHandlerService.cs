namespace OpinionsAnalytics.Application.Services
{
    using OpinionsAnalytics.Application.Repositories;
    using OpinionsAnalytics.Application.Result;
    using OpinionsAnalytics.Domain.Entities.Db;
    using OpinionsAnalytics.Domain.Repository;
    using System.Runtime.CompilerServices;

    public class ResenasHandlerService
    {
        private readonly IResenasRepository ResenasRepository;
        private readonly IComentariosApiRepository ComentariosApiRepository;
        private readonly ICsvEncuestasInternasFileReaderRepository CsvEncuestasInternasFileRepository;
        public ResenasHandlerService(IResenasRepository resenasRepository,
                                     IComentariosApiRepository Comentarios,
                                     ICsvEncuestasInternasFileReaderRepository csvEncuestasInternasFileRepository)
        {
            this.ResenasRepository = resenasRepository;
            this.ComentariosApiRepository = Comentarios;
            this.CsvEncuestasInternasFileRepository = csvEncuestasInternasFileRepository;
        }
        public async Task<ServiceResult> ProcessResenasDataAsync()
        {
            var resenas = await this.ResenasRepository.GetResenasDataAsync();
            var comentarios = await this.ComentariosApiRepository.GetComentariosAsync();
            var encuestas = await this.CsvEncuestasInternasFileRepository.ReadFileAsync(@"C:\Users\enman\Downloads\opiniones de clientes\Archivo CSV Análisis de Opiniones de Clientes-20250924\surveys_part1.csv");

            return new ServiceResult
            {
                IsSucess = true,
                Message = "Datos extraídos correctamente"
            };
        }
    }
}