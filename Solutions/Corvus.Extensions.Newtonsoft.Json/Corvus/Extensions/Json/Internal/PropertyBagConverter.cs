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
    using Newtonsoft.Json.Converters;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// A standard json converter for <see cref="PropertyBag"/>.
    /// </summary>
    public class PropertyBagConverter : JsonConverter
    {
        private readonly IServiceProvider serviceProvider;
        private readonly Lazy<IJsonSerializerSettingsProvider> jsonSerializerSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyBagConverter"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider for the context.</param>
        public PropertyBagConverter(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            this.jsonSerializerSettings = new Lazy<IJsonSerializerSettingsProvider>(() => this.serviceProvider.GetService<IJsonSerializerSettingsProvider>(), LazyThreadSafetyMode.ExecutionAndPublication);
        }

        /// <inheritdoc/>
        public override bool CanConvert(Type objectType)
        {
            return typeof(PropertyBag) == objectType;
        }

        /// <inheritdoc/>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var value = JToken.ReadFrom(reader) as JObject;
            return new PropertyBag(this.jsonSerializerSettings.Value.Instance) { Properties = value };
        }

        /// <inheritdoc/>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var propertyBag = value as PropertyBag;
            serializer.Serialize(writer, propertyBag.Properties);
        }
    }
}
