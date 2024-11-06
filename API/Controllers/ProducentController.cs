using Application.Features.Producents;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/v1/producent")]
    public class ProducentController : BaseController
    {
        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromForm] Create.Command command)
        {
            await Mediator.Send(command);
            return NoContent();
        }


        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            var result = await Mediator.Send(new GetList.Query());
            return Ok(result);
        }
    }
}
