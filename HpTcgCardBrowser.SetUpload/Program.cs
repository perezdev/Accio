using HpTcgCardBrowser.Business.Services.CardServices;
using HpTcgCardBrowser.Business.Services.LanguageServices;
using HpTcgCardBrowser.Data;
using HpTcgCardBrowser.SetUpload.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.IO;

namespace HpTcgCardBrowser.SetUpload
{
    class Program
    {
        private static IConfiguration _configuration { get; set; }

        private static CardService _cardService { get; set; }
        private static CardSetService _cardSetService { get; set; }
        private static CardTypeService _cardTypeService { get; set; }
        private static CardRarityService _cardRarityService { get; set; }
        private static CardDetailService _cardDetailService { get; set; }
        private static LanguageService _languageService { get; set; }

        private static void RegisterServices()
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"appsettings.json", true, true)
                .AddJsonFile($"appsettings.{environmentName}.json", true, true)
                .AddEnvironmentVariables();
            _configuration = builder.Build();

            var services = new ServiceCollection();

            services.AddDbContext<HpTcgContext>(options => options.UseSqlServer(_configuration.GetConnectionString("HpTcgConnection"), sqlServerOptions => sqlServerOptions.CommandTimeout(300)));
            services.AddSingleton(_configuration);
            services.AddTransient<CardService>();
            services.AddTransient<CardSetService>();
            services.AddTransient<CardTypeService>();
            services.AddTransient<CardRarityService>();
            services.AddTransient<CardDetailService>();
            services.AddTransient<LanguageService>();

            var provider = services.BuildServiceProvider();
            _cardService = provider.GetService<CardService>();
            _cardSetService = provider.GetService<CardSetService>();
            _cardTypeService = provider.GetService<CardTypeService>();
            _cardRarityService = provider.GetService<CardRarityService>();
            _cardDetailService = provider.GetService<CardDetailService>();
            _languageService = provider.GetService<LanguageService>();
        }

        private static void Main(string[] args)
        {
            RegisterServices();

            var baseSet = GetSet(SetType.Base);
            var adventureAtHogwartsSet = GetSet(SetType.AdventureAtHogwarts);
            var chamberOfSecretsSet = GetSet(SetType.ChamberOfSecrets);
            var diagonAlleySet = GetSet(SetType.DiagonAlley);
            var quidditchSet = GetSet(SetType.QuidditchCup);
        }

        private static SetModel GetSet(SetType setType)
        {
            switch (setType)
            {
                case SetType.Base:
                    var baseSetJsonUrl = "https://raw.githubusercontent.com/Tressley/hpjson/master/sets/base/cards.json";
                    var baseSetJson = GetJson(baseSetJsonUrl);
                    
                    return JsonConvert.DeserializeObject<SetModel>(baseSetJson);
                case SetType.AdventureAtHogwarts:
                    var aahSetJsonUrl = "https://raw.githubusercontent.com/Tressley/hpjson/master/sets/adventures%20at%20hogwarts/cards.json";
                    var aahSetJson = GetJson(aahSetJsonUrl);
                    
                    return JsonConvert.DeserializeObject<SetModel>(aahSetJson);
                case SetType.ChamberOfSecrets:
                    var cosSetJsonUrl = "https://raw.githubusercontent.com/Tressley/hpjson/master/sets/chamber%20of%20secrets/cards.json";
                    var cosSetJson = GetJson(cosSetJsonUrl);
                    
                    return JsonConvert.DeserializeObject<SetModel>(cosSetJson);
                case SetType.DiagonAlley:
                    var diagonAlleySetJsonUrl = "https://raw.githubusercontent.com/Tressley/hpjson/master/sets/diagon%20alley/cards.json";
                    var diagonAlleySetJson = GetJson(diagonAlleySetJsonUrl);

                    return JsonConvert.DeserializeObject<SetModel>(diagonAlleySetJson);
                case SetType.QuidditchCup:
                    var quidditchSetJsonUrl = "https://raw.githubusercontent.com/Tressley/hpjson/master/sets/quidditch%20cup/cards.json";
                    var quidditchSetJson = GetJson(quidditchSetJsonUrl);

                    return JsonConvert.DeserializeObject<SetModel>(quidditchSetJson);
                default:
                    return null;
            }
        }

        public static string GetJson(string url)
        {
            var contents = string.Empty;
            using (var wc = new System.Net.WebClient())
            {
                contents = wc.DownloadString(url);
            }

            return contents;
        }
    }
    public enum SetType
    {
        Base,
        AdventureAtHogwarts,
        ChamberOfSecrets,
        DiagonAlley,
        QuidditchCup,
    }

}
