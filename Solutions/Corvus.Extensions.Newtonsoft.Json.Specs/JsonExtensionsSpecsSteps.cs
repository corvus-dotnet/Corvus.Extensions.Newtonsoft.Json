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
        private readonly IPropertyBagFactory propertyBagFactory;
        private readonly IJsonNetPropertyBagFactory jnetPropertyBagFactory;
        private Dictionary<string, object?> properties = new Dictionary<string, object?>();
        private IPropertyBag? propertyBag;

        public JsonExtensionsSpecsSteps(FeatureContext featureContext, ScenarioContext scenarioContext)
        {
            this.featureContext = featureContext;
            this.scenarioContext = scenarioContext;
            this.propertyBagFactory = ContainerBindings.GetServiceProvider(featureContext).GetRequiredService<IPropertyBagFactory>();
            this.jnetPropertyBagFactory = ContainerBindings.GetServiceProvider(featureContext).GetRequiredService<IJsonNetPropertyBagFactory>();
        }

        private IPropertyBag Bag => this.propertyBag ??= this.propertyBagFactory.Create(this.properties);

        [Given(@"I set a property called ""(.*)"" to the value ""(.*)""")]
        public void GivenISetAPropertyCalledToTheValue(string propertyName, string? value)
        {
            this.properties.Add(propertyName, value);
        }

        [Given(@"I set a property called ""(.*)"" to the value (.*)")]
        public void GivenISetAPropertyCalledToTheValue(string propertyName, int value)
        {
            this.properties.Add(propertyName, value);
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
            if (this.Bag.TryGet(propertyName, out string? value))
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
            if (this.Bag.TryGet(propertyName, out PocObject? value))
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
            this.propertyBag = JsonConvert.DeserializeObject<IPropertyBag>(json, settingsProvider.Instance);
        }

        [Given("I serialize the property bag")]
        public void GivenISerializeThePropertyBag()
        {
            IJsonSerializerSettingsProvider settingsProvider = ContainerBindings.GetServiceProvider(this.featureContext).GetService<IJsonSerializerSettingsProvider>();

            this.scenarioContext.Set(JsonConvert.SerializeObject(this.Bag, settingsProvider.Instance), "Result");
        }

        [Given(@"I serialize a POCO with ""(.*)"", ""(.*)"", ""(.*)"", ""(.*)"", ""(.*)""")]
        public void GivenISerializeAPOCOWith(string value, string time, string nullableTime, string? culture, ExampleEnum someEnum)
        {
            var poco = new PocObject(value)
            {
                SomeCulture = string.IsNullOrEmpty(culture) ? null : CultureInfo.GetCultureInfo(culture),
                SomeDateTime = DateTimeOffset.Parse(time),
                SomeNullableDateTime = string.IsNullOrEmpty(nullableTime) ? null : (DateTimeOffset?)DateTimeOffset.Parse(nullableTime),
                SomeEnum = someEnum,
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
                SomeEnum = someEnum,
            };

            Assert.AreEqual(expected, poc);
        }

        [Then(@"the result should be ""(.*)""")]
        public void ThenTheResultShouldBe(string expected)
        {
            Assert.True(this.scenarioContext.ContainsKey("Result"));
            Assert.AreEqual(expected, this.scenarioContext.Get<string>("Result"));
        }

        [When("I cast to a JObject")]
        public void WhenICastToAJObject()
        {
            this.scenarioContext.Set(this.jnetPropertyBagFactory.AsJObject(this.Bag), "Result");
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
            foreach (TableRow row in table.Rows)
            {
                row.TryGetValue("Property", out string name);
                row.TryGetValue("Value", out string expected);
                row.TryGetValue("Type", out string type);
                switch (type)
                {
                    case "string":
                        {
                            Assert.IsTrue(this.Bag.TryGet(name, out string? actual));
                            Assert.AreEqual(expected, actual);
                            break;
                        }

                    case "integer":
                        {
                            Assert.IsTrue(this.Bag.TryGet(name, out int actual));
                            Assert.AreEqual(int.Parse(expected), actual);
                            break;
                        }

                    default:
                        throw new InvalidOperationException($"Unknown data type '{type}'");
                }
            }
        }

        [Then("the dictionary should contain the properties")]
        public void ThenTheDictionaryShouldContainTheProperties(Table table)
        {
            IReadOnlyDictionary<string, object?> dictionary = this.scenarioContext.Get<IReadOnlyDictionary<string, object?>>("Result");

            foreach (TableRow row in table.Rows)
            {
                row.TryGetValue("Property", out string name);
                row.TryGetValue("Value", out string expected);
                row.TryGetValue("Type", out string type);
                switch (type)
                {
                    case "string":
                        {
                            Assert.IsTrue(dictionary.TryGetValue(name, out object? actual));
                            Assert.AreEqual(expected, actual);
                            break;
                        }

                    case "integer":
                        {
                            Assert.IsTrue(dictionary.TryGetValue(name, out object? actual));
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
            this.propertyBag = this.jnetPropertyBagFactory.Create(this.scenarioContext.Get<JObject>());
        }

        [Given("I create a PropertyBag")]
        public void GivenICreateAPropertyBag(Table table)
        {
            this.propertyBag = this.CreatePropertyBagFromTable(table);
        }

        [Given("I create a Dictionary")]
        public void GivenICreateADictionary(Table table)
        {
            this.properties = CreateDictionaryFromTable(table);
        }

        [When("I create a PropertyBag from the Dictionary")]
        public void WhenIConstructAPropertyBagFromTheDictionary()
        {
            this.propertyBag = this.propertyBagFactory.Create(this.properties);
        }

        [Given("I setup default json serializer settings")]
        public void GivenISetupDefaultJsonSerializerSettings()
        {
            this.scenarioContext.Set(new JsonSerializerSettings(), "JsonSerializerSettings");
            JsonConvert.DefaultSettings = () => this.scenarioContext.Get<JsonSerializerSettings>("JsonSerializerSettings");
        }

        [When("I convert the PropertyBag to a Dictionary")]
        public void WhenIConvertThePropertyBagToADictionary()
        {
            this.scenarioContext.Set(this.Bag.AsDictionary(), "Result");
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

        private static Dictionary<string, object?> CreateDictionaryFromTable(Table table)
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

        private IPropertyBag CreatePropertyBagFromTable(Table table)
        {
            return this.propertyBagFactory.Create(CreateDictionaryFromTable(table));
        }
    }
}

#pragma warning restore SA1600 // Elements should be documented
#pragma warning restore CS1591 // Elements should be documented