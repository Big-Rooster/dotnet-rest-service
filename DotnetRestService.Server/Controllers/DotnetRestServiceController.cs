using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DotnetRestService.API.Dtos;
using DotnetRestService.Core;
using System.Diagnostics;

namespace DotnetRestService.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DotnetRestServiceController : ControllerBase
{
    private readonly ILogger<DotnetRestServiceController> _logger;
    private readonly DotnetRestServiceCore _service;
    
    public DotnetRestServiceController(DotnetRestServiceCore service, ILogger<DotnetRestServiceController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpPost]
    [Authorize(Roles = "admin,write")]
    public async Task<ActionResult<CreateDotnetRestResponse>> CreateDotnetRest([FromBody] DotnetRestDto request)
    {
        using var scope = _logger.BeginScope("REST: {Method}, User: {UserId}", 
            nameof(CreateDotnetRest), GetUserId());
            
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            _logger.LogDebug("REST CreateDotnetRest started for {Name}", request.Name);
            
            var response = await _service.CreateDotnetRest(request);
            
            stopwatch.Stop();
            _logger.LogInformation("REST CreateDotnetRest completed successfully in {Duration}ms", 
                stopwatch.ElapsedMilliseconds);
                
            return CreatedAtAction(nameof(GetDotnetRest), new { id = response.DotnetRest.Id }, response);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "REST CreateDotnetRest failed after {Duration}ms", 
                stopwatch.ElapsedMilliseconds);
            throw;
        }
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "admin,write,read")]
    public async Task<ActionResult<GetDotnetRestResponse>> GetDotnetRest(string id)
    {
        using var scope = _logger.BeginScope("REST: {Method}, User: {UserId}, Id: {Id}", 
            nameof(GetDotnetRest), GetUserId(), id);
            
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            _logger.LogDebug("REST GetDotnetRest started for ID {Id}", id);
            
            var response = await _service.GetDotnetRest(id);
            
            stopwatch.Stop();
            _logger.LogInformation("REST GetDotnetRest completed successfully in {Duration}ms", 
                stopwatch.ElapsedMilliseconds);
                
            return Ok(response);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "REST GetDotnetRest failed for ID {Id} after {Duration}ms", 
                id, stopwatch.ElapsedMilliseconds);
            throw;
        }
    }

    [HttpGet]
    [Authorize(Roles = "admin,write,read")]
    public async Task<ActionResult<GetDotnetRestsResponse>> GetDotnetRests([FromQuery] GetDotnetRestsRequest request)
    {
        using var scope = _logger.BeginScope("REST: {Method}, User: {UserId}, Page: {StartPage}, Size: {PageSize}", 
            nameof(GetDotnetRests), GetUserId(), request.StartPage, request.PageSize);
            
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            _logger.LogDebug("REST GetDotnetRests started for page {StartPage}, size {PageSize}", 
                request.StartPage, request.PageSize);
            
            var response = await _service.GetDotnetRests(request);
            
            stopwatch.Stop();
            _logger.LogInformation("REST GetDotnetRests completed successfully in {Duration}ms - returned {Count}/{Total} items", 
                stopwatch.ElapsedMilliseconds, response.DotnetRests.Count, response.TotalElements);
                
            return Ok(response);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "REST GetDotnetRests failed for page {StartPage}, size {PageSize} after {Duration}ms", 
                request.StartPage, request.PageSize, stopwatch.ElapsedMilliseconds);
            throw;
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "admin,write")]
    public async Task<ActionResult<UpdateDotnetRestResponse>> UpdateDotnetRest(string id, [FromBody] DotnetRestDto request)
    {
        request.Id = id; // Ensure the ID from the route is used
        
        using var scope = _logger.BeginScope("REST: {Method}, User: {UserId}, Id: {Id}", 
            nameof(UpdateDotnetRest), GetUserId(), request.Id);
            
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            _logger.LogDebug("REST UpdateDotnetRest started for ID {Id}", request.Id);
            
            var response = await _service.UpdateDotnetRest(request);
            
            stopwatch.Stop();
            _logger.LogInformation("REST UpdateDotnetRest completed successfully in {Duration}ms", 
                stopwatch.ElapsedMilliseconds);
                
            return Ok(response);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "REST UpdateDotnetRest failed for ID {Id} after {Duration}ms", 
                request.Id, stopwatch.ElapsedMilliseconds);
            throw;
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<DeleteDotnetRestResponse>> DeleteDotnetRest(string id)
    {
        using var scope = _logger.BeginScope("REST: {Method}, User: {UserId}, Id: {Id}", 
            nameof(DeleteDotnetRest), GetUserId(), id);
            
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            _logger.LogDebug("REST DeleteDotnetRest started for ID {Id}", id);
            
            var response = await _service.DeleteDotnetRest(id);
            
            stopwatch.Stop();
            _logger.LogInformation("REST DeleteDotnetRest completed successfully in {Duration}ms - deleted: {Deleted}", 
                stopwatch.ElapsedMilliseconds, response.Deleted);
                
            return Ok(response);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "REST DeleteDotnetRest failed for ID {Id} after {Duration}ms", 
                id, stopwatch.ElapsedMilliseconds);
            throw;
        }
    }
    
    /// <summary>
    /// Extracts user ID from HTTP context.
    /// </summary>
    private string? GetUserId()
    {
        // Try to get user ID from claims
        var userIdClaim = User.FindFirst("sub") ?? User.FindFirst("userId");
        if (userIdClaim != null)
            return userIdClaim.Value;
            
        // Try to get from headers as fallback
        if (Request.Headers.TryGetValue("X-User-Id", out var userId))
            return userId.FirstOrDefault();
            
        if (Request.Headers.TryGetValue("User-Id", out var userIdAlt))
            return userIdAlt.FirstOrDefault();
            
        return null;
    }
}