@setupContainerForJsonNetDateTimeOffsetConversion

Feature: Date time offset to ISO 8601 and Unix Time conversion
	In order to ensure DateTimeOffset values are serialized in a sortable and filterable way without losing time zone information
	As a developer
	I want to use a JsonConverter that supports serialization to ISO 8601 and unix time

Scenario Outline: Serialize an object with convertible properties
	Given I serialize a DateTimeOffset POCO with "<SomeDateTime>", "<SomeNullableDateTime>"
	Then the result should be "<Content>"

	Examples:
		| SomeDateTime                  | SomeNullableDateTime          | Content                                                                                                                                                                                                                           |
		| 2018-04-15T09:09:31.234+01:00 | 2018-04-15T09:09:31.234+01:00 | {"someDateTime":{"dateTimeOffset":"2018-04-15T09:09:31.2340000+01:00","unixTime":1523779771234},"someNullableDateTime":{"dateTimeOffset":"2018-04-15T09:09:31.2340000+01:00","unixTime":1523779771234}} |
		| 2018-04-15T09:09:31.234+01:00 | 2018-04-15T09:09:31.234+01:00 | {"someDateTime":{"dateTimeOffset":"2018-04-15T09:09:31.2340000+01:00","unixTime":1523779771234},"someNullableDateTime":{"dateTimeOffset":"2018-04-15T09:09:31.2340000+01:00","unixTime":1523779771234}} |
		| 2018-04-15T09:09:31.234+01:00 |                               | {"someDateTime":{"dateTimeOffset":"2018-04-15T09:09:31.2340000+01:00","unixTime":1523779771234}}                                                                                                        |

Scenario Outline: Deserialize an object with convertible properties
	Given I deserialize a POCO with the json string "<Content>"
	Then the result should have DateTimeOffset values "<SomeDateTime>", "<SomeNullableDateTime>"

	Examples:
		| SomeDateTime                  | SomeNullableDateTime          | Content                                                                                                                                                                                                                           |
		| 2018-04-15T09:09:31.234+01:00 | 2018-04-15T09:09:31.234+01:00 | {"someDateTime":{"dateTimeOffset":"2018-04-15T09:09:31.2340000+01:00","unixTime":1523779771234},"someNullableDateTime":{"dateTimeOffset":"2018-04-15T09:09:31.2340000+01:00","unixTime":1523779771234}} |
		| 2018-04-15T09:09:31.234+01:00 | 2018-04-15T09:09:31.234+01:00 | {"someDateTime":"2018-04-15T09:09:31.2340000+01:00","someNullableDateTime":"2018-04-15T09:09:31.2340000+01:00"}                                                                                         |
		| 2018-04-15T09:09:31.234+01:00 |                               | {"someDateTime":{"dateTimeOffset":"2018-04-15T09:09:31.2340000+01:00","unixTime":1523779771234}}                                                                                                        |
		| 2018-04-15T09:09:31.234+01:00 |                               | {"someDateTime":{"dateTimeOffset":"2018-04-15T09:09:31.2340000+01:00","unixTime":1523779771234}}                                                                                                        |