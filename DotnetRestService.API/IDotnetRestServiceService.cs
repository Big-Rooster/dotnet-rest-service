using DotnetRestService.API.Dtos;

namespace DotnetRestService.API;

public interface IDotnetRestServiceService
{
    Task<CreateDotnetRestResponse> CreateDotnetRest(DotnetRestDto request);
    Task<GetDotnetRestsResponse> GetDotnetRests(GetDotnetRestsRequest request);
    Task<GetDotnetRestResponse> GetDotnetRest(string id);
    Task<UpdateDotnetRestResponse> UpdateDotnetRest(DotnetRestDto request);
    Task<DeleteDotnetRestResponse> DeleteDotnetRest(string id);
}