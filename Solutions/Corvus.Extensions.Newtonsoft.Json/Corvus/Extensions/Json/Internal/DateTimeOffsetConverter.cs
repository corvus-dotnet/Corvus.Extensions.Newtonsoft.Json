// <copyright file="DateTimeOffsetConverter.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Corvus.Extensions.Json.Internal
{
    using System;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// A standard json converter for <see cref="DateTimeOffset"/>.
    /// </summary>
    public class DateTimeOffsetConverter : JsonConverter
    {
        /// <inheritdoc/>
        public override bool CanConvert(Type objectType)
        {
            if (objectType is null)
            {
                throw new ArgumentNullException(nameof(objectType));
            }

            return typeof(DateTimeOffset) == objectType || typeof(DateTimeOffset?) == objectType;
        }

        /// <inheritdoc/>
        public override object? ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader is null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            var token = JToken.Load(reader);

            if (token.Type == JTokenType.Object)
            {
                token = token["dateTimeOffset"];
            }

            JValue value = token as JValue ?? throw new InvalidOperationException($"DateTimeOffsetConverter could not deserialize token of type '{token.Type}' at '{reader.Path}'.");

            return value.Value switch
            {
                DateTime => throw new InvalidOperationException("Cannot use the DateTimeOffsetConverter when JsonSerializerSettings.DateParseHandling is set to DateTime."),
                string s => DateTimeOffset.Parse(s),
                _ => (DateTimeOffset)value
            };
        }

        /// <inheritdoc/>
        /// <remarks>
        /// <para>
        /// Note that this will write the <see cref="DateTimeOffset"/> as a complex object containing both an ISO Date Time string with timezone, and a unix time long.
        /// </para>
        /// <code>
        /// ![<![CDATA[ { "dateTimeOffset": "2009-06-15T13:45:30.0000000-07:00", "unixTime": 1245098730000]]>
        /// </code>
        /// </remarks>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (writer is null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            if (value is null)
            {
                writer.WriteNull();
            }
            else
            {
                var dto = (DateTimeOffset)value;

                writer.WriteStartObject();
                writer.WritePropertyName("dateTimeOffset");
                writer.WriteRawValue($"\"{dto:O}\"");
                writer.WritePropertyName("unixTime");
                writer.WriteValue(dto.ToUnixTimeMilliseconds());
                writer.WriteEndObject();
            }
        }
    }
}
