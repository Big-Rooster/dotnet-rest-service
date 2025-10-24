namespace DotnetRestService.API.Dtos;

public class GetDotnetRestsResponse
{
    public List<DotnetRestDto> DotnetRests { get; set; } = new();
    public bool HasNext { get; set; }
    public bool HasPrevious { get; set; }
    public int NextPage { get; set; }
    public int PreviousPage { get; set; }
    public int TotalPages { get; set; }
    public long TotalElements { get; set; }
}