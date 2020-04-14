// <copyright file="JsonSerializerSettingsProviderServiceCollectionExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Microsoft.Extensions.DependencyInjection
{
    using System.Linq;
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
            if (services is null)
            {
                throw new System.ArgumentNullException(nameof(services));
            }

            if (services.Any(s => typeof(IJsonSerializerSettingsProvider).IsAssignableFrom(s.ServiceType)))
            {
                // Already configured
                return services;
            }

            services.AddSingleton<IJsonNetPropertyBagFactory, JsonNetPropertyBagFactory>();
            services.AddSingleton<IPropertyBagFactory>(sp => sp.GetRequiredService<IJsonNetPropertyBagFactory>());

            services.AddSingleton<JsonConverter, CultureInfoConverter>();
            services.AddSingleton<JsonConverter, DateTimeOffsetConverter>();
            services.AddSingleton<JsonConverter, PropertyBagConverter>();
            services.AddSingleton<JsonConverter>(new StringEnumConverter(true));
            services.AddSingleton<IJsonSerializerSettingsProvider, JsonSerializerSettingsProvider>();
            return services;
        }
    }
}