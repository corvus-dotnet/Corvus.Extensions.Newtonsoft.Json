@perFeatureContainer
@setupContainerForJsonNetPropertyBag
@setupContainerForJsonNetCultureInfoConversion
@setupContainerForJsonNetDateTimeOffsetConversion

Feature: JsonNetPropertyBagSpecs
	In order to provide strongly typed, extensible properties for a class that serialize neatly as JSON
	As a developer
	I want to be able to use a property bag

Scenario: Create from a property, and get that property
	Given the creation properties include "hello" with the value "world"
	When I create the property bag from the creation properties
	Then the property bag should contain a property called "hello" with the value "world"

Scenario: Create from a property, and get a missing property
	Given the creation properties include "hello" with the value "world"
	When I create the property bag from the creation properties
	Then calling TryGet with "goodbye" should return false and the result should be null

Scenario: Get and set a badly serialized property
	Given the creation properties include "hello" with the value "jiggerypokery"
	When I create the property bag from the creation properties
	And I get the property called "hello" as a custom object expecting an exception
	Then TryGet should have thrown a SerializationException

Scenario: Convert to a JObject
	Given the creation properties include "hello" with the value "world"
	And the creation properties include "number" with the value 3
	And the creation properties include "fpnumber" with the floating point value 3.14
	And the creation properties include "date" with the date value "2020-04-17T07:06:10+01:00"
	And I create the property bag from the creation properties
	When I cast to a JObject
	Then the result should be the JObject
	| Property | Value                     | Type     |
	| hello    | world                     | string   |
	| number   | 3                         | integer  |
	| fpnumber | 3.14                      | fp       |
	| date     | 2020-04-17T07:06:10+01:00 | datetime |

Scenario: Retrieve an object property as an IPropertyBag
	Given I deserialize a property bag from the string
		"""
		{
			"hello": "world",
			"number": 3,
			"nested": {
				"nestedstring": "goodbye",
				"nestednumber": 4
			}
		}
		"""
	When I get the property called "nested" as an IPropertyBag and call it "nested"
	Then the IPropertyBag called "nested" should have the properties
	| Property     | Value   | Type    |
	| nestedstring | goodbye | string  |
	| nestednumber | 4       | integer |

Scenario: Serialize a property bag
	Given the creation properties include "hello" with the value "world"
	And the creation properties include "number" with the value 3
	And the creation properties include "date" with the date value "2020-04-17T07:06:10+03:00"
	And the creation properties include "preciseDate" with the date value "2020-04-17T07:06:10.12345+01:00"
	And I create the property bag from the creation properties
	When I serialize the property bag
	Then the result should be "{"hello":"world","number":3,"date":{"dateTimeOffset":"2020-04-17T07:06:10.0000000+03:00","unixTime":1587096370000},"preciseDate":{"dateTimeOffset":"2020-04-17T07:06:10.1234500+01:00","unixTime":1587103570123}}"

Scenario: Deserialize a property bag
	Given I deserialize a property bag from the string "{"hello":"world","number":3,"date":"2020-04-17T07:06:10+01:00"}"
	Then the result should have the properties
	| Property | Value                     | Type     |
	| hello    | world                     | string   |
	| number   | 3                         | integer  |
	| date     | 2020-04-17T07:06:10+01:00 | datetime |

Scenario Outline: POCO serialization and deserialization
	Given the creation properties include a DateTime POCO called "datetimepoco" with "<time>" "<nullableTime>"
	Given the creation properties include a CultureInfo POCO called "cultureinfopoco" with "<culture>"
	Given the creation properties include an enum value called "enumvalue" with value "<enum>"
	When I create the property bag from the creation properties
	And I serialize the property bag
	And I deserialize the serialized property bag
	Then the result should have a DateTime POCO named "datetimepoco" with values "<time>" "<nullableTime>"
	Then the result should have a CultureInfo POCO named "cultureinfopoco" with value "<culture>"
	Then the result should have an enum value named "enumvalue" with value "<enum>"
	Examples:
	| time                          | nullableTime                | culture | enum   |
	| 2020-04-17T07:06:10.0+01:00   | 2020-05-01T13:14:15.3+01:00 | en-GB   | First  |
	| 2020-05-04T00:00:00.0+00:00   |                             | en-US   | Second |
	| 2020-03-14T03:49:20.527+00:00 |                             |         | Third  |

