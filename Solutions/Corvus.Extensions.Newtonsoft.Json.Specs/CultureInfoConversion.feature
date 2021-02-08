@perFeatureContainer
@setupContainerForJsonNetCultureInfoConversion

Feature: CultureInfo conversion
	In order to ensure CultureInfo instances are serialized in a consistent way
	As a developer
	I want to be able to use a JsonConverter that supports serialization to culture name

Scenario Outline: Serialize an object with convertible properties
	Given I serialize a CultureInfo POCO with "<SomeValue>", "<SomeCulture>"
	Then the result should be "<Content>"

	Examples:
		| SomeValue   | SomeCulture | Content                                           |
		| Hello there | en-US       | {"someValue":"Hello there","someCulture":"en-US"} |
		| Hello there | en-US       | {"someValue":"Hello there","someCulture":"en-US"} |
		| Hello there |             | {"someValue":"Hello there"}                       |

Scenario Outline: Deserialize an object with convertible properties
	Given I deserialize a CultureInfo POCO with the json string "<Content>"
	Then the result should have CultureInfo values "<SomeValue>", "<SomeCulture>"

	Examples:
		| SomeValue   | SomeCulture | Content                                           |
		| Hello there | en-US       | {"someValue":"Hello there","someCulture":"en-US"} |
		| Hello there | en-US       | {"someValue":"Hello there","someCulture":"en-US"} |
		| Hello there | en-US       | {"someValue":"Hello there","someCulture":"en-US"} |
		| Hello there |             | {"someValue":"Hello there","someCulture":null}    |