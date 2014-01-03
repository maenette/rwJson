JSON
====

A small JSON library, written in C#, capable of reading and writing JSON files.

###Version 0.1.1401.02
Updated: 01/02/2013

* ExtractKey() function now publicly accessible in JSONDocument

###Version 0.1.1401.01
Updated: 01/02/2013

* Updated file headers to reflect new year

###Version 0.1.1352.01
Updated: 12/23/2013

* JSON number tags now hold float (Single) values, instead of doubles
* Floating point values can now be accessed through either AsFloat or AsInteger

###Version 0.1.1351.05
Updated: 12/17/2013

* JSONTag array/object tags are now accessible via the indexer operator

###Version 0.1.1351.04
Updated: 12/16/2013

* JSONDocument class now holds a dictionary of JSONObjectTags
* JSONDocument can now call ReadAppend() to read in multiple JSON files
* JSONDocument now holds an override flag that allows old JSON files to be removed automatically if keys match
* JSONTag classes now contain routines for converting to all derived types (array, bool, number, object, string) with type checking

###Version 0.1.1351.02
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
3. [Syntax](https://github.com/majestic53/json#syntax)
	* [JSON Object](https://github.com/majestic53/json#json-object)
	* [JSON BNF](https://github.com/majestic53/json#json-bnf)
4. [Example](https://github.com/majestic53/json#example)
5. [License](https://github.com/majestic53/json#license)

Usage
========

###Usage Example

Architecture
========

###Lexer

###Parser

###Document

Syntax
========

###JSON Object

###JSON BNF

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
