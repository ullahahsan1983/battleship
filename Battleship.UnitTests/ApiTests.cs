using Battleship.Core;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Xunit;

namespace Battleship.UnitTests;

public class ApiTests
{
    public readonly JsonSerializerOptions _options = new(JsonSerializerDefaults.Web);

    public ApiTests()
    {
        _options.Converters.Add(new JsonStringEnumConverter());
    }

    [Fact]
    public async Task WhenCallsStartNewGame_ThenReturnsGameReport()
    {
        await using var api = new WebApplicationFactory<Program>();

        var client = api.CreateClient();
        var result = await client.PostAsync("/battleship/StartNewGame", new JsonContent(null));

        var gameReport = await Json<GameReport>(result);

        Assert.NotNull(gameReport);
    }

    async Task<T> Json<T>(HttpResponseMessage message)
    {
        var json = await message.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<T>(json, _options);
    }

    public class JsonContent : StringContent
    {
        public JsonContent(object? obj) :
            base(obj is null ? string.Empty : JsonSerializer.Serialize(obj), Encoding.UTF8, "application/json")
        { }
    }
}