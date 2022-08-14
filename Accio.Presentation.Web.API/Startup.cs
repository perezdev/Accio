using Accio.Business.Services.CardSearchHistoryServices;
using Accio.Business.Services.CardServices;
using Accio.Business.Services.LanguageServices;
using Accio.Business.Services.LessonServices;
using Accio.Business.Services.SourceServices;
using Accio.Business.Services.AccountServices;
using Accio.Business.Services.AdvancedCardSearchSearchServices;
using Accio.Business.Services.ApiServices.UntapServices;
using Accio.Business.Services.AuthenticationHistoryServices;
using Accio.Business.Services.AuthenticationServices;
using Accio.Business.Services.ConfigurationServices;
using Accio.Business.Services.EmailServices;
using Accio.Business.Services.FormatServices;
using Accio.Business.Services.ImageServices;
using Accio.Business.Services.RoleServices;
using Accio.Business.Services.RulingRestrictionServices;
using Accio.Business.Services.RulingServices;
using Accio.Business.Services.TypeServices;
using Accio.Business.Services.AccountRoleServices;

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
            services.AddTransient<CardRulingService>();
            services.AddTransient<RulingService>();
            services.AddTransient<RulingSourceService>();
            services.AddTransient<RulingTypeService>();
            services.AddTransient<ConfigurationService>();
            services.AddTransient<CardSubTypeService>();
            services.AddTransient<SubTypeService>();
            services.AddTransient<CardProvidesLessonService>();
            services.AddTransient<AdvancedCardSearchService>();
            services.AddTransient<CardImageService>();
            services.AddTransient<ImageService>();
            services.AddTransient<ImageSizeService>();
            services.AddTransient<SingleCardService>();
            services.AddTransient<AccountService>();
            services.AddTransient<AuthenticationService>();
            services.AddTransient<AuthenticationHistoryService>();
            services.AddTransient<ConfigurationService>();
            services.AddTransient<EmailService>();
            services.AddTransient<AccountVerificationNumberService>();
            services.AddTransient<AccountVerificationService>();
            services.AddTransient<AuthenticationService>();
            services.AddTransient<AccountRoleService>();
            services.AddTransient<RoleService>();
            services.AddTransient<RulingRestrictionService>();
            services.AddTransient<CardRulingRestrictionService>();
            services.AddTransient<FormatService>();
            services.AddTransient<UntapService>();
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
