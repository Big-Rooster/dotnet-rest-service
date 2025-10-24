using System.ComponentModel.DataAnnotations;

namespace DotnetRestService.API.Dtos;

public class DotnetRestDto
{
    public string? Id { get; set; }
    
    [Required]
    public string Name { get; set; } = string.Empty;
}