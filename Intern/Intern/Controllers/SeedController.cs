using Intern.Services;
using Microsoft.AspNetCore.Mvc;

namespace Intern.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeedController : ControllerBase
    {
        private readonly DatabaseProcess _dbProcess;
        
        public SeedController(DatabaseProcess dbProcess)
        {
            _dbProcess = dbProcess;
        }

        [HttpPost("Initialize")]
        public async Task<IActionResult> Initialize()
        {
            var response = await _dbProcess.Seeder();
            return Ok(response);
        }
    }
}
