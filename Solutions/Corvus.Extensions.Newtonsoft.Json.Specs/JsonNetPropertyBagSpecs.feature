@setupContainer
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
	And the creation properties include "date" with the date value "2020-04-17T07:06:10+01:00"
	And I create the property bag from the creation properties
	When I cast to a JObject
	Then the result should be the JObject
	| Property | Value                     | Type     |
	| hello    | world                     | string   |
	| number   | 3                         | integer  |
	| date     | 2020-04-17T07:06:10+01:00 | datetime |

Scenario: Serialize a property bag
	Given the creation properties include "hello" with the value "world"
	And the creation properties include "number" with the value 3
	And the creation properties include "date" with the date value "2020-04-17T07:06:10+01:00"
	And the creation properties include "preciseDate" with the date value "2020-04-17T07:06:10.12345+01:00"
	And I create the property bag from the creation properties
	When I serialize the property bag
	Then the result should be "{"hello":"world","number":3,"date":"2020-04-17T07:06:10+01:00","preciseDate":"2020-04-17T07:06:10.12345+01:00"}"

Scenario: Deserialize a property bag
	Given I deserialize a property bag from the string "{"hello":"world","number":3,"date":"2020-04-17T07:06:10+01:00"}"
	Then the result should have the properties
	| Property | Value                     | Type     |
	| hello    | world                     | string   |
	| number   | 3                         | integer  |
	| date     | 2020-04-17T07:06:10+01:00 | datetime |

Scenario Outline: POCO serialization and deserialization
	Given the creation properties include a POCO called "poco" with "<value>" "<time>" "<nullableTime>" "<culture>" "<enum>"
	When I create the property bag from the creation properties
	And I serialize the property bag
	And I deserialize the serialized property bag
	Then the result should have a POCO named "poco" with values "<value>" "<time>" "<nullableTime>" "<culture>" "<enum>"
	Examples:
	| value | time                          | nullableTime                | culture | enum   |
	| hello | 2020-04-17T07:06:10.0+01:00   | 2020-05-01T13:14:15.3+01:00 | en-GB   | First  |
	| world | 2020-05-04T00:00:00.0+00:00   |                             | en-US   | Second |
	| pi    | 2020-03-14T03:49:20.527+00:00 |                             |         | Third  |

Scenario Outline: POCO deserialization
	Given I deserialize a property bag from the string "<bagJson>"
	Then the result should have a POCO named "poco" with values "<value>" "<time>" "<nullableTime>" "<culture>" "<enum>"
	Examples:
	| value | time                          | nullableTime                | culture | enum   | bagJson                                                                                                                                       |
	| hello | 2020-04-17T07:06:10.0+01:00   | 2020-05-01T13:14:15.3+01:00 | en-GB   | First  | {'poco':{'someValue':'hello','someDateTime':'2020-04-17T07:06:10.0+01:00','someNullableDateTime':'2020-05-01T13:14:15.3+01:00','someCulture':'en-GB','someEnum':'First'}} |
	| world | 2020-05-04T00:00:00.0+00:00   |                             | en-US   | Second | {'poco':{'someValue':'world','someDateTime':'2020-05-04T00:00:00.0+00:00','someCulture':'en-US','someEnum':'Second'}}                                             |
	| pi    | 2020-03-14T03:49:20.527+00:00 |                             |         | Third  | {'poco':{'someValue':'pi','someDateTime':'2020-03-14T03:49:20.527+00:00','someEnum':'Third'}}                                                                 |

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
	When I convert the PropertyBag to a Dictionary
	Then the dictionary should contain the properties
	| Property | Value | Type    |
	| hello    | world | string  |
	| number   | 3     | integer |

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
