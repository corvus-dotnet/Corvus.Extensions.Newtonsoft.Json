// <copyright file="PropertyBag.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Corvus.Extensions.Json
{
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// A property bag that serializes neatly.
    /// </summary>
    public class PropertyBag
    {
        /// <summary>
        /// Gets the fallback default JsonSerializerSettings.
        /// </summary>
        public static readonly JsonSerializerSettings DefaultJsonSerializerSettings = new JsonSerializerSettings();

        private JObject properties;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyBag"/> class.
        /// </summary>
        public PropertyBag()
            : this(default(JsonSerializerSettings))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyBag"/> class.
        /// </summary>
        /// <param name="jobject">The JObject from which to initialize the property bag.</param>
        /// <param name="serializerSettings">The serializer settings to use for the property bag.</param>
        public PropertyBag(JObject jobject, JsonSerializerSettings serializerSettings = null)
        {
            this.Properties = jobject ?? throw new System.ArgumentNullException(nameof(jobject));
            this.SerializerSettings = serializerSettings ?? JsonConvert.DefaultSettings?.Invoke() ?? DefaultJsonSerializerSettings;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyBag"/> class.
        /// </summary>
        /// <param name="dictionary">A dictionary with which to initialize the bag.</param>
        /// <param name="serializerSettings">The serializer settings to use for the property bag.</param>
        public PropertyBag(IDictionary<string, object> dictionary, JsonSerializerSettings serializerSettings = null)
        {
            if (dictionary is null)
            {
                throw new System.ArgumentNullException(nameof(dictionary));
            }

            foreach (KeyValuePair<string, object> kvp in dictionary)
            {
                this.Set(kvp.Key, kvp.Value);
            }

            this.SerializerSettings = serializerSettings ?? JsonConvert.DefaultSettings?.Invoke() ?? DefaultJsonSerializerSettings;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyBag"/> class.
        /// </summary>
        /// <param name="serializerSettingsProvider">The serializer settings provider.</param>
        public PropertyBag(IJsonSerializerSettingsProvider serializerSettingsProvider)
        {
            if (serializerSettingsProvider is null)
            {
                throw new System.ArgumentNullException(nameof(serializerSettingsProvider));
            }

            this.Properties = new JObject();
            this.SerializerSettings = serializerSettingsProvider.Instance;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyBag"/> class.
        /// </summary>
        /// <param name="serializerSettings">The serializer settings to use for the property bag.</param>
        public PropertyBag(JsonSerializerSettings serializerSettings)
        {
            this.Properties = new JObject();
            this.SerializerSettings = serializerSettings ?? JsonConvert.DefaultSettings?.Invoke() ?? DefaultJsonSerializerSettings;
        }

        /// <summary>
        /// Gets the serializer settings for the property bag.
        /// </summary>
        public JsonSerializerSettings SerializerSettings { get; private set; }

        /// <summary>
        /// Gets or sets the underlying JObject which captures the extension properties.
        /// </summary>
        internal JObject Properties
        {
            get
            {
                return this.properties ?? (this.properties = new JObject());
            }

            set
            {
                this.properties = value;
            }
        }

        /// <summary>
        /// Implicit cast from <see cref="PropertyBag"/> to <see cref="JObject"/>.
        /// </summary>
        /// <param name="bag">The property bag to cast.</param>
        public static implicit operator JObject(PropertyBag bag)
        {
            if (bag is null)
            {
                throw new System.ArgumentNullException(nameof(bag));
            }

            return bag.Properties;
        }

        /// <summary>
        /// Get a strongly typed property.
        /// </summary>
        /// <typeparam name="T">The type of the result.</typeparam>
        /// <param name="key">The property key.</param>
        /// <param name="result">The result.</param>
        /// <returns>True if the object was found.</returns>
        public bool TryGet<T>(string key, out T result)
        {
            JToken jtoken = this.Properties[key];
            if (jtoken == null)
            {
                result = default;
                return false;
            }

            if (jtoken.Type == JTokenType.Null)
            {
                result = default;
                return true;
            }

            // We do this weird double check to avoid the deepclone of a potentially deep jtoken
            // if it isn't a JToken, but such that we can still get the conversion-to-T efficiently, without
            // having to add a dependency on Corvus.Extensions CastTo for this single line.
            if (typeof(JToken).IsAssignableFrom(typeof(T)) && jtoken.DeepClone() is T tToken)
            {
                result = tToken;
                return true;
            }

            using (JsonReader reader = jtoken.CreateReader())
            {
                try
                {
                    result = JsonSerializer.Create(this.SerializerSettings).Deserialize<T>(reader);
                    return true;
                }
                catch (JsonSerializationException)
                {
                    result = default;
                    return false;
                }
            }
        }

        /// <summary>
        /// Set a strongly typed property.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="key">The key for the property.</param>
        /// <param name="value">The value of the property.</param>
        public void Set<T>(string key, T value)
        {
            Comparer<T> comparer = Comparer<T>.Default;

            if (typeof(T).IsValueType)
            {
                // We can just serialize because it is a value type
               ReplaceIfExists(this.Properties, key, this.ConvertToJToken(value));
            }
            else
            {
                // We have to deal with nulls if it is not a value type
                ReplaceIfExists(this.Properties, key, comparer.Compare(value, default) == 0 ? new JValue((object)null) : this.ConvertToJToken(value));
            }
        }

        /// <summary>
        /// Replaces an item if there is an item with that key in the dictionary, otherwise adds it.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="dictionary">The dictionary to which to add the item.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns>True if the item was replaced, false if it was added.</returns>
        private static bool ReplaceIfExists<TKey, TValue>(IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (dictionary.ContainsKey(key))
            {
                dictionary[key] = value;
                return true;
            }
            else
            {
                dictionary.Add(key, value);
                return false;
            }
        }

        private JToken ConvertToJToken<T>(T value)
        {
            if (value is JToken jt)
            {
                return jt.DeepClone();
            }

            return JToken.FromObject(value, JsonSerializer.Create(this.SerializerSettings));
        }
    }
}
