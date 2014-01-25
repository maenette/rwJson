/*
 * JSONDefines.cs
 * Copyright (C) 2013-2014 David Jolly
 * ------------------------------
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published 
 * by the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Text;

namespace JSON
{

    /*
     * JSON defines static class
     * -------------------------
     * Holds a series of entries defines by the JSON language
     */
    public static class JSONDefines
    {

        /*
         * JSON_DOC ::= <object>
         * ---------------------
         * array ::= \[(<pair_list>|<empty>)\]
         * boolean ::= true|false
         * comment ::= ;.*\n
         * key ::= <string>
         * number ::= (-)?[0-9]+(\.[0-9]*)?
         * object ::= <ws>\{(<pair_list>|<empty>)\}<ws>
         * pair ::= <ws><key><ws>\:<ws><value><ws>
         * pair_list ::= <pair>(,(<pair_list>)?)?
         * string ::= \".*\"
         * value ::= <array>|<boolean>|<number>|<object>|<string>|<empty>
         * ws ::= [ \t\r\n\f]*
         */

        /*
         * JSON symbols type
         */
        public enum JSONSymbolType
        {
            ArrayClose = ']',                                                           // close array bracket character
            ArrayOpen = '[',                                                            // open array bracket character
            Decimal = '.',                                                              // decimal point character
            Negative = '-',                                                             // negative value character
            ObjectClose = '}',                                                          // close object bracket character
            ObjectOpen = '{',                                                           // open object bracket character
            PairDelimiter = ',',                                                        // key/value pair delimiter character
            PairSeperator = ':',                                                        // key/value pair seperator character
            StringDelimiter = '\"',                                                     // string delimiter character
        }

        /*
         * JSON whitespace types
         */
        public enum JSONWhitespaceType
        {
            EndOfFile = '\0',                                                           // end of file character
            LineComment = ';',                                                          // line comment character
            LineFeed = '\n',                                                            // line feed character
            Space = ' ',                                                                // space character
            Tab = '\t',                                                                 // tab character
        }

        /*
         * JSON File Constants
         */
        public const string KEY_PATTERN = @".*\\([_\-a-zA-Z0-9]+)\.json$";              // JSON file key regex
        public const string ROOT_PATH_PATTERN = @"(.*)\\.*.json$";                      // JSON file root path regex

        /*
         * JSON Schema Constants
         */
        public const int SCHEMA_LENGTH_MAX_COUNT = 2;                                   // schema length tag maximum element count
        public const int SCHEMA_LENGTH_MAX_OFFSET = 1;                                  // schema length maximum value offset
        public const int SCHEMA_LENGTH_MIN_COUNT = 1;                                   // schema length tag minimum element count
        public const int SCHEMA_LENGTH_MIN_OFFSET = 0;                                  // schema length minimum value offset
        public const int SCHEMA_RANGE_MAX_COUNT = 2;                                    // schema range tag maximum element count
        public const int SCHEMA_RANGE_MAX_OFFSET = 1;                                   // schema range maximum value offset
        public const int SCHEMA_RANGE_MIN_COUNT = 1;                                    // schema range tag minimum element count
        public const int SCHEMA_RANGE_MIN_OFFSET = 0;                                   // schema range minimum value offset
        public const string SCHEMA_TAG_COUNT = "@count";                                // schema count tag
        public const string SCHEMA_TAG_ENTRY = "@entry";                                // schema entry tag
        public const string SCHEMA_TAG_LENGTH = "@length";                              // schema length tag
        public const string SCHEMA_TAG_OPTIONAL = "@optional";                          // schema optional tag
        public const string SCHEMA_TAG_PATTERN = "@pattern";                            // schema pattern tag
        public const string SCHEMA_TAG_RANGE = "@range";                                // schema range tag
        public const string SCHEMA_TAG_TYPE = "@type";                                  // schema type tag
        public const string SCHEMA_TAG_WILDCARD = "*";                                  // schema wildcard tag
        public const string SCHEMA_TAG_ARRAY = "array";                                 // schema array tag type
        public const string SCHEMA_TAG_BOOLEAN = "boolean";                             // schema boolean tag type
        public const string SCHEMA_TAG_FLOAT = "float";                                 // schema float tag type
        public const string SCHEMA_TAG_INTEGER = "integer";                             // schema integer tag type
        public const string SCHEMA_TAG_OBJECT = "object";                               // schema object tag type
        public const string SCHEMA_TAG_STRING = "string";                               // schema string tag type

        /*
         * JSON Library Constants
         */
        public const string LIB_NAME = "JSON Library";                                  // library name
        public const string LIB_COPYRIGHT = "Copyright (C) 2013-2014 David Jolly";      // library copyright
        public const uint LIB_MAJOR = 0;                                                // major version
        public const uint LIB_MINOR = 1;                                                // minor version
        public const uint LIB_REVISION = 3;                                             // revision
        public const uint LIB_WORKWEEK = 1404;                                          // year/workweek

        /*
         * Retrieve JSON library version
         * @param ShowTitle show library title (default: false)
         * @param ShowCopyright show library copyright (default: false)
         * @return JSON library version string (Syntax: MM.mm.YYWW.rr)
         */
        public static string LibraryVersion(bool ShowTitle = false, bool ShowCopyright = false)
        {
            StringBuilder stream = new StringBuilder();

            if (ShowTitle)
            {
                stream.Append(LIB_NAME + " ");
            }
            stream.Append("[v." + LIB_MAJOR + "." + LIB_MINOR + "." + LIB_WORKWEEK + "r" + LIB_REVISION + "]");

            if (ShowCopyright)
            {
                stream.Append("\n" + LIB_COPYRIGHT);
            }

            return stream.ToString();
        }
    }
}
