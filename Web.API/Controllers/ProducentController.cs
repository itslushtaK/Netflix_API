using Application.Features.Producents;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Policy;

namespace Web.API.Controllers
{
    [Route("api/v1/producent")]
    public class ProducentController : BaseController
    {
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Create.Command command)
        {
            await Mediator.Send(command);
            return NoContent();
        }


        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetList()
        {
            var result = await Mediator.Send(new GetList.Query());
            return Ok(result);
        }
    }
}