Scenario Outline: POCO deserialization
	Given I deserialize a property bag from the string "<bagJson>"
	Then the result should have a DateTime POCO named "datetimepoco" with values "<time>" "<nullableTime>"
	Then the result should have a CultureInfo POCO named "cultureinfopoco" with value "<culture>"
	Then the result should have an enum value named "enumvalue" with value "<enum>"
	Examples:
	| time                          | nullableTime                | culture | enum   | bagJson                                                                                                                                                                             |
	| 2020-04-17T07:06:10.0+01:00   | 2020-05-01T13:14:15.3+01:00 | en-GB   | First  | {'datetimepoco':{'someDateTime':'2020-04-17T07:06:10.0+01:00','someNullableDateTime':'2020-05-01T13:14:15.3+01:00'},'cultureinfopoco':{'someCulture':'en-GB'},'enumvalue':'First'}} |
	| 2020-05-04T00:00:00.0+00:00   |                             | en-US   | Second | {'datetimepoco':{'someDateTime':'2020-05-04T00:00:00.0+00:00'},'cultureinfopoco':{'someCulture':'en-US'},'enumvalue':'Second'}}                                                     |
	| 2020-03-14T03:49:20.527+00:00 |                             |         | Third  | {'datetimepoco':{'someDateTime':'2020-03-14T03:49:20.527+00:00'},'enumvalue':'Third'}}                                                                                              |

Scenario: Construct from a JObject
	Given I create a JObject
	| Property | Value | Type    |
	| hello    | world | string  |
	| number   | 3     | integer |
	When I construct a PropertyBag from the JObject
	Then the result should have the properties
	| Property | Value | Type    |
	| hello    | world | string  |
	| number   | 3     | integer |

Scenario: Construct from a Dictionary
	Given I create a Dictionary
	| Property | Value | Type    |
	| hello    | world | string  |
	| number   | 3     | integer |
	When I create a PropertyBag from the Dictionary
	Then the result should have the properties
	| Property | Value | Type    |
	| hello    | world | string  |
	| number   | 3     | integer |

Scenario: Convert to a Dictionary
	Given I create a PropertyBag
	| Property | Value | Type    |
	| hello    | world | string  |
	| number   | 3     | integer |
	When I convert the PropertyBag to a Dictionary and call it "result"
	Then the dictionary called "result" should contain the properties
	| Property | Value | Type    |
	| hello    | world | string  |
	| number   | 3     | integer |

Scenario: Construct with modifications from an existing property bag that contains nested objects
	Given I deserialize a property bag from the string
		"""
		{
			"hello": "world",
			"number": 3,
			"scalarArray": [1, 2, 3 ,4],
			"objectArray": [
				{ "prop1": "val1", "prop2": 1 },
				{ "prop1": "val2", "prop2": 2 },
				{ "prop1": "val3", "prop2": 3 }
			],
			"nested1": {
				"nestedstring": "goodbye",
				"nestednumber": 4,
				"nestedscalararray": ["a", "b", "c"],
				"nestedobject": {
					"nestedstring": "hello again",
					"nestednumber": 5
				}
			},
			"nested2": {
				"nestedstring": "hello again"
			}
		}
		"""
	When I add, modify, or remove properties
	| Property | Value | Type   | Action   |
	| foo      | bar   | string | addOrSet |
	| nested2  |       |        | remove   |
	And I get the property called "nested1" as an IPropertyBag and call it "nestedbag"
	Then the result should have the properties
	| Property    | Value | Type         |
	| hello       | world | string       |
	| number      | 3     | integer      |
	| foo         | bar   | string       |
	| objectArray |       | object[]     |
	| scalarArray |       | object[]     |
	| nested1     |       | IPropertyBag |
	And the result should not have the properties
	| Property |
	| nested2  |
	And the IPropertyBag called "nestedbag" should have the properties
	| Property     | Value   | Type   |
	| nestedstring | goodbye | string |

