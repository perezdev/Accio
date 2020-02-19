using HpTcgCardBrowser.SetUpload.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace HpTcgCardBrowser.SetUpload.CustomJsonConverters
{
    public class DescriptionConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var token = JToken.Load(reader);

            /*
             * The description property changes type based on the value.Which changes based on the card type.If it's a normal card,
             * it only has one description.But if it's an adventure, it has a few fields describing the actions of the card.            
             * These would normally be different types of objects, but I decided to just have one description object that holds
             * the value for a normal card and the adventure card. Later, we'll do null checks on the properties to determine how
             * to display them.
             */

            /*
             * The logic here is very simple. If the object type is a string, we know that the card is a normal card.
             * So we just create a new model and set the text.
             * But if it's an adventure card, we'll convert to to a desc model and the text property will be ignored.
             */

            if (token.Type == JTokenType.String)
            {
                return new DescriptionModel()
                {
                    Text = token.Value<string>()
                };
            }
            else if (token.Type == JTokenType.Object)
            {
                return token.ToObject<DescriptionModel>();
            }
            
            return null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
