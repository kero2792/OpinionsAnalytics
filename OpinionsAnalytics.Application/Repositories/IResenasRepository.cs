namespace OpinionsAnalytics.Application.Repositories

{
    using OpinionsAnalytics.Domain.Entities.Db;
    public interface IResenasRepository
    {
        Task<IEnumerable<Resenas>> GetResenasDataAsync();
    }
}