Scenario: Add properties
	Given I create a Dictionary
	| Property | Value | Type    |
	| hello    | world | string  |
	| number   | 3     | integer |
	And I create a PropertyBag from the Dictionary
	When I add, modify, or remove properties
	| Property | Value | Type    | Action   |
	| foo      | bar   | string  | addOrSet |
	| quux     | 4     | integer | addOrSet |
	Then the result should have the properties
	| Property | Value | Type    |
	| hello    | world | string  |
	| number   | 3     | integer |
	| foo      | bar   | string  |
	| quux     | 4     | integer |

Scenario: Modify properties
	Given I create a Dictionary
	| Property | Value | Type    |
	| hello    | world | string  |
	| number   | 3     | integer |
	And I create a PropertyBag from the Dictionary
	When I add, modify, or remove properties
	| Property | Value | Type    | Action   |
	| hello    | bar   | string  | addOrSet |
	| number   | 4     | integer | addOrSet |
	Then the result should have the properties
	| Property | Value | Type    |
	| hello    | bar   | string  |
	| number   | 4     | integer |

Scenario: Remove properties
	Given I create a Dictionary
	| Property | Value | Type    |
	| hello    | world | string  |
	| number   | 3     | integer |
	| foo      | bar   | string  |
	And I create a PropertyBag from the Dictionary
	When I add, modify, or remove properties
	| Property | Action |
	| number   | remove |
	| foo      | remove |
	Then the result should have the properties
	| Property | Value | Type   |
	| hello    | world | string |

Scenario: Add, modify, and remove properties
	Given I create a Dictionary
	| Property | Value | Type    |
	| hello    | world | string  |
	| number   | 3     | integer |
	| bar      | foo   | string  |
	And I create a PropertyBag from the Dictionary
	When I add, modify, or remove properties
	| Property | Value | Type    | Action   |
	| hello    | bar   | string  | addOrSet |
	| quux     | 4     | integer | addOrSet |
	| number   |       |         | remove   |
	Then the result should have the properties
	| Property | Value | Type    |
	| hello    | bar   | string  |
	| quux     | 4     | integer |

Scenario: Convert a property bag containing nested objects to a dictionary
	Given I deserialize a property bag from the string
		"""
		{
			"hello": "world",
			"number": 3,
			"nested": {
				"nestedstring": "goodbye",
				"nestednumber": 4
			}
		}
		"""
	When I convert the PropertyBag to a Dictionary and call it "result"
	And I get the property called "nested" as an IPropertyBag and call it "nestedbag"
	Then the dictionary called "result" should contain the properties
	| Property | Value | Type         |
	| hello    | world | string       |
	| number   | 3     | integer      |
	| nested   |       | IPropertyBag |
	Then the IPropertyBag called "nestedbag" should have the properties
	| Property     | Value   | Type    |
	| nestedstring | goodbye | string  |
	| nestednumber | 4       | integer |

Scenario: Convert a PropertyBag containing an array of a scalar type to a Dictionary
	Given I deserialize a property bag from the string
		"""
		{
			"hello": "world",
			"number": 3,
			"scalarArray": [1, 2, 3 ,4]
		}
		"""
	When I convert the PropertyBag to a Dictionary and call it "result"
	And I get the key called "scalarArray" from the dictionary called "result" as an array and call it "scalarArray"
	Then the dictionary called "result" should contain the properties
	| Property    | Value | Type     |
	| hello       | world | string   |
	| number      | 3     | integer  |
	| scalarArray |       | object[] |
	And the array called "scalarArray" should contain 4 entries
	And the array called "scalarArray" should contain items of type "long"

