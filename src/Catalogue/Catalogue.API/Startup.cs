using Catalogue.API.Data;
using Catalogue.API.Data.Interfaces;
using Catalogue.API.Repositories;
using Catalogue.API.Repositories.Interfaces;
using Catalogue.API.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

namespace Catalogue.API
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
            services.AddControllers();

            services.Configure<CatalogueDatabaseSettings>(Configuration.GetSection(nameof(CatalogueDatabaseSettings)));
            services.AddSingleton<ICatalogueDatabaseSettings>(s => s.GetRequiredService<IOptions<CatalogueDatabaseSettings>>().Value);

            services.AddTransient<ICatalogueContext, CatalogueContext>();
            services.AddTransient<IProductRepository, ProductRepository>();

            services.AddSwaggerGen(s =>
            {
                s.SwaggerDoc("v1", new OpenApiInfo { Title = "Catalogue API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();
            app.UseSwaggerUI(s =>
            {
                s.SwaggerEndpoint("/swagger/v1/swagger.json", "Catalogue API V1");
            });
        }
    }
}
