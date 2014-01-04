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
         * key ::= <string>
         * number ::= [0-9]+(\.[0-9]*)?
         * object ::= <ws>\{(<pair_list>|<empty>)\}<ws>
         * pair ::= <ws><key><ws>\:<ws><value><ws>
         * pair_list ::= <pair>(,<pair_list>)?
         * string ::= \".*\"
         * value ::= <array>|<boolean>|<number>|<object>|<string>|<empty>
         * ws ::= [ \t\r\n\f]*
         */

        /*
         * JSON symbols type
         */
        public enum JSONSymbolType
        {
            ArrayClose = ']',                       // close array bracket character
            ArrayOpen = '[',                        // open array bracket character
            Decimal = '.',                          // decimal point character
            Negative = '-',                         // negative value character
            ObjectClose = '}',                      // close object bracket character
            ObjectOpen = '{',                       // open object bracket character
            PairDelimiter = ',',                    // key/value pair delimiter character
            PairSeperator = ':',                    // key/value pair seperator character
            StringDelimiter = '\"',                 // string delimiter character
        }

        /*
         * JSON whitespace types
         */
        public enum JSONWhitespaceType
        {
            EndOfFile = '\0',                       // end of file character
            LineFeed = '\n',                        // line feed character
            Space = ' ',                            // space character
            Tab = '\t',                             // tab character
        }

        /*
         * JSON Constants
         */
        public const string LIBNAME = "JSON";       // library name
        public const uint LIBMAJOR = 0;             // major version
        public const uint LIBMINOR = 1;             // minor version
        public const uint LIBREVISION = 2;          // revision
        public const uint LIBWORKWEEK = 1401;       // year/workweek


        /*
         * Retrieve JSON library version
         * @return JSON library version string (Syntax: MM.mm.YYWW.rr)
         */
        public static string LibraryVersion()
        {
            return LIBMAJOR + "." + LIBMINOR + "." + LIBWORKWEEK + "." + LIBREVISION;
        }
    }
}
