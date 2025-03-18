using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DPoPApiDefault.Controllers;

[Authorize(Policy = "protectedScope")]
[Route("api/[controller]")]
public class ValuesController : Controller
{
    [HttpGet]
    public IEnumerable<string> Get()
    {
        var sessionId = GetSessionId();
        // debugging info
        var authHeader = Request.Headers.Authorization;
        var claims = User.Claims.Select(c => new { c.Type, c.Value });

        return
        [
            "data 1 from the api protected using OAuth DPoP",
            $"data 2 from the api, sessionId from AT: {sessionId}"
        ];
    }

    private string? GetSessionId()
    {
        var sessionIdClaim = User.Claims.FirstOrDefault(c => c.Type == "scope" && c.Value.StartsWith("sessionId"));
        var sessionId = sessionIdClaim?.Value.Replace("sessionId:", "");
        return sessionId;
    }
}
