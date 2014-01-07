/*
 * JSONException.cs
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
     * JSON exception class
     * --------------------
     * Throw whenever the JSON library encounters an exception. Assume all
     * functions can potentially throw this exception
     */
    public class JSONException : Exception
    {

        /*
         * JSON exception types
         */
        public enum JSONExceptionType
        {
            CharacterOutOfBounds = 0,               // character exceeds max character in stream
            ExpectingArrayTagCloseBrace,            // expecting array tag close brace
            ExpectingArrayTagOpenBrace,             // expecting array tag open brace
            ExpectingBoolean,                       // expecting boolean string
            ExpectingBooleanValue,                  // expecting boolean value string
            ExpectingKeyString,                     // expecting key string
            ExpectingNamedArrayChildTag,            // expecting named array child tag
            ExpectingNamedBooleanChildTag,          // expecting named boolean child tag
            ExpectingNamedNumberChildTag,           // expecting named number child tag
            ExpectingNamedObjectTag,                // expecting named object tag
            ExpectingNamedObjectChildTag,           // expecting named object child tag
            ExpectingNamedStringChildTag,           // expecting named string child tag
            ExpectingNumericValue,                  // expecting numeric value string
            ExpectingObjectTagCloseBracket,         // expecting object tag close bracket
            ExpectingObjectTagOpenBracket,          // expecting object tag open bracket
            ExpectingPairDelimiter,                 // expecting pair delimiter
            ExpectingSymbol,                        // expecting symbol character
            ExpectingToken,                         // expecting tokanizable character
            FileException,                          // generic file io exception occurred
            InvalidKey,                             // invalid key string
            InvalidSchemaTagType,                   // invalid schema tag type
            KeyNotFound,                            // key not found
            KeyNotUnique,                           // key is not unique
            MissingSchemaTag,                       // missing required schema tag
            NoNextCharacter,                        // no next character exists in character stream
            NoNextToken,                            // no next token exists in token stream
            NoPreviousCharacter,                    // no previous character exists in character stream
            NoPreviousToken,                        // no previous token exists in token stream
            NotTypeArray,                           // not array tag type
            NotTypeBoolean,                         // not boolean tag type
            NotTypeNumber,                          // not number tag type
            NotTypeObject,                          // not object tag type
            NotTypeString,                          // not string tag type
            RowOutOfBounds,                         // character row does not exist
            SchemaMismatch,                         // schema does not match input
            TokenAlreadyExists,                     // token already exists
            TokenNotFound,                          // token does not exist
            TokenOutOfBounds,                       // token exceeds max token in stream
            UnexpectedEndOfStream,                  // unexpected end of stream
            UnknownSchemaTagType,                   // unknown schema tag type string
            UnterminatedString,                     // unterminated string
        }

        /*
         * JSON exception constructor
         * @param Type JSON exception type
         */
        public JSONException(JSONExceptionType Type)
            : base(
#if DEBUG
                "[" + JSONDefines.LIB_NAME + "] " +
#endif // DEBUG
                Type.ToString()
                )
        {
            return;
        }

        /*
         * JSON exception constructor
         * @param Type JSON exception type
         * @param Message exception message
         */
        public JSONException(JSONExceptionType Type, string Message)
            : base(
#if DEBUG
                "[" + JSONDefines.LIB_NAME + "] " +
#endif // DEBUG
                Type.ToString() + ": " + Message
                )
        {
            return;
        }

        public JSONException(JSONExceptionType Type, Exception InnerException)
            : base(
#if DEBUG
                "[" + JSONDefines.LIB_NAME + "] " +
#endif // DEBUG
                Type.ToString() + ": " + InnerException.Message,
                InnerException
                )
        {
            return;
        }

        /*
         * JSON exception constructor
         * @param Type JSON exception type
         * @param Message exception message
         * @param InnerException internal exception object
         */
        public JSONException(JSONExceptionType Type, string Message, Exception InnerException)
            : base(
#if DEBUG
                "[" + JSONDefines.LIB_NAME + "] " +
#endif // DEBUG
                Type.ToString() + ": " + Message + " (" + InnerException.Message + ")", 
                InnerException
                )
        {
            return;
        }
    }
}
