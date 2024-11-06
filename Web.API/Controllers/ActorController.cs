using Application.Features.Actors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.API.Controllers
{
    [Route("api/v1/actors")]
    public class ActorController : BaseController
    {
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] Create.Command command)
        {
            await Mediator.Send(command);
            return NoContent();
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            var result = await Mediator.Send(new GetList.Query());
            return Ok(result);
        }

        [HttpGet("id")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await Mediator.Send(new GetById.Query(id));
            return Ok(result);
        }

        [HttpPut("id")]
        public async Task<IActionResult>Update([FromForm] Update.Command command)
        {
            await Mediator.Send(command);
            return NoContent();
        }


        //[HttpPost("{id}/addimage")]
        //public async Task<IActionResult> AddPhoto([FromRoute] int id , IFormFile image)
        //{
        //    await Mediator.Send(new UpdatePhoto.Command(id , image));
        //    return NoContent();
        //}

        [HttpGet("{id}/movies")]
        public async Task<IActionResult> GetMovies(int id)
        {
            var result = await Mediator.Send(new GetMovies.Query(id));
            return Ok(result);
        }
    }
}
