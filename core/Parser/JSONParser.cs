/*
 * JSONParser.cs
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
using System.Collections.Generic;
using System.Text;

namespace JSON
{

    /*
     * JSON parser class
     * -----------------
     * Enumerate root JSON object tag
     */
    class JSONParser : JSONLexer
    {

        /*
         * JSON parser constructor
         */
        public JSONParser()
        {
            rootTag = new JSONObjectTag(string.Empty);
            ClearRootTag();
        }

        /*
         * JSON parser constructor
         * @param Input JSON data or file path
         * @param IsFile determine if input in a file path
         */
        public JSONParser(string Input, bool IsFile)
        {
            rootTag = new JSONObjectTag(string.Empty);
            SetRootTag(Input, IsFile);
        }

        /*
         * Advance lexer token
         * @param ExpectEnd expect end of stream (default: true)
         * @return next token in stream
         */
        private JSONToken AdvanceToken(bool ExpectEnd = true)
        {
            JSONToken nextToken = new JSONToken(string.Empty);

            if (HasNextToken())
            {
                nextToken = MoveNextToken();
            }
            else if (!ExpectEnd)
            {
                throw new JSONException(
                    JSONException.JSONExceptionType.UnexpectedEndOfStream,
                    GetToken().ToString()
                    );
            }

            return nextToken;
        }

        /*
         * Clear JSON parser
         */
        public void ClearRootTag()
        {
            ResetTokens();
            rootTag.Clear();
        }

        /*
         * Enumerate root JSON object
         */
        public void DiscoverRootTag()
        {
            rootTag = EnumerateRootTag();
        }

        /*
         * Enumerate JSON array tag
         * @param SkipKey skip key parsing (default: false)
         * @retrun JSON array tag
         */
        private JSONArrayTag EnumerateArray(bool SkipKey = false)
        {
            JSONToken currentToken;
            JSONArrayTag newTag = new JSONArrayTag(string.Empty);

            if (!SkipKey)
            {
                newTag.Key = EnumerateKey();
            }
            currentToken = GetToken();

            if ((currentToken.Type != JSONToken.JSONTokenType.Symbol)
                || ((char)currentToken.SubType != (char)JSONDefines.JSONSymbolType.ArrayOpen))
            {
                throw new JSONException(
                    JSONException.JSONExceptionType.ExpectingArrayTagOpenBrace,
                    currentToken.ToString()
                    );
            }
            currentToken = AdvanceToken();

            if ((currentToken.Type != JSONToken.JSONTokenType.Symbol)
                || ((char)currentToken.SubType != (char)JSONDefines.JSONSymbolType.ArrayClose))
            {

                while (HasNextToken())
                {
                    newTag.Add(EnumerateTag(true));
                    currentToken = GetToken();

                    if ((currentToken.Type != JSONToken.JSONTokenType.Symbol)
                        || ((char)currentToken.SubType != (char)JSONDefines.JSONSymbolType.PairDelimiter))
                    {
                        break;
                    }
                    currentToken = AdvanceToken();

                    if ((currentToken.Type == JSONToken.JSONTokenType.Symbol)
                        && ((char)currentToken.SubType == (char)JSONDefines.JSONSymbolType.ArrayClose))
                    {
                        break;
                    }
                }
            }

            if ((currentToken.Type != JSONToken.JSONTokenType.Symbol)
                || ((char)currentToken.SubType != (char)JSONDefines.JSONSymbolType.ArrayClose))
            {
                throw new JSONException(
                    JSONException.JSONExceptionType.ExpectingArrayTagCloseBrace,
                    currentToken.ToString()
                    );
            }
            AdvanceToken();

            return newTag;
        }

        /*
         * Enumerate JSON tag key string
         * @retrun JSON tag key string
         */
        private string EnumerateKey()
        {
            string newKey;
            JSONToken currentToken;

            currentToken = GetToken();

            if (currentToken.Type != JSONToken.JSONTokenType.String)
            {
                throw new JSONException(
                    JSONException.JSONExceptionType.ExpectingKeyString,
                    currentToken.ToString()
                    );
            }
            newKey = currentToken.Text;
            currentToken = AdvanceToken(false);

            if ((currentToken.Type != JSONToken.JSONTokenType.Symbol)
                || ((char)currentToken.SubType != (char)JSONDefines.JSONSymbolType.PairSeperator))
            {
                throw new JSONException(
                    JSONException.JSONExceptionType.ExpectingPairDelimiter,
                    currentToken.ToString()
                    );
            }
            AdvanceToken(false);

            return newKey;
        }

        /*
         * Enumerate JSON object tag
         * @param SkipKey skip key parsing (default: false)
         * @retrun JSON object tag
         */
        private JSONObjectTag EnumerateObject(bool SkipKey = false)
        {
            IJSONTag newSubTag;
            JSONToken currentToken;
            JSONObjectTag newTag = new JSONObjectTag(string.Empty);

            if (!SkipKey)
            {
                newTag.Key = EnumerateKey();
            }
            currentToken = GetToken();

            if ((currentToken.Type != JSONToken.JSONTokenType.Symbol)
                || ((char)currentToken.SubType != (char)JSONDefines.JSONSymbolType.ObjectOpen))
            {
                throw new JSONException(
                    JSONException.JSONExceptionType.ExpectingObjectTagOpenBracket,
                    currentToken.ToString()
                    );
            }
            currentToken = AdvanceToken();

            if ((currentToken.Type != JSONToken.JSONTokenType.Symbol)
                || ((char)currentToken.SubType != (char)JSONDefines.JSONSymbolType.ObjectClose))
            {

                while (HasNextToken())
                {
                    newSubTag = EnumerateTag();

                    try
                    {
                        newTag.Add(newSubTag.GetKey(), newSubTag);
                    }
                    catch (Exception exception)
                    {
                        throw new JSONException(
                            JSONException.JSONExceptionType.KeyNotUnique,
                            "\'" + newSubTag.GetKey() + "\'",
                            exception
                            );
                    }
                    currentToken = GetToken();

                    if ((currentToken.Type != JSONToken.JSONTokenType.Symbol)
                        || ((char)currentToken.SubType != (char)JSONDefines.JSONSymbolType.PairDelimiter))
                    {
                        break;
                    }
                    currentToken = AdvanceToken();

                    if ((currentToken.Type == JSONToken.JSONTokenType.Symbol)
                        && ((char)currentToken.SubType == (char)JSONDefines.JSONSymbolType.ObjectClose))
                    {
                        break;
                    }
                }
            }

            if ((currentToken.Type != JSONToken.JSONTokenType.Symbol)
                || ((char)currentToken.SubType != (char)JSONDefines.JSONSymbolType.ObjectClose))
            {
                throw new JSONException(
                    JSONException.JSONExceptionType.ExpectingObjectTagCloseBracket,
                    currentToken.ToString()
                    );
            }
            AdvanceToken();

            return newTag;
        }

        /*
         * Enumerate JSON root object tags
         * @retrun JSON root object tags
         */
        private JSONObjectTag EnumerateRootTag()
        {
            JSONToken currentToken;
            JSONObjectTag rootTag = new JSONObjectTag(string.Empty);

            ClearRootTag();
            currentToken = GetToken();

            if (HasNextToken() && (currentToken.Type == JSONToken.JSONTokenType.Begin))
            {
                currentToken = AdvanceToken();
            }

            if (HasNextToken())
            {
                rootTag = EnumerateObject(true);
            }

            return rootTag;
        }

        /*
         * Enumerate JSON tag
         * @param SkipKey skip key parsing (default: false)
         * @retrun JSON tag
         */
        private IJSONTag EnumerateTag(bool SkipKey = false)
        {
            IJSONTag newTag;
            bool newTagBoolValue;
            JSONToken currentToken;
            float newTagFloatValue;
            string newTagKey = string.Empty;

            if (!SkipKey)
            {
                newTagKey = EnumerateKey();
            }
            currentToken = GetToken();

            switch (currentToken.Type)
            {
                case JSONToken.JSONTokenType.Boolean:
                    
                    if (!Boolean.TryParse((string)currentToken.SubType, out newTagBoolValue))
                    {
                        throw new JSONException(
                            JSONException.JSONExceptionType.ExpectingBooleanValue,
                            currentToken.ToString()
                            );
                    }
                    newTag = new JSONBooleanTag(newTagKey, newTagBoolValue);
                    AdvanceToken();
                    break;
                case JSONToken.JSONTokenType.Number:

                    if (!Single.TryParse(currentToken.Text, out newTagFloatValue))
                    {
                        throw new JSONException(
                            JSONException.JSONExceptionType.ExpectingNumericValue,
                            currentToken.ToString()
                            );
                    }
                    newTag = new JSONNumberTag(newTagKey, newTagFloatValue);
                    AdvanceToken();
                    break;
                case JSONToken.JSONTokenType.String:
                    newTag = new JSONStringTag(newTagKey, currentToken.Text);
                    AdvanceToken();
                    break;
                case JSONToken.JSONTokenType.Symbol:

                    switch ((char)currentToken.SubType)
                    {
                        case (char)JSONDefines.JSONSymbolType.ArrayOpen:
                            newTag = EnumerateArray(true);
                            newTag.SetKey(newTagKey);
                            break;
                        case (char)JSONDefines.JSONSymbolType.ObjectOpen:
                            newTag = EnumerateObject(true);
                            newTag.SetKey(newTagKey);
                            break;
                        default:
                            throw new JSONException(
                                JSONException.JSONExceptionType.ExpectingToken,
                                currentToken.ToString()
                                );
                    }
                    break;
                default:
                    throw new JSONException(
                        JSONException.JSONExceptionType.ExpectingToken,
                        currentToken.ToString()
                        );
            }

            return newTag;
        }

        /*
         * Retreat lexer token
         * @param ExpectEnd expect end of stream (default: true)
         * @return previous token in stream
         */
        private JSONToken RetreatToken()
        {
            JSONToken previousToken = new JSONToken(string.Empty);

            if (HasPreviousToken())
            {
                previousToken = MovePreviousToken();
            }

            return previousToken;
        }

        /*
         * Set JSON parser input
         * @param Input JSON data or file path
         * @param IsFile determine if input in a file path
         */
        public void SetRootTag(string Input, bool IsFile)
        {
            SetTokens(Input, IsFile);
            DiscoverRootTag();
        }

        /*
         * JSON parser string representation
         * @return parser string representation
         */
        public override string ToString()
        {
            return rootTag.ToString();
        }

        /*
         * Parser root object tag
         */
        public JSONObjectTag RootTag
        {
            get { return rootTag; }
        }

        private JSONObjectTag rootTag;
    }
}
