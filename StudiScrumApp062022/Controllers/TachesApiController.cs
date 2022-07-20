using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ScrumApp.Models;
using SrumApp.Repository;

namespace StudiScrumApp062022.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TachesApiController : ControllerBase
    {
        private readonly TachesRepository _tachesRepository;

        public TachesApiController(TachesRepository tachesRepository)
        {
            _tachesRepository = tachesRepository;
        }

        [HttpGet("ChangeStatusTache")]
        public IActionResult ChangeStatusTache(int idTache, StatusTache status)
        {
            //Faire la modification en BDD
            _tachesRepository.ChangeStatusTache(idTache, status);

            return Ok();
        }
    }
}
