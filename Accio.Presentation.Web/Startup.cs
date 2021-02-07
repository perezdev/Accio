using Accio.Business.Services.AccountRoleServices;
using Accio.Business.Services.AccountServices;
using Accio.Business.Services.AdvancedCardSearchSearchServices;
using Accio.Business.Services.AuthenticationHistoryServices;
using Accio.Business.Services.AuthenticationServices;
using Accio.Business.Services.CardSearchHistoryServices;
using Accio.Business.Services.CardServices;
using Accio.Business.Services.ConfigurationServices;
using Accio.Business.Services.EmailServices;
using Accio.Business.Services.FormatServices;
using Accio.Business.Services.ImageServices;
using Accio.Business.Services.LanguageServices;
using Accio.Business.Services.LessonServices;
using Accio.Business.Services.RoleServices;
using Accio.Business.Services.RulingRestrictionServices;
using Accio.Business.Services.RulingServices;
using Accio.Business.Services.SourceServices;
using Accio.Business.Services.TypeServices;
using Accio.Data;
using Accio.Presentation.Web.PresentationServices;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddRazorPages().AddRazorRuntimeCompilation();
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => false;
                options.MinimumSameSitePolicy = Microsoft.AspNetCore.Http.SameSiteMode.None;
            });
            services.AddAntiforgery(o => o.HeaderName = "XSRF-TOKEN");
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddDbContext<AccioContext>(options => options.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()))
                                                                  .UseSqlServer(Configuration.GetConnectionString("AccioConnection"), sqlServerOptions => sqlServerOptions.CommandTimeout(300))
                                                );
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(
                CookieAuthenticationDefaults.AuthenticationScheme,
                options =>
            {
                options.Cookie.HttpOnly = true;
                options.LoginPath = "/Login";
            });

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
            services.AddTransient<ClaimService>();
            services.AddTransient<RulingRestrictionService>();
            services.AddTransient<CardRulingRestrictionService>();
            services.AddTransient<FormatService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseHsts();
            app.UseStatusCodePagesWithReExecute("/404");
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
            app.UseAuthentication();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
