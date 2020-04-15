using HpTcgCardBrowser.Business.Services.CardSearchHistoryServices;
using HpTcgCardBrowser.Business.Services.CardServices;
using HpTcgCardBrowser.Business.Services.LanguageServices;
using HpTcgCardBrowser.Business.Services.LessonServices;
using HpTcgCardBrowser.Business.Services.SourceServices;
using HpTcgCardBrowser.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HpTcgCardBrowser.Web.API
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

            services.AddDbContext<HpTcgContext>(options => options.UseSqlServer(Configuration.GetConnectionString("HpTcgConnection"), sqlServerOptions => sqlServerOptions.CommandTimeout(300)));

            services.AddTransient<CardService>();
            services.AddTransient<SetService>();
            services.AddTransient<TypeService>();
            services.AddTransient<RarityService>();
            services.AddTransient<CardDetailService>();
            services.AddTransient<LanguageService>();
            services.AddTransient<LessonService>();
            services.AddTransient<CardSearchHistoryService>();
            services.AddTransient<SourceService>();
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
    }
}
