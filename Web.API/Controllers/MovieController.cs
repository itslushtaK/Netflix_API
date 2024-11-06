
using Application.Features.Movies;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Web.API.Controllers
{
    [Route("api/v1/movies")]

    public class MovieController : BaseController
    {
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] Add.Command command)
        {
            await Mediator.Send(command);
            return NoContent();
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> GetList([FromHeader]string? name , Category? category)
        {
            var result = await Mediator.Send(new GetList.Query(name ,category));
            return Ok(result);
        }

        [HttpGet("id")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await Mediator.Send(new GetById.Query(id));
            return Ok(result);
        }

        [HttpPost("{id}/actors")]
        public async Task<IActionResult> AddActor(int id , IEnumerable<int> actorIds)
        {
            await Mediator.Send(new AddActor.Command(id, actorIds));
            return NoContent();
        }

        [HttpPost("{id}/addphoto")]

        public async Task<IActionResult> AddPhoto([FromRoute] int id, IFormFile photo)
        {
            await Mediator.Send(new AddUpdatePhoto.Command(id, photo));
            return NoContent();
        }

        [HttpPost("{id}/like")]
        public async Task<IActionResult> AddLike([FromRoute] int id, [FromForm] bool? like)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
           
            await Mediator.Send(new AddLike.Command(id, like, userId));
            return NoContent();
        }


        [HttpPut("id")]
        public async Task<IActionResult> Update([FromForm] Update.Command command)
        {
            await Mediator.Send(command);
            return NoContent();
        }

        [HttpGet("actors")]

        public async Task<IActionResult> getActors(int id)
        {
            var result = await Mediator.Send(new GetActorList.Query(id));
            return Ok(result);
        }

        [HttpGet("category")]
        public async Task<IActionResult> GetCategory()
        {
            var result = await Mediator.Send(new GetCategories.Query());
            return Ok(result);

        }

    }
}
