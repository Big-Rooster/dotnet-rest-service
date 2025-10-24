using System.ComponentModel.DataAnnotations;

namespace DotnetRestService.API.Dtos;

public class DeleteDotnetRestRequest  
{
    [Required]
    public string Id { get; set; } = string.Empty;
}