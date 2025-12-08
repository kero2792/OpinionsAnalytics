using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OpinionsAnalytics.Application.DTO;
using OpinionsAnalytics.Application.Result;

namespace OpinionsAnalytics.Application.Repositories
{
    public interface IDwhRepository
    {
        Task<ServiceResult> LoadDimensionsAsync(IEnumerable<DimClienteDto>? clientes = null,
                                               IEnumerable<DimFuenteDto>? fuentes = null,
                                               IEnumerable<DimClasificacionDto>? clasificaciones = null,
                                               CancellationToken cancellationToken = default);

        Task<ServiceResult> LoadFactsFromResenasAsync(CancellationToken cancellationToken = default);

        // Helpers exposed for unit tests / orchestration
        Task<ServiceResult> LoadFactsAsync(IEnumerable<FactOpinionDto> facts, CancellationToken cancellationToken = default);
    }
}
