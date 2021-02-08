// <copyright file="JsonExtensionsContainerBindings.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Corvus.Extensions.Json.Specs
{
    using Corvus.SpecFlow.Extensions;
    using Microsoft.Extensions.DependencyInjection;
    using TechTalk.SpecFlow;

    /// <summary>
    /// Provides Specflow bindings for Endjin Composition.
    /// </summary>
    [Binding]
    public static class JsonExtensionsContainerBindings
    {
        /// <summary>
        /// Setup the endjin container for a feature.
        /// </summary>
        /// <remarks>We expect features run in parallel to be executing in separate app domains.</remarks>
        /// <param name="featureContext">The SpecFlow test context.</param>
        [BeforeFeature("@setupContainerForJsonNetPropertyBag", Order = ContainerBeforeFeatureOrder.PopulateServiceCollection)]
        public static void SetupFeatureForJsonNetPropertyBag(FeatureContext featureContext)
        {
            ContainerBindings.ConfigureServices(
                featureContext,
                serviceCollection =>
                {
                    serviceCollection.AddJsonNetSerializerSettingsProvider();
                    serviceCollection.AddJsonNetPropertyBag();
                });
        }

        /// <summary>
        /// Setup the endjin container for a feature.
        /// </summary>
        /// <remarks>We expect features run in parallel to be executing in separate app domains.</remarks>
        /// <param name="featureContext">The SpecFlow test context.</param>
        [BeforeFeature("@setupContainerForJsonNetCultureInfoConversion", Order = ContainerBeforeFeatureOrder.PopulateServiceCollection)]
        public static void SetupFeatureForJsonNetCultureInfoConversion(FeatureContext featureContext)
        {
            ContainerBindings.ConfigureServices(
                featureContext,
                serviceCollection =>
                {
                    serviceCollection.AddJsonNetSerializerSettingsProvider();
                    serviceCollection.AddJsonNetCultureInfoConverter();
                });
        }

        /// <summary>
        /// Setup the endjin container for a feature.
        /// </summary>
        /// <remarks>We expect features run in parallel to be executing in separate app domains.</remarks>
        /// <param name="featureContext">The SpecFlow test context.</param>
        [BeforeFeature("@setupContainerForJsonNetDateTimeOffsetConversion", Order = ContainerBeforeFeatureOrder.PopulateServiceCollection)]
        public static void SetupFeatureForJsonNetDateTimeOffsetConversion(FeatureContext featureContext)
        {
            ContainerBindings.ConfigureServices(
                featureContext,
                serviceCollection =>
                {
                    serviceCollection.AddJsonNetSerializerSettingsProvider();
                    serviceCollection.AddJsonNetDateTimeOffsetToIso8601AndUnixTimeConverter();
                });
        }
    }
}
