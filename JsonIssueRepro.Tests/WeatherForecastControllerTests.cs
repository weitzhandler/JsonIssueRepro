using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Bogus;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace JsonIssueRepro.Tests
{
    public class WeatherForecastControllerTests
    {
        const string Endpoint = "api/columnmetadatas";

        [Fact]
        public async Task Should_get()
        {
            // arrange
            using var app = new WebApplicationFactory<Startup>();
            var jsonSerializerOptions = app.Services.GetRequiredService<IOptions<JsonSerializerOptions>>().Value;
            
            using var client = app.CreateDefaultClient();

            // act
            var result = await client.GetFromJsonAsync<ODataCollectionResult<ColumnMetadata>>(
                requestUri: Endpoint,
                options: jsonSerializerOptions);

            // assert     
            Assert.NotEmpty(result?.Value);
        }

        [Fact]
        public async Task Should_fail_on_post_data1()
        {
            // arrange
            var weatherForecast = CreateRandomColumnMetadata();
            using var app = new WebApplicationFactory<Startup>();
            var jsonSerializerOptions = app.Services.GetRequiredService<IOptions<JsonSerializerOptions>>().Value;

            using var client = app.CreateDefaultClient();

            // act
            var response = await client.PostAsJsonAsync(
                requestUri: Endpoint,
                value: weatherForecast,
                options: jsonSerializerOptions);

            // assert
            Assert.True(response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task Should_fail_on_post_data2()
        {
            // arrange
            var weatherForecast = CreateRandomColumnMetadata();
            using var app = new WebApplicationFactory<Startup>();
            var jsonSerializerOptions = app.Services.GetRequiredService<IOptions<JsonSerializerOptions>>().Value;

            using var client = app.CreateDefaultClient();
            var jsonContent = JsonContent.Create(weatherForecast, options: jsonSerializerOptions);

            // act
            var response = await client.PostAsync(Endpoint, jsonContent);

            // assert
            Assert.True(response.IsSuccessStatusCode);
        }


        [Fact]
        public async Task Should_success_on_post_data1()
        {
            // arrange
            var weatherForecast = CreateRandomColumnMetadata();
            using var app = new WebApplicationFactory<Startup>();
            var jsonSerializerOptions = app.Services.GetRequiredService<IOptions<JsonSerializerOptions>>().Value;

            using var client = app.CreateDefaultClient();
            var jsonContent = JsonContent.Create(weatherForecast, options: jsonSerializerOptions);
            // this line magically makes it work.
            await jsonContent.ReadAsStringAsync();

            // act
            var response = await client.PostAsync(Endpoint, jsonContent);

            // assert
            Assert.True(response.IsSuccessStatusCode);
        }

        static ColumnMetadata CreateRandomColumnMetadata()
        {
            var allIsoCodes =
              CultureInfo.GetCultures(CultureTypes.NeutralCultures)
              .Select(ci => ci.ThreeLetterISOLanguageName)
              .Distinct()
              .ToList();

            var languageTitles = new Faker<LanguageTitle>()
              .RuleFor(title => title.LanguageId, faker => faker.PickRandom(items: allIsoCodes))
              .RuleFor(title => title.Title, faker => faker.Lorem.Sentence(3))
              .RuleFor(title => title.Description, f => f.Lorem.Sentence(7))
              .Generate(3);

            var cmdFaker = new Faker<ColumnMetadata>()
              .RuleFor(cmd => cmd.Id, Guid.NewGuid())
              .RuleFor(cmd => cmd.LanguageTitles, languageTitles);

            return cmdFaker.Generate();
        }

        class ODataCollectionResult<T>
        {
            [JsonPropertyName("@odata.context")]
            public string? Metadata { get; set; }

            [JsonPropertyName("value")]
            public List<T>? Value { get; set; }
        }

    }
}
