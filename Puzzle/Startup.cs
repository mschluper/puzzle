using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WordExercise.Services;

namespace Valant
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services
                .AddScoped<IEnglishDictionaryService, EnglishDictionaryService>()
                .AddRouting();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
            //app.UseMvc(routes =>
            //{
            //    routes.MapRoute("DoesWordExist", "Words/{board}/{word}",
            //             defaults: new { controller = "Words", action = "DoesWordExist" });
            //    routes.MapRoute("GetValidWords", "Words/{board}",
            //             defaults: new { controller = "Words", action = "GetValidWords" });
            //});
        }
    }
}
