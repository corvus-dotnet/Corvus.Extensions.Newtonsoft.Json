// <copyright file="IJsonNetPropertyBag.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Corvus.Extensions.Json
{
    using System.Collections.Generic;
    using Corvus.Json;

    /// <summary>
    /// Json.NET-specific property bag functionality.
    /// </summary>
    public interface IJsonNetPropertyBag : IPropertyBag
    {
        /// <summary>
        /// Retrieves the properties as a dictionary.
        /// </summary>
        /// <returns>A dictionary containing all of the properties in the bag.</returns>
        /// <remarks>
        /// <para>
        /// The types of the individual entries will depend on how the property bag was created.
        /// If it was created with <see cref="IPropertyBagFactory.Create(IEnumerable{KeyValuePair{string, object}})"/>,
        /// this will typically return the exact same information. However, if the property bag was
        /// deserialized from JSON, the values' types may be specific to the property bag implementation
        /// in use. (For example, if you use the Json.NET implementation, you might get <c>JObject</c>
        /// values.)
        /// </para>
        /// </remarks>
        IReadOnlyDictionary<string, object> AsDictionary();
    }
}