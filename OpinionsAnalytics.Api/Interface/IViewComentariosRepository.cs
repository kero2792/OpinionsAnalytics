using OpinionsAnalytics.Api.Entities;

namespace OpinionsAnalytics.Api.Interface
{
    public interface IViewComentariosRepository
    {
        public Task<IEnumerable<ViewComentarios>> GetAllComentariosAsync();
    }
}
