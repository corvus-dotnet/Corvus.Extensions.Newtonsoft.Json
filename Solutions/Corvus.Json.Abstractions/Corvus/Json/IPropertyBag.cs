// <copyright file="IPropertyBag.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Corvus.Json
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// A read-only key/value collection, typically implemented over a serialized store.
    /// </summary>
    public interface IPropertyBag
    {
        /// <summary>
        /// Determines whether the bag contains a value with the specified key, and if so retreives
        /// it. This will typically deserialize the relevant data.
        /// </summary>
        /// <typeparam name="T">The expected value type.</typeparam>
        /// <param name="key">The key identifying the entry in the bag.</param>
        /// <param name="result">
        /// The variable in which to store the result. Will be set to <c>default(T)</c> if no entry
        /// with the specified key exists.</param>
        /// <returns>
        /// True if a value with the specified key was present. False if not.
        /// </returns>
        /// <exception cref="SerializationException">
        /// Thrown if the data is present but cannot be deserialized to the specified type.
        /// </exception>
        bool TryGet<T>(string key, [MaybeNullWhen(false)] out T result);

        /// <summary>
        /// Retrieves the properties as a dictionary.
        /// </summary>
        /// <returns>A dictionary containing all of the properties in the bag.</returns>
        IReadOnlyDictionary<string, object?> AsDictionary();
    }
}