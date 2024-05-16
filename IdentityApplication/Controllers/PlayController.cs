using Azure.Messaging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IdentityApplication.API.Controllers
{

    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PlayController : ControllerBase
    {
        [HttpGet("get-players")]
        public IActionResult Play()
        {

            return Ok(new JsonResult(new {message = "Only authorize users can view playres" } ));
        }

    }
}
