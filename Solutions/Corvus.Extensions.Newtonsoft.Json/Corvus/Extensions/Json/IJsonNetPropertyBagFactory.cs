// <copyright file="IJsonNetPropertyBagFactory.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Corvus.Extensions.Json
{
    using System;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Provides Json.NET-specific operations for working with an <see cref="IPropertyBag"/>.
    /// </summary>
    public interface IJsonNetPropertyBagFactory : IPropertyBagFactory
    {
        /// <summary>
        /// Creates a new <see cref="IPropertyBag"/> with the specified properties.
        /// </summary>
        /// <param name="jObject">
        /// The JSON from which to populate the property bag.
        /// </param>
        /// <returns>
        /// A new property bag.
        /// </returns>
        IPropertyBag Create(JObject jObject);

        /// <summary>
        /// Converts an <see cref="IPropertyBag"/> to a <see cref="JObject"/>.
        /// </summary>
        /// <param name="propertyBag">The property bag.</param>
        /// <returns>The serialized property bag.</returns>
        /// <exception cref="ArgumentException">
        /// Thrown if the property bag does not support conversion to <see cref="JObject"/>.
        /// The property bag must have been created by ths Json.NET property bag factory
        /// or associated deserialization logic for this call to succeed.
        /// </exception>
        JObject AsJObject(IPropertyBag propertyBag);
    }
}
