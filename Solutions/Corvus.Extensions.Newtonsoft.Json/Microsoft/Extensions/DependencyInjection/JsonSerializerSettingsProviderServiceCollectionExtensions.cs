// <copyright file="JsonSerializerSettingsProviderServiceCollectionExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Microsoft.Extensions.DependencyInjection
{
    using System;
    using System.Globalization;
    using System.Linq;
    using Corvus.Extensions.Json;
    using Corvus.Extensions.Json.Internal;
    using Corvus.Json;
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
        /// <remarks>
        /// <para>
        /// Calling this method is now equivalent to calling the following methods:
        /// </para>
        /// <list type="bullet">
        ///     <item><c>AddJsonNetSerializerSettingsProvider</c></item>
        ///     <item><c>AddJsonNetPropertyBag</c></item>
        ///     <item><c>AddJsonNetCultureInfoConverter</c></item>
        ///     <item><c>AddJsonNetDateTimeOffsetConverter</c></item>
        /// </list>
        /// <para>
        /// It also adds the <see cref="StringEnumConverter"/>, specifying that enumerations are written as camel cased
        /// strings.
        /// </para>
        /// <para>
        /// The equivalent replacement code for this method is as follows:
        /// </para>
        /// <code>
        /// <![CDATA[
        /// services.AddJsonNetSerializerSettingsProvider();
        /// services.AddJsonNetPropertyBag();
        /// services.AddJsonNetCultureInfoConverter();
        /// services.AddJsonNetDateTimeOffsetConverter();
        /// services.AddSingleton<JsonConverter>(new StringEnumConverter(true));
        /// ]]>
        /// </code>
        /// </remarks>
        [Obsolete("This method is replaced by separate methods to register the different types of component in this library. See the remarks for the equivalent code you should use.")]
        public static IServiceCollection AddJsonSerializerSettings(this IServiceCollection services)
        {
            services.AddJsonNetSerializerSettingsProvider();
            services.AddJsonNetPropertyBag();
            services.AddJsonNetCultureInfoConverter();
            services.AddJsonNetDateTimeOffsetConverter();

            services.AddSingleton<JsonConverter>(new StringEnumConverter(true));
            return services;
        }

        /// <summary>
        /// Add the default JSON serialization settings provider.
        /// </summary>
        /// <param name="services">The target service collection.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddJsonNetSerializerSettingsProvider(this IServiceCollection services)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (!services.Any(s => typeof(IJsonSerializerSettingsProvider).IsAssignableFrom(s.ServiceType)))
            {
                services.AddSingleton<IJsonSerializerSettingsProvider, JsonSerializerSettingsProvider>();
            }

            return services;
        }

        /// <summary>
        /// Add the JSON.NET implementation of <see cref="IPropertyBagFactory"/> and <see cref="IPropertyBag"/>.
        /// </summary>
        /// <param name="services">The target service collection.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddJsonNetPropertyBag(this IServiceCollection services)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (!services.Any(s => typeof(IJsonNetPropertyBagFactory).IsAssignableFrom(s.ServiceType)))
            {
                services.AddSingleton<IJsonNetPropertyBagFactory, JsonNetPropertyBagFactory>();
                services.AddSingleton<IPropertyBagFactory>(sp => sp.GetRequiredService<IJsonNetPropertyBagFactory>());

                services.AddSingleton<JsonConverter, PropertyBagConverter>();
            }

            return services;
        }

        /// <summary>
        /// Add the <see cref="DateTimeOffsetConverter"/> to the service collection. This ensures that members of type
        /// <see cref="DateTimeOffset"/> are serialized as an object containing the date/time in ISO 8601 form, as well
        /// a unix time long.
        /// </summary>
        /// <param name="services">The target service collection.</param>
        /// <returns>The service collection.</returns>
        /// <remarks>
        /// <para>
        /// This converter is useful when the resultant serialized data is used with a store that supports querying
        /// and filtering. By default, ISO 8601 date/time strings include a timezone offset, but this means that
        /// it's not possible to use standard string comparison for sorting/filtering. Using the converter means
        /// that the Unix time in the resulting object can be used for sorting/filtering, while the ISO 8601 form
        /// is retained for deserialization so that timezone information is not lost.
        /// </para>
        /// <para>
        /// If you don't wish to use this converter, the alternative is to ensure all date/time values are stored as
        /// UTC. This means that standard string comparisons are viable, at the expense of not retaining the original
        /// time zone values.
        /// </para>
        /// </remarks>
        public static IServiceCollection AddJsonNetDateTimeOffsetConverter(this IServiceCollection services)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (!services.Any(s => s.ImplementationType == typeof(DateTimeOffsetConverter)))
            {
                services.AddSingleton<JsonConverter, DateTimeOffsetConverter>();
            }

            return services;
        }

        /// <summary>
        /// Add the <see cref="CultureInfoConverter"/> to the service collection. This ensures that members of type
        /// <see cref="CultureInfo"/> are serialized as their <see cref="CultureInfo.Name"/>.
        /// </summary>
        /// <param name="services">The target service collection.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddJsonNetCultureInfoConverter(this IServiceCollection services)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (!services.Any(s => s.ImplementationType == typeof(CultureInfoConverter)))
            {
                services.AddSingleton<JsonConverter, CultureInfoConverter>();
            }

            return services;
        }
    }
}