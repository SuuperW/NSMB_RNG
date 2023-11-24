using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NSMB_RNG_WebApp.Controllers
{
    [ApiController]
    [Route($"{CONST.ApiRotuePrefix}seeds")]
    public class SeedsController : ControllerBase
    {
        [HttpGet]
        [Route("{row1:alpha}")]
        public uint[] Seeds(string row1)
        {
            // TODO
            return new uint[] { 0, 5, 1298 };
        }
    }
}
