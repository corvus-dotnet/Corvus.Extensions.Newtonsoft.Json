// <copyright file="IPropertyBagFactory.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Corvus.Json
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Provides operations for creating, and modifying an <see cref="IPropertyBag"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <see cref="IPropertyBag"/> is a read-only abstraction, since it designed primarily as a
    /// view over a serialized representation. The presumption is that some serialized form exists
    /// somewhere, and that we don't want to deserialize it into objects until we know that the
    /// relevant information is definitely needed. However, there are occasions when new property
    /// bags need to be built, most notably when modifying or creating new instances of some type
    /// that has a property bag. (For example, Corvus.Tenancy's ITenant includes a property bag,
    /// and in cases where we create a new tenant, or we modify the properties of an existing
    /// tenant, <see cref="IPropertyBag"/>'s read-only abstraction is no longer sufficient.)
    /// </para>
    /// <para>
    /// This interface supports these "create" and "modify" scenarios. Code that needs to work
    /// with property bags can use this interface without needing to know which particular
    /// implementation is in use. (E.g. Json.NET vs System.Text.Json.)
    /// </para>
    /// </remarks>
    public interface IPropertyBagFactory
    {
        /// <summary>
        /// Creates a new <see cref="IPropertyBag"/> with the specified properties.
        /// </summary>
        /// <param name="values">
        /// The values with which to populate the property bag.
        /// </param>
        /// <returns>
        /// A new property bag.
        /// </returns>
        /// <remarks>
        /// This supports scenarios where a serialized form of the properties does not yet exist
        /// (e.g., creation of new entities).
        /// </remarks>
        IPropertyBag Create(IEnumerable<KeyValuePair<string, object>> values);

        /// <summary>
        /// Creates a new <see cref="IPropertyBag"/> based on an existing bag, but with some
        /// properties either added, updated, or removed.
        /// </summary>
        /// <param name="input">The property bag on which to base the new bag.</param>
        /// <param name="propertiesToSetOrAdd">
        /// Key and value pairs to update or add.
        /// </param>
        /// <param name="propertiesToRemove">
        /// The keys of the properties to remove.
        /// </param>
        /// <returns>A new <see cref="IPropertyBag"/> with the specified changes.</returns>
        /// <exception cref="ArgumentNullException">
        /// At least one of <c>propertiesToSetOrAdd</c> or <c>propertiesToRemove</c> must be non-null.
        /// If both are null, the call would have no effect, so we throw this exception.
        /// </exception>
        /// <remarks>
        /// This supports scenarios where properties are being modified.
        /// </remarks>
        IPropertyBag CreateModified(
            IPropertyBag input,
            IEnumerable<KeyValuePair<string, object>>? propertiesToSetOrAdd,
            IEnumerable<string>? propertiesToRemove);
    }
}