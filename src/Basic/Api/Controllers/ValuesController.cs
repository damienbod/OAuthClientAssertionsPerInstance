﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Authorize(Policy = "protectedScope")]
[Route("api/[controller]")]
public class ValuesController : Controller
{
    [HttpGet]
    public IEnumerable<string> Get()
    {
        // debugging info
        var authHeader = Request.Headers.Authorization;
        var claims = User.Claims.Select(c => new { c.Type, c.Value });

        return
        [
            "data 1 from the api protected using OAuth client assertions and standard AT",
            "data 2 from the api"
        ];
    }
}
