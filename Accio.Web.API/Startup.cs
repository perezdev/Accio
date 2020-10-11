using Accio.Business.Services.CardSearchHistoryServices;
using Accio.Business.Services.CardServices;
using Accio.Business.Services.LanguageServices;
using Accio.Business.Services.LessonServices;
using Accio.Business.Services.SourceServices;
using Accio.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Accio.Presentation.Web.API
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
