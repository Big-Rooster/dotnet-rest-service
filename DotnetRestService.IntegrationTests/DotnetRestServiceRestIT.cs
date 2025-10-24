using System.Net;
using System.Net.Http;
using DotnetRestService.API;
using DotnetRestService.API.Dtos;
using DotnetRestService.Client;
using Xunit.Abstractions;
using Xunit;

namespace DotnetRestService.IntegrationTests;

[Collection("ApplicationCollection")]
[Trait("Category", "Integration")]
public class DotnetRestServiceRestIt(ITestOutputHelper testOutputHelper, ApplicationFixture applicationFixture)
{
    private readonly ApplicationFixture _applicationFixture = applicationFixture;
    private readonly DotnetRestServiceClient _client = applicationFixture.GetClient();
    [Fact]
    public async Task Test_CreateDotnetRest()
    {
        //Arrange
    
        //Act
        var createRequest = new DotnetRestDto { Name = Guid.NewGuid().ToString() };
        var response = await _client.CreateDotnetRest(createRequest);
        
        //Assert
        var dto = response.DotnetRest;
        Assert.NotNull(dto.Id);
        Assert.Equal(createRequest.Name, dto.Name);
    }
    
    [Fact]
    public async Task Test_GetDotnetRests()
    {
        testOutputHelper.WriteLine("Test_GetDotnetRests");
        
        //Arrange
        var beforeTotal = (await _client.GetDotnetRests(new GetDotnetRestsRequest {StartPage = 1, PageSize = 4})).TotalElements;
        
        //Act
        var createRequest = new DotnetRestDto { Name = Guid.NewGuid().ToString() };
        await _client.CreateDotnetRest(createRequest);
        var response = await _client.GetDotnetRests(new GetDotnetRestsRequest {StartPage = 1, PageSize = 4});
        
        //Assert
        
        Assert.Equal(beforeTotal + 1, response.TotalElements);
    }
    
    [Fact]
    public async Task Test_GetDotnetRest()
    {
        //Arrange
        var request = new DotnetRestDto { Name = Guid.NewGuid().ToString() };
        var createResponse = await _client.CreateDotnetRest(request);
    
        //Act
        var response = await _client.GetDotnetRest(createResponse.DotnetRest.Id!);
        
        //Assert
        var dto = response.DotnetRest;
        Assert.NotNull(dto.Id);
        Assert.Equal(request.Name, dto.Name);
    }

    [Fact]
    public async Task Test_UpdateDotnetRest()
    {
        //Arrange
        var request = new DotnetRestDto { Name = Guid.NewGuid().ToString() };
        var createResponse = await _client.CreateDotnetRest(request);
    
        //Act
        var response = await _client.UpdateDotnetRest(new DotnetRestDto() {Id = createResponse.DotnetRest.Id, Name = "Updated"});
        
        //Assert
        var dto = response.DotnetRest;
        Assert.NotNull(dto.Id);
        Assert.Equal("Updated", response.DotnetRest.Name);
    }
    
    [Fact]
    public async Task Test_DeleteDotnetRest()
    {
        //Arrange
        var request = new DotnetRestDto { Name = Guid.NewGuid().ToString() };
        var createResponse = await _client.CreateDotnetRest(request);
    
        //Act
        var response = await _client.DeleteDotnetRest(createResponse.DotnetRest.Id!);
        
        //Assert
        Assert.True(response.Deleted);
    }

    [Fact]
    public async Task Test_DeleteDotnetRest_NotFound()
    {
        //Arrange

        //Act
        var exception = await Assert.ThrowsAsync<HttpRequestException>(async () => 
        {
            await _client.DeleteDotnetRest(Guid.NewGuid().ToString());
        });
       
        //Assert
        Assert.Contains("404", exception.Message);
    }

}