﻿using Accio.Business.Models.AdvancedCardSearchSearchModels;
using Accio.Business.Models.CardModels;
using Accio.Business.Models.LanguageModels;
using Accio.Business.Models.LessonModels;
using Accio.Business.Models.RarityModels;
using Accio.Business.Models.SetModels;
using Accio.Business.Models.TypeModels;
using Accio.Business.Services.CardSearchHistoryServices;
using Accio.Business.Services.CardServices;
using Accio.Business.Services.LanguageServices;
using Accio.Business.Services.LessonServices;
using Accio.Business.Services.TypeServices;
using Accio.Data;
using Microsoft.EntityFrameworkCore;
//using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Accio.Business.Services.AdvancedCardSearchSearchServices
{
    public class AdvancedCardSearchService
    {
        private AccioContext _context { get; set; }
        private LanguageService _languageService { get; set; }
        private TypeService _cardTypeService { get; set; }
        private CardSubTypeService _cardSubTypeService { get; set; }
        private LessonService _lessonService { get; set; }
        private RarityService _rarityService { get; set; }
        private SetService _setService { get; set; }
        private SubTypeService _subTypeService { get; set; }
        private CardService _cardService { get; set; }
        private CardSearchHistoryService _cardSearchHistoryService { get; set; }

        public AdvancedCardSearchService(AccioContext context, LanguageService languageService, TypeService typeService,
                                         CardSubTypeService cardSubTypeService, LessonService lessonService, RarityService rarityService,
                                         SetService setService, SubTypeService subTypeService, CardService cardService,
                                         CardSearchHistoryService cardSearchHistoryService)
        {
            _languageService = languageService;
            _context = context;
            _cardTypeService = typeService;
            _cardSubTypeService = cardSubTypeService;
            _lessonService = lessonService;
            _rarityService = rarityService;
            _setService = setService;
            _subTypeService = subTypeService;
            _cardService = cardService;
            _cardSearchHistoryService = cardSearchHistoryService;
        }

        public List<CardModel> SearchCards(AdvancedSearchParameters param)
        {
            var utcNow = DateTime.UtcNow;

            var validSearchParams = ValidateAdvancedSearchParams(param.AdvancedSearchText);
            if (!validSearchParams)
                return new List<CardModel>();

            var query = GetQuery(param);
            var cardModels = query.Select(x => CardService.GetCardModel(
                                          x.Card, x.Set, x.Rarity, x.CardType, x.CardDetail,
                                          x.Language, x.LessonType, x.CardProvidesLessonLessonType, x.CardProvidesLesson)
                                         ).ToList().DistinctBy(x => new { x.CardId, x.Detail.Language.LanguageId }).ToList();

            //This isn't ideal, but there aren't a ton of sub types and it's easier to just pull all and assign than to do a complicated join
            var cardSubTypeModels = _cardSubTypeService.GetAllCardSubTypes();
            foreach (var cardModel in cardModels)
            {
                var cardSubTypes = cardSubTypeModels.Where(x => x.CardId == cardModel.CardId).ToList();
                if (cardSubTypes != null && cardSubTypes.Count > 0)
                {
                    cardModel.SubTypes = cardSubTypes;
                }
            }

            cardModels = _cardService.GetCardsWithImages(cardModels);

            if (cardModels?.Count > 1 && !string.IsNullOrEmpty(param.SortBy) && !string.IsNullOrEmpty(param.SortOrder))
            {
                cardModels = _cardService.GetCardModelsSorted(cardModels, param.SortBy, param.SortOrder);
            }

            _cardSearchHistoryService.PersistCardSearchHistory(param, utcNow, utcNow);

            return cardModels;
        }
        /// <summary>
        /// Provides the full query for the advanced card search
        /// </summary>
        private IQueryable<AdvancedSearchCardQuery> GetQuery(AdvancedSearchParameters param)
        {
            var detailTable = GetDetailTable(param.AdvancedSearchText);
            var cardTable = GetCardTable(param.AdvancedSearchText);

            var query = (from card in cardTable
                         join cardDetail in detailTable on card.CardId equals cardDetail.CardId
                         join language in _context.Languages on cardDetail.LanguageId equals language.LanguageId
                         join cardSet in _context.Sets on card.CardSetId equals cardSet.SetId
                         join cardRarity in _context.Rarities on card.CardRarityId equals cardRarity.RarityId
                         join cardType in _context.CardTypes on card.CardTypeId equals cardType.CardTypeId
                         join provides in _context.CardProvidesLessons on card.CardId equals provides.CardId into cardsProvidesLesson
                         from provides in cardsProvidesLesson.DefaultIfEmpty()
                         join plesson in _context.LessonTypes on provides.LessonId equals plesson.LessonTypeId into providesLesson
                         from plesson in providesLesson.DefaultIfEmpty()
                         join lessonType in _context.LessonTypes on card.LessonTypeId equals lessonType.LessonTypeId into lessonTypeDefault
                         from lessonType in lessonTypeDefault.DefaultIfEmpty()
                         join cardSubType in _context.CardSubTypes on card.CardId equals cardSubType.CardId into cst
                         from cardSubType in cst.DefaultIfEmpty()
                         join subType in _context.SubTypes on cardSubType.SubTypeId equals subType.SubTypeId into st
                         from subType in st.DefaultIfEmpty()
                         where !card.Deleted && !cardSet.Deleted && !cardRarity.Deleted && !cardType.Deleted
                         select new AdvancedSearchCardQuery()
                         {
                             Card = card,
                             CardDetail = cardDetail,
                             Set = cardSet,
                             Rarity = cardRarity,
                             CardType = cardType,
                             Language = language,
                             LessonType = lessonType,
                             CardProvidesLesson = provides,
                             CardProvidesLessonLessonType = plesson,
                             SubType = subType,
                         });

            //We can't setup the table before hand like we do with the card and card detail table. Since provides lesson is optional,
            //if we were to set it up and WHERE it, all the rows would return because it's a left join and adding the WHERE clause
            //during the table setup created a subquery. Which slows the query down and doesn't apply the clause at the end of the query
            var providesLessonFields = GetCardProvidesLessonTableFields(param.AdvancedSearchText);
            if (providesLessonFields.Any())
            {
                query = GetProvidesLessonWhereClause(query, providesLessonFields);
            }

            var keyWordsFields = GetSubTypeTableFields(param.AdvancedSearchText);
            if (keyWordsFields.Any())
            {
                query = GetKeywordsWhereClause(query, keyWordsFields);
            }

            return query;
        }

        /// <summary>
        /// Applies the filters on the details table before the join
        /// </summary>
        private IQueryable<CardDetail> GetDetailTable(string advancedSearchString)
        {
            var detailTable = (from cardDetail in _context.CardDetails
                               where !cardDetail.Deleted
                               select cardDetail);

            var fields = GetCardDetailTableFields(advancedSearchString);
            foreach (var field in fields)
            {
                //Contains
                var fieldsContains = fields.Where(x => x.Field == field.Field && x.Expression == AdvancedSearchFieldExpression.Contains).ToList();
                if (fieldsContains.Count > 0)
                {
                    if (fieldsContains.Any(x => x.Value.Contains(AdvancedSearchExpressions.Or)))
                        fieldsContains = GetOrFields(fieldsContains);

                    //When the user searches for the "text" of a card, we can't just compare it against the text field of a card. Some cards
                    //have their "text" spread across several fields. Like matches and adventures. Since we don't really know the type of the
                    //card, we'll search against all possible fields when search for text. All other fields will search against their pre-set column name
                    var textFields = fieldsContains.Where(x => x.Field == AdvancedSearchField.Text).ToList();
                    if (textFields.Any())
                    {
                        var propertyNames = GetTextPropertyNames();
                        detailTable = GetCardDetailQueryWithWhereClause(detailTable, fieldsContains.Select(x => $"%{x.Value}%").ToArray(), propertyNames);
                    }
                    else
                    {
                        detailTable = GetCardDetailQueryWithWhereClause(detailTable, fieldsContains.Select(x => $"%{x.Value}%").ToArray(), field.DatabaseColumnName);
                    }
                }

                //Exact
                var fieldsExact = fields.Where(x => x.Field == field.Field && x.Expression == AdvancedSearchFieldExpression.Exact).ToList();
                if (fieldsExact.Count > 0)
                {
                    if (fieldsExact.Any(x => x.Value.Contains(AdvancedSearchExpressions.Or)))
                        fieldsExact = GetOrFields(fieldsExact);

                    var textFields = fieldsContains.Where(x => x.Field == AdvancedSearchField.Text).ToList();
                    if (textFields.Any())
                    {
                        var propertyNames = GetTextPropertyNames();
                        detailTable = GetCardDetailQueryWithWhereClause(detailTable, fieldsExact.Select(x => $"{x.Value}").ToArray(), propertyNames);
                    }
                    else
                    {
                        detailTable = GetCardDetailQueryWithWhereClause(detailTable, fieldsExact.Select(x => $"{x.Value}").ToArray(), field.DatabaseColumnName);
                    }
                }
            }

            return detailTable;

            List<string> GetTextPropertyNames()
            {
                var names = new List<string>();
                names.Add(nameof(CardDetail.Text));
                names.Add(nameof(CardDetail.Effect));
                names.Add(nameof(CardDetail.ToSolve));
                names.Add(nameof(CardDetail.Reward));

                return names;
            }
        }

        /// <summary>
        /// Applies the filters on the card table before the join
        /// </summary>
        private IQueryable<Card> GetCardTable(string advancedSearchString)
        {
            var cardTable = (from card in _context.Cards
                             where !card.Deleted
                             select card);

            var fields = GetCardTableFields(advancedSearchString);
            foreach (var field in fields)
            {
                //Contains
                var fieldsContains = fields.Where(x => x.Field == field.Field && x.Expression == AdvancedSearchFieldExpression.Contains).ToList();
                if (fieldsContains.Count > 0)
                {
                    if (fieldsContains.Any(x => x.Value.Contains(AdvancedSearchExpressions.Or)))
                        fieldsContains = GetOrFields(fieldsContains);

                    cardTable = GetCardQueryWithWhereClause(cardTable, fieldsContains.Select(x => $"%{x.Value}%").ToArray(), field.DatabaseColumnName);
                }

                //Exact
                var fieldsExact = fields.Where(x => x.Field == field.Field && x.Expression == AdvancedSearchFieldExpression.Exact).ToList();
                if (fieldsExact.Count > 0)
                {
                    if (fieldsExact.Any(x => x.Value.Contains(AdvancedSearchExpressions.Or)))
                        fieldsExact = GetOrFields(fieldsExact);

                    //TODO: clean this shit up

                    if (field.Field == AdvancedSearchField.LessonType)
                    {
                        var guids = fieldsExact.Select(x => Guid.Parse(x.Value)).ToList();
                        cardTable = cardTable.Where(x => guids.Contains((Guid)x.LessonTypeId));
                    }
                    else if (field.Field == AdvancedSearchField.Rarity)
                    {
                        var guids = fieldsExact.Select(x => Guid.Parse(x.Value)).ToList();
                        cardTable = cardTable.Where(x => guids.Contains((Guid)x.CardRarityId));
                    }
                    else if (field.Field == AdvancedSearchField.Set)
                    {
                        var guids = fieldsExact.Select(x => Guid.Parse(x.Value)).ToList();
                        cardTable = cardTable.Where(x => guids.Contains((Guid)x.CardSetId));
                    }
                    else if (field.Field == AdvancedSearchField.Type)
                    {
                        var guids = fieldsExact.Select(x => Guid.Parse(x.Value)).ToList();
                        cardTable = cardTable.Where(x => guids.Contains((Guid)x.CardTypeId));
                    }
                    else if (field.Field == AdvancedSearchField.Power)
                    {
                        var values = fieldsExact.Select(x => x.Value.ToIntNullable()).ToList();
                        cardTable = cardTable.Where(x => values.Contains(x.LessonCost));
                    }
                    else if (field.Field == AdvancedSearchField.Health)
                    {
                        var values = fieldsExact.Select(x => x.Value.ToIntNullable()).ToList();
                        cardTable = cardTable.Where(x => values.Contains(x.Health));
                    }
                    else if (field.Field == AdvancedSearchField.Damage)
                    {
                        var values = fieldsExact.Select(x => x.Value.ToIntNullable()).ToList();
                        cardTable = cardTable.Where(x => values.Contains(x.Damage));
                    }
                    else
                    {
                        cardTable = GetCardQueryWithWhereClause(cardTable, fieldsExact.Select(x => x.Value).ToArray(), field.DatabaseColumnName);
                    }
                }

                //Greater than or equal to
                var fieldsGreaterThanOrEqualTo = fields.Where(x => x.Field == field.Field && x.Expression == AdvancedSearchFieldExpression.GreaterThanOrEqualTo).ToList();
                if (fieldsGreaterThanOrEqualTo?.Count > 0)
                {
                    fieldsGreaterThanOrEqualTo = GetOrFields(fieldsGreaterThanOrEqualTo);
                    var values = fieldsGreaterThanOrEqualTo.Select(x => x.Value.ToIntNullable()).ToArray();
                    cardTable = GetCardQueryByCostValue(cardTable, values, field.Field, AdvancedSearchFieldExpression.GreaterThanOrEqualTo);
                }

                //Greater than
                var fieldsGreaterThan = fields.Where(x => x.Field == field.Field && x.Expression == AdvancedSearchFieldExpression.GreaterThan).ToList();
                if (fieldsGreaterThan?.Count > 0)
                {
                    fieldsGreaterThan = GetOrFields(fieldsGreaterThan);
                    var values = fieldsGreaterThan.Select(x => x.Value.ToIntNullable()).ToArray();
                    cardTable = GetCardQueryByCostValue(cardTable, values, field.Field, AdvancedSearchFieldExpression.GreaterThan);
                }

                //Less than or equal to
                var fieldsLessThanOrEqualTo = fields.Where(x => x.Field == field.Field && x.Expression == AdvancedSearchFieldExpression.LessThanOrEqualTo).ToList();
                if (fieldsLessThanOrEqualTo?.Count > 0)
                {
                    fieldsLessThanOrEqualTo = GetOrFields(fieldsLessThanOrEqualTo);
                    var values = fieldsLessThanOrEqualTo.Select(x => x.Value.ToIntNullable()).ToArray();
                    cardTable = GetCardQueryByCostValue(cardTable, values, field.Field, AdvancedSearchFieldExpression.LessThanOrEqualTo);
                }

                //Less than
                var fieldsLessThan = fields.Where(x => x.Field == field.Field && x.Expression == AdvancedSearchFieldExpression.LessThan).ToList();
                if (fieldsLessThan?.Count > 0)
                {
                    fieldsLessThan = GetOrFields(fieldsLessThan);
                    var values = fieldsLessThan.Select(x => x.Value.ToIntNullable()).ToArray();
                    cardTable = GetCardQueryByCostValue(cardTable, values, field.Field, AdvancedSearchFieldExpression.LessThan);
                }
            }

            return cardTable;
        }

        /* Swap all the "tablefields" methods to a single one with an enum for the table. Maybe */

        private List<AdvancedSearchFieldValue> GetCardDetailTableFields(string advancedSearchString)
        {
            var fields = new List<AdvancedSearchFieldValue>();

            var types = new List<AdvancedSearchField>()
            {
                AdvancedSearchField.Artist,
                AdvancedSearchField.FlavorText,
                AdvancedSearchField.Text,
                AdvancedSearchField.Name,
            };

            foreach (var type in types)
            {
                var values = GetAdvancedSearchFieldValues(type, advancedSearchString);
                if (values?.Count > 0)
                    fields.AddRange(values);
            }

            return fields;
        }
        private List<AdvancedSearchFieldValue> GetCardTableFields(string advancedSearchString)
        {
            var fields = new List<AdvancedSearchFieldValue>();

            var types = new List<AdvancedSearchField>()
            {
                AdvancedSearchField.LessonType,
                AdvancedSearchField.Number,
                AdvancedSearchField.Power,
                AdvancedSearchField.Rarity,
                AdvancedSearchField.Set,
                AdvancedSearchField.Type,
                AdvancedSearchField.Health,
                AdvancedSearchField.Damage,
            };

            foreach (var type in types)
            {
                var values = GetAdvancedSearchFieldValues(type, advancedSearchString);
                if (values?.Count > 0)
                    fields.AddRange(values);
            }

            return fields;
        }
        private List<AdvancedSearchFieldValue> GetCardProvidesLessonTableFields(string advancedSearchString)
        {
            var fields = new List<AdvancedSearchFieldValue>();

            var types = new List<AdvancedSearchField>()
            {
                AdvancedSearchField.Provides,
            };

            foreach (var type in types)
            {
                var values = GetAdvancedSearchFieldValues(type, advancedSearchString);
                if (values?.Count > 0)
                    fields.AddRange(values);
            }

            return fields;
        }
        private List<AdvancedSearchFieldValue> GetSubTypeTableFields(string advancedSearchString)
        {
            var fields = new List<AdvancedSearchFieldValue>();

            var types = new List<AdvancedSearchField>()
            {
                AdvancedSearchField.Keywords,
            };

            foreach (var type in types)
            {
                var values = GetAdvancedSearchFieldValues(type, advancedSearchString);
                if (values?.Count > 0)
                    fields.AddRange(values);
            }

            return fields;
        }

        /// <summary>
        /// Returns the query string used when redirecting to the search page from advanced search page
        /// </summary>
        public string GetAdvancedSearchUrlValue(string cardName, string cardText, string cardTypes, string lessonTypes,
                                                string power, string sets, string rarity, string flavorText, string artist,
                                                string cardNumber, string provides, string keyword, string stats)
        {
            var urlList = new List<string>();

            if (!string.IsNullOrEmpty(cardName))
            {
                urlList.Add($"{AdvancedSearchKeywords.Name}{AdvancedSearchExpressions.Contains}{cardName.ToStringWithoutIllegalCharacters().ToDoubleQuotedString()}");
            }
            if (!string.IsNullOrEmpty(cardText))
            {
                urlList.Add($"{AdvancedSearchKeywords.Text}{AdvancedSearchExpressions.Contains}{cardText.ToStringWithoutIllegalCharacters().ToDoubleQuotedString()}");
            }
            if (!string.IsNullOrEmpty(cardTypes))
            {
                urlList.Add($"{AdvancedSearchKeywords.Type}{AdvancedSearchExpressions.Exact}{cardTypes.ToPipeDelimitedFromCommaDelimited()}");
            }
            if (!string.IsNullOrEmpty(lessonTypes))
            {
                urlList.Add($"{AdvancedSearchKeywords.LessonType}{AdvancedSearchExpressions.Exact}{lessonTypes.ToPipeDelimitedFromCommaDelimited()}");
            }
            if (!string.IsNullOrEmpty(power))
            {
                urlList.Add($"{AdvancedSearchKeywords.Power}{AdvancedSearchExpressions.Exact}{power.ToStringWithoutIllegalCharacters().ToDoubleQuotedString()}");
            }
            if (!string.IsNullOrEmpty(sets))
            {
                urlList.Add($"{AdvancedSearchKeywords.Set}{AdvancedSearchExpressions.Exact}{sets.ToLower().ToPipeDelimitedFromCommaDelimited()}");
            }
            if (!string.IsNullOrEmpty(rarity))
            {
                urlList.Add($"{AdvancedSearchKeywords.Rarity}{AdvancedSearchExpressions.Exact}{rarity.ToLower().ToPipeDelimitedFromCommaDelimited()}");
            }
            if (!string.IsNullOrEmpty(flavorText))
            {
                urlList.Add($"{AdvancedSearchKeywords.FlavorText}{AdvancedSearchExpressions.Contains}{flavorText.ToStringWithoutIllegalCharacters().ToDoubleQuotedString()}");
            }
            if (!string.IsNullOrEmpty(artist))
            {
                urlList.Add($"{AdvancedSearchKeywords.Artist}{AdvancedSearchExpressions.Contains}{artist.ToStringWithoutIllegalCharacters().ToDoubleQuotedString()}");
            }
            if (!string.IsNullOrEmpty(cardNumber))
            {
                urlList.Add($"{AdvancedSearchKeywords.Number}{AdvancedSearchExpressions.Exact}{cardNumber.ToPipeDelimitedFromCommaDelimited().ToDoubleQuotedString()}");
            }
            if (!string.IsNullOrEmpty(provides))
            {
                urlList.Add($"{AdvancedSearchKeywords.Provides}{AdvancedSearchExpressions.Exact}{provides.ToPipeDelimitedFromCommaDelimited()}");
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                urlList.Add($"{AdvancedSearchKeywords.Keywords}{AdvancedSearchExpressions.Exact}{keyword.ToPipeDelimitedFromCommaDelimited().ToDoubleQuotedString()}");
            }
            if (!string.IsNullOrEmpty(stats))
            {
                var concatenatedStats = "";
                var items = GetAdvancedSearchStatItemsFromDelimitedString(stats);
                foreach (var item in items)
                {
                    concatenatedStats += $"{item.Keyword}{item.Expression}{item.Value} ";
                }

                urlList.Add(concatenatedStats.TrimEnd(' '));
            }

            return string.Join('+', urlList);
        }
        private List<AdvancedSearchStatItemModel> GetAdvancedSearchStatItemsFromDelimitedString(string value)
        {
            var items = new List<AdvancedSearchStatItemModel>();

            var delimitedLines = value.TrimEnd(';').Split(';');
            for (int i = 0; i < delimitedLines.Length; i++)
            {
                var delimitedItems = delimitedLines[i].Split('|');
                var keyword = "";
                var delimitedType = delimitedItems[0].ToLower();
                if (delimitedType == AdvancedSearchKeywords.Health || delimitedItems[0] == AdvancedSearchKeywords.HealthAlias)
                {
                    keyword = AdvancedSearchKeywords.Health;
                }
                else if (delimitedType == AdvancedSearchKeywords.Damage || delimitedItems[0] == AdvancedSearchKeywords.DamageAlias)
                {
                    keyword = AdvancedSearchKeywords.Damage;
                }
                var expression = "";
                var delimitedExpression = delimitedItems[1].ToLower();
                if (delimitedExpression == AdvancedSearchExpressions.EqualToName)
                {
                    expression = AdvancedSearchExpressions.Exact;
                }
                else if (delimitedExpression == AdvancedSearchExpressions.GreaterThanName)
                {
                    expression = AdvancedSearchExpressions.GreaterThan;
                }
                else if (delimitedExpression == AdvancedSearchExpressions.GreaterThanOrEqualToName)
                {
                    expression = AdvancedSearchExpressions.GreaterThanOrEqualTo;
                }
                else if (delimitedExpression == AdvancedSearchExpressions.LessThanName)
                {
                    expression = AdvancedSearchExpressions.LessThan;
                }
                else if (delimitedExpression == AdvancedSearchExpressions.LessThanOrEqualToName)
                {
                    expression = AdvancedSearchExpressions.LessThanOrEqualTo;
                }

                var item = new AdvancedSearchStatItemModel()
                {
                    Keyword = keyword,
                    Expression = expression,
                    Value = delimitedItems[2],
                };
                items.Add(item);
            }

            return items;
        }

        /// <summary>
        /// Takes a list of fields and converts it to the same list, parting out the ORs from the OR operator
        /// </summary>
        private List<AdvancedSearchFieldValue> GetOrFields(List<AdvancedSearchFieldValue> fields)
        {
            var orFields = new List<AdvancedSearchFieldValue>();

            foreach (var field in fields)
            {
                var separatedOrValues = field.Value.Split(AdvancedSearchExpressions.Or).ToList();
                foreach (var item in separatedOrValues)
                {
                    orFields.Add(new AdvancedSearchFieldValue()
                    {
                        Field = field.Field,
                        DatabaseColumnName = field.DatabaseColumnName,
                        Expression = field.Expression,
                        Value = item,
                    });
                }
            }

            return orFields;
        }

        /// <summary>
        /// Determines if the search text is an advanced search request by checking if the text contains at least one
        /// keyword and one expression. It's not comprehensive and can return a false positive if the keyword and expression
        /// aren't placed together. Example: "name asldknasd :" would come back positive when that's not a valid search expression.
        /// But this is probably good enough
        /// UPDATE: it's not good enough. Need to fix this
        /// </summary>
        /// <param name="searchText">Search page search text</param>
        public bool IsAdvancedSearch(string searchText)
        {
            if (string.IsNullOrEmpty(searchText))
                return false;

            var containsKeyword = false;
            var containsExpression = false;

            containsKeyword = searchText.Contains(AdvancedSearchKeywords.Artist) || searchText.Contains(AdvancedSearchKeywords.ArtistAlias) ||
                              searchText.Contains(AdvancedSearchKeywords.Damage) || searchText.Contains(AdvancedSearchKeywords.DamageAlias) ||
                              searchText.Contains(AdvancedSearchKeywords.FlavorText) || searchText.Contains(AdvancedSearchKeywords.FlavorTextAlias) ||
                              searchText.Contains(AdvancedSearchKeywords.Health) || searchText.Contains(AdvancedSearchKeywords.HealthAlias) ||
                              searchText.Contains(AdvancedSearchKeywords.Keywords) || searchText.Contains(AdvancedSearchKeywords.KeywordsAlias) ||
                              searchText.Contains(AdvancedSearchKeywords.LessonType) || searchText.Contains(AdvancedSearchKeywords.LessonTypeAlias) ||
                              searchText.Contains(AdvancedSearchKeywords.Name) || searchText.Contains(AdvancedSearchKeywords.NameAlias) ||
                              searchText.Contains(AdvancedSearchKeywords.Number) || searchText.Contains(AdvancedSearchKeywords.NumberAlias) ||
                              searchText.Contains(AdvancedSearchKeywords.Power) || searchText.Contains(AdvancedSearchKeywords.PowerAlias) ||
                              searchText.Contains(AdvancedSearchKeywords.Provides) || searchText.Contains(AdvancedSearchKeywords.ProvidesAlias) ||
                              searchText.Contains(AdvancedSearchKeywords.Rarity) || searchText.Contains(AdvancedSearchKeywords.RarityAlias) ||
                              searchText.Contains(AdvancedSearchKeywords.Set) || searchText.Contains(AdvancedSearchKeywords.SetAlias) ||
                              searchText.Contains(AdvancedSearchKeywords.Text) || searchText.Contains(AdvancedSearchKeywords.TextAlias) ||
                              searchText.Contains(AdvancedSearchKeywords.Type) || searchText.Contains(AdvancedSearchKeywords.TypeAlias);

            containsExpression = searchText.Contains(AdvancedSearchExpressions.And) || searchText.Contains(AdvancedSearchExpressions.Contains) ||
                                 /*searchText.Contains(AdvancedSearchExpressions.EqualTo) || */ searchText.Contains(AdvancedSearchExpressions.Exact) ||
                                 searchText.Contains(AdvancedSearchExpressions.GreaterThan) || searchText.Contains(AdvancedSearchExpressions.LessThan) ||
                                 searchText.Contains(AdvancedSearchExpressions.Or);

            return containsKeyword && containsExpression;
        }

        private List<AdvancedSearchFieldValue> GetAdvancedSearchFieldValues(AdvancedSearchField field, string advancedSearchString)
        {
            var fullFieldValues = new List<AdvancedSearchFieldValue>();

            //The fields are space delimted, with the field itself being char delimited for the field name, expression, and value
            //Example: name*hagrid - cards that have a name that contain hagrid. But we allow multiple fields of the same type
            //So the initial split will just get all the fields and on each field, we'll split it down further
            var fields = GetFields(advancedSearchString.ToLower());

            var rawFields = new List<string>();
            var fieldValues = new List<AdvancedSearchFieldValue>();

            switch (field)
            {
                case AdvancedSearchField.Name:
                    rawFields = fields.Where(x => x.StartsWith(AdvancedSearchKeywords.Name)).ToList();
                    fieldValues = GetAdvancedFieldValueFromRawField(field, rawFields);
                    if (fieldValues.Count > 0)
                    {
                        fieldValues.Select(c => { c.DatabaseColumnName = nameof(CardDetail.Name); return c; }).ToList();
                        fullFieldValues.AddRange(fieldValues);
                    }

                    break;
                case AdvancedSearchField.Text:
                    rawFields = fields.Where(x => x.StartsWith(AdvancedSearchKeywords.Text)).ToList();
                    fieldValues = GetAdvancedFieldValueFromRawField(field, rawFields);
                    if (fieldValues.Count > 0)
                    {
                        fieldValues.Select(c => { c.DatabaseColumnName = nameof(CardDetail.Text); return c; }).ToList();
                        fullFieldValues.AddRange(fieldValues);
                    }

                    break;
                case AdvancedSearchField.Type:
                    rawFields = fields.Where(x => x.StartsWith(AdvancedSearchKeywords.Type)).ToList();
                    fieldValues = GetAdvancedFieldValueFromRawField(field, rawFields);
                    if (fieldValues.Count > 0)
                    {
                        fieldValues.Select(c => { c.DatabaseColumnName = nameof(Card.CardTypeId); return c; }).ToList();
                        fieldValues = GetCardTypeFieldValues(fieldValues);
                        fullFieldValues.AddRange(fieldValues);
                    }

                    break;
                case AdvancedSearchField.Artist:
                    rawFields = fields.Where(x => x.StartsWith(AdvancedSearchKeywords.Artist)).ToList();
                    fieldValues = GetAdvancedFieldValueFromRawField(field, rawFields);
                    if (fieldValues.Count > 0)
                    {
                        fieldValues.Select(c => { c.DatabaseColumnName = nameof(CardDetail.Illustrator); return c; }).ToList();
                        fullFieldValues.AddRange(fieldValues);
                    }

                    break;
                case AdvancedSearchField.FlavorText:
                    rawFields = fields.Where(x => x.StartsWith(AdvancedSearchKeywords.FlavorText)).ToList();
                    fieldValues = GetAdvancedFieldValueFromRawField(field, rawFields);
                    if (fieldValues.Count > 0)
                    {
                        fieldValues.Select(c => { c.DatabaseColumnName = nameof(CardDetail.FlavorText); return c; }).ToList();
                        fullFieldValues.AddRange(fieldValues);
                    }

                    break;
                case AdvancedSearchField.LessonType:
                    rawFields = fields.Where(x => x.StartsWith(AdvancedSearchKeywords.LessonType)).ToList();
                    fieldValues = GetAdvancedFieldValueFromRawField(field, rawFields);
                    if (fieldValues.Count > 0)
                    {
                        fieldValues.Select(c => { c.DatabaseColumnName = nameof(Card.LessonTypeId); return c; }).ToList();
                        fieldValues = GetLessonTypeFieldValues(fieldValues);
                        fullFieldValues.AddRange(fieldValues);
                    }

                    break;
                case AdvancedSearchField.Keywords:
                    rawFields = fields.Where(x => x.StartsWith(AdvancedSearchKeywords.Keywords)).ToList();
                    fieldValues = GetAdvancedFieldValueFromRawField(field, rawFields);
                    if (fieldValues.Count > 0)
                    {
                        fieldValues.Select(c => { c.DatabaseColumnName = nameof(SubType.Name); return c; }).ToList();
                        fieldValues = GetOrFields(fieldValues);
                        fullFieldValues.AddRange(fieldValues);
                    }

                    break;
                case AdvancedSearchField.Rarity:
                    rawFields = fields.Where(x => x.StartsWith(AdvancedSearchKeywords.Rarity)).ToList();
                    fieldValues = GetAdvancedFieldValueFromRawField(field, rawFields);
                    if (fieldValues.Count > 0)
                    {
                        fieldValues.Select(c => { c.DatabaseColumnName = nameof(Card.CardRarityId); return c; }).ToList();
                        fieldValues = GetRarityFieldValues(fieldValues);
                        fullFieldValues.AddRange(fieldValues);
                    }

                    break;
                case AdvancedSearchField.Set:
                    rawFields = fields.Where(x => x.StartsWith(AdvancedSearchKeywords.Set)).ToList();
                    fieldValues = GetAdvancedFieldValueFromRawField(field, rawFields);
                    if (fieldValues.Count > 0)
                    {
                        fieldValues.Select(c => { c.DatabaseColumnName = nameof(Card.CardSetId); return c; }).ToList();
                        fieldValues = GetSetFieldValues(fieldValues);
                        fullFieldValues.AddRange(fieldValues);
                    }

                    break;
                case AdvancedSearchField.Number:
                    rawFields = fields.Where(x => x.StartsWith(AdvancedSearchKeywords.Number)).ToList();
                    fieldValues = GetAdvancedFieldValueFromRawField(field, rawFields);
                    if (fieldValues.Count > 0)
                    {
                        fieldValues.Select(c => { c.DatabaseColumnName = nameof(Card.CardNumber); return c; }).ToList();
                        fullFieldValues.AddRange(fieldValues);
                    }

                    break;
                case AdvancedSearchField.Power:
                    rawFields = fields.Where(x => x.StartsWith(AdvancedSearchKeywords.Power)).ToList();
                    fieldValues = GetAdvancedFieldValueFromRawField(field, rawFields);
                    if (fieldValues.Count > 0)
                    {
                        fieldValues.Select(c => { c.DatabaseColumnName = nameof(Card.LessonCost); return c; }).ToList();
                        fullFieldValues.AddRange(fieldValues);
                    }

                    break;
                case AdvancedSearchField.Health:
                    rawFields = fields.Where(x => x.StartsWith(AdvancedSearchKeywords.Health)).ToList();
                    fieldValues = GetAdvancedFieldValueFromRawField(field, rawFields);
                    if (fieldValues.Count > 0)
                    {
                        fieldValues.Select(c => { c.DatabaseColumnName = nameof(Card.Health); return c; }).ToList();
                        fullFieldValues.AddRange(fieldValues);
                    }

                    break;
                case AdvancedSearchField.Damage:
                    rawFields = fields.Where(x => x.StartsWith(AdvancedSearchKeywords.Damage)).ToList();
                    fieldValues = GetAdvancedFieldValueFromRawField(field, rawFields);
                    if (fieldValues.Count > 0)
                    {
                        fieldValues.Select(c => { c.DatabaseColumnName = nameof(Card.Damage); return c; }).ToList();
                        fullFieldValues.AddRange(fieldValues);
                    }


                    break;
                case AdvancedSearchField.Provides:
                    rawFields = fields.Where(x => x.StartsWith(AdvancedSearchKeywords.Provides)).ToList();
                    fieldValues = GetAdvancedFieldValueFromRawField(field, rawFields);
                    if (fieldValues.Count > 0)
                    {
                        fieldValues.Select(c => { c.DatabaseColumnName = nameof(LessonType.LessonTypeId); return c; }).ToList();
                        fieldValues = GetLessonTypeFieldValues(fieldValues);
                        fullFieldValues.AddRange(fieldValues);
                    }

                    break;
                default:
                    break;
            }

            return fullFieldValues;

            List<AdvancedSearchFieldValue> GetAdvancedFieldValueFromRawField(AdvancedSearchField field, List<string> rawFields)
            {
                var fieldValues = new List<AdvancedSearchFieldValue>();

                foreach (var rawField in rawFields)
                {
                    var fieldValue = new AdvancedSearchFieldValue() { Field = field };
                    if (rawField.Contains(AdvancedSearchExpressions.Exact))
                    {
                        fieldValue.Expression = AdvancedSearchFieldExpression.Exact;
                        fieldValue.Value = GetFieldValue(rawField, AdvancedSearchExpressions.Exact);
                        fieldValues.Add(fieldValue);
                    }
                    else if (rawField.Contains(AdvancedSearchExpressions.Contains))
                    {
                        fieldValue.Expression = AdvancedSearchFieldExpression.Contains;
                        fieldValue.Value = GetFieldValue(rawField, AdvancedSearchExpressions.Contains);
                        fieldValues.Add(fieldValue);
                    }
                    else if (rawField.Contains(AdvancedSearchExpressions.GreaterThanOrEqualTo))
                    {
                        fieldValue.Expression = AdvancedSearchFieldExpression.GreaterThanOrEqualTo;
                        fieldValue.Value = GetFieldValue(rawField, AdvancedSearchExpressions.GreaterThanOrEqualTo);
                        fieldValues.Add(fieldValue);
                    }
                    else if (rawField.Contains(AdvancedSearchExpressions.GreaterThan))
                    {
                        fieldValue.Expression = AdvancedSearchFieldExpression.GreaterThan;
                        fieldValue.Value = GetFieldValue(rawField, AdvancedSearchExpressions.GreaterThan);
                        fieldValues.Add(fieldValue);
                    }
                    else if (rawField.Contains(AdvancedSearchExpressions.LessThanOrEqualTo))
                    {
                        fieldValue.Expression = AdvancedSearchFieldExpression.LessThanOrEqualTo;
                        fieldValue.Value = GetFieldValue(rawField, AdvancedSearchExpressions.LessThanOrEqualTo);
                        fieldValues.Add(fieldValue);
                    }
                    else if (rawField.Contains(AdvancedSearchExpressions.LessThan))
                    {
                        fieldValue.Expression = AdvancedSearchFieldExpression.LessThan;
                        fieldValue.Value = GetFieldValue(rawField, AdvancedSearchExpressions.LessThan);
                        fieldValues.Add(fieldValue);
                    }
                    //if (rawField.Contains(AdvancedSearchExpressions.EqualTo))
                    //{
                    //    fieldValue.Expression = AdvancedSearchFieldExpression.EqualTo;
                    //    fieldValue.Value = GetFieldValue(rawField, AdvancedSearchExpressions.EqualTo);
                    //    fieldValues.Add(fieldValue);
                    //}
                }

                return fieldValues;
            }
            //Field is name, expression, value. Example: name*hagrid
            string GetFieldValue(string val, string expression)
            {
                return val.Split(expression)[1];
            }
        }

        /* 
         * TODO: see if I can swap these three methods into 1. Some sort of dynamic table param
         * Or maybe just an extension...
         */

        /// <summary>
        /// Builds a dynamic where predicate using EF.Functions.Like
        /// https://stackoverflow.com/a/57098685/1339826
        /// </summary>
        /// <param name="query"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        private IQueryable<CardDetail> GetCardDetailQueryWithWhereClause(IQueryable<CardDetail> query, string[] values, string propertyName)
        {
            var likeMethod = typeof(DbFunctionsExtensions).GetMethod(nameof(DbFunctionsExtensions.Like), new[] { typeof(DbFunctions), typeof(string), typeof(string) });

            var entityProperties = new List<PropertyInfo>();
            var ep = typeof(CardDetail).GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
            entityProperties.Add(ep);

            // EF.Functions.Like(x.OrderNumber, v1) || EF.Functions.Like(x.OrderNumber, v2)...
            Expression likePredicate = null;

            var efFunctionsInstance = Expression.Constant(EF.Functions);

            // Will be the predicate paramter (the 'x' in x => EF.Functions.Like(x.OrderNumber, v1)...)
            var lambdaParam = Expression.Parameter(typeof(CardDetail));
            foreach (var number in values)
            {
                // EF.Functions.Like(x.OrderNumber, v1)
                //                                 |__|
                var numberValue = Expression.Constant(number);

                foreach (var entityProperty in entityProperties)
                {
                    // EF.Functions.Like(x.OrderNumber, v1)
                    //                  |_____________|
                    var propertyAccess = Expression.Property(lambdaParam, entityProperty);

                    // EF.Functions.Like(x.OrderNumber, v1)
                    //|____________________________________|
                    var likeMethodCall = Expression.Call(likeMethod, efFunctionsInstance, propertyAccess, numberValue);

                    // Aggregating the current predicate with "OR" (||)
                    likePredicate = likePredicate == null
                                        ? (Expression)likeMethodCall
                                        : Expression.OrElse(likePredicate, likeMethodCall);
                }
            }

            // x => EF.Functions.Like(x.OrderNumber, v1) || EF.Functions.Like(x.OrderNumber, v2)...
            var lambdaPredicate = Expression.Lambda<Func<CardDetail, bool>>(likePredicate, lambdaParam);

            var filteredQuery = query.Where(lambdaPredicate);
            return filteredQuery;
        }
        private IQueryable<CardDetail> GetCardDetailQueryWithWhereClause(IQueryable<CardDetail> query, string[] values, List<string> propertyNames)
        {
            var likeMethod = typeof(DbFunctionsExtensions).GetMethod(nameof(DbFunctionsExtensions.Like), new[] { typeof(DbFunctions), typeof(string), typeof(string) });

            var entityProperties = new List<PropertyInfo>();
            foreach (var propertyName in propertyNames)
            {
                var ep = typeof(CardDetail).GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
                entityProperties.Add(ep);
            }

            // EF.Functions.Like(x.OrderNumber, v1) || EF.Functions.Like(x.OrderNumber, v2)...
            Expression likePredicate = null;

            var efFunctionsInstance = Expression.Constant(EF.Functions);

            // Will be the predicate paramter (the 'x' in x => EF.Functions.Like(x.OrderNumber, v1)...)
            var lambdaParam = Expression.Parameter(typeof(CardDetail));
            foreach (var number in values)
            {
                // EF.Functions.Like(x.OrderNumber, v1)
                //                                 |__|
                var numberValue = Expression.Constant(number);

                foreach (var entityProperty in entityProperties)
                {
                    // EF.Functions.Like(x.OrderNumber, v1)
                    //                  |_____________|
                    var propertyAccess = Expression.Property(lambdaParam, entityProperty);

                    // EF.Functions.Like(x.OrderNumber, v1)
                    //|____________________________________|
                    var likeMethodCall = Expression.Call(likeMethod, efFunctionsInstance, propertyAccess, numberValue);

                    // Aggregating the current predicate with "OR" (||)
                    likePredicate = likePredicate == null
                                        ? (Expression)likeMethodCall
                                        : Expression.OrElse(likePredicate, likeMethodCall);
                }
            }

            // x => EF.Functions.Like(x.OrderNumber, v1) || EF.Functions.Like(x.OrderNumber, v2)...
            var lambdaPredicate = Expression.Lambda<Func<CardDetail, bool>>(likePredicate, lambdaParam);

            var filteredQuery = query.Where(lambdaPredicate);
            return filteredQuery;
        }
        private IQueryable<Card> GetCardQueryWithWhereClause(IQueryable<Card> query, object[] values, string propertyName)
        {
            var likeMethod = typeof(DbFunctionsExtensions).GetMethod(nameof(DbFunctionsExtensions.Like), new[] { typeof(DbFunctions), typeof(string), typeof(string) });

            var entityProperties = new List<PropertyInfo>();
            var ep = typeof(Card).GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
            entityProperties.Add(ep);

            // EF.Functions.Like(x.OrderNumber, v1) || EF.Functions.Like(x.OrderNumber, v2)...
            Expression likePredicate = null;

            var efFunctionsInstance = Expression.Constant(EF.Functions);

            // Will be the predicate paramter (the 'x' in x => EF.Functions.Like(x.OrderNumber, v1)...)
            var lambdaParam = Expression.Parameter(typeof(Card));
            foreach (var val in values)
            {
                // EF.Functions.Like(x.OrderNumber, v1)
                //                                 |__|
                var numberValue = Expression.Constant(val);

                foreach (var entityProperty in entityProperties)
                {
                    // EF.Functions.Like(x.OrderNumber, v1)
                    //                  |_____________|
                    var propertyAccess = Expression.Property(lambdaParam, entityProperty);

                    // EF.Functions.Like(x.OrderNumber, v1)
                    //|____________________________________|
                    var likeMethodCall = Expression.Call(likeMethod, efFunctionsInstance, propertyAccess, numberValue);

                    // Aggregating the current predicate with "OR" (||)
                    likePredicate = likePredicate == null
                                        ? (Expression)likeMethodCall
                                        : Expression.OrElse(likePredicate, likeMethodCall);
                }
            }

            // x => EF.Functions.Like(x.OrderNumber, v1) || EF.Functions.Like(x.OrderNumber, v2)...
            var lambdaPredicate = Expression.Lambda<Func<Card, bool>>(likePredicate, lambdaParam);

            var filteredQuery = query.Where(lambdaPredicate);
            return filteredQuery;
        }

        private IQueryable<AdvancedSearchCardQuery> GetProvidesLessonWhereClause(IQueryable<AdvancedSearchCardQuery> query, List<AdvancedSearchFieldValue> values)
        {
            var guids = values.Select(x => Guid.Parse(x.Value)).ToList();
            return query.Where(x => guids.Contains(x.CardProvidesLesson.LessonId));
        }
        private IQueryable<AdvancedSearchCardQuery> GetKeywordsWhereClause(IQueryable<AdvancedSearchCardQuery> query, List<AdvancedSearchFieldValue> values)
        {
            foreach (var field in values)
            {
                var fieldsExact = values.Where(x => x.Field == field.Field && x.Expression == AdvancedSearchFieldExpression.Exact).ToList();
                if (fieldsExact.Any())
                {
                    var keywords = fieldsExact.Select(x => x.Value).ToList();
                    query = query.Where(x => keywords.Contains(x.SubType.Name));
                }

                var fieldsContains = values.Where(x => x.Field == field.Field && x.Expression == AdvancedSearchFieldExpression.Contains).ToList();
                if (fieldsContains.Any())
                {
                    query = GetSubTypeQueryWithWhereClause(query, fieldsContains.Select(x => $"%{x.Value}%").ToArray(), $"{nameof(SubType)}.{field.DatabaseColumnName}");
                }
            }

            return query;
        }

        private IQueryable<AdvancedSearchCardQuery> GetSubTypeQueryWithWhereClause(IQueryable<AdvancedSearchCardQuery> query, object[] values, string propertyName)
        {
            var likeMethod = typeof(DbFunctionsExtensions).GetMethod(nameof(DbFunctionsExtensions.Like), new[] { typeof(DbFunctions), typeof(string), typeof(string) });

            var entityProperties = new List<PropertyInfo>();
            var ep = typeof(AdvancedSearchCardQuery).GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
            entityProperties.Add(ep);

            // EF.Functions.Like(x.OrderNumber, v1) || EF.Functions.Like(x.OrderNumber, v2)...
            Expression likePredicate = null;

            var efFunctionsInstance = Expression.Constant(EF.Functions);

            // Will be the predicate paramter (the 'x' in x => EF.Functions.Like(x.OrderNumber, v1)...)
            var lambdaParam = Expression.Parameter(typeof(AdvancedSearchCardQuery));
            foreach (var val in values)
            {
                // EF.Functions.Like(x.OrderNumber, v1)
                //                                 |__|
                var numberValue = Expression.Constant(val);

                foreach (var entityProperty in entityProperties)
                {
                    // EF.Functions.Like(x.OrderNumber, v1)
                    //                  |_____________|
                    var propertyAccess = Expression.Property(lambdaParam, entityProperty);

                    // EF.Functions.Like(x.OrderNumber, v1)
                    //|____________________________________|
                    var likeMethodCall = Expression.Call(likeMethod, efFunctionsInstance, propertyAccess, numberValue);

                    // Aggregating the current predicate with "OR" (||)
                    likePredicate = likePredicate == null
                                        ? (Expression)likeMethodCall
                                        : Expression.OrElse(likePredicate, likeMethodCall);
                }
            }

            // x => EF.Functions.Like(x.OrderNumber, v1) || EF.Functions.Like(x.OrderNumber, v2)...
            var lambdaPredicate = Expression.Lambda<Func<AdvancedSearchCardQuery, bool>>(likePredicate, lambdaParam);

            var filteredQuery = query.Where(lambdaPredicate);
            return filteredQuery;
        }

        /// <summary>
        /// Applies the greater than and lesson than where clause for the power, health, damage
        /// </summary>
        IQueryable<Card> GetCardQueryByCostValue(IQueryable<Card> query, int?[] numbers, AdvancedSearchField field, AdvancedSearchFieldExpression expression)
        {
            for (int i = 0; i < numbers.Length; i++)
            {
                var number = numbers[i];
                if (expression == AdvancedSearchFieldExpression.GreaterThanOrEqualTo)
                {
                    if (field == AdvancedSearchField.Power)
                    {
                        query = query.Where(p => p.LessonCost >= number);
                    }
                    else if (field == AdvancedSearchField.Health)
                    {
                        query = query.Where(p => p.Health >= number);
                    }
                    else if (field == AdvancedSearchField.Damage)
                    {
                        query = query.Where(p => p.Damage >= number);
                    }
                }
                else if (expression == AdvancedSearchFieldExpression.GreaterThan)
                {
                    if (field == AdvancedSearchField.Power)
                    {
                        query = query.Where(p => p.LessonCost > number);
                    }
                    else if (field == AdvancedSearchField.Health)
                    {
                        query = query.Where(p => p.Health > number);
                    }
                    else if (field == AdvancedSearchField.Damage)
                    {
                        query = query.Where(p => p.Damage > number);
                    }
                }
                else if (expression == AdvancedSearchFieldExpression.LessThanOrEqualTo)
                {
                    if (field == AdvancedSearchField.Power)
                    {
                        query = query.Where(p => p.LessonCost <= number);
                    }
                    else if (field == AdvancedSearchField.Health)
                    {
                        query = query.Where(p => p.Health <= number);
                    }
                    else if (field == AdvancedSearchField.Damage)
                    {
                        query = query.Where(p => p.Damage <= number);
                    }
                }
                else if (expression == AdvancedSearchFieldExpression.LessThan)
                {
                    if (field == AdvancedSearchField.Power)
                    {
                        query = query.Where(p => p.LessonCost < number);
                    }
                    else if (field == AdvancedSearchField.Health)
                    {
                        query = query.Where(p => p.Health < number);
                    }
                    else if (field == AdvancedSearchField.Damage)
                    {
                        query = query.Where(p => p.Damage < number);
                    }
                }
            }

            return query;
        }

        /// <summary>
        /// Splits the advanced search values on spaces and maintains space for words wrapped in quotes
        /// </summary>
        private List<string> GetFields(string advancedSearchVal)
        {
            var fields = new List<string>();
            fields = SplitAdvancedSearchText(advancedSearchVal, ' ');

            return fields;
        }

        /// <summary>
        /// This splits the fields by space, but ignores spaces within double quotes so we can type out a full card name or the like.
        /// I tried doing this with regex, but I think that other characters we use (*|) screwed it up. So this works.
        /// https://stackoverflow.com/a/8568133/1339826
        /// </summary>
        public static List<string> SplitAdvancedSearchText(string stringToSplit, params char[] delimiters)
        {
            var results = new List<string>();

            var inQuote = false;
            var currentToken = new StringBuilder();
            for (int index = 0; index < stringToSplit.Length; ++index)
            {
                char currentCharacter = stringToSplit[index];
                if (currentCharacter == '"')
                {
                    // When we see a ", we need to decide whether we are
                    // at the start or send of a quoted section...
                    inQuote = !inQuote;
                }
                else if (delimiters.Contains(currentCharacter) && inQuote == false)
                {
                    // We've come to the end of a token, so we find the token,
                    // trim it and add it to the collection of results...
                    var result = currentToken.ToString().Trim();
                    if (result != "") results.Add(result);

                    // We start a new token...
                    currentToken = new StringBuilder();
                }
                else
                {
                    // We've got a 'normal' character, so we add it to
                    // the curent token...
                    currentToken.Append(currentCharacter);
                }
            }

            // We've come to the end of the string, so we add the last token...
            var lastResult = currentToken.ToString().Trim();
            if (lastResult != "") results.Add(lastResult);

            return results;
        }
        /// <summary>
        /// Replaces card type strings with card type IDs
        /// </summary>
        public List<AdvancedSearchFieldValue> GetCardTypeFieldValues(List<AdvancedSearchFieldValue> fields)
        {
            var cardTypeFields = new List<AdvancedSearchFieldValue>();

            fields = GetOrFields(fields);

            foreach (var field in fields)
            {
                var cardType = new CardTypeModel();

                if (field.Value == "adventure")
                    cardType = _cardTypeService.GetCardType(TypeOfCard.Adventure);
                else if (field.Value == "character")
                    cardType = _cardTypeService.GetCardType(TypeOfCard.Character);
                else if (field.Value == "creature")
                    cardType = _cardTypeService.GetCardType(TypeOfCard.Creature);
                else if (field.Value == "item")
                    cardType = _cardTypeService.GetCardType(TypeOfCard.Item);
                else if (field.Value == "lesson")
                    cardType = _cardTypeService.GetCardType(TypeOfCard.Lesson);
                else if (field.Value == "location")
                    cardType = _cardTypeService.GetCardType(TypeOfCard.Location);
                else if (field.Value == "match")
                    cardType = _cardTypeService.GetCardType(TypeOfCard.Match);
                else if (field.Value == "spell")
                    cardType = _cardTypeService.GetCardType(TypeOfCard.Spell);

                cardTypeFields.Add(new AdvancedSearchFieldValue()
                {
                    Field = field.Field,
                    Expression = field.Expression,
                    Value = cardType.CardTypeId.ToString(),
                    DatabaseColumnName = field.DatabaseColumnName,
                });
            }

            return cardTypeFields;
        }
        /// <summary>
        /// Replaces lesson type strings with lesson type IDs
        /// </summary>
        public List<AdvancedSearchFieldValue> GetLessonTypeFieldValues(List<AdvancedSearchFieldValue> fields)
        {
            var cardTypeFields = new List<AdvancedSearchFieldValue>();

            fields = GetOrFields(fields);

            foreach (var field in fields)
            {
                var lessonType = new LessonTypeModel();

                if (field.Value == "care of magical creatures" || field.Value == "comc")
                    lessonType = _lessonService.GetLessonType(TypeOfLesson.CareOfMagicalCreatures);
                else if (field.Value == "charms" || field.Value == "c")
                    lessonType = _lessonService.GetLessonType(TypeOfLesson.Charms);
                else if (field.Value == "potions" || field.Value == "p")
                    lessonType = _lessonService.GetLessonType(TypeOfLesson.Potions);
                else if (field.Value == "quidditch" || field.Value == "q")
                    lessonType = _lessonService.GetLessonType(TypeOfLesson.Quidditch);
                else if (field.Value == "transfiguration" || field.Value == "transfig" || field.Value == "t")
                    lessonType = _lessonService.GetLessonType(TypeOfLesson.Transfiguration);

                cardTypeFields.Add(new AdvancedSearchFieldValue()
                {
                    Field = field.Field,
                    Expression = field.Expression,
                    Value = lessonType.LessonTypeId.ToString(),
                    DatabaseColumnName = field.DatabaseColumnName,
                });
            }

            return cardTypeFields;
        }
        /// <summary>
        /// Replaces rarity strings with rarity IDs
        /// </summary>
        public List<AdvancedSearchFieldValue> GetRarityFieldValues(List<AdvancedSearchFieldValue> fields)
        {
            var rarityFields = new List<AdvancedSearchFieldValue>();

            fields = GetOrFields(fields);

            foreach (var field in fields)
            {
                var rarity = new RarityModel();

                if (field.Value == "common" || field.Value == "c")
                    rarity = _rarityService.GetRarity(TypeOfRare.Common);
                else if (field.Value == "rare" || field.Value == "r")
                    rarity = _rarityService.GetRarity(TypeOfRare.Rare);
                else if (field.Value == "uncommon" || field.Value == "uc")
                    rarity = _rarityService.GetRarity(TypeOfRare.Uncommon);
                else if (field.Value == "foil" || field.Value == "f")
                    rarity = _rarityService.GetRarity(TypeOfRare.FoilPremium);
                else if (field.Value == "holoportrait" || field.Value == "hp")
                    rarity = _rarityService.GetRarity(TypeOfRare.HoloPortraitPremium);

                rarityFields.Add(new AdvancedSearchFieldValue()
                {
                    Field = field.Field,
                    Expression = field.Expression,
                    Value = rarity.RarityId.ToString(),
                    DatabaseColumnName = field.DatabaseColumnName,
                });
            }

            return rarityFields;
        }
        /// <summary>
        /// Replaces set strings with set IDs
        /// </summary>
        public List<AdvancedSearchFieldValue> GetSetFieldValues(List<AdvancedSearchFieldValue> fields)
        {
            var setFields = new List<AdvancedSearchFieldValue>();

            fields = GetOrFields(fields);

            foreach (var field in fields)
            {
                var set = new SetModel();

                if (field.Value == "adventuresathogwarts" || field.Value == "aah")
                    set = _setService.GetSet(TypeOfSet.AdventuresAtHogwarts);
                else if (field.Value == "base" || field.Value == "bs")
                    set = _setService.GetSet(TypeOfSet.Base);
                else if (field.Value == "chamberofsecrets" || field.Value == "cos")
                    set = _setService.GetSet(TypeOfSet.ChamberOfSecrets);
                else if (field.Value == "diagonalley" || field.Value == "da")
                    set = _setService.GetSet(TypeOfSet.DiagonAlley);
                else if (field.Value == "heirofslytherin" || field.Value == "hos")
                    set = _setService.GetSet(TypeOfSet.HeirOfSlytherin);
                else if (field.Value == "quidditch" || field.Value == "qc")
                    set = _setService.GetSet(TypeOfSet.QuidditchCup);

                setFields.Add(new AdvancedSearchFieldValue()
                {
                    Field = field.Field,
                    Expression = field.Expression,
                    Value = set.SetId.ToString(),
                    DatabaseColumnName = field.DatabaseColumnName,
                });
            }

            return setFields;
        }
        /// <summary>
        /// Takes the search string from the URL query and parses it through the field methods. These
        /// methods parse the string individually and returns a list of fields for each table. If any
        /// of those fields exist, this method returns true.
        /// </summary>
        public bool ValidateAdvancedSearchParams(string advancedSearchString)
        {
            var cardTableFields = GetCardTableFields(advancedSearchString);
            var cardTableDetailFields = GetCardDetailTableFields(advancedSearchString);
            var cardProvidesLessonTableFields = GetCardProvidesLessonTableFields(advancedSearchString);
            var cardsubTypeTableFields = GetSubTypeTableFields(advancedSearchString);

            return (cardTableFields.Any() || cardTableDetailFields.Any() || cardProvidesLessonTableFields.Any() || cardsubTypeTableFields.Any());
        }
    }
    public static class CharacterExtensionMethods
    {
        public static bool IsGuid(this string text)
        {
            return Guid.TryParse(text, out Guid guid);
        }
        public static bool IsNumber(this string text)
        {
            return int.TryParse(text, out int i);
        }
    }
    //https://stackoverflow.com/a/51632318/1339826
    public static class CharacterConverterExtensionMethods
    {
        public static short ToShort(this string s, short defaultValue = 0) => short.TryParse(s, out var v) ? v : defaultValue;
        public static int ToInt(this string s, int defaultValue = 0) => int.TryParse(s, out var v) ? v : defaultValue;
        public static long ToLong(this string s, long defaultValue = 0) => long.TryParse(s, out var v) ? v : defaultValue;
        public static decimal ToDecimal(this string s, decimal defaultValue = 0) => decimal.TryParse(s, out var v) ? v : defaultValue;
        public static float ToFloat(this string s, float defaultValue = 0) => float.TryParse(s, out var v) ? v : defaultValue;
        public static double ToDouble(this string s, double defaultValue = 0) => double.TryParse(s, out var v) ? v : defaultValue;

        public static short? ToShortNullable(this string s, short? defaultValue = null) => short.TryParse(s, out var v) ? v : defaultValue;
        public static int? ToIntNullable(this string s, int? defaultValue = null) => int.TryParse(s, out var v) ? v : defaultValue;
        public static long? ToLongNullable(this string s, long? defaultValue = null) => long.TryParse(s, out var v) ? v : defaultValue;
        public static decimal? ToDecimalNullable(this string s, decimal? defaultValue = null) => decimal.TryParse(s, out var v) ? v : defaultValue;
        public static float? ToFloatNullable(this string s, float? defaultValue = null) => float.TryParse(s, out var v) ? v : defaultValue;
        public static double? ToDoubleNullable(this string s, double? defaultValue = null) => double.TryParse(s, out var v) ? v : defaultValue;

        public static string ToPipeDelimitedFromCommaDelimited(this string s) => s.Replace(',', '|');
        public static string ToStringWithoutIllegalCharacters(this string s) => s.Replace("+", "").Replace("&", "").Replace("*", "").Replace(":", "")
                                                                                .Replace(">", "").Replace("<", "").Replace("=", "");
        //Adds double quotes to a string that contains spaces
        public static string ToDoubleQuotedString(this string s) => s.Contains(" ") ? $"\"{s.Replace("\"", "")}\"" : s;
    }
}