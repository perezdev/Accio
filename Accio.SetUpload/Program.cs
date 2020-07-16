using Accio.Business.Models.ImportModels;
using Accio.Business.Services.CardSearchHistoryServices;
using Accio.Business.Services.CardServices;
using Accio.Business.Services.LanguageServices;
using Accio.Business.Services.LessonServices;
using Accio.Business.Services.TypeServices;
using Accio.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Accio.SetUpload
{
    class Program
    {
        private static IConfiguration _configuration { get; set; }

        private static CardService _cardService { get; set; }
        private static SetService _cardSetService { get; set; }
        private static TypeService _cardTypeService { get; set; }
        private static RarityService _cardRarityService { get; set; }
        private static CardDetailService _cardDetailService { get; set; }
        private static LanguageService _languageService { get; set; }
        private static LessonService _lessonService { get; set; }
        private static CardSearchHistoryService _cardSearchHistoryService { get; set; }
        private static CardSubTypeService _cardSubTypeService { get; set; }
        private static SubTypeService _subTypeService { get; set; }
        private static CardProvidesLessonService _cardProvidesLessonService { get; set; }

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

            services.AddDbContext<AccioContext>(options => options.UseSqlServer(_configuration.GetConnectionString("AccioConnection"), sqlServerOptions => sqlServerOptions.CommandTimeout(300)));
            services.AddSingleton(_configuration);
            services.AddTransient<CardService>();
            services.AddTransient<SetService>();
            services.AddTransient<TypeService>();
            services.AddTransient<RarityService>();
            services.AddTransient<CardDetailService>();
            services.AddTransient<LanguageService>();
            services.AddTransient<LessonService>();
            services.AddTransient<CardSearchHistoryService>();
            services.AddTransient<CardSubTypeService>();
            services.AddTransient<SubTypeService>();
            services.AddTransient<CardProvidesLessonService>();

            var provider = services.BuildServiceProvider();
            _cardService = provider.GetService<CardService>();
            _cardSetService = provider.GetService<SetService>();
            _cardTypeService = provider.GetService<TypeService>();
            _cardRarityService = provider.GetService<RarityService>();
            _cardDetailService = provider.GetService<CardDetailService>();
            _languageService = provider.GetService<LanguageService>();
            _lessonService = provider.GetService<LessonService>();
            _cardSearchHistoryService = provider.GetService<CardSearchHistoryService>();
            _cardSubTypeService = provider.GetService<CardSubTypeService>();
            _subTypeService = provider.GetService<SubTypeService>();
            _cardProvidesLessonService = provider.GetService<CardProvidesLessonService>();
        }

        private static void Main(string[] args)
        {
            RegisterServices();
            //ImportSets();
            //ImportSubTypes();
            //ImportMatches();
            ImportCardProvidesLessons();
        }
        private static void ImportCardProvidesLessons()
        {
            var sets = GetSets();
            var cards = _cardService.GetAllCards();
            var lessons = _lessonService.GetLessonTypes();

            foreach (var card in cards)
            {
                var set = sets.Single(x => x.Name == card.CardSet.Name);
                var jsonCard = set.Cards.SingleOrDefault(x => x.Name == card.Detail.Name && (card.Detail.Name != "Hermione Granger" && card.Detail.Name != "Draco Malfoy"));
                if (jsonCard?.Provides?.Length == 2)
                {
                    var lesson = lessons.Single(x => x.Name == jsonCard.Provides[1]);
                    _cardProvidesLessonService.PersistCardProvidesLesson(card.CardId, lesson.LessonTypeId, Convert.ToInt32(jsonCard.Provides[0]));
                }
            }

        }
        private static void ImportMatches()
        {
            var sets = GetSets();
            var cards = _cardService.GetAllCards().Where(x => x.CardType.Name == "Match");
            foreach (var card in cards)
            {
                var set = sets.Single(x => x.Name == card.CardSet.Name);
                var jsonCard = set.Cards.SingleOrDefault(x => x.Name == card.Detail.Name);

                var toWin = jsonCard.Description.ToWin;
                var prize = jsonCard.Description.Prize;

                _cardService.UpdateMatchData(card.CardId, toWin, prize);
            }
        }
        private static void ImportSubTypes()
        {
            var sets = GetSets();
            var subTypes = _subTypeService.GetAllSubTypes();

            foreach (var card in _cardService.GetAllCards())
            {
                var set = sets.Single(x => x.Name == card.CardSet.Name);
                var jsonCard = set.Cards.SingleOrDefault(x => x.Name == card.Detail.Name && (card.Detail.Name != "Hermione Granger" && card.Detail.Name != "Draco Malfoy"));

                if (jsonCard == null)
                {
                    Console.WriteLine($"{card.Detail.Name}");
                }
                else
                {
                    if (jsonCard.SubTypes != null && jsonCard.SubTypes.Length > 0)
                    {
                        foreach (var jsonSubType in jsonCard.SubTypes)
                        {
                            var subType = subTypes.Single(x => x.Name == jsonSubType);
                            _cardSubTypeService.PersistCardSubType(card.CardId, subType.SubTypeId);
                        }
                    }
                }
            }
        }
        private static void ImportSets()
        {
            var sets = GetSets();
            _cardService.ImportCardsFromSets(sets);
        }
        private static ImportSetModel GetSet(SetType setType)
        {
            switch (setType)
            {
                case SetType.Base:
                    var baseSetJsonUrl = "https://raw.githubusercontent.com/Tressley/hpjson/master/sets/base/cards.json";
                    var baseSetJson = GetJson(baseSetJsonUrl);

                    return JsonConvert.DeserializeObject<ImportSetModel>(baseSetJson);
                case SetType.AdventureAtHogwarts:
                    var aahSetJsonUrl = "https://raw.githubusercontent.com/Tressley/hpjson/master/sets/adventures%20at%20hogwarts/cards.json";
                    var aahSetJson = GetJson(aahSetJsonUrl);

                    return JsonConvert.DeserializeObject<ImportSetModel>(aahSetJson);
                case SetType.ChamberOfSecrets:
                    var cosSetJsonUrl = "https://raw.githubusercontent.com/Tressley/hpjson/master/sets/chamber%20of%20secrets/cards.json";
                    var cosSetJson = GetJson(cosSetJsonUrl);

                    return JsonConvert.DeserializeObject<ImportSetModel>(cosSetJson);
                case SetType.DiagonAlley:
                    var diagonAlleySetJsonUrl = "https://raw.githubusercontent.com/Tressley/hpjson/master/sets/diagon%20alley/cards.json";
                    var diagonAlleySetJson = GetJson(diagonAlleySetJsonUrl);

                    return JsonConvert.DeserializeObject<ImportSetModel>(diagonAlleySetJson);
                case SetType.QuidditchCup:
                    var quidditchSetJsonUrl = "https://raw.githubusercontent.com/Tressley/hpjson/master/sets/quidditch%20cup/cards.json";
                    var quidditchSetJson = GetJson(quidditchSetJsonUrl);

                    return JsonConvert.DeserializeObject<ImportSetModel>(quidditchSetJson);
                default:
                    return null;
            }
        }
        private static List<ImportSetModel> GetSets()
        {
            var baseSet = GetSet(SetType.Base);
            var adventureAtHogwartsSet = GetSet(SetType.AdventureAtHogwarts);
            var chamberOfSecretsSet = GetSet(SetType.ChamberOfSecrets);
            var diagonAlleySet = GetSet(SetType.DiagonAlley);
            var quidditchSet = GetSet(SetType.QuidditchCup);

            var sets = new List<ImportSetModel>();
            sets.Add(baseSet);
            sets.Add(adventureAtHogwartsSet);
            sets.Add(chamberOfSecretsSet);
            sets.Add(diagonAlleySet);
            sets.Add(quidditchSet);

            return sets;
        }

        private static string GetJson(string url)
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
