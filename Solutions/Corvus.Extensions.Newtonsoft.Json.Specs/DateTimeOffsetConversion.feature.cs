﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (https://www.specflow.org/).
//      SpecFlow Version:3.7.0.0
//      SpecFlow Generator Version:3.7.0.0
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
#pragma warning disable
namespace Corvus.Extensions.Json.Specs
{
    using TechTalk.SpecFlow;
    using System;
    using System.Linq;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "3.7.0.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [NUnit.Framework.TestFixtureAttribute()]
    [NUnit.Framework.DescriptionAttribute("Date time offset to ISO 8601 and Unix Time conversion")]
    [NUnit.Framework.CategoryAttribute("perFeatureContainer")]
    [NUnit.Framework.CategoryAttribute("setupContainerForJsonNetDateTimeOffsetConversion")]
    public partial class DateTimeOffsetToISO8601AndUnixTimeConversionFeature
    {
        
        private TechTalk.SpecFlow.ITestRunner testRunner;
        
        private string[] _featureTags = new string[] {
                "perFeatureContainer",
                "setupContainerForJsonNetDateTimeOffsetConversion"};
        
#line 1 "DateTimeOffsetConversion.feature"
#line hidden
        
        [NUnit.Framework.OneTimeSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "", "Date time offset to ISO 8601 and Unix Time conversion", "\tIn order to ensure DateTimeOffset values are serialized in a sortable and filter" +
                    "able way without losing time zone information\r\n\tAs a developer\r\n\tI want to use a" +
                    " JsonConverter that supports serialization to ISO 8601 and unix time", ProgrammingLanguage.CSharp, new string[] {
                        "perFeatureContainer",
                        "setupContainerForJsonNetDateTimeOffsetConversion"});
            testRunner.OnFeatureStart(featureInfo);
        }
        
        [NUnit.Framework.OneTimeTearDownAttribute()]
        public virtual void FeatureTearDown()
        {
            testRunner.OnFeatureEnd();
            testRunner = null;
        }
        
        [NUnit.Framework.SetUpAttribute()]
        public virtual void TestInitialize()
        {
        }
        
        [NUnit.Framework.TearDownAttribute()]
        public virtual void TestTearDown()
        {
            testRunner.OnScenarioEnd();
        }
        
        public virtual void ScenarioInitialize(TechTalk.SpecFlow.ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioInitialize(scenarioInfo);
            testRunner.ScenarioContext.ScenarioContainer.RegisterInstanceAs<NUnit.Framework.TestContext>(NUnit.Framework.TestContext.CurrentContext);
        }
        
        public virtual void ScenarioStart()
        {
            testRunner.OnScenarioStart();
        }
        
        public virtual void ScenarioCleanup()
        {
            testRunner.CollectScenarioErrors();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Serialize an object with convertible properties")]
        [NUnit.Framework.TestCaseAttribute("2018-04-15T09:09:31.234+01:00", "2018-04-15T09:09:31.234+01:00", "{\"someDateTime\":{\"dateTimeOffset\":\"2018-04-15T09:09:31.2340000+01:00\",\"unixTime\":" +
            "1523779771234},\"someNullableDateTime\":{\"dateTimeOffset\":\"2018-04-15T09:09:31.234" +
            "0000+01:00\",\"unixTime\":1523779771234}}", null)]
        [NUnit.Framework.TestCaseAttribute("2018-04-15T09:09:31.234+01:00", "2018-04-15T09:09:31.234+01:00", "{\"someDateTime\":{\"dateTimeOffset\":\"2018-04-15T09:09:31.2340000+01:00\",\"unixTime\":" +
            "1523779771234},\"someNullableDateTime\":{\"dateTimeOffset\":\"2018-04-15T09:09:31.234" +
            "0000+01:00\",\"unixTime\":1523779771234}}", null)]
        [NUnit.Framework.TestCaseAttribute("2018-04-15T09:09:31.234+01:00", "", "{\"someDateTime\":{\"dateTimeOffset\":\"2018-04-15T09:09:31.2340000+01:00\",\"unixTime\":" +
            "1523779771234}}", null)]
        public virtual void SerializeAnObjectWithConvertibleProperties(string someDateTime, string someNullableDateTime, string content, string[] exampleTags)
        {
            string[] tagsOfScenario = exampleTags;
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            argumentsOfScenario.Add("SomeDateTime", someDateTime);
            argumentsOfScenario.Add("SomeNullableDateTime", someNullableDateTime);
            argumentsOfScenario.Add("Content", content);
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Serialize an object with convertible properties", null, tagsOfScenario, argumentsOfScenario, this._featureTags);
#line 9
this.ScenarioInitialize(scenarioInfo);
#line hidden
            bool isScenarioIgnored = default(bool);
            bool isFeatureIgnored = default(bool);
            if ((tagsOfScenario != null))
            {
                isScenarioIgnored = tagsOfScenario.Where(__entry => __entry != null).Where(__entry => String.Equals(__entry, "ignore", StringComparison.CurrentCultureIgnoreCase)).Any();
            }
            if ((this._featureTags != null))
            {
                isFeatureIgnored = this._featureTags.Where(__entry => __entry != null).Where(__entry => String.Equals(__entry, "ignore", StringComparison.CurrentCultureIgnoreCase)).Any();
            }
            if ((isScenarioIgnored || isFeatureIgnored))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 10
 testRunner.Given(string.Format("I serialize a DateTimeOffset POCO with \"{0}\", \"{1}\"", someDateTime, someNullableDateTime), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 11
 testRunner.Then(string.Format("the result should be \"{0}\"", content), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Deserialize an object with convertible properties")]
        [NUnit.Framework.TestCaseAttribute("2018-04-15T09:09:31.234+01:00", "2018-04-15T09:09:31.234+01:00", "{\"someDateTime\":{\"dateTimeOffset\":\"2018-04-15T09:09:31.2340000+01:00\",\"unixTime\":" +
            "1523779771234},\"someNullableDateTime\":{\"dateTimeOffset\":\"2018-04-15T09:09:31.234" +
            "0000+01:00\",\"unixTime\":1523779771234}}", null)]
        [NUnit.Framework.TestCaseAttribute("2018-04-15T09:09:31.234+01:00", "2018-04-15T09:09:31.234+01:00", "{\"someDateTime\":\"2018-04-15T09:09:31.2340000+01:00\",\"someNullableDateTime\":\"2018-" +
            "04-15T09:09:31.2340000+01:00\"}", null)]
        [NUnit.Framework.TestCaseAttribute("2018-04-15T09:09:31.234+01:00", "", "{\"someDateTime\":{\"dateTimeOffset\":\"2018-04-15T09:09:31.2340000+01:00\",\"unixTime\":" +
            "1523779771234}}", null)]
        [NUnit.Framework.TestCaseAttribute("2018-04-15T09:09:31.234+01:00", "", "{\"someDateTime\":{\"dateTimeOffset\":\"2018-04-15T09:09:31.2340000+01:00\",\"unixTime\":" +
            "1523779771234}}", null)]
        public virtual void DeserializeAnObjectWithConvertibleProperties(string someDateTime, string someNullableDateTime, string content, string[] exampleTags)
        {
            string[] tagsOfScenario = exampleTags;
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            argumentsOfScenario.Add("SomeDateTime", someDateTime);
            argumentsOfScenario.Add("SomeNullableDateTime", someNullableDateTime);
            argumentsOfScenario.Add("Content", content);
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Deserialize an object with convertible properties", null, tagsOfScenario, argumentsOfScenario, this._featureTags);
#line 19
this.ScenarioInitialize(scenarioInfo);
#line hidden
            bool isScenarioIgnored = default(bool);
            bool isFeatureIgnored = default(bool);
            if ((tagsOfScenario != null))
            {
                isScenarioIgnored = tagsOfScenario.Where(__entry => __entry != null).Where(__entry => String.Equals(__entry, "ignore", StringComparison.CurrentCultureIgnoreCase)).Any();
            }
            if ((this._featureTags != null))
            {
                isFeatureIgnored = this._featureTags.Where(__entry => __entry != null).Where(__entry => String.Equals(__entry, "ignore", StringComparison.CurrentCultureIgnoreCase)).Any();
            }
            if ((isScenarioIgnored || isFeatureIgnored))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 20
 testRunner.Given(string.Format("I deserialize a DateTimeOffset POCO with the json string \"{0}\"", content), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 21
 testRunner.Then(string.Format("the result should have DateTimeOffset values \"{0}\", \"{1}\"", someDateTime, someNullableDateTime), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion
