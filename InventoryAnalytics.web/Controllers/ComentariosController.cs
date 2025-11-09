using Microsoft.AspNetCore.Mvc;
using OpinionsAnalytics.Application.Repositories;

namespace InventoryAnalytics.web.Controllers
{
    public class ComentariosController : Controller
    {
        private readonly IComentariosApiRepository _comentariosApi;

        public ComentariosController(IComentariosApiRepository comentariosApi) 
        {
            _comentariosApi = comentariosApi;
        }
        public async Task<IActionResult> Index()
        {
            var comentarios = await _comentariosApi.GetComentariosAsync();
            return View(comentarios);
        }
    }
}
