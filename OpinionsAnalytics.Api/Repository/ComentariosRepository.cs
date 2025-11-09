using Microsoft.EntityFrameworkCore;
using OpinionsAnalytics.Api.Context;
using OpinionsAnalytics.Api.Entities;
using OpinionsAnalytics.Api.Interface;

namespace OpinionsAnalytics.Api.Repository
{
    public class ComentariosRepository : IViewComentariosRepository
    {
        private readonly ComentariosContex context;

        public ComentariosRepository(ComentariosContex contex) 
        {
            this.context = contex;
        }
        public async Task<IEnumerable<ViewComentarios>> GetAllComentariosAsync()
        {
            return await this.context.ViewComentarios.ToArrayAsync();
        }
    }
}
