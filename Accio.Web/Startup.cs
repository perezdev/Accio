using Accio.Business.Services.CardSearchHistoryServices;
using Accio.Business.Services.CardServices;
using Accio.Business.Services.ConfigurationServices;
using Accio.Business.Services.LanguageServices;
using Accio.Business.Services.LessonServices;
using Accio.Business.Services.RulingServices;
using Accio.Business.Services.SourceServices;
using Accio.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;

namespace Accio.Web
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
            services.AddRazorPages().AddRazorRuntimeCompilation();
            services.AddAntiforgery(o => o.HeaderName = "XSRF-TOKEN");
            services.AddDbContext<AccioContext>(options => options.UseSqlServer(Configuration.GetConnectionString("AccioConnection"), sqlServerOptions => sqlServerOptions.CommandTimeout(300)));

            services.AddTransient<CardService>();
            services.AddTransient<SetService>();
            services.AddTransient<TypeService>();
            services.AddTransient<RarityService>();
            services.AddTransient<CardDetailService>();
            services.AddTransient<LanguageService>();
            services.AddTransient<LessonService>();
            services.AddTransient<CardSearchHistoryService>();
            services.AddTransient<SourceService>();
            services.AddTransient<CardRulingService>();
            services.AddTransient<RulingService>();
            services.AddTransient<RulingSourceService>();
            services.AddTransient<RulingTypeService>();
            services.AddTransient<ConfigurationService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = ctx =>
                {
                    const int durationInSeconds = 60 * 60 * 24;
                    ctx.Context.Response.Headers[HeaderNames.CacheControl] = "public,max-age=" + durationInSeconds;
                }
            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
