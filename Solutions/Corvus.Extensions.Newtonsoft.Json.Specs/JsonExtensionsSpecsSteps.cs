// <copyright file="JsonExtensionsSpecsSteps.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

#pragma warning disable SA1600 // Elements should be documented
#pragma warning disable CS1591 // Elements should be documented

namespace Corvus.Extensions.Json.Specs
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using Corvus.Extensions.Json.Specs.Samples;
    using Corvus.SpecFlow.Extensions;
    using Microsoft.Extensions.DependencyInjection;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using NUnit.Framework;
    using TechTalk.SpecFlow;

    [Binding]
    public class JsonExtensionsSpecsSteps
    {
        private readonly FeatureContext featureContext;
        private readonly ScenarioContext scenarioContext;

        public JsonExtensionsSpecsSteps(FeatureContext featureContext, ScenarioContext scenarioContext)
        {
            this.featureContext = featureContext;
            this.scenarioContext = scenarioContext;
            this.scenarioContext.Set(new PropertyBag(ContainerBindings.GetServiceProvider(featureContext).GetRequiredService<IJsonSerializerSettingsProvider>().Instance));
        }

        [Given(@"I set a property called ""(.*)"" to the value ""(.*)""")]
        public void GivenISetAPropertyCalledToTheValue(string propertyName, string value)
        {
            PropertyBag bag = this.scenarioContext.Get<PropertyBag>();
            bag.Set(propertyName, value);
        }

        [Given(@"I set a property called ""(.*)"" to null")]
        public void GivenISetAPropertyCalledToNull(string propertyName)
        {
            this.GivenISetAPropertyCalledToTheValue(propertyName, null);
        }

        [Then("the result should be null")]
        public void ThenTheResultShouldBeNull()
        {
            Assert.True(this.scenarioContext.ContainsKey("Result"));
            Assert.AreEqual("(null)", this.scenarioContext.Get<string>("Result"));
        }

        [When(@"I get the property called ""(.*)""")]
        public void WhenIGetThePropertyCalled(string propertyName)
        {
            if (this.scenarioContext.Get<PropertyBag>().TryGet(propertyName, out string? value))
            {
                this.scenarioContext.Set(value ?? "(null)", "Result");
            }
            else
            {
                this.scenarioContext.Set("(null)", "Result");
            }
        }

        [When(@"I get the property called ""(.*)"" as a custom object")]
        public void WhenIGetThePropertyCalledAsACustomObject(string propertyName)
        {
            if (this.scenarioContext.Get<PropertyBag>().TryGet(propertyName, out PocObject? value))
            {
                this.scenarioContext.Set(value, "Result");
            }
            else
            {
                this.scenarioContext.Set("(null)", "Result");
            }
        }

        [Given(@"I deserialize a property bag from the string ""(.*)""")]
        public void GivenIDeserializeAPropertyBagFromTheStringHelloWorldNumber(string json)
        {
            IJsonSerializerSettingsProvider settingsProvider = ContainerBindings.GetServiceProvider(this.featureContext).GetService<IJsonSerializerSettingsProvider>();
            this.scenarioContext.Set(JsonConvert.DeserializeObject<PropertyBag>(json, settingsProvider.Instance), "Result");
        }

        [Given("I serialize the property bag")]
        public void GivenISerializeThePropertyBag()
        {
            IJsonSerializerSettingsProvider settingsProvider = ContainerBindings.GetServiceProvider(this.featureContext).GetService<IJsonSerializerSettingsProvider>();

            PropertyBag propertyBag = this.scenarioContext.Get<PropertyBag>();

            this.scenarioContext.Set(JsonConvert.SerializeObject(propertyBag, settingsProvider.Instance), "Result");
        }

        [Given(@"I serialize a POCO with ""(.*)"", ""(.*)"", ""(.*)"", ""(.*)"", ""(.*)""")]
        public void GivenISerializeAPOCOWith(string value, string time, string nullableTime, string? culture, ExampleEnum someEnum)
        {
            var poco = new PocObject(value) 
            { 
                SomeCulture = string.IsNullOrEmpty(culture) ? null : CultureInfo.GetCultureInfo(culture), 
                SomeDateTime = DateTimeOffset.Parse(time), 
                SomeNullableDateTime = string.IsNullOrEmpty(nullableTime) ? null : (DateTimeOffset?)DateTimeOffset.Parse(nullableTime), 
                SomeEnum = someEnum 
            };
            
            IJsonSerializerSettingsProvider settingsProvider = ContainerBindings.GetServiceProvider(this.featureContext).GetService<IJsonSerializerSettingsProvider>();
            this.scenarioContext.Set(JsonConvert.SerializeObject(poco, settingsProvider.Instance), "Result");
        }

        [Given(@"I deserialize a POCO with the json string ""(.*)""")]
        public void GivenIDeserializeAPOCOWithTheJsonString(string json)
        {
            IJsonSerializerSettingsProvider settingsProvider = ContainerBindings.GetServiceProvider(this.featureContext).GetService<IJsonSerializerSettingsProvider>();
            this.scenarioContext.Set(JsonConvert.DeserializeObject<PocObject>(json, settingsProvider.Instance), "Result");
        }

        [Then(@"the result should have values ""(.*)"", ""(.*)"", ""(.*)"", ""(.*)"", ""(.*)""")]
        public void ThenTheResultShouldHaveValues(string value, string time, string nullableTime, string culture, ExampleEnum someEnum)
        {
            PocObject poc = this.scenarioContext.Get<PocObject>("Result");
            var expected = new PocObject(value) 
            { 
                SomeCulture = string.IsNullOrEmpty(culture) ? null : CultureInfo.GetCultureInfo(culture), 
                SomeDateTime = DateTimeOffset.Parse(time), 
                SomeNullableDateTime = string.IsNullOrEmpty(nullableTime) ? null : (DateTimeOffset?)DateTimeOffset.Parse(nullableTime), 
                SomeEnum = someEnum 
            };

            Assert.AreEqual(expected, poc);
        }

        [Then(@"the result should be ""(.*)""")]
        public void ThenTheResultShouldBe(string expected)
        {
            Assert.True(this.scenarioContext.ContainsKey("Result"));
            Assert.AreEqual(expected, this.scenarioContext.Get<string>("Result"));
        }

        [Given(@"I set a property called ""(.*)"" to the value (.*)")]
        public void GivenISetAPropertyCalledToTheValue(string propertyName, int value)
        {
            PropertyBag bag = this.scenarioContext.Get<PropertyBag>();
            bag.Set(propertyName, value);
        }

        [When("I cast to a JObject")]
        public void WhenICastToAJObject()
        {
            this.scenarioContext.Set<JObject>(this.scenarioContext.Get<PropertyBag>(), "Result");
        }

        [Then("the result should be the JObject")]
        public void ThenTheResultShouldBeTheJObject(Table table)
        {
            JObject expected = CreateJObjectFromTable(table);

            JObject actual = this.scenarioContext.Get<JObject>("Result");

            Assert.IsTrue(JToken.DeepEquals(expected, actual), $"Expected: {expected} but actually saw {actual}");
        }

        [Given("I create a JObject")]
        public void GivenICreateAJObject(Table table)
        {
            this.scenarioContext.Set(CreateJObjectFromTable(table));
        }

        [Then("the result should have the properties")]
        public void ThenTheResultShouldHaveTheProperties(Table table)
        {
            PropertyBag bag = this.scenarioContext.Get<PropertyBag>("Result");

            foreach (TableRow row in table.Rows)
            {
                row.TryGetValue("Property", out string name);
                row.TryGetValue("Value", out string expected);
                row.TryGetValue("Type", out string type);
                switch (type)
                {
                    case "string":
                        {
                            Assert.IsTrue(bag.TryGet(name, out string? actual));
                            Assert.AreEqual(expected, actual);
                            break;
                        }

                    case "integer":
                        {
                            Assert.IsTrue(bag.TryGet(name, out int actual));
                            Assert.AreEqual(int.Parse(expected), actual);
                            break;
                        }

                    default:
                        throw new InvalidOperationException($"Unknown data type '{type}'");
                }
            }
        }

        [When("I construct a PropertyBag from the JObject")]
        public void WhenIConstructAPropertyBagFromTheJObject()
        {
            this.scenarioContext.Set(new PropertyBag(this.scenarioContext.Get<JObject>(), ContainerBindings.GetServiceProvider(this.featureContext).GetRequiredService<IJsonSerializerSettingsProvider>().Instance), "Result");
        }

        [When("I construct a PropertyBag with no serializer settings")]
        public void WhenIConstructAPropertyBagWithNoSerializerSettings()
        {
            this.scenarioContext.Set(new PropertyBag(), "Result");
        }

        [Given("I reset default json serializer settings")]
        public void GivenIResetDefaultJsonSerializerSettings()
        {
            JsonConvert.DefaultSettings = null;
        }

        [Given("I create a Dictionary")]
        public void GivenICreateADictionary(Table table)
        {
            this.scenarioContext.Set(CreateDictionaryFromTable(table));
        }

        [When("I construct a PropertyBag from the Dictionary")]
        public void WhenIConstructAPropertyBagFromTheDictionary()
        {
            this.scenarioContext.Set(new PropertyBag(this.scenarioContext.Get<IDictionary<string, object>>(), ContainerBindings.GetServiceProvider(this.featureContext).GetRequiredService<IJsonSerializerSettingsProvider>().Instance), "Result");
        }

        [Given("I setup default json serializer settings")]
        public void GivenISetupDefaultJsonSerializerSettings()
        {
            this.scenarioContext.Set(new JsonSerializerSettings(), "JsonSerializerSettings");
            JsonConvert.DefaultSettings = () => this.scenarioContext.Get<JsonSerializerSettings>("JsonSerializerSettings");
        }

        [When("I construct a PropertyBag from the JObject with no serializer settings")]
        public void WhenIConstructAPropertyBagFromTheJObjectWithNoSerializerSettings()
        {
            this.scenarioContext.Set(new PropertyBag(this.scenarioContext.Get<JObject>()), "Result");
        }

        [When("I construct a PropertyBag from the Dictionary with no serializer settings")]
        public void WhenIConstructAPropertyBagFromTheDictionaryWithNoSerializerSettings()
        {
            this.scenarioContext.Set(new PropertyBag(this.scenarioContext.Get<IDictionary<string, object>>()), "Result");
        }

        [Then("the result should have the default serializer settings")]
        public void ThenTheResultShouldHaveTheDefaultSerializerSettings()
        {
            PropertyBag propertyBag = this.scenarioContext.Get<PropertyBag>("Result");
            Assert.AreEqual(JsonConvert.DefaultSettings?.Invoke() ?? PropertyBag.DefaultJsonSerializerSettings, propertyBag.SerializerSettings);
        }

        private static JObject CreateJObjectFromTable(Table table)
        {
            var expected = new JObject();
            foreach (TableRow row in table.Rows)
            {
                row.TryGetValue("Property", out string name);
                row.TryGetValue("Value", out string value);
                row.TryGetValue("Type", out string type);
                expected[name] = type switch
                {
                    "string"  => value == "(null)" ? null : value,
                    "integer" => int.Parse(value),
                    _ => throw new InvalidOperationException($"Unknown data type '{type}'"),
                };
            }

            return expected;
        }

        private static IDictionary<string, object?> CreateDictionaryFromTable(Table table)
        {
            var expected = new Dictionary<string, object?>();
            foreach (TableRow row in table.Rows)
            {
                row.TryGetValue("Property", out string name);
                row.TryGetValue("Value", out string value);
                row.TryGetValue("Type", out string type);
                expected[name] = type switch
                {
                    "string"  => value == "(null)" ? null : value,
                    "integer" => int.Parse(value),
                    _ => throw new InvalidOperationException($"Unknown data type '{type}'"),
                };
            }

            return expected;
        }
    }
}

#pragma warning restore SA1600 // Elements should be documented
#pragma warning restore CS1591 // Elements should be documented