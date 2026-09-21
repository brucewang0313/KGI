using Microsoft.AspNetCore.Mvc;
using WarrantRisk.Domain;

namespace WarrantRisk.Api;

[ApiController, Route("api/warrants")]
public sealed class WarrantsController(WarrantService service) : ControllerBase
{
    [HttpGet] public async Task<ActionResult<List<Warrant>>> Search([FromQuery] string? keyword) => Ok(await service.SearchAsync(keyword));
    [HttpGet("{id}")] public async Task<ActionResult<Warrant>> Get(string id)
    {
        var warrant = (await service.SearchAsync(id)).SingleOrDefault(x => x.WarrantId == id);
        return warrant is null ? NotFound() : Ok(warrant);
    }
    [HttpGet("{id}/trials/recent")] public async Task<ActionResult<List<TrialLog>>> Recent(string id) => Ok(await service.RecentAsync(id));
    [HttpPost("{id}/trials")]
    public async Task<IActionResult> Trial(string id, [FromBody] TrialRequest request)
    {
        if (request.MarketPrice <= 0) return BadRequest(new { message = "標的股價必須大於 0。" });
        try { var result = await service.TrialAsync(id, request.MarketPrice); return Ok(result.Result); }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        catch (ArgumentOutOfRangeException ex) { return BadRequest(new { message = ex.Message }); }
    }
}
