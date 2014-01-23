JSON
====

A small JSON library, written in C#, capable of reading and writing JSON files.

Table of Contents
=================

1. [Changelog](https://github.com/majestic53/json#changelog)
2. [Usage](https://github.com/majestic53/json#usage)
3. [Architecture](https://github.com/majestic53/json#architecture)
	* [Lexer](https://github.com/majestic53/json#lexer)
	* [Parser](https://github.com/majestic53/json#parser)
	* [Document](https://github.com/majestic53/json#document)
	* [Schema](https://github.com/majestic53/json#schema)
4. [Syntax](https://github.com/majestic53/json#syntax)
	* [JSON Objects](https://github.com/majestic53/json#json-objects)
	* [JSON Syntax](https://github.com/majestic53/json#json-syntax)
5. [Examples](https://github.com/majestic53/json#examples)
	* [Reading](https://github.com/majestic53/json#reading)
	* [Accessing Tags](https://github.com/majestic53/json#accessing-tags)
	* [Writing](https://github.com/majestic53/json#writing)
	* [Validation](https://github.com/majestic53/json#validation)
	* [Schema Validation](https://github.com/majestic53/json#schema-validation)
6. [License](https://github.com/majestic53/json#license)

Changelog
=========

###Version 0.1.1404r2
Updated: 01/22/2013

* Added line comment support (see examples under test directory)
* ``` ; <comment> \n```` Comments can be inserted anywhere in the JSON/JSON schema file
* Removed restriction on trailing pair seperators ','
* Added a UDL language definition for use with Notepad++

###Version 0.1.1404r1
Updated: 01/21/2013

* Added a wildcard schema tag (see "test/test/test_wildcard.json/json_schema" for an example)

###Version 0.1.1402r5
Updated: 01/09/2013

* Fixed a bug where JSONParser would fail when given an empty string as input
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

Usage
=====
To use JSON, simply compile the core project as a library and set the resulting DLL as a reference in your project. 
Alternatively, import the project files directly into your project.

Architecture
============
JSON is a JavaScript Object Notation library, capable of reading and writing files and strings, as well as verifying 
files and strings against predefined schemas. The library uses a very small number of objects to accomplish this, 
making it simple and easy to use.

JSON uses a very simple LL(1) approach to parsing files and strings. The following sections cover the various phases 
of parsing.

###Lexer
The first step breaks the input string into descrete pieces, through a process known as lexical analysis. These 
descrete pieces are called tokens, and are used to check for syntactic correctness in the next step.

###Parser
Once the code has been broken into tokens, the token ordering is used to determine if the code's syntax is correct. 
This is known as syntactic analysis. During this step the tokens are placed into a JSON root object, called the JSON 
document. This document object can then be accessed by a user to request any number of nested tags.

###Document

###Schemas

Syntax
======
Compared to other human-readable languages, such as XML, JSON is exceedingly simple and easy to use. 
The following sections briefly covers the various tag types and associated syntax used by JSON.

###JSON Objects

###JSON Syntax
Listed below is the Backus–Naur Form (BNF) used by JSON.

```
JSON_DOC ::= <object>

array ::= [ ] | [ <pair_list> ]

object ::= { } | { <pair_list> }

pair ::= <key> : <value>

pair_list ::= <pair> | <pair> , <pair_list>

value ::= <array> | <boolean> | <number> | <object> | <string>
```

Examples
========
Listed below are a series of examples showing how to use various JSON library features.

###Reading

###Accessing Tags

###Writing

###Validation

###Schema Validation

License
=======

Copyright(C) 2013-2014 David Jolly <majestic53@gmail.com>

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Lesser General Public License for more details.
