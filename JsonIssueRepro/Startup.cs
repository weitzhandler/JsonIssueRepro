using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.OData;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OData.Edm;

namespace JsonIssueRepro
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddControllers()
                .AddJsonOptions(jsonOptions =>
                {
                    ConfigureJsonSerializerOptions(jsonOptions.JsonSerializerOptions);
                });

            services
                .Configure<JsonSerializerOptions>(ConfigureJsonSerializerOptions);

            services.AddTransient<EdmModelBuilder>();
            services.AddTransient(serviceProvider =>
              serviceProvider.GetRequiredService<EdmModelBuilder>().GetEdmModel());

            services
              .AddOptions<ODataOptions>()
              .Configure<IEdmModel>((odataOptions, edmModel) =>
                odataOptions                
                .Select()
                .Expand()
                .Filter()
                .OrderBy()
                .SetMaxTop(128)
                .Count()
                .AddModel("api", edmModel));

            services
              .AddOData(options => {

              });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        static void ConfigureJsonSerializerOptions(JsonSerializerOptions options)
        {
            options.Converters.Add(new JsonStringEnumConverter());
            options.IgnoreNullValues = true;
            options.WriteIndented = true;
        }
    }
}
