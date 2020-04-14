// <copyright file="IPropertyBag.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Corvus.Extensions.Json
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

#pragma warning disable SA1629 // Documentation text should end with a period - this is a work in progress, hence the outstanding question marks
    /// <summary>
    /// A read-only key/value collection, typically implemented over a serialized store.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>TODO:</b> this does not belong in <c>Corvus.Extensions.Newtonsoft.Json</c>. This is
    /// currently in prototype form, but before we're done we should create something such as
    /// <c>Corvus.Extensions.Json.Abstractions</c> or <c>Corvus.Json.Abstractions</c> or
    /// <c>Corvus.Serialization.Abstractions</c> as a place for non-serialization-stack-specific
    /// serialization-related types.
    /// </para>
    /// <para>
    /// <b>TODO:</b> do we explicitly commit to the idea that the serialized store is JSON-like?
    /// Currently, we expect it to be. The benefit of baking that into this interface is that
    /// it clarifies exactly what sorts of types might reasonable live in a property bag. The
    /// downside is the computing industry's tendency to invent new serialization formats every
    /// few year. What if we decide that GRPC's wire representation is something we want to use?
    /// </para>
    /// </remarks>
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
        /// <remarks>
        /// <para>
        /// <b>TODO:</b> what do we do if the serialized data wants to be of some type that is not
        /// compatible with T?. Also, do we have any special handling for deserialization errors?
        /// </para>
        /// </remarks>
        bool TryGet<T>(string key, [MaybeNullWhen(false)] out T result);

        /// <summary>
        /// Retrieves the properties as a dictionary.
        /// </summary>
        /// <returns>A dictionary containing all of the properties in the bag.</returns>
        IReadOnlyDictionary<string, object?> AsDictionary();
    }
#pragma warning restore SA1629 // Documentation text should end with a period
}
