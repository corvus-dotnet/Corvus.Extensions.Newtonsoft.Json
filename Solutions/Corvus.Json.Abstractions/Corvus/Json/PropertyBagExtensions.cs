// <copyright file="PropertyBagExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Corvus.Json
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Extension methods for <see cref="IPropertyBag"/>.
    /// </summary>
    public static class PropertyBagExtensions
    {
        /// <summary>
        /// Creates a property bag from a callback that produces a collection of key pair values.
        /// </summary>
        /// <param name="propertyBagFactory">The property bag factory.</param>
        /// <param name="builder">A function that builds the property collection.</param>
        /// <returns>A new property bag.</returns>
        /// <remarks>
        /// <para>
        /// This supports property builders designed to be chained together, e.g.:
        /// </para>
        /// <code><![CDATA[
        /// IPropertyBag childProperties = this.propertyBagFactory.Create(start =>
        ///     start.AddBlobStorageConfiguration(ContainerDefinition, tenancyStorageConfiguration));
        /// ]]></code>
        /// <para>
        /// It calls <see cref="IPropertyBagFactory.Create(IEnumerable{KeyValuePair{string, object}})"/>
        /// with the resulting properties.
        /// </para>
        /// </remarks>
        public static IPropertyBag Create(
            this IPropertyBagFactory propertyBagFactory,
            Func<IEnumerable<KeyValuePair<string, object>>, IEnumerable<KeyValuePair<string, object>>> builder)
        {
            IEnumerable<KeyValuePair<string, object>> values = builder(PropertyBagValues.Empty);
            return propertyBagFactory.Create(values);
        }

        /// <summary>
        /// Creates a new <see cref="IPropertyBag"/> based on an existing bag, but with some
        /// properties either added, updated, or removed, using a callback that produces a
        /// collection of key pair values to describe the properties to add or change.
        /// </summary>
        /// <param name="propertyBagFactory">The property bag factory.</param>
        /// <param name="input">The property bag on which to base the new one.</param>
        /// <param name="builder">A function that builds the collection describing the properties to add.</param>
        /// <param name="propertiesToRemove">Optional list of properties to remove.</param>
        /// <returns>A collection of key pair values.</returns>
        /// <remarks>
        /// <para>
        /// Similar to
        /// <see cref="Create(IPropertyBagFactory, Func{IEnumerable{KeyValuePair{string, object}}, IEnumerable{KeyValuePair{string, object}}})"/>,
        /// this supports property builders designed to be chained together. Whereas that method
        /// is for creating a new property bag from scratch, this invokes
        /// <see cref="IPropertyBagFactory.CreateModified(IPropertyBag, IEnumerable{KeyValuePair{string, object}}?, IEnumerable{string}?)"/>.
        /// For example:
        /// </para>
        /// <code><![CDATA[
        /// IPropertyBag childProperties = propertyBagFactory.CreateModified(
        ///     existingPropertyBag,
        ///     values => values.AddBlobStorageConfiguration(ContainerDefinition, tenancyStorageConfiguration));
        /// ]]></code>
        /// </remarks>
        public static IPropertyBag CreateModified(
            this IPropertyBagFactory propertyBagFactory,
            IPropertyBag input,
            Func<IEnumerable<KeyValuePair<string, object>>, IEnumerable<KeyValuePair<string, object>>> builder,
            IEnumerable<string>? propertiesToRemove = null)
        {
            return propertyBagFactory.CreateModified(
                input,
                builder(PropertyBagValues.Empty),
                propertiesToRemove);
        }

        /// <summary>
        /// Retrieves the properties as a dictionary.
        /// </summary>
        /// <param name="propertyBag">The <see cref="IPropertyBag"/> to convert.</param>
        /// <returns>A dictionary containing all of the properties in the bag.</returns>
        /// <remarks>
        /// <para>
        /// This method extends the <see cref="IPropertyBag.AsDictionary"/> method by recursively processing the
        /// result dictionary and converting any nested <see cref="IPropertyBag"/> instances to further
        /// <see cref="IReadOnlyDictionary{TKey, TValue}"/>s.
        /// </para>
        /// <para>
        /// As a result, this is a potentially expensive operation. It should not be called repeatedly; the result from
        /// a call should be stored and reused.
        /// </para>
        /// </remarks>
        public static IReadOnlyDictionary<string, object> AsDictionaryRecursive(this IPropertyBag propertyBag)
        {
            return RecursivelyProcessDictionary(propertyBag.AsDictionary());
        }

        private static IReadOnlyDictionary<string, object> RecursivelyProcessDictionary(IReadOnlyDictionary<string, object> input)
        {
            return input.ToDictionary(
                x => x.Key,
                x => ProcessPropertyBagItem(x.Value));
        }

        private static object ProcessPropertyBagItem(object input)
        {
            return input switch
            {
                IEnumerable<object> enumerableInput => enumerableInput.Select(ProcessPropertyBagItem).ToArray(),
                IPropertyBag propertyBagInput => propertyBagInput.AsDictionaryRecursive(),
                _ => input,
            };
        }
    }
}