// <copyright file="JsonNetPropertyBagFactory.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Corvus.Extensions.Json.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Corvus.Extensions.Json;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// TODO: this needs to move into wherever IPropertyBag ends up being defined.
    /// </summary>
    internal class JsonNetPropertyBagFactory : IJsonNetPropertyBagFactory
    {
        private readonly JsonSerializerSettings serializerSettings;

        /// <summary>
        /// Creates a <see cref="JsonNetPropertyBagFactory"/>.
        /// </summary>
        /// <param name="serializerSettingsProvider">Provides serialization settings.</param>
        public JsonNetPropertyBagFactory(IJsonSerializerSettingsProvider serializerSettingsProvider)
        {
            if (serializerSettingsProvider is null)
            {
                throw new ArgumentNullException(nameof(serializerSettingsProvider));
            }

            this.serializerSettings = serializerSettingsProvider.Instance;
        }

        /// <inheritdoc/>
        public JObject AsJObject(IPropertyBag propertyBag)
        {
            if (!(propertyBag is JsonNetPropertyBag jsonNetPropertyBag))
            {
                throw new ArgumentException($"Not a {nameof(JsonNetPropertyBag)}", nameof(propertyBag));
            }

            return jsonNetPropertyBag;
        }

        /// <inheritdoc/>
        public IPropertyBag Create(IEnumerable<KeyValuePair<string, object?>> values)
        {
            return new JsonNetPropertyBag(values.ToDictionary(kv => kv.Key, kv => kv.Value), this.serializerSettings);
        }

        /// <inheritdoc/>
        public IPropertyBag Create(JObject jObject)
        {
            return new JsonNetPropertyBag(jObject, this.serializerSettings);
        }

        /// <inheritdoc/>
        public IPropertyBag CreateModified(
            IPropertyBag input,
            IEnumerable<KeyValuePair<string, object?>>? propertiesToSetOrAdd,
            IEnumerable<string>? propertiesToRemove)
        {
            var pb = (JsonNetPropertyBag)input;
            IReadOnlyDictionary<string, object?> existingProperties = pb.AsDictionary();
            Dictionary<string, object?> newProperties = propertiesToSetOrAdd?.ToDictionary(kv => kv.Key, kv => kv.Value)
                ?? new Dictionary<string, object?>();
            HashSet<string>? remove = propertiesToRemove == null ? null : new HashSet<string>(propertiesToRemove);
            foreach (KeyValuePair<string, object?> existingKv in existingProperties)
            {
                string key = existingKv.Key;
                bool newPropertyWithThisNameExists = newProperties.ContainsKey(key);
                bool existingPropertyIsToBeRemoved = remove?.Contains(key) == true;
                if (newPropertyWithThisNameExists && existingPropertyIsToBeRemoved)
                {
                    throw new ArgumentException($"Property {key} appears in both {nameof(propertiesToSetOrAdd)} and {nameof(propertiesToRemove)}");
                }

                if (!newPropertyWithThisNameExists && !existingPropertyIsToBeRemoved)
                {
                    newProperties.Add(key, existingKv.Value);
                }
            }

            return new JsonNetPropertyBag(newProperties, this.serializerSettings);
        }
    }
}