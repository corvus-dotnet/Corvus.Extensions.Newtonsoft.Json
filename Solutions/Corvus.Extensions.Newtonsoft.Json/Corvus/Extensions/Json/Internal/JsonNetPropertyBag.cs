﻿// <copyright file="JsonNetPropertyBag.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Corvus.Extensions.Json.Internal
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Corvus.Json;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// A property bag that serializes neatly.
    /// </summary>
    internal class JsonNetPropertyBag : IPropertyBag
    {
        private readonly JsonSerializerSettings serializerSettings;
        private readonly JObject properties;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonNetPropertyBag"/> class.
        /// </summary>
        /// <param name="jobject">The JObject from which to initialize the property bag.</param>
        /// <param name="serializerSettings">The serializer settings to use for the property bag.</param>
        public JsonNetPropertyBag(JObject jobject, JsonSerializerSettings serializerSettings)
        {
            this.properties = jobject ?? throw new System.ArgumentNullException(nameof(jobject));
            this.serializerSettings = serializerSettings;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonNetPropertyBag"/> class.
        /// </summary>
        /// <param name="dictionary">A dictionary with which to initialize the bag.</param>
        /// <param name="serializerSettings">The serializer settings to use for the property bag.</param>
        public JsonNetPropertyBag(IDictionary<string, object?> dictionary, JsonSerializerSettings serializerSettings)
        {
            if (dictionary is null)
            {
                throw new System.ArgumentNullException(nameof(dictionary));
            }

            this.properties = new JObject();
            foreach (KeyValuePair<string, object?> kvp in dictionary)
            {
                (string key, object? value) = (kvp.Key, kvp.Value);

                // We have to deal with nulls if it is not a value type
                this.properties[key] = value is null ? JValue.CreateNull() : this.ConvertToJToken(value);
            }

            this.serializerSettings = serializerSettings;
        }

        /// <summary>
        /// Implicit cast from <see cref="JsonNetPropertyBag"/> to <see cref="JObject"/>.
        /// </summary>
        /// <param name="bag">The property bag to cast.</param>
        public static implicit operator JObject(JsonNetPropertyBag bag)
        {
            if (bag is null)
            {
                throw new System.ArgumentNullException(nameof(bag));
            }

            return bag.properties;
        }

        /// <summary>
        /// Get a strongly typed property.
        /// </summary>
        /// <typeparam name="T">The type of the result.</typeparam>
        /// <param name="key">The property key.</param>
        /// <param name="result">The result.</param>
        /// <returns>True if the object was found.</returns>
        public bool TryGet<T>(string key, [MaybeNullWhen(false)] out T result)
        {
            JToken jtoken = this.properties[key];
            if (jtoken == null)
            {
                result = default!;
                return false;
            }

            // Note that this may seem like it violates the [MaybeNullWhen(false)] promise made in
            // the method signature, as the main purpose of that is to enable code that calls this
            // method to avoid nullability warnings in the body of an <c>if</c> that calls this
            // method. However, this is logically equivalent to what happens with an
            // IDictionary<TKey, TValue>. In cases where TValue is non-nullable, the MayBeNull(false)
            // in IDictionary<TKey, TValue>.TryGetvalue indicates that the out argument may sometimes
            // be null. However, in cases where TValue is nullable it has no effect, because nulls
            // are expected whether an entry with the relevant key is present or not. And it's the same
            // here: if a caller expects the "present, and null" case, they would call, say,
            // TryGet<Result?> instead of TryGet<Result>.
            // This is why we use MaybeNullWhen instead of NotNullWhen.
            if (jtoken.Type == JTokenType.Null)
            {
                result = default!;
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

            using JsonReader reader = jtoken.CreateReader();
            try
            {
                result = JsonSerializer.Create(this.serializerSettings).Deserialize<T>(reader);
                return true;
            }
            catch (JsonSerializationException ex)
            {
                throw new SerializationException(ex);
            }
        }

        /// <inheritdoc/>
        public IReadOnlyDictionary<string, object?> AsDictionary() => this.properties.ToObject<Dictionary<string, object?>>();

        private JToken ConvertToJToken<T>(T value)
        {
            if (value is JToken jt)
            {
                return jt.DeepClone();
            }

            return JToken.FromObject(value, JsonSerializer.Create(this.serializerSettings));
        }
    }
}