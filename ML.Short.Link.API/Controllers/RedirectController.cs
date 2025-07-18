using Microsoft.AspNetCore.Mvc;

namespace ML.Short.Link.API.Controllers
{
    [ApiController]
    [Route("")]
    public class RedirectController : ControllerBase
    {
        [HttpGet("{shortCode}")]
        public async Task<IActionResult> RedirectUrl(string shortCode)
        {
            //var url = await _db.ShortUrls.FirstOrDefaultAsync(u => u.ShortCode == shortCode);
            //if (url == null) return NotFound();

            //url.ClickCount++;
            //await _db.SaveChangesAsync();

            return Redirect("https://localhost:7105/swagger/index.html");
        }
    }
}
