using System.Net;
using System.Net.Http.Json;
using Api.Data;
using Api.Endpoints;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Tests.Endpoints;

[TestSubject(typeof(SwCharactersEndpoints))]
public class SwCharactersEndpointsTest : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> m_Factory;
    public SwCharactersEndpointsTest(WebApplicationFactory<Program> factory)
    {
        m_Factory = factory;
        
        // Reset Database
        try
        {
            File.Delete("app.db");
        }
        catch
        {
            // ignored
        }
    }
    
    [Fact]
    public async Task GetSwCharacters_ReturnsOk()
    {
        var client = m_Factory.CreateClient();
        var response = await client.GetAsync("/sw-characters");
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetSwCharacterById_ReturnsNotFound_ForInvalidId()
    {
        var client = m_Factory.CreateClient();
        var response = await client.GetAsync("/sw-characters/999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task AddSwCharacter_ReturnsCreated()
    {
        var client = m_Factory.CreateClient();
        var character = new SwCharacter
        {
            Name = "Character2",
            Faction = "Faction2",
            Homeworld = "HomeWorld2",
            Species = "Species3"
        };
        var response = await client.PostAsJsonAsync("/sw-characters", character);
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task AddSwCharacter_ReturnsConflict_ForDuplicateName()
    {
        var client = m_Factory.CreateClient();
        var character = new SwCharacter
        {
            Name = "Character1",
            Faction = "Faction1",
            Homeworld = "HomeWorld1",
            Species = "Species1"
        };
        await client.PostAsJsonAsync("/sw-characters", character);
        var response = await client.PostAsJsonAsync("/sw-characters", character);
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task UpdateSwCharacter_ReturnsOk()
    {
        var client = m_Factory.CreateClient();
        var character = new SwCharacter
        {
            Name = "Character1",
            Faction = "Faction2",
            Homeworld = "HomeWorld1",
            Species = "Species1"
        };
        var postResponse = await client.PostAsJsonAsync("/sw-characters", character);
        var createdCharacter = await postResponse.Content.ReadFromJsonAsync<SwCharacterEntity>();

        createdCharacter!.Name = "Leia Skywalker";
        var putResponse = await client.PutAsJsonAsync($"/sw-characters/{createdCharacter.Id}", createdCharacter);
        putResponse.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, putResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteSwCharacter_ReturnsOk()
    {
        var client = m_Factory.CreateClient();
        var character = new SwCharacter
        {
            Name = "Character5",
            Faction = "Faction1",
            Homeworld = "HomeWorld1",
            Species = "Species2"
        };
        var postResponse = await client.PostAsJsonAsync("/sw-characters", character);
        var createdCharacter = await postResponse.Content.ReadFromJsonAsync<SwCharacterEntity>();

        var deleteResponse = await client.DeleteAsync($"/sw-characters/{createdCharacter!.Id}");
        deleteResponse.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);
    }
}