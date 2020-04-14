// <copyright file="PropertyBagConverter.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Corvus.Extensions.Json.Internal
{
    using System;
    using System.Threading;
    using Corvus.Extensions.Json;
    using Microsoft.Extensions.DependencyInjection;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// A standard json converter for <see cref="JsonNetPropertyBag"/>.
    /// </summary>
    public class PropertyBagConverter : JsonConverter
    {
        private readonly Lazy<IJsonSerializerSettingsProvider> jsonSerializerSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyBagConverter"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider for the context.</param>
        public PropertyBagConverter(IServiceProvider serviceProvider)
        {
            this.jsonSerializerSettings = new Lazy<IJsonSerializerSettingsProvider>(() => serviceProvider.GetService<IJsonSerializerSettingsProvider>(), LazyThreadSafetyMode.ExecutionAndPublication);
        }

        /// <inheritdoc/>
        public override bool CanConvert(Type objectType)
        {
            return typeof(IPropertyBag) == objectType || typeof(JsonNetPropertyBag) == objectType;
        }

        /// <inheritdoc/>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader is null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            var value = (JObject)JToken.ReadFrom(reader);
            return new JsonNetPropertyBag(value, this.jsonSerializerSettings.Value.Instance);
        }

        /// <inheritdoc/>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (writer is null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            if (serializer is null)
            {
                throw new ArgumentNullException(nameof(serializer));
            }

            if (value is null)
            {
                writer.WriteNull();
            }
            else
            {
                var propertyBag = (JObject)(JsonNetPropertyBag)value;
                propertyBag.WriteTo(writer);
            }
        }
    }
}