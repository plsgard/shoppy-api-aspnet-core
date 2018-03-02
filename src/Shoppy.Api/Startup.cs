using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shoppy.Application.Commons;
using Shoppy.Application.Items;
using Shoppy.Application.Lists;
using Shoppy.Core.Data;
using Shoppy.Data;
using Shoppy.Data.Repositories;

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
            services.AddDbContext<ShoppyContext>(options =>
                options.UseInMemoryDatabase("ShoppyDb")
            );

            services.AddMvc();

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

            app.UseMvc();
        }
    }
}
