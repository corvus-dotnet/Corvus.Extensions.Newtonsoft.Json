// <copyright file="PropertyBagExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Corvus.Json
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

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

        /// <summary>
        /// Use a list of non-nullable key value pairs in a place declared as accepting nullable
        /// values.
        /// </summary>
        /// <param name="source">A sequence of non-nullable key value pairs.</param>
        /// <returns>The source sequence, but typed so that the values look nullable.</returns>
        /// <remarks>
        /// <para>
        /// This supports a scenario you'd hope would just work thanks to covariance, but which
        /// unfortunately does not.
        /// </para>
        /// <para>
        /// In general any IEnumerable&lt;T&gt; supports implicit reference conversion to
        /// IEnumerable&lt;B&gt; in any case where a T is a B. For example, if you have an
        /// IEnumerable&lt;string&gt;, an implicit reference conversion to
        /// IEnumerable&lt;object&gt; exists, and the justification for this is that any code
        /// that works with an IEnumerable&lt;object&gt; is prepared for literally anything
        /// to emerge from the sequence, and will therefore be perfectly happy with a
        /// sequence that only ever produces strings. This same logic applies to nullability:
        /// anything expecting to work with an <c>object?</c> can certainly cope with a value
        /// of type <c>object</c>, so an implicit reference conversion exists from
        /// IEnumerable&lt;object&gt; to IEnumerable&lt;object?&gt;. However, this breaks
        /// down when the element type is a <see cref="KeyValuePair{TKey, TValue}"/>,
        /// because that is invariant in both its type arguments. That means that there is
        /// no implicit type conversion between
        /// IEnumerable&lt;KeyValuePair&lt;string, object&gt;&gt; and
        /// IEnumerable&lt;KeyValuePair&lt;string, object?&gt;&gt;. This is frustrating because
        /// their runtime representation is identical—as far as the CLR is concerned,
        /// KeyValuePair&lt;string, object&gt; and KeyValuePair&lt;string, object?&gt; are the
        /// same type. The distinction between these types exists only in the C# type system.
        /// But because only interfaces and delegates support variance, there's no way
        /// for C# to infer safely that <c>KeyValuePair</c> is effectively covariant in both
        /// of its type arguments in cases where the types vary only in nullability. (It's not
        /// true in general for all generic structs. If a struct accepts one of its type
        /// arguments for any sort of input parameter, this covariance will not hold.)
        /// </para>
        /// <para>
        /// This extension method offers a simple explicit conversion from
        /// IEnumerable&lt;KeyValuePair&lt;string, object&gt;&gt; to
        /// IEnumerable&lt;KeyValuePair&lt;string, object?&gt;&gt; to enable code to use property
        /// bags with definitely null values. (Property bags support keys with null values, but
        /// not everything wants to use that.)
        /// </para>
        /// </remarks>
        public static IEnumerable<KeyValuePair<string, object?>> NonNullToNullable(
            this IEnumerable<KeyValuePair<string, object>> source)
        {
            // The way to handle this entirely within the realm of C#'s null-aware type system is
            // to project through a Select, but that's unnecessarily inefficient. We know that
            // the CLR representation is identical, so the easiest way to manage this conversion is
            // to duck temporarily into the old pre-null-aware type system.
#nullable disable
            return source;
#nullable restore
        }

        /// <summary>
        /// Use a list of key value pairs with nullable values in a place that requires
        /// non-nullable values. This throws an exception if any of the values are null.
        /// </summary>
        /// <param name="source">
        /// A sequence of key value pairs typed as having nullable values, but where the
        /// values are all non-null.
        /// </param>
        /// <returns>The source sequence, but typed so that the values look nullable.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if any elements turn out to have non-null values. This is not an
        /// <see cref="ArgumentException"/> because it is thrown when the enumerable is
        /// evaluated, which will be after this method returns.
        /// </exception>
        /// <remarks>
        /// <para>
        /// You can avoid the runtime check with <see cref="NullableToNonNullUnsafe(IEnumerable{KeyValuePair{string, object?}})"/>.
        /// </para>
        /// </remarks>
        public static IEnumerable<KeyValuePair<string, object>> NullableToNonNull(
            this IEnumerable<KeyValuePair<string, object?>> source)
        {
            return source.Select(kv => new KeyValuePair<string, object>(kv.Key, kv.Value ?? throw new InvalidOperationException("Not all elements had non-null values")));
        }

        /// <summary>
        /// Use a list of key value pairs with nullable values in a place that requires
        /// non-nullable values, without checking at runtime that the values are non-null.
        /// </summary>
        /// <param name="source">
        /// A sequence of key value pairs typed as having nullable values, but where the
        /// values are all non-null.
        /// </param>
        /// <returns>The source sequence, but typed so that the values look nullable.</returns>
        /// <remarks>
        /// <para>
        /// You should use this only if you are confident that the source really contains
        /// only non-null values. If in doubt, use <see cref="NonNullToNullable(IEnumerable{KeyValuePair{string, object}})"/>.
        /// </para>
        /// </remarks>
        public static IEnumerable<KeyValuePair<string, object>> NullableToNonNullUnsafe(
            this IEnumerable<KeyValuePair<string, object?>> source)
        {
#nullable disable
            return source;
#nullable restore
        }

        /// <summary>
        /// Creates a property bag from a collection of key pair values where the values are all
        /// typed as non-null objects.
        /// </summary>
        /// <param name="propertyBagFactory">The property bag factory.</param>
        /// <param name="values">The properties for the new bag.</param>
        /// <returns>A new property bag.</returns>
        public static IPropertyBag CreateWithNonNullValues(
            this IPropertyBagFactory propertyBagFactory,
            IEnumerable<KeyValuePair<string, object>> values) =>
                propertyBagFactory.Create(values.NonNullToNullable());

        /// <summary>
        /// Creates a property bag from a callback that produces a collection of key pair values
        /// where the values are all typed as non-null objects.
        /// </summary>
        /// <param name="propertyBagFactory">The property bag factory.</param>
        /// <param name="builder">A function that builds the property collection.</param>
        /// <returns>A new property bag.</returns>
        /// <remarks>
        /// <para>
        /// This supports property builders designed to be chained together, e.g.:
        /// </para>
        /// <code><![CDATA[
        /// IPropertyBag childProperties = this.propertyBagFactory.CreateWithNonNullValues(start =>
        ///     start.AddBlobStorageConfiguration(ContainerDefinition, tenancyStorageConfiguration));
        /// ]]></code>
        /// </remarks>
        public static IPropertyBag CreateWithNonNullValues(
            this IPropertyBagFactory propertyBagFactory,
            Func<IEnumerable<KeyValuePair<string, object>>, IEnumerable<KeyValuePair<string, object>>> builder)
        {
            IEnumerable<KeyValuePair<string, object>> values = builder(PropertyBagValues.EmptyNonNull);
            return propertyBagFactory.CreateWithNonNullValues(values);
        }

        /// <summary>
        /// Creates a new <see cref="IPropertyBag"/> based on an existing bag, but with some
        /// properties either added, updated, or removed, using a callback that produces a
        /// collection of key pair values where the values are all typed as non-null objects to
        /// describe the properties to add or change.
        /// </summary>
        /// <param name="propertyBagFactory">The property bag factory.</param>
        /// <param name="input">The property bag on which to base the new one.</param>
        /// <param name="builder">A function that builds the collection describing the properties to add.</param>
        /// <param name="propertiesToRemove">Optional list of properties to remove.</param>
        /// <returns>A collection of key pair values.</returns>
        /// <remarks>
        /// <para>
        /// Similar to
        /// <see cref="CreateWithNonNullValues(IPropertyBagFactory, Func{IEnumerable{KeyValuePair{string, object}}, IEnumerable{KeyValuePair{string, object}}})"/>,
        /// this supports property builders designed to be chained together. Whereas that method
        /// is for creating a new property bag from scratch, this is suitable for use with
        /// <see cref="IPropertyBagFactory.CreateModified(IPropertyBag, IEnumerable{KeyValuePair{string, object?}}?, IEnumerable{string}?)"/>.
        /// For example:
        /// </para>
        /// <code><![CDATA[
        /// IPropertyBag childProperties = propertyBagFactory.CreateModified(
        ///     existingPropertyBag,
        ///     values => values.AddBlobStorageConfiguration(ContainerDefinition, tenancyStorageConfiguration));
        /// ]]></code>
        /// </remarks>
        public static IPropertyBag CreateModifiedWithNonNullValues(
            this IPropertyBagFactory propertyBagFactory,
            IPropertyBag input,
            Func<IEnumerable<KeyValuePair<string, object>>, IEnumerable<KeyValuePair<string, object>>> builder,
            IEnumerable<string>? propertiesToRemove = null)
        {
            return propertyBagFactory.CreateModified(
                input,
                PropertyBagValues.BuildNonNull(builder),
                propertiesToRemove);
        }
    }
}