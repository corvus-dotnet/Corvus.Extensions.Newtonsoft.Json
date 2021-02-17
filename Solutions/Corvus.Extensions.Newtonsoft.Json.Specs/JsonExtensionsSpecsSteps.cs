// <copyright file="JsonExtensionsSpecsSteps.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Corvus.Extensions.Json.Specs
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using Corvus.Extensions.Json.Specs.Samples;
    using Corvus.Json;
    using Corvus.Testing.SpecFlow;
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
        private readonly IJsonSerializerSettingsProvider jsonSerializerSettingsProvider;
        private Dictionary<string, object> creationProperties = new Dictionary<string, object>();
        private IPropertyBag? propertyBag;
        private SerializationException? exception;

        public JsonExtensionsSpecsSteps(FeatureContext featureContext, ScenarioContext scenarioContext)
        {
            this.featureContext = featureContext;
            this.scenarioContext = scenarioContext;
            this.propertyBagFactory = ContainerBindings.GetServiceProvider(featureContext).GetService<IPropertyBagFactory>();
            this.jnetPropertyBagFactory = ContainerBindings.GetServiceProvider(featureContext).GetService<IJsonNetPropertyBagFactory>();
            this.jsonSerializerSettingsProvider = ContainerBindings.GetServiceProvider(featureContext).GetRequiredService<IJsonSerializerSettingsProvider>();
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
        public void TheCreationPropertiesInclude(string propertyName, DateTimeOffset value)
        {
            this.creationProperties.Add(propertyName, value);
        }

        [Given(@"the creation properties include a POCO called ""(.*)"" with ""(.*)"" ""(.*)"" ""(.*)"" ""(.*)"" ""(.*)""")]
        public void TheCreationPropertiesIncludeAPOCOWith(string name, string value, string time, string nullableTime, string? culture, ExampleEnum someEnum)
        {
        }

        [Given(@"the creation properties include a DateTime POCO called ""(.*)"" with ""(.*)"" ""(.*)""")]
        public void GivenTheCreationPropertiesIncludeADateTimePOCOCalledWith(string name, string time, string nullableTime)
        {
            DateTimeOffsetPocObject poco = MakeDateTimeOffsetPoco(time, nullableTime);

            this.creationProperties.Add(name, poco);
        }

        [Given(@"the creation properties include a CultureInfo POCO called ""(.*)"" with ""(.*)""")]
        public void GivenTheCreationPropertiesIncludeACultureInfoPOCOCalledWith(string name, string culture)
        {
            if (!string.IsNullOrEmpty(culture))
            {
            CultureInfoPocObject poco = MakeCultureInfoPoco(culture);

            this.creationProperties.Add(name, poco);
            }
        }

        [Given(@"the creation properties include an enum value called ""(.*)"" with value ""(.*)""")]
        public void GivenTheCreationPropertiesIncludeAnEnumValueCalledWithValue(string name, ExampleEnum value)
        {
            this.creationProperties.Add(name, value);
        }

        [Given(@"I serialize a CultureInfo POCO with ""(.*)""")]
        public void GivenISerializeACultureInfoPOCOWith(string? culture)
        {
            CultureInfoPocObject poco = MakeCultureInfoPoco(culture);

            IJsonSerializerSettingsProvider settingsProvider = ContainerBindings.GetServiceProvider(this.featureContext).GetService<IJsonSerializerSettingsProvider>();
            this.scenarioContext.Set(JsonConvert.SerializeObject(poco, settingsProvider.Instance), "Result");
        }

        [Given(@"I serialize a DateTimeOffset POCO with ""(.*)"", ""(.*)""")]
        public void GivenISerializeADateTimeOffsetPOCOWith(string time, string nullableTime)
        {
            DateTimeOffsetPocObject poco = MakeDateTimeOffsetPoco(time, nullableTime);

            IJsonSerializerSettingsProvider settingsProvider = ContainerBindings.GetServiceProvider(this.featureContext).GetService<IJsonSerializerSettingsProvider>();
            this.scenarioContext.Set(JsonConvert.SerializeObject(poco, settingsProvider.Instance), "Result");
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
            if (this.Bag.TryGet(propertyName, out CultureInfoPocObject? value))
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
                this.Bag.TryGet(propertyName, out CultureInfoPocObject? _);
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

        [Given("I deserialize a property bag from the string")]
        public void GivenIDeserializeAPropertyBagFromTheMultilineString(string multiLineJson)
        {
            IJsonSerializerSettingsProvider settingsProvider = ContainerBindings.GetServiceProvider(this.featureContext).GetService<IJsonSerializerSettingsProvider>();
            this.propertyBag = JsonConvert.DeserializeObject<IPropertyBag>(multiLineJson, settingsProvider.Instance);
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

        [Given(@"I deserialize a CultureInfo POCO with the json string ""(.*)""")]
        public void GivenIDeserializeACultureInfoPOCOWithTheJsonString(string json)
        {
            IJsonSerializerSettingsProvider settingsProvider = ContainerBindings.GetServiceProvider(this.featureContext).GetService<IJsonSerializerSettingsProvider>();
            this.scenarioContext.Set(JsonConvert.DeserializeObject<CultureInfoPocObject>(json, settingsProvider.Instance), "Result");
        }

        [Given(@"I deserialize a DateTimeOffset POCO with the json string ""(.*)""")]
        public void GivenIDeserializeADateTimeOffsetPOCOWithTheJsonString(string json)
        {
            IJsonSerializerSettingsProvider settingsProvider = ContainerBindings.GetServiceProvider(this.featureContext).GetService<IJsonSerializerSettingsProvider>();
            this.scenarioContext.Set(JsonConvert.DeserializeObject<DateTimeOffsetPocObject>(json, settingsProvider.Instance), "Result");
        }

        [Then(@"the result should have a DateTime POCO named ""(.*)"" with values ""(.*)"" ""(.*)""")]
        public void ThenTheResultShouldHaveADateTimePOCONamedWithValues(string name, string time, string nullableTime)
        {
            Assert.IsTrue(this.Bag.TryGet(name, out DateTimeOffsetPocObject poc), "TryGet return value");
            CheckPocosAreEqual(time, nullableTime, poc);
        }

        [Then(@"the result should have a CultureInfo POCO named ""(.*)"" with value ""(.*)""")]
        public void ThenTheResultShouldHaveACultureInfoPocoNamed(string name, string culture)
        {
            if (!string.IsNullOrEmpty(culture))
            {
                Assert.IsTrue(this.Bag.TryGet(name, out CultureInfoPocObject poc), "TryGet return value");
                CheckPocosAreEqual(culture, poc);
            }
            else
            {
                Assert.IsFalse(this.Bag.TryGet(name, out CultureInfoPocObject _), "TryGet return value");
            }
        }

        [Then(@"the result should have an enum value named ""(.*)"" with value ""(.*)""")]
        public void ThenTheResultShouldHaveAnEnumValueNamedWithValue(string name, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                Assert.IsTrue(this.Bag.TryGet(name, out ExampleEnum actual), "TryGet return value");
                Assert.AreEqual(value, actual.ToString());
            }
            else
            {
                Assert.IsFalse(this.Bag.TryGet(name, out ExampleEnum _), "TryGet return value");
            }
        }

        [Then(@"the result should have CultureInfo values ""(.*)""")]
        public void ThenTheResultShouldHaveCultureInfoValues(string culture)
        {
            CultureInfoPocObject poc = this.scenarioContext.Get<CultureInfoPocObject>("Result");
            CheckPocosAreEqual(culture, poc);
        }

        [Then(@"the result should have DateTimeOffset values ""(.*)"", ""(.*)""")]
        public void ThenTheResultShouldHaveDateTimeOffsetValues(string time, string nullableTime)
        {
            DateTimeOffsetPocObject poc = this.scenarioContext.Get<DateTimeOffsetPocObject>("Result");
            CheckPocosAreEqual(time, nullableTime, poc);
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
            JObject expected = this.CreateJObjectFromTable(table);

            JObject actual = this.scenarioContext.Get<JObject>("Result");

            Assert.IsTrue(JToken.DeepEquals(expected, actual), $"Expected: {expected} but actually saw {actual}");
        }

        [Given("I create a JObject")]
        public void GivenICreateAJObject(Table table)
        {
            this.scenarioContext.Set(this.CreateJObjectFromTable(table));
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

                    case "IPropertyBag":
                        {
                            Assert.IsTrue(dictionary.TryGetValue(name, out object? actual));
                            Assert.IsInstanceOf<IPropertyBag>(actual);
                            break;
                        }

                    case "object[]":
                        {
                            Assert.IsTrue(dictionary.TryGetValue(name, out object? actual));
                            Assert.IsInstanceOf<object[]>(actual);
                            break;
                        }

                    default:
                        throw new InvalidOperationException($"Unknown data type '{type}'");
                }
            }
        }

        [Then(@"the array stored in the dictionary as ""(.*)"" should contain (.*) entries")]
        public void ThenTheArrayStoredInTheDictionaryAsShouldContainEntries(string key, int entryCount)
        {
            IReadOnlyDictionary<string, object> dictionary = this.scenarioContext.Get<IReadOnlyDictionary<string, object>>("Result");
            object[] array = (object[])dictionary[key];
            Assert.AreEqual(entryCount, array.Length);
        }

        [Then(@"the array stored in the dictionary as ""(.*)"" should contain items of type ""(.*)""")]
        public void ThenTheArrayStoredInTheDictionaryAsShouldContainItemsOfType(string key, string type)
        {
            IReadOnlyDictionary<string, object> dictionary = this.scenarioContext.Get<IReadOnlyDictionary<string, object>>("Result");
            object[] array = (object[])dictionary[key];

            Type targetType = type switch
            {
                "long" => typeof(long),
                "IPropertyBag" => typeof(IPropertyBag),
                _ => throw new InvalidOperationException($"Unknown data type '{type}'"),
            };

            foreach (object current in array)
            {
                Assert.IsTrue(targetType.IsAssignableFrom(current.GetType()));
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
            this.creationProperties = this.CreateDictionaryFromTable(table);
        }

        [Given("I create a PropertyBag from the Dictionary")]
        [When("I create a PropertyBag from the Dictionary")]
        public void WhenIConstructAPropertyBagFromTheDictionary()
        {
            this.propertyBag = this.propertyBagFactory.Create(this.creationProperties);
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

        private JObject CreateJObjectFromTable(Table table)
        {
            var expected = new JObject();
            foreach (TableRow row in table.Rows)
            {
                row.TryGetValue("Property", out string name);
                expected[name] = this.GetRowValue(row);
            }

            return expected;
        }

        private Dictionary<string, object> CreateDictionaryFromTable(Table table)
        {
            var expected = new Dictionary<string, object>();
            foreach (TableRow row in table.Rows)
            {
                row.TryGetValue("Property", out string name);
                expected[name] = this.GetRowValue(row);
            }

            return expected;
        }

        private JToken GetRowValue(TableRow row)
        {
            row.TryGetValue("Value", out string value);
            row.TryGetValue("Type", out string type);
            return JToken.FromObject(
                type switch
                {
                    "string" => value,
                    "integer" => int.Parse(value),
                    "datetime" => DateTimeOffset.Parse(value),
                    _ => throw new InvalidOperationException($"Unknown data type '{type}'"),
                },
                JsonSerializer.Create(this.jsonSerializerSettingsProvider.Instance));
        }

        private IPropertyBag CreatePropertyBagFromTable(Table table)
        {
            return this.propertyBagFactory.Create(this.CreateDictionaryFromTable(table));
        }

        private static CultureInfoPocObject MakeCultureInfoPoco(string? culture)
        {
            return new CultureInfoPocObject
            {
                SomeCulture = string.IsNullOrEmpty(culture) ? null : CultureInfo.GetCultureInfo(culture),
            };
        }

        private static DateTimeOffsetPocObject MakeDateTimeOffsetPoco(string time, string nullableTime)
        {
            return new DateTimeOffsetPocObject()
            {
                SomeDateTime = DateTimeOffset.Parse(time),
                SomeNullableDateTime = string.IsNullOrEmpty(nullableTime) ? null : (DateTimeOffset?)DateTimeOffset.Parse(nullableTime),
            };
        }

        private static void CheckPocosAreEqual(string culture, CultureInfoPocObject poc)
        {
            var expected = new CultureInfoPocObject
            {
                SomeCulture = string.IsNullOrEmpty(culture) ? null : CultureInfo.GetCultureInfo(culture),
            };

            Assert.AreEqual(expected, poc);
        }

        private static void CheckPocosAreEqual(string time, string nullableTime, DateTimeOffsetPocObject poc)
        {
            var expected = new DateTimeOffsetPocObject()
            {
                SomeDateTime = DateTimeOffset.Parse(time),
                SomeNullableDateTime = string.IsNullOrEmpty(nullableTime) ? null : (DateTimeOffset?)DateTimeOffset.Parse(nullableTime),
            };

            Assert.AreEqual(expected, poc);
        }
    }
}