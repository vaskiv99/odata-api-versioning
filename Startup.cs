using Microsoft.AspNet.OData.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Examples.Interfaces;
using Microsoft.OpenApi.Models;

namespace Microsoft.Examples
{
    using Microsoft.AspNet.OData.Builder;
    using Microsoft.AspNet.OData.Extensions;
    using AspNetCore.Builder;
    using Microsoft.Extensions.Configuration;
    using Extensions.DependencyInjection;

    public class Startup
    {
        public Startup( IConfiguration configuration )
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices( IServiceCollection services )
        {
            services.AddControllers();
            services.AddApiVersioning(
                options =>
                {
                    // reporting api versions will return the headers "api-supported-versions" and "api-deprecated-versions"
                    options.ReportApiVersions = true;
                } );
            services.AddOData().EnableApiVersioning();
            services.AddVersionedApiExplorer();
            services.AddODataApiExplorer(x => x.SubstituteApiVersionInUrl = true );

            services.AddScoped<IODataFeature>( container => container.GetRequiredService<IHttpContextAccessor>().HttpContext.ODataFeature() );
            services.AddScoped<IODataQueryValuesExtractor, ODataQueryValuesExtractor>();
            services.AddScoped<IODataQueryOptionsSlimBuilder, ODataQueryOptionsSlimBuilder>();

            ConfigureSwaggerService( services );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure( IApplicationBuilder app, VersionedODataModelBuilder modelBuilder )
        {
            app.UseRouting();
            app.UseEndpoints(
                endpoints =>
                {
                    endpoints.MapControllers();


                    endpoints.MapVersionedODataRoute( "odata-bypath", "api/v{version:apiVersion}", modelBuilder );
                } );

            var provider = app.ApplicationServices.GetRequiredService<IApiVersionDescriptionProvider>();
            app.UseSwagger();
            app.UseSwaggerUI( c =>
            {
                foreach ( var description in provider.ApiVersionDescriptions )
                {
                    c.SwaggerEndpoint( $"/swagger/{description.GroupName}/swagger.json", $"REST API v{description.ApiVersion}" );
                }
            } );
        }

        public void ConfigureSwaggerService( IServiceCollection services )
        {
            var swaggerConfig = new OpenApiInfo
            {
                Title = "OData API versioned"
            };

            services.AddSwaggerGen( c =>
            {
                using var serviceProvider = services.BuildServiceProvider();
                var provider = serviceProvider.GetRequiredService<IApiVersionDescriptionProvider>();

                foreach ( var description in provider.ApiVersionDescriptions )
                {
                    swaggerConfig.Version = description.ApiVersion.ToString();
                    c.SwaggerDoc( $"{description.GroupName}", swaggerConfig );
                }
                c.OperationFilter<SwaggerExtensions.ODataQueryOptionsSlimFilter>( );
                c.DescribeAllParametersInCamelCase();
            } );
        }
    }
}