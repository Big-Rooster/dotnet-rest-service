using DotnetRestService.API.Dtos;

namespace DotnetRestService.Core.Services;

/// <summary>
/// Service for validating requests and business rules
/// </summary>
public interface IValidationService
{
    /// <summary>
    /// Validates a DotnetRest creation request
    /// </summary>
    void ValidateCreateRequest(DotnetRestDto request);

    /// <summary>
    /// Validates a DotnetRest update request
    /// </summary>
    void ValidateUpdateRequest(DotnetRestDto request);

    /// <summary>
    /// Validates pagination parameters
    /// </summary>
    void ValidatePaginationRequest(GetDotnetRestsRequest request);

    /// <summary>
    /// Validates an entity ID
    /// </summary>
    Guid ValidateAndParseId(string id, string fieldName = "Id");
}