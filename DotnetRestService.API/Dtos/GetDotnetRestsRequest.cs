using System.ComponentModel.DataAnnotations;

namespace DotnetRestService.API.Dtos;

public class GetDotnetRestsRequest
{
    [Range(1, int.MaxValue)]
    public int StartPage { get; set; } = 1;
    
    [Range(1, 100)]
    public int PageSize { get; set; } = 10;
}