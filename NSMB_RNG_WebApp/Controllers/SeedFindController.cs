using Microsoft.AspNetCore.Mvc;
using NSMB_RNG_WebApp.Models;

namespace NSMB_RNG_WebApp.Controllers
{
    [ApiController]
    [Route($"{Config.ApiRotuePrefix}seedfindingresults")]
    public class SeedFindController : ControllerBase
    {
        private readonly ILogger<SeedFindController> _logger;

        public SeedFindController(ILogger<SeedFindController> logger)
        {
            _logger = logger;
        }

        // TODO: I get a 500 internal server error (AmbiguousMatchException) if the post request's content doesn't match the model.
        // Obviously, such a request should be a 400 Bad Request instead.
        [HttpPost]
        public void LogSeedFindResults(SeedFindResultRaw result)
        {
            // TODO: Actually do stuff.
            Console.WriteLine("Received result data.");
            if (result == null)
                Console.WriteLine("Data is null");
            else if (result.macInput == null)
                Console.WriteLine("macInput is null");
            else
            {
                Console.WriteLine(result.macInput);
                Console.WriteLine(result.consoleType);
                Console.WriteLine(result.seed);
            }
        }
    }
}