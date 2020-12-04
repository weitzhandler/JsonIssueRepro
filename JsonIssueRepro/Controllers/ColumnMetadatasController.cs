using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Bogus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.Extensions.Logging;

namespace JsonIssueRepro.Controllers
{
    public class ColumnMetadatasController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<ColumnMetadatasController> _logger;

        public ColumnMetadatasController(ILogger<ColumnMetadatasController> logger)
        {
            _logger = logger;
        }

        [EnableQuery]
        public IEnumerable<ColumnMetadata> Get()
        {
            var count = new Randomizer().Number(min: 2, max: 5);

            return Enumerable
                .Range(start: 0, count)
                .Select(i => CreateRandomColumnMetadata());
        }

        public IActionResult Post([FromBody] ColumnMetadata value) =>
            value switch
            {
                null => BadRequest(),
                _ => Ok()
            };

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
    }
}