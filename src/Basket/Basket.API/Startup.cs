using AutoMapper;
using Basket.API.Data;
using Basket.API.Data.Interfaces;
using Basket.API.Repositories;
using Basket.API.Repositories.Interfaces;
using EventBusRabbitMQ;
using EventBusRabbitMQ.Producer;
using EventBusRabbitMQ.Producer.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using RabbitMQ.Client;
using StackExchange.Redis;

namespace Basket.API
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
            services.AddSingleton(s =>
            {
                var config = ConfigurationOptions.Parse(Configuration.GetConnectionString("Redis"), true);
                return ConnectionMultiplexer.Connect(config);
            });

            services.AddTransient<IBasketContext, BasketContext>();
            services.AddTransient<IBasketRepository, BasketRepository>();
            services.AddAutoMapper(typeof(Startup));
            services.AddSingleton<IEventBusRabbitMQProducer, EventBusRabbitMQProducer>();

            services.AddSwaggerGen(s =>
            {
                s.SwaggerDoc("v1", new OpenApiInfo { Title = "Basket API", Version = "v1" });
            });

            services.AddSingleton<IRabbitMQConnection>(ServiceProvider =>
            {
                var factory = new ConnectionFactory
                {
                    HostName = Configuration["EventBus:HostName"]
                };

                if (!string.IsNullOrEmpty(Configuration["EventBus:UserName"]))
                    factory.UserName = Configuration["EventBus:UserName"];

                if (!string.IsNullOrEmpty(Configuration["EventBus:Password"]))
                    factory.UserName = Configuration["EventBus:Password"];

                return new RabbitMQConnection(factory);
            });

            services.AddControllers();
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
                s.SwaggerEndpoint("/swagger/v1/swagger.json", "Basket API V1");
            });
        }
    }
}
