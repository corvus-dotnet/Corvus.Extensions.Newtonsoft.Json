// <copyright file="PropertyBagEntryType.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Corvus.Json
{
    using System.Collections.Generic;

    /// <summary>
    /// Describes the data type of an entry in an <see cref="IPropertyBag"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Implementations of <see cref="IPropertyBag"/> that allow consumers to discover the contents
    /// at runtime will implement <see cref="IEnumerable{T}"/> of <c>(string key, PropertyBagEntryType type</c>
    /// tuples.
    /// </para>
    /// </remarks>
    public enum PropertyBagEntryType
    {
        /// <summary>
        /// A null value.
        /// </summary>
        Null,

        /// <summary>
        /// A string value.
        /// </summary>
        String,

        /// <summary>
        /// A true or false value.
        /// </summary>
        Boolean,

        /// <summary>
        /// A number with no decimal point.
        /// </summary>
        Integer,

        /// <summary>
        /// A number with a decimal point in it.
        /// </summary>
        Decimal,

        /// <summary>
        /// An array.
        /// </summary>
        Array,

        /// <summary>
        /// A nested object.
        /// </summary>
        Object,
    }
}