// <copyright file="PropertyBagValues.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Corvus.Extensions.Json
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Helps build values in the form required by <see cref="IPropertyBagFactory"/>.
    /// </summary>
    /// <remarks>
    /// <b>TODO:</b> should we introduce a formal PropertyBagBuilder instead of just using
    /// IEnumerable for everything? Not sure if this makes the null/non-null problem better
    /// or worse.
    /// </remarks>
    public static class PropertyBagValues
    {
        /// <summary>
        /// Gets an empty configuration for a property bag.
        /// </summary>
        /// <remarks>
        /// <para>
        /// There are two main scenarios for this property.
        /// </para>
        /// <para>
        /// One is if you need to create an empty property bag. If you have an implementation of
        /// <see cref="IPropertyBagFactory"/> to hand you can pass this to its
        /// <see cref="IPropertyBagFactory.Create(IEnumerable{KeyValuePair{string, object?}})"/>
        /// method:
        /// </para>
        /// <code><![CDATA[
        /// IPropertyBag empty = propertyBagFactory.Create(PropertyBagConfiguration.Empty);
        /// ]]></code>
        /// <para>
        /// The second is when building up property bags with functions that chain together,
        /// and which therefore require an initial enumerable sequence to start with. For
        /// example, Corvus.Tenancy's tenanted storage providers offer extension methods for
        /// adding their configuration to an existing collection. This can act as the starting
        /// point for a chain of such methods, e.g.:
        /// </para>
        /// <code><![CDATA[
        /// IPropertyBag childProperties = this.propertyBagFactory.Create(
        ///     PropertyBagValues
        ///         .Empty
        ///         .AddBlobStorageConfiguration(ContainerDefinition, tenancyStorageConfiguration)
        ///         .CreateWithNonNullValues());
        /// ]]></code>
        /// </remarks>
        public static IEnumerable<KeyValuePair<string, object>> Empty { get; }
            = Enumerable.Empty<KeyValuePair<string, object>>();
    }
}