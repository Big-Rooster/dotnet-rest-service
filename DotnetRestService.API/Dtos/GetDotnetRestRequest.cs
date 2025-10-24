using System.ComponentModel.DataAnnotations;

namespace DotnetRestService.API.Dtos;

public class GetDotnetRestRequest
{
    [Required]
    public string Id { get; set; } = string.Empty;
}