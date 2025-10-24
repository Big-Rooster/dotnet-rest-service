using DotnetRestService.API;
using DotnetRestService.API.Dtos;
using DotnetRestService.API.Logger;
using DotnetRestService.Core.Services;
using DotnetRestService.Core.Exceptions;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Collections.Concurrent;

namespace DotnetRestService.Core;

public class DotnetRestServiceCore : IDotnetRestServiceService
{
    private readonly IValidationService _validationService;
    private readonly ILogger<DotnetRestServiceCore> _logger;
    private readonly ConcurrentDictionary<string, DotnetRestDto> _inMemoryStore = new();
       
    public DotnetRestServiceCore(
        IValidationService validationService,
        ILogger<DotnetRestServiceCore> logger) 
    {
        _validationService = validationService;
        _logger = logger;
    }

    public Task<CreateDotnetRestResponse> CreateDotnetRest(DotnetRestDto request)
    {
        var id = string.IsNullOrEmpty(request.Id) ? Guid.NewGuid().ToString() : request.Id;
        var dotnetRest = new DotnetRestDto
        {
            Id = id,
            Name = request.Name
        };
        _inMemoryStore[id] = dotnetRest;

        return Task.FromResult(new CreateDotnetRestResponse
        {
            DotnetRest = dotnetRest
        });
    }

    public Task<GetDotnetRestsResponse> GetDotnetRests(GetDotnetRestsRequest request)
    {
        var allDotnetRests = _inMemoryStore.Values.ToList();
        var pageSize = request.PageSize > 0 ? request.PageSize : 10;
        var startPage = request.StartPage > 0 ? request.StartPage : 1;

        var totalElements = allDotnetRests.Count;
        var totalPages = (int)Math.Ceiling((double)totalElements / pageSize);

        var skip = (startPage - 1) * pageSize;
        var pagedDotnetRests = allDotnetRests
            .Skip(skip)
            .Take(pageSize)
            .ToList();

        return Task.FromResult(new GetDotnetRestsResponse
        {
            DotnetRests = pagedDotnetRests,
            TotalElements = totalElements,
            TotalPages = totalPages,
        });
    }

    public Task<GetDotnetRestResponse> GetDotnetRest(string id)
    { 
        if (!_inMemoryStore.TryGetValue(id, out var dotnetRest))
        {
            throw new EntityNotFoundException("DotnetRest", id);
        }

        return Task.FromResult(new GetDotnetRestResponse
        {
            DotnetRest = dotnetRest
        });
    }

    public Task<UpdateDotnetRestResponse> UpdateDotnetRest(DotnetRestDto dotnetRest)
    {
        if (string.IsNullOrEmpty(dotnetRest.Id) || !_inMemoryStore.ContainsKey(dotnetRest.Id))
        {
            throw new EntityNotFoundException("DotnetRest", dotnetRest.Id ?? "null");
        }

        _inMemoryStore[dotnetRest.Id] = dotnetRest;

        return Task.FromResult(new UpdateDotnetRestResponse
        {
            DotnetRest = dotnetRest
        });
    }

    public Task<DeleteDotnetRestResponse> DeleteDotnetRest(string id)
    {
        if (!_inMemoryStore.TryRemove(id, out _))
        {
            throw new EntityNotFoundException("DotnetRest", id);
        }

        return Task.FromResult(new DeleteDotnetRestResponse { Deleted = true });
    }
}