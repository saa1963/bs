using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace bs
{
    public class OrderProductSnJConverter : JsonConverter<OrderProductSn>
    {
        public OrderProductSnJConverter()
        {

        }
        public override bool CanConvert(Type objectType)
        {
            return typeof(OrderProductSn).IsAssignableFrom(objectType);
        }

        public override OrderProductSn? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            Utf8JsonReader readerClone = reader;

            if (readerClone.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }

            readerClone.Read();
            if (readerClone.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException();
            }

            string? propertyName = readerClone.GetString();
            if (propertyName != "templateId")
            {
                throw new JsonException();
            }

            readerClone.Read();
            if (readerClone.TokenType != JsonTokenType.Number)
            {
                throw new JsonException();
            }

            var templateId = readerClone.GetInt32();
            OrderProductSn? op = templateId switch
            {
                1 => JsonSerializer.Deserialize<OrderProductShoesSn>(ref reader),
                7 => JsonSerializer.Deserialize<OrderProductTiresSn>(ref reader),
                10 => JsonSerializer.Deserialize<OrderProductLpSn>(ref reader),
                _ => throw new JsonException()
            };
            return op;
        }

        public override void Write(Utf8JsonWriter writer, OrderProductSn value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
