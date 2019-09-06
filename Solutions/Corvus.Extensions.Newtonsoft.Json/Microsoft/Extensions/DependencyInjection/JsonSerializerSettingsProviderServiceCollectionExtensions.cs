// <copyright file="JsonSerializerSettingsProviderServiceCollectionExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Microsoft.Extensions.DependencyInjection
{
    using Corvus.Extensions.Json;
    using Corvus.Extensions.Json.Internal;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// An installer for standard <see cref="JsonConverter"/>s.
    /// </summary>
    public static class JsonSerializerSettingsProviderServiceCollectionExtensions
    {
        /// <summary>
        /// Add the default JSON serialization settings.
        /// </summary>
        /// <param name="services">The target service collection.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddJsonSerializerSettings(this IServiceCollection services)
        {
            services.AddSingleton<JsonConverter, CultureInfoConverter>();
            services.AddSingleton<JsonConverter, DateTimeOffsetConverter>();
            services.AddSingleton<JsonConverter, PropertyBagConverter>();
            services.AddSingleton<JsonConverter>(new StringEnumConverter(true));
            services.AddSingleton<IJsonSerializerSettingsProvider, JsonSerializerSettingsProvider>();
            return services;
        }
    }
}