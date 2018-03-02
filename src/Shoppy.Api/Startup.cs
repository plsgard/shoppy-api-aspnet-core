using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shoppy.Api.Configurations;
using Shoppy.Application.Commons;
using Shoppy.Application.Items;
using Shoppy.Application.Lists;
using Shoppy.Core.Data;
using Shoppy.Data;
using Shoppy.Data.Repositories;
using Swashbuckle.AspNetCore.Swagger;

namespace Shoppy.Api
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
            ConfigureInjection(services);

            services.AddMvc();
            services.AddAutoMapper();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Title = "Shoppy API",
                    Version = "v1",
                    Description = "A simple API for Shoppy App.",
                    Contact = new Contact
                    {
                        Email = "hello@plsgd.com",
                        Name = "Pier-Lionel Sgard",
                        Url = "http://plsgd.com"
                    }
                });

                c.DocumentFilter<LowercaseDocumentFilter>();

                // Set the comments path for the Swagger JSON and UI.
                var basePath = AppContext.BaseDirectory;
                foreach (var file in Directory.GetFiles(basePath, "*.xml"))
                    c.IncludeXmlComments(file);
            });
        }

        private void ConfigureInjection(IServiceCollection services)
        {
            services.AddDbContext<ShoppyContext>(options =>
                options.UseInMemoryDatabase("ShoppyDb")
            );

            services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
            services.AddTransient(typeof(IAppService<,,,>), typeof(AppService<,,,,>));
            services.AddTransient<IItemAppService, ItemAppService>();
            services.AddTransient<IListAppService, ListAppService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            app.UseMvc();
        }
    }
}
