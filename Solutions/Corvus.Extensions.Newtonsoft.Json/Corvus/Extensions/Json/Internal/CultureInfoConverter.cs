// <copyright file="CultureInfoConverter.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Corvus.Extensions.Json.Internal
{
    using System;
    using System.Globalization;
    using Newtonsoft.Json;

    /// <summary>
    /// A standard json converter for <see cref="CultureInfo"/>.
    /// </summary>
    public class CultureInfoConverter : JsonConverter
    {
        /// <inheritdoc/>
        public override bool CanConvert(Type objectType)
        {
            if (objectType is null)
            {
                throw new ArgumentNullException(nameof(objectType));
            }

            return typeof(CultureInfo) == objectType;
        }

        /// <inheritdoc/>
        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            if (reader is null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            if (serializer is null)
            {
                throw new ArgumentNullException(nameof(serializer));
            }

            string? value = (string?)reader.Value;
            if (value != null)
            {
                return new CultureInfo(value);
            }

            return null;
        }

        /// <inheritdoc/>
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if (writer is null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            if (value is null)
            {
                writer.WriteNull();
            }

            if (value is CultureInfo ci)
            {
                writer.WriteValue(ci.Name);
            }
            else
            {
                throw new ArgumentException("The object passed was not a CultureInfo", nameof(value));
            }
        }
    }
}