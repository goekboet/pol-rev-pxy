using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace authz
{
    public class Startup
    {
        IConfiguration Configuration;

        public Startup(
            IConfiguration conf
        )
        {
            Configuration = conf;
        }
        const string DevAuth = "DevAuth";
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(
            IServiceCollection services)
        {
            services.AddAuthentication(b => {
                b.DefaultScheme = DevAuth;
            }).AddScheme<DevAuthAuthenticationSchemeOptions, DevAuthAuthenticationHandler>(DevAuth, op => {});

            services.AddAuthorization();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app, 
            IWebHostEnvironment env)
        {
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            var routes = Configuration.GetSection("Routes").Get<Route[]>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Index");
                }).RequireAuthorization();

                foreach (var r in routes)
                {
                    endpoints.MapGet($"/{r.Url}", async context =>
                    {
                        await context.Response.WriteAsync(r.Url);
                    });
                }
            });
        }
    }
}
