JSON
====

A small JSON library, written in C#, capable of reading and writing JSON files.

###Version 0.1.1402r5
Updated: 01/09/2013

* Fixed a bug where JSONParser would fail is given an empty string
* JSONException objects now hold JSONExceptionType as a member
* Code cleanup

###Version 0.1.1402r4
Updated: 01/08/2013

* Added more json file schema tags:
* ```"@optional": true|false``` Marks schema tag as optional (if the source json files doesn't contain the optional 
tag, it is skipped)

###Version 0.1.1402r3
Updated: 01/07/2013

* Added more json file schema tags:
* ```"@count": <maxCount>``` Array/Object child count
* ```"@pattern": "<patternString>"``` String regex pattern
* ```"@length": [ <minLength>, <maxLength> ]``` String length
* ```"@range": [ <minValue>, <maxValue> ]``` Value range

###Version 0.1.1402r2
Updated: 01/06/2013

* Added json file schema support through the statically available validation routines in JSONDocument

###Version 0.1.1402r1
Updated: 01/05/2013

* ExtractRootPath() function now publicly accessible in JSONDocument
* Fixed a bug where JSON files with trailing whitespace might fail to parse

###Version 0.1.1401r1 - 0.1.1401r5
Updated: 01/02/2013 - 01/04/2013

* JSONNumberTag now supports negative values
* JSONDocument constructors are now consistent
* ExtractKey() function now publicly accessible in JSONDocument
* Various other smaller changes

###Version 0.1.1352r1
Updated: 12/23/2013

* JSON number tags now hold float (Single) values, instead of doubles
* Floating point values can now be accessed through either AsFloat or AsInteger

###Version 0.1.1351r5
Updated: 12/17/2013

* JSONTag array/object tags are now accessible via the indexer operator

###Version 0.1.1351r4
Updated: 12/16/2013

* JSONDocument class now holds a dictionary of JSONObjectTags
* JSONDocument can now call ReadAppend() to read in multiple JSON files
* JSONDocument now holds an override flag that allows old JSON files to be removed automatically if keys match
* JSONTag classes now contain routines for converting to all derived types (array, bool, number, object, string) 
with type checking

###Version 0.1.1351r2
Updated: 12/15/2013

* Initial Release

Table of Contents
========

1. [Usage](https://github.com/majestic53/json#usage)
	* [Usage Example](https://github.com/majestic53/json#usage-example)
2. [Architecture](https://github.com/majestic53/json#architecture)
	* [Lexer](https://github.com/majestic53/json#lexer)
	* [Parser](https://github.com/majestic53/json#parser)
	* [Document](https://github.com/majestic53/json#document)
	* [Schema](https://github.com/majestic53/json#schema)
3. [Syntax](https://github.com/majestic53/json#syntax)
	* [JSON Objects](https://github.com/majestic53/json#json-objects)
	* [JSON BNF](https://github.com/majestic53/json#json-bnf)
4. [License](https://github.com/majestic53/json#license)

Usage
========

###Usage Example

Architecture
========

###Lexer

###Parser

###Document

###Schemas

Syntax
========
Compared to other human-readable languages, such as XML, JSON is exceedingly simple and easy to use. 
The following section briefly covers the various tag types and associated syntax used by JSON.

###JSON Objects

###JSON BNF
Listed below is the Backus–Naur Form (BNF) used by JSON.

```
JSON_DOC ::= <object>

array ::= [ ] | [ <pair_list> ]

object ::= { } | { <pair_list> }

pair ::= <key> : <value>

pair_list ::= <pair> | <pair> , <pair_list>

value ::= <array> | <boolean> | <number> | <object> | <string>
```

Example
========

License
======

Copyright(C) 2013-2014 David Jolly <majestic53@gmail.com>

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Lesser General Public License for more details.
