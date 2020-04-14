// <copyright file="PropertyBagExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Corvus.Extensions.Json
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Extension methods for <see cref="IPropertyBag"/>.
    /// </summary>
    public static class PropertyBagExtensions
    {
        /// <summary>
        /// Tries to get a value from a <see cref="IPropertyBag"/> when the value will be non-null
        /// if present.
        /// </summary>
        /// <typeparam name="T">The type of the property required.</typeparam>
        /// <param name="propertyBag">The property bag.</param>
        /// <param name="key">The key of the property required.</param>
        /// <param name="result">
        /// If an entry with the specified key exists, the value will be written to this variable.
        /// Otherwise, this will be set to null.
        /// </param>
        /// <returns>True if an entry with the specified key exists, false otherwise.</returns>
        /// <remarks>
        /// <para>
        /// This deals with the fact that <see cref="IPropertyBag"/> supports present-and-null
        /// entries. It is valid to set a key to null, and this is distinct from the key not
        /// having been set. (When serializing to JSON, the distinction is between the key not
        /// being present, and relevant key being present and set to the JSON <c>null</c> literal.)
        /// In cases where this is not required—where if there is no entry for the key, the value
        /// will be non-null value—the normal <see cref="IPropertyBag.TryGet{T}(string, out T)"/>
        /// interacts unhelpfully with nullable references: in order to avoid warnings at the
        /// point of consumption we need to let the compiler know that the type argument is of
        /// the non-nullable form even though we're passing a nullable argument. If you do the
        /// obvious thing, it doesn't work e.g.:
        /// </para>
        /// <code>
        /// if (properties.TryGetValue(key, out string? value))
        /// </code>
        /// <para>
        /// Here, the compiler will infer that the type argument is <c>string?</c>. In fact that's
        /// not what we intend. (And if you write just <c>out string</c>, the compiler will produce
        /// a warning, because that output will be set to null if no entry with the specified key
        /// exists.) So we have to write this:
        /// </para>
        /// <code>!<![CDATA[
        /// if (properties.TryGetValue<string>(key, out string? value))
        /// ]]></code>
        /// <para>
        /// Here we've used the type argument to state that if there is a value, we definitely
        /// expect it to be a non-nullable <c>string</c>, and the use of <c>string?</c> as the
        /// method argument type is needed to deal with the fact that this will be null in the
        /// case where <c>TryGetValue</c> returns false.
        /// </para>
        /// <para>
        /// This extension method enables callers to avoid spelling out the type name twice, once
        /// as an explicit type argument (in non-nullable form) and then again as an argument type
        /// (in nullable form):
        /// </para>
        /// <code>
        /// if (properties.TryGetNonNullValue(key, out string? value))
        /// </code>
        /// <para>
        /// In this case, the type argument will correctly be inferred as the non-nullable
        /// <c>string</c>.
        /// </para>
        /// </remarks>
        public static bool TryGetNonNullValue<T>(
            this IPropertyBag propertyBag,
            string key,
            [NotNullWhen(true)] out T? result)
            where T : class
        {
            return propertyBag.TryGet<T>(key, out result);
        }
    }
}