/*
 * JSONLexer.cs
 * Copyright (C) 2013 David Jolly
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
using System.IO;
using System.Text;

namespace JSON
{

    /*
     * JSON token class
     * ----------------
     * Holds token information, including token types and text
     */
    class JSONToken
    {

        /*
         * JSON token types
         */
        public enum JSONTokenType
        {
            Begin,                          // begin sentinel token type
            Boolean,                        // boolean token type
            End,                            // end sentinel token type
            Number,                         // number token type
            String,                         // string token type
            Symbol,                         // symbol token type
        }

        /*
         * JSON token constructor
         * @param Text token text
         */
        public JSONToken(string Text)
        {
            line = 0;
            source = string.Empty;
            subType = null;
            text = Text;
            Type = JSONTokenType.Begin;
            uniqueID = Guid.NewGuid();
        }

        /*
         * JSON token constructor
         * @param Text token text
         * @param Type token type
         * @param SubType token subtype (default: null)
         */
        public JSONToken(string Text, JSONTokenType Type, object SubType = null)
        {
            line = 0;
            source = string.Empty;
            subType = SubType;
            text = Text;
            type = Type;
            uniqueID = Guid.NewGuid();
        }

        /*
         * JSON token string representation
         * @return token string representation
         */
        public override string ToString()
        {
            StringBuilder stream = new StringBuilder();

#if DEBUG
            stream.Append("{" + uniqueID.ToString() + "} ");
#endif // DEBUG
            stream.Append("[" + type.ToString());

            if (subType != null)
            {
                stream.Append(", " + subType.ToString());
            }
            stream.Append("]");

            if ((type != JSONTokenType.Begin) && (type != JSONTokenType.End))
            {
                if (text.Length > 0)
                {
                    stream.Append(" \'" + text + "\'");
                }

                stream.Append(" (");

                if (source.Length > 0)
                {
                    stream.Append("\'" + source + "\': ");
                }
                stream.Append(line + ")");
            }

            return stream.ToString();
        }

        /*
         * Token line
         */
        public int Line
        {
            get { return line; }
            set { line = value; }
        }

        /*
         * Token source
         */
        public string Source
        {
            get { return source; }
            set { source = value; }
        }

        /*
         * Token subtype
         */
        public object SubType
        {
            get { return subType; }
            set { subType = value; }
        }

        /*
         * Token text
         */
        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        /*
         * Token type
         */
        public JSONTokenType Type
        {
            get { return type; }
            set { type = value; }
        }

        /*
         * Token unique identifier
         */
        public Guid UniqueID
        {
            get { return uniqueID; }
        }

        private int line;
        private string source;
        private object subType;
        private string text;
        private JSONTokenType type;
        private Guid uniqueID;
    }

    /*
     * JSON lexer base class
     * ---------------------
     * Enumerate a series of characters
     */
    class JSONLexerBase
    {

        /*
         * JSON character types
         */
        public enum JSONCharacterType
        {
            Alpha = 0,                          // alpha character type
            Digit,                              // digit character type
            Symbol,                             // symbol character type
            Whitespace,                         // whitespace character type
        }

        /*
         * JSON lexer base constructor
         */
        public JSONLexerBase()
        {
            characterColumnLength = new Dictionary<int, int>();
            ClearCharacters();
        }

        /*
         * JSON lexer base constructor
         * @param Input JSON data or file path
         * @param IsFile determine if input in a file path
         */
        public JSONLexerBase(string Input, bool IsFile)
        {
            characterColumnLength = new Dictionary<int, int>();
            SetCharacters(Input, IsFile);
        }

        /*
         * Clear JSON lexer base
         */
        public void ClearCharacters()
        {
            characterColumn = 0;
            characterColumnLength.Clear();
            characterPosition = 0;
            characterRow = 0;
            input += (char)JSONDefines.JSONWhitespaceType.EndOfFile;
            path = string.Empty;
            DetermineCharacterType();
        }

        /*
         * Determine character type
         */
        private void DetermineCharacterType()
        {
            char currentCharacter = GetCharacter();

            characterType = JSONCharacterType.Symbol;

            if (Char.IsLetter(currentCharacter))
            {
                characterType = JSONCharacterType.Alpha;
            }
            else if (Char.IsDigit(currentCharacter))
            {
                characterType = JSONCharacterType.Digit;
            }
            else if (Char.IsWhiteSpace(currentCharacter))
            {
                characterType = JSONCharacterType.Whitespace;
            }
        }

        /*
         * Retrieve character at current position
         * @return character at current position
         */
        public char GetCharacter()
        {
            return GetCharacter(characterPosition);
        }

        /*
         * Retrieve character at a specified position
         * @param Position character position (0 <= Position < input.Length)
         * @return character at a specified position
         */
        public char GetCharacter(int Position)
        {
            if ((Position < 0) || (Position >= input.Length))
            {
                throw new JSONException(
                    JSONException.JSONExceptionType.CharacterOutOfBounds,
                    "pos. " + Position.ToString()
                    );
            }

            return Input[Position];
        }

        /*
         * Determine if next character exists
         * @return true if exists, false otherwise
         */
        public bool HasNextCharacter()
        {
            return GetCharacter() != (char)JSONDefines.JSONWhitespaceType.EndOfFile;
        }

        /*
         * Determine if previous character exists
         * @return true if exists, false otherwise
         */
        public bool HasPreviousCharacter()
        {
            return characterPosition > 0;
        }

        /*
         * Advance to the next character
         * @param ExpectEnd expect end of stream (default: true)
         * @return next character
         */
        public char MoveNextCharacter(bool ExpectEnd = true)
        {
            int knownColumnLength;

            if (!HasNextCharacter())
            {
                throw new JSONException(JSONException.JSONExceptionType.NoNextCharacter);
            }

            if (GetCharacter() == (char)JSONDefines.JSONWhitespaceType.LineFeed)
            {

                if (!characterColumnLength.TryGetValue(characterRow, out knownColumnLength))
                {
                    characterColumnLength.Add(characterRow++, characterColumn);
                }
                characterColumn = 0;
            }
            else
            {
                ++characterColumn;
            }
            ++characterPosition;
            DetermineCharacterType();

            if (!ExpectEnd && !HasNextCharacter())
            {
                throw new JSONException(JSONException.JSONExceptionType.UnexpectedEndOfStream);
            }

            return GetCharacter();
        }

        /*
         * Retreate to the previous character
         * @return previous character
         */
        public char MovePreviousCharacter()
        {
            if (!HasPreviousCharacter())
            {
                throw new JSONException(JSONException.JSONExceptionType.NoPreviousCharacter);
            }
            --characterPosition;
            DetermineCharacterType();

            if (GetCharacter() == (char)JSONDefines.JSONWhitespaceType.LineFeed)
            {

                if (!characterColumnLength.TryGetValue(--characterRow, out characterColumn))
                {
                    throw new JSONException(
                        JSONException.JSONExceptionType.RowOutOfBounds,
                        "row. " + characterRow.ToString()
                        );
                }
            }
            else
            {
                --characterColumn;
            }

            return GetCharacter();
        }

        /*
         * Reset JSON lexer base
         */
        public void ResetCharacters()
        {
            characterColumn = 0;
            characterColumnLength.Clear();
            characterPosition = 0;
            characterRow = 0;
            DetermineCharacterType();
        }

        /*
         * Set JSON lexer base input
         * @param Input JSON data or file path
         * @param IsFile determine if input in a file path
         */
        public void SetCharacters(string Input, bool IsFile)
        {
            StreamReader stream;

            ClearCharacters();
            input = Input;

            if (IsFile)
            {
                path = Input;

                try
                {
                    stream = new StreamReader(Input);
                    input = stream.ReadToEnd();
                    stream.Close();
                }
                catch (Exception exception)
                {
                    throw new JSONException(
                        JSONException.JSONExceptionType.FileException,
                        exception.Message,
                        exception
                        );
                }
            }
            else
            {
                path = string.Empty;
            }
            input += (char)JSONDefines.JSONWhitespaceType.EndOfFile;
            DetermineCharacterType();
        }

        /*
         * JSON lexer base string representation
         * @return lexer base string representation
         */
        public override string ToString()
        {
            char currentCharacter = GetCharacter();
            StringBuilder stream = new StringBuilder();

#if DEBUG
            stream.Append("[" + characterType.ToString() + "] ");
#endif // DEBUG

            if (characterType != JSONCharacterType.Whitespace)
            {
                stream.Append("\'" + currentCharacter + "\'");
            }
            stream.Append(" (col. " + characterColumn + ", row. " + characterRow + ", pos. "
                + characterPosition + ")");

            return stream.ToString();
        }

        /*
         * Current character column
         */
        public int CharacterColumn
        {
            get { return characterColumn; }
        }

        /*
         * Current character position
         */
        public int CharacterPosition
        {
            get { return characterPosition; }
        }

        /*
         * Current character row
         */
        public int CharacterRow
        {
            get { return characterRow; }
        }

        /*
         * Current character type
         */
        public JSONCharacterType CharacterType
        {
            get { return characterType; }
        }

        /*
         * JSON data
         */
        public string Input
        {
            get { return input; }
        }

        /*
         * JSON data path
         */
        public string Path
        {
            get { return path; }
        }

        private int characterColumn;
        private Dictionary<int, int> characterColumnLength;
        private int characterPosition;
        private int characterRow;
        private JSONCharacterType characterType;
        private string input;
        private string path;
    }

    /*
     * JSON lexer class
     * ----------------
     * Enumerate a series of tokens
     */
    class JSONLexer : JSONLexerBase
    {
        /*
         * JSON lexer constructor
         */
        public JSONLexer()
        {
            tokenList = new List<Guid>();
            tokenMap = new Dictionary<Guid, JSONToken>();
            ClearTokens();
        }

        /*
         * JSON lexer constructor
         * @param Input JSON data or file path
         * @param IsFile determine if input in a file path
         */
        public JSONLexer(string Input, bool IsFile)
        {
            tokenList = new List<Guid>();
            tokenMap = new Dictionary<Guid, JSONToken>();
            SetTokens(Input, IsFile);
        }

        /*
         * Add lexer token
         * @param Token token object
         * @return token unique identifier
         */
        private Guid AddToken(JSONToken Token)
        {
            JSONToken testToken;

            if (tokenMap.TryGetValue(Token.UniqueID, out testToken))
            {
                throw new JSONException(
                    JSONException.JSONExceptionType.TokenAlreadyExists
#if DEBUG
                    , "{" + Token.UniqueID.ToString() + "}"
#endif // DEBUG
                    );
            }
            tokenList.Add(Token.UniqueID);
            tokenMap.Add(Token.UniqueID, Token);

            return Token.UniqueID;
        }

        /*
         * Add lexer token
         * @param Text token text
         * @return token unique identifier
         */
        private Guid AddToken(string Text)
        {
            JSONToken newToken = new JSONToken(Text);

            tokenList.Add(newToken.UniqueID);
            tokenMap.Add(newToken.UniqueID, newToken);

            return newToken.UniqueID;
        }

        /*
         * Add lexer token
         * @param Text token text
         * @param Type token type
         * @param SubType token subtype (default: null)
         * @return token unique identifier
         */
        private Guid AddToken(string Text, JSONToken.JSONTokenType Type, object SubType = null)
        {
            JSONToken newToken = new JSONToken(Text, Type, SubType);

            tokenList.Add(newToken.UniqueID);
            tokenMap.Add(newToken.UniqueID, newToken);

            return newToken.UniqueID;
        }

        /*
         * Advance lexer base character
         * @param ExpectEnd expect end of stream (default: true)
         * @return next character in stream
         */
        private char AdvanceCharacter(bool ExpectEnd = true)
        {
            char nextCharacter = (char)JSONDefines.JSONWhitespaceType.EndOfFile;

            if (HasNextCharacter())
            {
                nextCharacter = MoveNextCharacter();
            }
            else if (!ExpectEnd)
            {
                throw new JSONException(JSONException.JSONExceptionType.UnexpectedEndOfStream);
            }

            return nextCharacter;
        }

        /*
         * Clear JSON lexer
         */
        public void ClearTokens()
        {
            ResetCharacters();
            tokenPosition = 0;
            tokenList.Clear();
            tokenMap.Clear();
            AddToken(string.Empty, JSONToken.JSONTokenType.Begin);
            AddToken(string.Empty, JSONToken.JSONTokenType.End);
        }

        /*
         * Discover all lexer tokens
         */
        public int DiscoverTokens()
        {
            ClearTokens();

            while (HasNextToken())
            {
                MoveNextToken();
            }
            ResetTokens();

            return tokenList.Count;
        }

        /*
         * Enumerate boolean token
         * @return boolean token
         */
        private JSONToken EnumerateBoolean()
        {
            JSONToken newToken = new JSONToken(string.Empty, JSONToken.JSONTokenType.Boolean);

            newToken.Text += GetCharacter();
            newToken.Line = CharacterRow;
            newToken.Source = Path;

            while (HasNextToken())
            {
                AdvanceCharacter();

                if (CharacterType != JSONCharacterType.Alpha)
                {
                    break;
                }
                newToken.Text += GetCharacter();
            }
            newToken.SubType = newToken.Text;

            if (!newToken.Text.Equals("false")
                && !newToken.Text.Equals("true"))
            {
                throw new JSONException(
                    JSONException.JSONExceptionType.ExpectingBoolean,
                    "\'" + newToken.Text + "\'"
                    );
            }
            newToken.Text = string.Empty;

            return newToken;
        }

        /*
         * Enumerate number token
         * @return number token
         */
        private JSONToken EnumerateNumber()
        {
            char currentCharacter = GetCharacter();
            JSONToken newToken = new JSONToken(string.Empty, JSONToken.JSONTokenType.Number);

            newToken.Text += currentCharacter;
            newToken.Line = CharacterRow;
            newToken.Source = Path;

            while (HasNextToken())
            {
                currentCharacter = AdvanceCharacter();

                if (CharacterType != JSONCharacterType.Digit)
                {
                    break;
                }

                newToken.Text += currentCharacter;
            }
            currentCharacter = GetCharacter();

            if (GetCharacter() == (char)JSONDefines.JSONSymbolType.Decimal)
            {
                newToken.Text += currentCharacter;

                while (HasNextToken())
                {
                    currentCharacter = AdvanceCharacter();

                    if (CharacterType != JSONCharacterType.Digit)
                    {
                        break;
                    }
                    newToken.Text += currentCharacter;
                }
            }

            return newToken;
        }

        /*
         * Enumerate string token
         * @param CaptureDelimiters include string delimiter characters in token text (default: false)
         * @return string token
         */
        private JSONToken EnumerateString(bool CaptureDelimiters = false)
        {
            bool closed = false;
            char currentCharacter = GetCharacter();
            string startCharacter = base.ToString();
            JSONToken newToken = new JSONToken(string.Empty, JSONToken.JSONTokenType.String);

            newToken.Line = CharacterRow;
            newToken.Source = Path;

            if (CaptureDelimiters)
            {
                newToken.Text += currentCharacter;
            }

            while (HasNextCharacter())
            {
                currentCharacter = AdvanceCharacter();

                if (currentCharacter == (char)JSONDefines.JSONSymbolType.StringDelimiter)
                {

                    if (CaptureDelimiters)
                    {
                        newToken.Text += currentCharacter;
                    }
                    closed = true;
                    break;
                }
                newToken.Text += currentCharacter;
            }

            if (!closed)
            {
                throw new JSONException(
                    JSONException.JSONExceptionType.UnterminatedString,
                    startCharacter
                    );
            }
            AdvanceCharacter();

            return newToken;
        }

        /*
         * Enumerate symbol token
         * @return symbol token
         */
        private JSONToken EnumerateSymbol()
        {
            JSONToken newToken = new JSONToken(string.Empty, JSONToken.JSONTokenType.Symbol, GetCharacter());

            newToken.Line = CharacterRow;
            newToken.Source = Path;

            switch (GetCharacter())
            {
                case (char)JSONDefines.JSONSymbolType.ArrayClose:
                case (char)JSONDefines.JSONSymbolType.ArrayOpen:
                case (char)JSONDefines.JSONSymbolType.ObjectClose:
                case (char)JSONDefines.JSONSymbolType.ObjectOpen:
                case (char)JSONDefines.JSONSymbolType.PairDelimiter:
                case (char)JSONDefines.JSONSymbolType.PairSeperator:
                    AdvanceCharacter();
                    break;
                default:
                    throw new JSONException(
                        JSONException.JSONExceptionType.ExpectingSymbol,
                        base.ToString()
                        );
            }

            return newToken;
        }

        /*
         * Enumerate token
         * @return token
         */
        private JSONToken EnumerateToken()
        {
            JSONToken newToken;

            switch (CharacterType)
            {
                case JSONCharacterType.Alpha:
                    newToken = EnumerateBoolean();
                    break;
                case JSONCharacterType.Digit:
                    newToken = EnumerateNumber();
                    break;
                case JSONCharacterType.Symbol:

                    switch (GetCharacter())
                    {
                        case (char)JSONDefines.JSONSymbolType.StringDelimiter:
                            newToken = EnumerateString();
                            break;
                        default:
                            newToken = EnumerateSymbol();
                            break;
                    }
                    break;
                default:
                    throw new JSONException(
                        JSONException.JSONExceptionType.ExpectingToken,
                        base.ToString()
                        );
            }

            return newToken;
        }

        /*
         * Retrieve token with specified unique identifier
         * @param uniqueID token unique identrifier
         * @return token with specified unique identifier
         */
        private JSONToken FindToken(Guid UniqueID)
        {
            JSONToken token;

            if (!tokenMap.TryGetValue(UniqueID, out token))
            {
                throw new JSONException(
                    JSONException.JSONExceptionType.TokenNotFound
#if DEBUG
                    , "{" + UniqueID.ToString() + "}"
#endif // DEBUG
                    );
            }

            return token;
        }

        /*
         * Retrieve begin sentinel token
         * @return begin sentinel token
         */
        public JSONToken GetBeginToken()
        {
            return GetToken(0);
        }

        /*
         * Retrieve end sentinel token
         * @return end sentinel token
         */
        public JSONToken GetEndToken()
        {
            return GetToken(tokenList.Count - 1);
        }

        /*
         * Retrieve token at current position
         * @return token at current position
         */
        public JSONToken GetToken()
        {
            return GetToken(tokenPosition);
        }

        /*
         * Retrieve token at a specified position
         * @param Position token position (0 <= Position < TokenList.Length)
         * @return token at a specified position
         */
        public JSONToken GetToken(int Position)
        {
            if ((Position < 0) || (Position >= tokenList.Count))
            {
                throw new JSONException(
                    JSONException.JSONExceptionType.TokenOutOfBounds,
                    "pos. " + Position.ToString()
                    );
            }

            return FindToken(tokenList[Position]);
        }

        /*
         * Determine if next token exists
         * @return true if exists, false otherwise
         */
        public bool HasNextToken()
        {
            return GetToken().Type != JSONToken.JSONTokenType.End;
        }

        /*
         * Determine if previous token exists
         * @return true if exists, false otherwise
         */
        public bool HasPreviousToken()
        {
            return tokenPosition > 0;
        }

        /*
         * Insert lexer token
         * @param Token token object
         * @return token unique identifier
         */
        private Guid InsertToken(JSONToken Token)
        {
            JSONToken testToken;

            if (tokenMap.TryGetValue(Token.UniqueID, out testToken))
            {
                throw new JSONException(
                    JSONException.JSONExceptionType.TokenAlreadyExists
#if DEBUG
                    , "{" + Token.UniqueID.ToString() + "}"
#endif // DEBUG
                    );
            }
            tokenList.Insert(tokenPosition + 1, Token.UniqueID);
            tokenMap.Add(Token.UniqueID, Token);

            return Token.UniqueID;
        }

        /*
         * Insert lexer token
         * @param Text token text
         * @return token unique identifier
         */
        private Guid InsertToken(string Text)
        {
            JSONToken newToken = new JSONToken(Text);

            tokenList.Insert(tokenPosition + 1, newToken.UniqueID);
            tokenMap.Add(newToken.UniqueID, newToken);

            return newToken.UniqueID;
        }

        /*
         * Insert lexer token
         * @param Text token text
         * @param Type token type
         * @param SubType token subtype (default: null)
         * @return token unique identifier
         */
        private Guid InsertToken(string Text, JSONToken.JSONTokenType Type, object SubType = null)
        {
            JSONToken newToken = new JSONToken(Text, Type, SubType);

            tokenList.Insert(tokenPosition + 1, newToken.UniqueID);
            tokenMap.Add(newToken.UniqueID, newToken);

            return newToken.UniqueID;
        }

        /*
         * Advance to the next token
         * @param ExpectEnd expect end of stream (default: true)
         * @return next token
         */
        public JSONToken MoveNextToken(bool ExpectEnd = true)
        {
            if (!HasNextToken())
            {
                throw new JSONException(JSONException.JSONExceptionType.NoNextToken);
            }

            if (HasNextCharacter() && (tokenPosition <= (tokenList.Count - 2)))
            {

                while (HasNextCharacter() && (CharacterType == JSONCharacterType.Whitespace))
                {
                    MoveNextCharacter();
                }
                InsertToken(EnumerateToken());
            }
            ++tokenPosition;

            if (!ExpectEnd && !HasNextToken())
            {
                throw new JSONException(JSONException.JSONExceptionType.UnexpectedEndOfStream);
            }

            return GetToken();
        }

        /*
         * Retreate to the previous token
         * @return previous token
         */
        public JSONToken MovePreviousToken()
        {
            if (!HasPreviousToken())
            {
                throw new JSONException(JSONException.JSONExceptionType.NoPreviousToken);
            }
            --tokenPosition;

            return GetToken();
        }

        /*
         * Reset JSON lexer
         */
        public void ResetTokens()
        {
            tokenPosition = 0;
        }

        /*
         * Retreat lexer base character
         * @param ExpectEnd expect end of stream (default: true)
         * @return previous character in stream
         */
        private char RetreatCharacter()
        {
            char previousCharacter = (char)JSONDefines.JSONWhitespaceType.EndOfFile;

            if (HasPreviousCharacter())
            {
                previousCharacter = MovePreviousCharacter();
            }

            return previousCharacter;
        }

        /*
         * Set JSON lexer input
         * @param Input JSON data or file path
         * @param IsFile determine if input in a file path
         */
        public void SetTokens(string Input, bool IsFile)
        {
            SetCharacters(Input, IsFile);
            ClearTokens();
        }

        /*
         * JSON lexer string representation
         * @return lexer string representation
         */
        public override string ToString()
        {
            return GetToken().ToString();
        }

        /*
         * Current token position
         */
        public int TokenPosition
        {
            get { return tokenPosition; }
        }

        /*
         * Current token list
         */
        public List<Guid> TokenList
        {
            get { return tokenList; }
        }

        /*
         * Current token map
         */
        public Dictionary<Guid, JSONToken> TokenMap
        {
            get { return tokenMap; }
        }

        private int tokenPosition;
        private List<Guid> tokenList;
        private Dictionary<Guid, JSONToken> tokenMap;
    }
}
