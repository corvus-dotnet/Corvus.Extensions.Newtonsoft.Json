# Corvus.Extensions.Newtonsoft.Json
[![Build Status](https://dev.azure.com/endjin-labs/Corvus.Extensions.Newtonsoft.Json/_apis/build/status/corvus-dotnet.Corvus.Extensions.Newtonsoft.Json?branchName=master)](https://dev.azure.com/endjin-labs/Corvus.Extensions.Newtonsoft.Json/_build/latest?definitionId=4&branchName=master)
[![GitHub license](https://img.shields.io/badge/License-Apache%202-blue.svg)](https://raw.githubusercontent.com/corvus-dotnet/Corvus.Extensions.Newtonsoft.Json/master/LICENSE)
[![IMM](https://endimmfuncdev.azurewebsites.net/api/imm/github/corvus-dotnet/Corvus.Extensions.Newtonsoft.Json/total?cache=false)](https://endimmfuncdev.azurewebsites.net/api/imm/github/corvus-dotnet/Corvus.Extensions.Newtonsoft.Json/total?cache=false)

This provides opinionated configuration and DI support for Newtonsoft.Json serialization.

It is built for netstandard2.0.

## Features

### `IJsonSerializationSettingsProvider`
It is common to need to configure and manage a consistent set of JsonSerializerSettings across multiple components, to ensure succesful interop between services, both within and between hosts.

In order to support this, we have an `IJsonSerializerSettingsProvider` service which has a single `Instance` property which (as the naming implies) gives you an instance of Newtonsoft's `JsonSerializerSettings`, in a known configuration.

We also supply a standard implementation of this service called `JsonSerializationSettingsProvider`.

This is configured for enum serialization as strings, `camelCase` property names, no dictionary key mapping, ignored null values, and no special-case DateTime handling. 

[You can see the current defaults here.](https://github.com/corvus-dotnet/Corvus.Extensions.Newtonsoft.Json/blob/master/Solutions/Corvus.Extensions.Newtonsoft.Json/Corvus/Extensions/Json/Internal/JsonSerializerSettingsProvider.cs)

One feature of this implementation is that it takes an enumerable of `JsonConverter` objects in its constructor. If you register it in the `Microsoft.Extensions.DependencyInjection` container using the `IServiceCollection` extension method called `AddJsonSerializerSettings()`, then you get the powerful feature that it will then have its converters configured from the container too. Components that wish to add their converters to the standard settings need only add them to the container.

The default implementation does not add any converters, but we provide several standard converters as part of the library. These can be individually added via `IServiceCollection` extension methods defined in `JsonSerializerSettingsProviderServiceCollectionExtensions`.

### Provided `JsonConverters`
 
#### `DateTimeOffsetConverter`
(Nullable) DateTimeOffset (which converts to/from json of the form `{"dateTimeOffset":"<Roundtrippable string format>", "unixTime": <long of unix milliseconds>}`, and from standard JSON date strings.

This is useful when serializing `DateTimeOffset` instances to a store that provides indexing and querying functionality (e.g. CosmosDb). Standard serialization of `DateTimeOffset` uses ISO 8601 format, which includes the timezone offset in the resultant value. This makes it impossible to sort or perform range queries on the stored date/time values.

When the `DateTimeOffsetConverter` is used, the contained `dateTimeOffset` value retains the original value (including timezone information) and the "unixTime" value contains a numeric representation of the date that can be used for sorting and filtering.

#### `CultureInfoConverter`
which converts to/from the culture name string e.g. `en-GB`, `fr-FR`

### `PropertyBag`

A handy serializable property bag which converts to/from strongly typed key value pairs, internally stored in a JSON representation.

PropertyBag support is enabled by using the `JsonSerializerSettingsProviderServiceCollectionExtensions.AddJsonNetPropertyBag` extension method as part of your DI configuration.

`PropertyBag` instances are created via the `IPropertyBagFactory` implementation. The current implementation uses Json.Net for serialization/deserialization.

You can construct an empty `PropertyBag`

```cs
IPropertyBag propertyBag = propertyBagFactory.Create(PropertyBagValues.Empty);
```

Or from an `IDictionary<string,object>`

```cs
IDictionary<string,object> someDictionary;
IPropertyBag propertyBag = propertyBagFactory.Create(someDictionary);
```

If you need to create an `IPropertyBag` from an existing `JObject`, you must take a dependency on `IJsonNetPropertyBagFactory` instead of `IPropertyBagFactory`.

```cs
JObject someJObject;
IPropertyBag propertyBag = jsonNetPropertyBagFactory.Create(someJObject);
```

You can then retrieve strongly typed values from the property bag.

```cs

int myValue = 3;
var myObject = new SomeType("Hello world", 134.6);

propertyBag.TryGet("property1", out int myRetrievedValue); // returns true
propertyBag.TryGet("property2", out SomeType myRetrievedObject); // returns true
propertyBag.TryGet("property3", out double wontWork); // returns false 
```

Internally, it stores the values using a JSON representation. This means that you can happily set as one type, and retrieve as another, as long as your serializer supports that conversion.

This means that .NET types that don't have a direct JSON equivalent (e.g. DateTime) will be stored internally as strings and can be retrieved either as the native .NET type or as a string.

```cs
public class SomeType
{
  public SomeType()
  {
  }
  
  public SomeType(string property1, int property2)
  {
    this.Property1 = property1;
    this.Property2 = property2;
  }
  
  public string Property1 { get; set; }
  public int Property2 { get; set; }
}

public class SomeSemanticallySimilarType
{
  public SomeSemanticallySimilarType()
  {
  }
  
  public SomeSemanticallySimilarType(string property1, int property2)
  {
    this.Property1 = property1;
    this.Property2 = property2;
  }
  
  public string Property1 { get; set; }
  public int Property2 { get; set; }
  public bool? Property3 { get; set; }
}

propertyBag.Set("key1", new SomeType("Hello", 3));
propertyBag.TryGet("key1", out SomeSemanticallySimilarType myRetrievedObject); // returns true
```

PropertyBags are immutable. Modified bags can be created using the `IPropertyBagFactory.CreateModified` method.

```cs
IPropertyBag someExistingPropertyBag;

IDictionary<string,object> itemsToAddOrUpdate;
IList<string> keysOfItemsToRemove;

IPropertyBag propertyBag = propertyBagFactory.CreateModified(
    someExistingPropertyBag,
    itemsToAddOrUpdate,
    keysOfItemsToRemove);
```

#### Dynamic discovery of items

There are multiple ways in which the contents of a PropertyBag can be discovered at runtime.

`IPropertyBag` instances are expected to implement `IEnumerable<(string Key, PropertyBagEntryType Type)>`. This allows you to enumerate an `IPropertyBag` to discover its members and their types. The `PropertyBagEntryType` enumeration lists the types supported in JSON, with the additional nuance that it differentiates between integer and floating point numbers.

We also provide an extension method `AsDictionary()` on `IPropertyBag` which uses the enumeration to convert the property bag to a `IReadOnlyDictionary<string, object>`. Values in this dictionary will be .NET native types where possible:
- Integers will be represented as `long`.
- Floating point numbers will be represented as `double`.
- Nested objects will be added to the dictionary as further `IPropertyBag` instances.
- Arrays become `object[]`, with members converted according to the same rules.

It is also possible to use the `AsDictionaryRecursive` extension method which follows the same rules as `AsDictionary()` with the exception that nested objects are also converted to `IReadOnlyDictionary<string, object>`.

When using the Json.NET-based implementation of `IPropertyBagFactory` and `IPropertyBag`,  instances of `IPropertyBag` can be converted to a `JObject` (which can also be used for `dynamic` scenarios). This is done via the `IJsonNetPropertyBagFactory`:

```cs
IPropertyBag someExistingPropertyBag;
JObject result = jsonNetPropertyBagFactory.AsJObject(someExistingPropertyBag);
```

##### `JsonSerializerSettings`

Internally, all the property values are stored as properties on a `JObject`, which requires serializion/deserialization. `IPropertyBag` instances are supplied their `JsonSerializerSettings` by the `IJsonNetPropertyBagFactory` implementation, which receives them from the service provider on creation via the `IJsonSerializerSettingsProvider`.

Note that if you modify the default serializer settings, this can have an impact on the behaviour of the `AsDictionary` and `AsDictionaryRecursive` extension methods. For example, if you set `DateParseHandling` to `DateParseHandling.DateTime` or `DateParseHandling.DateTimeOffset` and then convert a deserialized `IPropertyBag` instance to a dictionary, you may find that your date/time fields have been converted to strings using a non-standard format.

## Licenses

[![GitHub license](https://img.shields.io/badge/License-Apache%202-blue.svg)](https://raw.githubusercontent.com/corvus-dotnet/Corvus.Extensions.Newtonsoft.Json/master/LICENSE)

Corvus.Extensions.Newtonsoft.Json is available under the Apache 2.0 open source license.

For any licensing questions, please email [&#108;&#105;&#99;&#101;&#110;&#115;&#105;&#110;&#103;&#64;&#101;&#110;&#100;&#106;&#105;&#110;&#46;&#99;&#111;&#109;](&#109;&#97;&#105;&#108;&#116;&#111;&#58;&#108;&#105;&#99;&#101;&#110;&#115;&#105;&#110;&#103;&#64;&#101;&#110;&#100;&#106;&#105;&#110;&#46;&#99;&#111;&#109;)

## Project Sponsor

This project is sponsored by [endjin](https://endjin.com), a UK based Microsoft Gold Partner for Cloud Platform, Data Platform, Data Analytics, DevOps, and a Power BI Partner.

For more information about our products and services, or for commercial support of this project, please [contact us](https://endjin.com/contact-us). 

We produce two free weekly newsletters; [Azure Weekly](https://azureweekly.info) for all things about the Microsoft Azure Platform, and [Power BI Weekly](https://powerbiweekly.info).

Keep up with everything that's going on at endjin via our [blog](https://blogs.endjin.com/), follow us on [Twitter](https://twitter.com/endjin), or [LinkedIn](https://www.linkedin.com/company/1671851/).

Our other Open Source projects can be found on [GitHub](https://endjin.com/open-source)

## Code of conduct

This project has adopted a code of conduct adapted from the [Contributor Covenant](http://contributor-covenant.org/) to clarify expected behavior in our community. This code of conduct has been [adopted by many other projects](http://contributor-covenant.org/adopters/). For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact [&#104;&#101;&#108;&#108;&#111;&#064;&#101;&#110;&#100;&#106;&#105;&#110;&#046;&#099;&#111;&#109;](&#109;&#097;&#105;&#108;&#116;&#111;:&#104;&#101;&#108;&#108;&#111;&#064;&#101;&#110;&#100;&#106;&#105;&#110;&#046;&#099;&#111;&#109;) with any additional questions or comments.

## IP Maturity Matrix (IMM)

The IMM is endjin's IP quality framework.

[![Shared Engineering Standards](https://endimmfuncdev.azurewebsites.net/api/imm/github/corvus-dotnet/Corvus.Extensions.Newtonsoft.Json/rule/74e29f9b-6dca-4161-8fdd-b468a1eb185d?nocache=true)](https://endimmfuncdev.azurewebsites.net/api/imm/github/corvus-dotnet/Corvus.Extensions.Newtonsoft.Json/rule/74e29f9b-6dca-4161-8fdd-b468a1eb185d?cache=false)

[![Coding Standards](https://endimmfuncdev.azurewebsites.net/api/imm/github/corvus-dotnet/Corvus.Extensions.Newtonsoft.Json/rule/f6f6490f-9493-4dc3-a674-15584fa951d8?cache=false)](https://endimmfuncdev.azurewebsites.net/api/imm/github/corvus-dotnet/Corvus.Extensions.Newtonsoft.Json/rule/f6f6490f-9493-4dc3-a674-15584fa951d8?cache=false)

[![Executable Specifications](https://endimmfuncdev.azurewebsites.net/api/imm/github/corvus-dotnet/Corvus.Extensions.Newtonsoft.Json/rule/bb49fb94-6ab5-40c3-a6da-dfd2e9bc4b00?cache=false)](https://endimmfuncdev.azurewebsites.net/api/imm/github/corvus-dotnet/Corvus.Extensions.Newtonsoft.Json/rule/bb49fb94-6ab5-40c3-a6da-dfd2e9bc4b00?cache=false)

[![Code Coverage](https://endimmfuncdev.azurewebsites.net/api/imm/github/corvus-dotnet/Corvus.Extensions.Newtonsoft.Json/rule/0449cadc-0078-4094-b019-520d75cc6cbb?cache=false)](https://endimmfuncdev.azurewebsites.net/api/imm/github/corvus-dotnet/Corvus.Extensions.Newtonsoft.Json/rule/0449cadc-0078-4094-b019-520d75cc6cbb?cache=false)

[![Benchmarks](https://endimmfuncdev.azurewebsites.net/api/imm/github/corvus-dotnet/Corvus.Extensions.Newtonsoft.Json/rule/64ed80dc-d354-45a9-9a56-c32437306afa?cache=false)](https://endimmfuncdev.azurewebsites.net/api/imm/github/corvus-dotnet/Corvus.Extensions.Newtonsoft.Json/rule/64ed80dc-d354-45a9-9a56-c32437306afa?cache=false)

[![Reference Documentation](https://endimmfuncdev.azurewebsites.net/api/imm/github/corvus-dotnet/Corvus.Extensions.Newtonsoft.Json/rule/2a7fc206-d578-41b0-85f6-a28b6b0fec5f?cache=false)](https://endimmfuncdev.azurewebsites.net/api/imm/github/corvus-dotnet/Corvus.Extensions.Newtonsoft.Json/rule/2a7fc206-d578-41b0-85f6-a28b6b0fec5f?cache=false)

[![Design & Implementation Documentation](https://endimmfuncdev.azurewebsites.net/api/imm/github/corvus-dotnet/Corvus.Extensions.Newtonsoft.Json/rule/f026d5a2-ce1a-4e04-af15-5a35792b164b?cache=false)](https://endimmfuncdev.azurewebsites.net/api/imm/github/corvus-dotnet/Corvus.Extensions.Newtonsoft.Json/rule/f026d5a2-ce1a-4e04-af15-5a35792b164b?cache=false)

[![How-to Documentation](https://endimmfuncdev.azurewebsites.net/api/imm/github/corvus-dotnet/Corvus.Extensions.Newtonsoft.Json/rule/145f2e3d-bb05-4ced-989b-7fb218fc6705?cache=false)](https://endimmfuncdev.azurewebsites.net/api/imm/github/corvus-dotnet/Corvus.Extensions.Newtonsoft.Json/rule/145f2e3d-bb05-4ced-989b-7fb218fc6705?cache=false)

[![Date of Last IP Review](https://endimmfuncdev.azurewebsites.net/api/imm/github/corvus-dotnet/Corvus.Extensions.Newtonsoft.Json/rule/da4ed776-0365-4d8a-a297-c4e91a14d646?cache=false)](https://endimmfuncdev.azurewebsites.net/api/imm/github/corvus-dotnet/Corvus.Extensions.Newtonsoft.Json/rule/da4ed776-0365-4d8a-a297-c4e91a14d646?cache=false)

[![Framework Version](https://endimmfuncdev.azurewebsites.net/api/imm/github/corvus-dotnet/Corvus.Extensions.Newtonsoft.Json/rule/6c0402b3-f0e3-4bd7-83fe-04bb6dca7924?cache=false)](https://endimmfuncdev.azurewebsites.net/api/imm/github/corvus-dotnet/Corvus.Extensions.Newtonsoft.Json/rule/6c0402b3-f0e3-4bd7-83fe-04bb6dca7924?cache=false)

[![Associated Work Items](https://endimmfuncdev.azurewebsites.net/api/imm/github/corvus-dotnet/Corvus.Extensions.Newtonsoft.Json/rule/79b8ff50-7378-4f29-b07c-bcd80746bfd4?cache=false)](https://endimmfuncdev.azurewebsites.net/api/imm/github/corvus-dotnet/Corvus.Extensions.Newtonsoft.Json/rule/79b8ff50-7378-4f29-b07c-bcd80746bfd4?cache=false)

[![Source Code Availability](https://endimmfuncdev.azurewebsites.net/api/imm/github/corvus-dotnet/Corvus.Extensions.Newtonsoft.Json/rule/30e1b40b-b27d-4631-b38d-3172426593ca?cache=false)](https://endimmfuncdev.azurewebsites.net/api/imm/github/corvus-dotnet/Corvus.Extensions.Newtonsoft.Json/rule/30e1b40b-b27d-4631-b38d-3172426593ca?cache=false)

[![License](https://endimmfuncdev.azurewebsites.net/api/imm/github/corvus-dotnet/Corvus.Extensions.Newtonsoft.Json/rule/d96b5bdc-62c7-47b6-bcc4-de31127c08b7?cache=false)](https://endimmfuncdev.azurewebsites.net/api/imm/github/corvus-dotnet/Corvus.Extensions.Newtonsoft.Json/rule/d96b5bdc-62c7-47b6-bcc4-de31127c08b7?cache=false)

[![Production Use](https://endimmfuncdev.azurewebsites.net/api/imm/github/corvus-dotnet/Corvus.Extensions.Newtonsoft.Json/rule/87ee2c3e-b17a-4939-b969-2c9c034d05d7?cache=false)](https://endimmfuncdev.azurewebsites.net/api/imm/github/corvus-dotnet/Corvus.Extensions.Newtonsoft.Json/rule/87ee2c3e-b17a-4939-b969-2c9c034d05d7?cache=false)

[![Insights](https://endimmfuncdev.azurewebsites.net/api/imm/github/corvus-dotnet/Corvus.Extensions.Newtonsoft.Json/rule/71a02488-2dc9-4d25-94fa-8c2346169f8b?cache=false)](https://endimmfuncdev.azurewebsites.net/api/imm/github/corvus-dotnet/Corvus.Extensions.Newtonsoft.Json/rule/71a02488-2dc9-4d25-94fa-8c2346169f8b?cache=false)

[![Packaging](https://endimmfuncdev.azurewebsites.net/api/imm/github/corvus-dotnet/Corvus.Extensions.Newtonsoft.Json/rule/547fd9f5-9caf-449f-82d9-4fba9e7ce13a?cache=false)](https://endimmfuncdev.azurewebsites.net/api/imm/github/corvus-dotnet/Corvus.Extensions.Newtonsoft.Json/rule/547fd9f5-9caf-449f-82d9-4fba9e7ce13a?cache=false)

[![Deployment](https://endimmfuncdev.azurewebsites.net/api/imm/github/corvus-dotnet/Corvus.Extensions.Newtonsoft.Json/rule/edea4593-d2dd-485b-bc1b-aaaf18f098f9?cache=false)](https://endimmfuncdev.azurewebsites.net/api/imm/github/corvus-dotnet/Corvus.Extensions.Newtonsoft.Json/rule/edea4593-d2dd-485b-bc1b-aaaf18f098f9?cache=false)
