using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpinionsAnalytics.Api.Interface;
using OpinionsAnalytics.Api.Repository;

namespace OpinionsAnalytics.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComentariosApiController : ControllerBase
    {
        private readonly IViewComentariosRepository viewComentarios;

        public ComentariosApiController(IViewComentariosRepository viewComentarios) 
        {
            this.viewComentarios = viewComentarios;
        }
        [HttpGet("GetAllComentarios")]

        public async Task<IActionResult> GetAllComentariosAsync() 
        {
            var comentarios = await this.viewComentarios.GetAllComentariosAsync();
            return Ok(comentarios);
        }
    }
}