Scenario: Convert a PropertyBag containing an array of objects to a Dictionary
	Given I deserialize a property bag from the string
		"""
		{
			"hello": "world",
			"number": 3,
			"objectArray": [
				{ "prop": "val1" },
				{ "prop": "val2" },
				{ "prop": "val3" },
				{ "prop": "val4" },
				{ "prop": "val5" }
			]
		}
		"""
	When I convert the PropertyBag to a Dictionary and call it "result"
	And I get the key called "objectArray" from the dictionary called "result" as an array and call it "objectArray"
	Then the dictionary called "result" should contain the properties
	| Property    | Value | Type     |
	| hello       | world | string   |
	| number      | 3     | integer  |
	| objectArray |       | object[] |
	And the array called "objectArray" should contain 5 entries
	And the array called "objectArray" should contain items of type "IPropertyBag"

Scenario: Convert a PropertyBag containing an array with a null entry to a Dictionary
	Given I deserialize a property bag from the string
		"""
		{
			"hello": "world",
			"number": 3,
			"objectArray": [
				{ "prop": "val1" },
				"Hello",
				42,
				null
			]
		}
		"""
	When I convert the PropertyBag to a Dictionary and call it "result"
	And I get the key called "objectArray" from the dictionary called "result" as an array and call it "objectArray"
	Then the dictionary called "result" should contain the properties
	| Property    | Value | Type     |
	| hello       | world | string   |
	| number      | 3     | integer  |
	| objectArray |       | object[] |
	And the array called "objectArray" should contain
	| Value | Type         |
	|       | IPropertyBag |
	| Hello | string       |
	| 42    | integer      |
	|       | null         |

Scenario: Recursively convert a PropertyBag to a Dictionary
	Given I deserialize a property bag from the string
		"""
		{
			"hello": "world",
			"number": 3,
			"scalarArray": [1, 2, 3 ,4],
			"objectArray": [
				{ "prop1": "val1", "prop2": 1 },
				{ "prop1": "val2", "prop2": 2 },
				{ "prop1": "val3", "prop2": 3 }
			],
			"nested": {
				"nestedstring": "goodbye",
				"nestednumber": 4,
				"nestedscalararray": ["a", "b", "c"],
				"nestedobject": {
					"nestedstring": "hello again",
					"nestednumber": 5
				}
			}
		}
		"""
	When I recursively convert the PropertyBag to a Dictionary and call it "result"
	And I get the key called "nested" from the dictionary called "result" as an IReadOnlyDictionary<string, object> and call it "nested"
	And I get the key called "scalarArray" from the dictionary called "result" as an array and call it "scalarArray"
	And I get the key called "objectArray" from the dictionary called "result" as an array and call it "objectArray"
	Then the dictionary called "result" should contain the properties
	| Property    | Value | Type                                |
	| hello       | world | string                              |
	| number      | 3     | integer                             |
	| nested      |       | IReadOnlyDictionary<string, object> |
	| scalarArray |       | object[]                            |
	| objectArray |       | object[]                            |
	And the array called "scalarArray" should contain items of type "long"
	And the array called "objectArray" should contain items of type "IReadOnlyDictionary<string, object>"
	And the nested dictionary with key "nested" contained in the dictionary called "result" should contain the properties
	| Property          | Value   | Type                                |
	| nestedstring      | goodbye | string                              |
	| nestednumber      | 4       | integer                             |
	| nestedobject      |         | IReadOnlyDictionary<string, object> |
	| nestedscalararray |         | object[]                            |
	And the nested dictionary with key "nested.nestedobject" contained in the dictionary called "result" should contain the properties
	| Property     | Value       | Type    |
	| nestedstring | hello again | string  |
	| nestednumber | 5           | integer |
