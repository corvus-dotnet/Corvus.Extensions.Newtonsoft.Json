@setupContainer
Feature: PropertyBagSpecs
	In order to provide strongly typed, extensible properties for a class
	As a developer
	I want to be able to use a property bag

Scenario: Create from a property, and get that property
	Given I set a property called "hello" to the value "world"
	When I get the property called "hello"
	Then the result should be "world"

Scenario: Create from a property, and get a missing property
	Given I set a property called "hello" to the value "world"
	When I get the property called "goodbye"
	Then TryGet should have returned false and the result should be null

Scenario: Get and set a badly serialized property
	Given I set a property called "hello" to the value "jiggerypokery"
	When I get the property called "hello" as a custom object expecting an exception
	Then TryGet should have thrown a SerializationException

Scenario: Convert to a JObject
	Given I set a property called "hello" to the value "world"
	And I set a property called "number" to the value 3
	When I cast to a JObject
	Then the result should be the JObject
	| Property | Value | Type    |
	| hello    | world | string  |
	| number   | 3     | integer |

Scenario: Serialize a property bag
	Given I set a property called "hello" to the value "world"
	And I set a property called "number" to the value 3
	And I serialize the property bag
	Then the result should be "{"hello":"world","number":3}"

Scenario: Deserialize a property bag
	Given I deserialize a property bag from the string "{"hello":"world","number":3}"
	Then the result should have the properties
	| Property | Value | Type    |
	| hello    | world | string  |
	| number   | 3     | integer |

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
