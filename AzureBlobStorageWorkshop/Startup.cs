using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using AzureBlobStorageWorkshop.Services.Interfaces;
using AzureBlobStorageWorkshop.Services;
using NLog.Web;
using NLog.Extensions.Logging;
using Microsoft.AspNetCore.Http;

namespace AzureBlobStorageWorkshop
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
            string cloudStorageConnection = Configuration["Data:CloudStorageConnectionString:CloudDataStorage"];

            //TODO read - think https://medium.com/volosoft/asp-net-core-dependency-injection-best-practices-tips-tricks-c6e9c67f9d96
            services.AddScoped<IBlobStorageService>(s => new BlobStorageService(cloudStorageConnection));
            services.AddScoped<IQueueStorageService>(s => new QueueStorageService(cloudStorageConnection));
            services.AddScoped<IImageService, ImageService>();

            //needed for NLog.Web
            //services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            //add NLog to ASP.NET Core
            //loggerFactory.AddNLog();

            //add NLog.Web
            //app.AddNLogWeb();

            //app.AddNLogWeb();
            // env.ConfigureNLog(env.WebRootPath + "/nlog.config");//("Nlog.config");          

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
