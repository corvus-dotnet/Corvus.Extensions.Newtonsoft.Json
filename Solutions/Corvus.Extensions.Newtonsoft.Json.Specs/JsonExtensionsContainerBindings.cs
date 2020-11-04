// <copyright file="JsonExtensionsContainerBindings.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Corvus.Extensions.Json.Specs
{
    using Corvus.SpecFlow.Extensions;
    using Microsoft.Extensions.DependencyInjection;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
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
        [BeforeFeature("@setupContainer", Order = ContainerBeforeFeatureOrder.PopulateServiceCollection)]
        public static void SetupFeature(FeatureContext featureContext)
        {
            ContainerBindings.ConfigureServices(
                featureContext,
                serviceCollection =>
                {
                    serviceCollection.AddJsonNetSerializerSettingsProvider();
                    serviceCollection.AddJsonNetPropertyBag();
                    serviceCollection.AddJsonNetCultureInfoConverter();
                    serviceCollection.AddJsonNetDateTimeOffsetConverter();
                    serviceCollection.AddSingleton<JsonConverter>(new StringEnumConverter(true));
                });
        }
    }
}
