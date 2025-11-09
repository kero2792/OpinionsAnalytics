using OpinionsAnalytics.Domain.Entities.Api;

namespace OpinionsAnalytics.Application.Repositories
{
    public interface IComentariosApiRepository
    {
        Task<IEnumerable<Comentarios>> GetComentariosAsync();
    }
}
