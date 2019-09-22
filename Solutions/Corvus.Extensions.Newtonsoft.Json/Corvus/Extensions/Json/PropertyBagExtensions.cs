// <copyright file="PropertyBagExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Corvus.Extensions.Json
{
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Extension methods for <see cref="PropertyBag"/>.
    /// </summary>
    public static class PropertyBagExtensions
    {
        /// <summary>
        /// Get the property bag as a <see cref="IDictionary{TKey, TValue}"/> of <see cref="string"/> to <see cref="object"/>.
        /// </summary>
        /// <param name="propertyBag">The property bag.</param>
        /// <returns>The property bag as a dictionary of objects (which can be cast to <see cref="JToken"/>).</returns>
        public static IDictionary<string, object> AsDictionary(this PropertyBag propertyBag)
        {
            return propertyBag.Properties.ToObject<Dictionary<string, object>>();
        }
    }
}
