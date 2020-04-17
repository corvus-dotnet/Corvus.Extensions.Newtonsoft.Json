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
    using Corvus.Json;
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
        private Dictionary<string, object> creationProperties = new Dictionary<string, object>();
        private IPropertyBag? propertyBag;
        private SerializationException? exception;

        public JsonExtensionsSpecsSteps(FeatureContext featureContext, ScenarioContext scenarioContext)
        {
            this.featureContext = featureContext;
            this.scenarioContext = scenarioContext;
            this.propertyBagFactory = ContainerBindings.GetServiceProvider(featureContext).GetRequiredService<IPropertyBagFactory>();
            this.jnetPropertyBagFactory = ContainerBindings.GetServiceProvider(featureContext).GetRequiredService<IJsonNetPropertyBagFactory>();
        }

        private IPropertyBag Bag => this.propertyBag ?? throw new InvalidOperationException("The test is trying to use property bag before it has been created");

        [Given(@"the creation properties include ""(.*)"" with the value ""(.*)""")]
        public void TheCreationPropertiesInclude(string propertyName, string value)
        {
            this.creationProperties.Add(propertyName, value);
        }

        [Given(@"the creation properties include ""(.*)"" with the value (.*)")]
        public void TheCreationPropertiesInclude(string propertyName, int value)
        {
            this.creationProperties.Add(propertyName, value);
        }

        [Given(@"the creation properties include ""(.*)"" with the date value ""(.*)""")]
        public void TheCreationPropertiesInclude(string propertyName, DateTime value)
        {
            this.creationProperties.Add(propertyName, value);
        }

        [Given(@"the creation properties include a POCO called ""(.*)"" with ""(.*)"" ""(.*)"" ""(.*)"" ""(.*)"" ""(.*)""")]
        public void TheCreationPropertiesIncludeAPOCOWith(string name, string value, string time, string nullableTime, string? culture, ExampleEnum someEnum)
        {
            var poco = new PocObject(value)
            {
                SomeCulture = string.IsNullOrEmpty(culture) ? null : CultureInfo.GetCultureInfo(culture),
                SomeDateTime = DateTimeOffset.Parse(time),
                SomeNullableDateTime = string.IsNullOrEmpty(nullableTime) ? null : (DateTimeOffset?)DateTimeOffset.Parse(nullableTime),
                SomeEnum = someEnum,
            };

            this.creationProperties.Add(name, poco);
        }

        [Given("I create the property bag from the creation properties")]
        [When("I create the property bag from the creation properties")]
        public void WhenICreateThePropertyBagFromTheCreationProperties()
        {
            this.propertyBag = this.propertyBagFactory.Create(this.creationProperties);
        }

        [Then(@"the property bag should contain a property called ""(.*)"" with the value ""(.*)""")]
        public void ThenThePropertyBagShouldContainAPropertyCalledWithTheValue(string key, string expectedValue)
        {
            Assert.True(this.Bag.TryGet(key, out string value), "TryGet return value");
            Assert.AreEqual(expectedValue, value);
        }

        [Then(@"calling TryGet with ""(.*)"" should return false and the result should be null")]
        public void ThenCallingTryGetWithShouldReturnFalseAndTheResultShouldBeNull(string key)
        {
            Assert.False(this.Bag.TryGet(key, out string? value), "TryGet return value");
            Assert.IsNull(value);
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

        [When(@"I get the property called ""(.*)"" as a custom object expecting an exception")]
        public void WhenIGetThePropertyCalledAsACustomObjectExpectingAnException(string propertyName)
        {
            try
            {
                this.Bag.TryGet(propertyName, out PocObject? _);
            }
            catch (SerializationException x)
            {
                this.exception = x;
            }
        }

        [Given(@"I deserialize a property bag from the string ""(.*)""")]
        public void GivenIDeserializeAPropertyBagFromTheString(string json)
        {
            IJsonSerializerSettingsProvider settingsProvider = ContainerBindings.GetServiceProvider(this.featureContext).GetService<IJsonSerializerSettingsProvider>();
            this.propertyBag = JsonConvert.DeserializeObject<IPropertyBag>(json, settingsProvider.Instance);
        }

        [When("I serialize the property bag")]
        public void GivenISerializeThePropertyBag()
        {
            IJsonSerializerSettingsProvider settingsProvider = ContainerBindings.GetServiceProvider(this.featureContext).GetService<IJsonSerializerSettingsProvider>();

            this.scenarioContext.Set(JsonConvert.SerializeObject(this.Bag, settingsProvider.Instance), "Result");
        }

        [When("I deserialize the serialized property bag")]
        public void WhenIDeserializeTheSerializedPropertyBag()
        {
            string serializedBag = this.scenarioContext.Get<string>("Result");
            IJsonSerializerSettingsProvider settingsProvider = ContainerBindings.GetServiceProvider(this.featureContext).GetService<IJsonSerializerSettingsProvider>();
            this.propertyBag = JsonConvert.DeserializeObject<IPropertyBag>(serializedBag, settingsProvider.Instance);
        }

        [Given(@"I deserialize a POCO with the json string ""(.*)""")]
        public void GivenIDeserializeAPOCOWithTheJsonString(string json)
        {
            IJsonSerializerSettingsProvider settingsProvider = ContainerBindings.GetServiceProvider(this.featureContext).GetService<IJsonSerializerSettingsProvider>();
            this.scenarioContext.Set(JsonConvert.DeserializeObject<PocObject>(json, settingsProvider.Instance), "Result");
        }

        [Then(@"the result should have a POCO named ""(.*)"" with values ""(.*)"" ""(.*)"" ""(.*)"" ""(.*)"" ""(.*)""")]
        public void ThenTheResultShouldHaveValues(string name, string value, string time, string nullableTime, string culture, ExampleEnum someEnum)
        {
            Assert.IsTrue(this.Bag.TryGet(name, out PocObject poc), "TryGet return value");
            CheckPocosAreEqual(value, time, nullableTime, culture, someEnum, poc);
        }

        [Then(@"the result should have values ""(.*)"", ""(.*)"", ""(.*)"", ""(.*)"", ""(.*)""")]
        public void ThenTheResultShouldHaveValues(string value, string time, string nullableTime, string culture, ExampleEnum someEnum)
        {
            PocObject poc = this.scenarioContext.Get<PocObject>("Result");
            CheckPocosAreEqual(value, time, nullableTime, culture, someEnum, poc);
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

        [When("I add, modify, or remove properties")]
        public void WhenIAddModifyOrRemoveProperties(Table table)
        {
            var propertiesToRemove = new List<string>();
            var propertiesToSetOrAdd = new Dictionary<string, object>();
            foreach (TableRow row in table.Rows)
            {
                string propertyName = row["Property"];
                switch (row["Action"])
                {
                    case "remove":
                        propertiesToRemove.Add(propertyName);
                        break;

                    case "addOrSet":
                        string value = row["Value"];
                        string type = row["Type"];
                        object actualValue = type switch
                        {
                            "string" => value,
                            "integer" => int.Parse(value),
                            _ => throw new InvalidOperationException($"Unknown data type '{type}'")
                        };
                        propertiesToSetOrAdd.Add(propertyName, actualValue);
                        break;

                    default:
                        Assert.Fail("Unknown action in add/modify/remove table: " + row["Action"]);
                        break;
                }
            }

            this.propertyBag = this.propertyBagFactory.CreateModified(
                this.Bag,
                propertiesToSetOrAdd.Count == 0 ? null : propertiesToSetOrAdd,
                propertiesToRemove.Count == 0 ? null : propertiesToRemove);
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

                    case "datetime":
                        {
                            Assert.IsTrue(this.Bag.TryGet(name, out DateTimeOffset actual));
                            Assert.AreEqual(DateTimeOffset.Parse(expected), actual);
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
            IReadOnlyDictionary<string, object> dictionary = this.scenarioContext.Get<IReadOnlyDictionary<string, object>>("Result");

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
            this.creationProperties = CreateDictionaryFromTable(table);
        }

        [Given("I create a PropertyBag from the Dictionary")]
        [When("I create a PropertyBag from the Dictionary")]
        public void WhenIConstructAPropertyBagFromTheDictionary()
        {
            this.propertyBag = this.propertyBagFactory.Create(this.creationProperties);
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

        [Then("TryGet should have thrown a SerializationException")]
        public void ThenTryGetShouldHaveThrownASerializationException()
        {
            Assert.IsInstanceOf<SerializationException>(this.exception);
        }

        private static JObject CreateJObjectFromTable(Table table)
        {
            var expected = new JObject();
            foreach (TableRow row in table.Rows)
            {
                row.TryGetValue("Property", out string name);
                expected[name] = GetRowValue(row);
            }

            return expected;
        }

        private static Dictionary<string, object> CreateDictionaryFromTable(Table table)
        {
            var expected = new Dictionary<string, object>();
            foreach (TableRow row in table.Rows)
            {
                row.TryGetValue("Property", out string name);
                expected[name] = GetRowValue(row);
            }

            return expected;
        }

        private static JToken GetRowValue(TableRow row)
        {
            row.TryGetValue("Value", out string value);
            row.TryGetValue("Type", out string type);
            return type switch
            {
                "string" => value,
                "integer" => int.Parse(value),
                "datetime" => DateTimeOffset.Parse(value),
                _ => throw new InvalidOperationException($"Unknown data type '{type}'"),
            };
        }

        private IPropertyBag CreatePropertyBagFromTable(Table table)
        {
            return this.propertyBagFactory.Create(CreateDictionaryFromTable(table));
        }

        private static void CheckPocosAreEqual(string value, string time, string nullableTime, string culture, ExampleEnum someEnum, PocObject poc)
        {
            var expected = new PocObject(value)
            {
                SomeCulture = string.IsNullOrEmpty(culture) ? null : CultureInfo.GetCultureInfo(culture),
                SomeDateTime = DateTimeOffset.Parse(time),
                SomeNullableDateTime = string.IsNullOrEmpty(nullableTime) ? null : (DateTimeOffset?)DateTimeOffset.Parse(nullableTime),
                SomeEnum = someEnum,
            };

            Assert.AreEqual(expected, poc);
        }
    }
}

#pragma warning restore SA1600 // Elements should be documented
#pragma warning restore CS1591 // Elements should be documented