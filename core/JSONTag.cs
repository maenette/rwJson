/*
 * JSONTag.cs
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
     * JSON Tag types
     */
    public enum JSONTagType
    {
        Array = 0,               // Array tag type
        Boolean,                 // Boolean tag type
        Number,                  // Number (double) tag type
        Object,                  // Object tag type
        String,                  // String tag type
    }

    /*
     * JSON tag interface
     * ------------------
     * Holds JSON tag key and type information.
     */
    public interface IJSONTag
    {

        /*
         * Array indexer overload
         * @param Index array index
         */
        IJSONTag this[int Index] { get; set; }

        /*
         * Object indexer overload
         * @param Key key string
         */
        IJSONTag this[string Key] { get; set; }

        /*
         * Retrieve array tag value
         * @return array tag value
         */
        JSONArrayTag AsArray();

        /*
         * Retrieve boolean tag value
         * @return boolean tag value
         */
        bool AsBoolean();

        /*
         * Retrieve number tag value
         * @return number tag value
         */
        float AsFloat();

        /*
         * Retrieve number tag value
         * @return number tag value
         */
        int AsInteger();

        /*
         * Retrieve object tag value
         * @return object tag value
         */
        JSONObjectTag AsObject();

        /*
         * Retrieve string tag value
         * @return string tag value
         */
        string AsString();

        /*
         * Retrieve tag key string
         * @return tag key string
         */
        string GetKey();

        /*
         * Retrieve tag type
         * @return tag type
         */
        JSONTagType GetTagType();

        /*
         * Serialization routine
         * @param Tab JSON syntax tab amount (default: 0)
         * @return JSON style string representing a tag
         */
        string Serialize(uint Tab = 0);

        /*
         * Set tag key string
         * @param Key tag key string
         */
        void SetKey(string Key);
    }

    /*
     * JSON array tag class
     * --------------------
     * Holds an array of JSON types (not always of the same type)
     * Syntax "<key>" : [ <pair_list> ]
     */
    public class JSONArrayTag : List<IJSONTag>, IJSONTag
    {

        /*
         * JSON array tag constructor
         * @param Key tag key string
         */
        public JSONArrayTag(string Key)
        {
            key = Key;
            type = JSONTagType.Array;
        }

        /*
         * JSON array tag constructor
         * @param Key tag key string
         * @param Value tag type
         */
        public JSONArrayTag(string Key, List<IJSONTag> Value)
        {
            key = Key;
            type = JSONTagType.Array;

            foreach (IJSONTag tag in Value)
            {
                Add(tag);
            }
        }

        /*
         * Object indexer overload
         * @param Key key string
         */
        public IJSONTag this[string Key]
        {
            get { 
                throw new JSONException(
                    JSONException.JSONExceptionType.NotTypeObject, 
                    type.ToString()
                    ); 
            }

            set { 
                throw new JSONException(
                    JSONException.JSONExceptionType.NotTypeObject, 
                    type.ToString()
                    ); 
            }
        }

        /*
         * Retrieve array tag value
         * @return array tag value
         */
        public JSONArrayTag AsArray()
        {
            return this;
        }

        /*
         * Retrieve boolean tag value
         * @return boolean tag value
         */
        public bool AsBoolean()
        {
            throw new JSONException(
                JSONException.JSONExceptionType.NotTypeBoolean,
                type.ToString()
                );
        }

        /*
         * Retrieve number tag value
         * @return number tag value
         */
        public float AsFloat()
        {
            throw new JSONException(
                JSONException.JSONExceptionType.NotTypeNumber,
                type.ToString()
                );
        }

        /*
         * Retrieve number tag value
         * @return number tag value
         */
        public int AsInteger()
        {
            throw new JSONException(
                JSONException.JSONExceptionType.NotTypeNumber,
                type.ToString()
                );
        }

        /*
         * Retrieve object tag value
         * @return object tag value
         */
        public JSONObjectTag AsObject()
        {
            throw new JSONException(
                JSONException.JSONExceptionType.NotTypeObject,
                type.ToString()
                );
        }

        /*
         * Retrieve string tag value
         * @return string tag value
         */
        public string AsString()
        {
            throw new JSONException(
                JSONException.JSONExceptionType.NotTypeString,
                type.ToString()
                );
        }

        /*
         * Retrieve tag key string
         * @return tag key string
         */
        public string GetKey()
        {
            return key;
        }

        /*
         * Retrieve tag type
         * @return tag type
         */
        public JSONTagType GetTagType()
        {
            return type;
        }

        /*
         * Serialization routine
         * @param Tab JSON syntax tab amount (default: 0)
         * @return JSON style string representing a tag
         */
        public string Serialize(uint Tab = 0)
        {
            uint tabIter;
            bool firstIter = true;
            StringBuilder stream = new StringBuilder();

            for (tabIter = 0; tabIter < Tab; ++tabIter)
            {
                stream.Append((char)JSONDefines.JSONWhitespaceType.Tab);
            }

            if (key.Length > 0)
            {
                stream.Append((char)JSONDefines.JSONSymbolType.StringDelimiter + key + (char)JSONDefines.JSONSymbolType.StringDelimiter
                    + (char)JSONDefines.JSONSymbolType.PairSeperator + (char)JSONDefines.JSONWhitespaceType.Space);
            }
            stream.Append((char)JSONDefines.JSONSymbolType.ArrayOpen);

            if (Count > 0)
            {
                stream.Append((char)JSONDefines.JSONWhitespaceType.LineFeed);
            }

            foreach (IJSONTag tag in this)
            {
                if (!firstIter)
                {
                    stream.Append((char)JSONDefines.JSONSymbolType.PairDelimiter).Append((char)JSONDefines.JSONWhitespaceType.LineFeed);
                }
                else
                {
                    firstIter = false;
                }
                stream.Append(tag.Serialize(Tab + 1));
            }

            if (Count > 0)
            {
                stream.Append((char)JSONDefines.JSONWhitespaceType.LineFeed);

                for (tabIter = 0; tabIter < Tab; ++tabIter)
                {
                    stream.Append((char)JSONDefines.JSONWhitespaceType.Tab);
                }
            }
            stream.Append((char)JSONDefines.JSONSymbolType.ArrayClose);

            return stream.ToString();
        }

        /*
         * Set tag key string
         * @param Key tag key string
         */
        public void SetKey(string Key)
        {
            key = Key;
        }

        /*
         * JSON array tag string representation
         * @return array tag string representation
         */
        public override string ToString()
        {
            return Serialize();
        }

        /*
         * Tag key
         */
        public string Key
        {
            get { return key; }
            set { key = value; }
        }

        /*
         * Tag type
         */
        public JSONTagType Type
        {
            get { return type; }
        }

        private string key;
        private JSONTagType type;
    }

    /*
     * JSON boolean tag class
     * ----------------------
     * Holds a boolean value
     * Syntax "<key>" : (true|false)
     */
    public class JSONBooleanTag : IJSONTag
    {

        /*
         * JSON boolean tag constructor
         * @param Key tag key string
         */
        public JSONBooleanTag(string Key)
        {
            key = Key;
            type = JSONTagType.Boolean;
            objectValue = false;
        }

        /*
         * JSON boolean tag constructor
         * @param Key tag key string
         * @param Value tag type
         */
        public JSONBooleanTag(string Key, bool Value)
        {
            key = Key;
            type = JSONTagType.Boolean;
            objectValue = Value;
        }

        /*
         * Array indexer overload
         * @param Index array index
         */
        public IJSONTag this[int Index]
        {
            get
            {
                throw new JSONException(
                    JSONException.JSONExceptionType.NotTypeArray,
                    type.ToString()
                    );
            }

            set
            {
                throw new JSONException(
                    JSONException.JSONExceptionType.NotTypeArray,
                    type.ToString()
                    );
            }
        }

        /*
         * Object indexer overload
         * @param Key key string
         */
        public IJSONTag this[string Key]
        {
            get
            {
                throw new JSONException(
                    JSONException.JSONExceptionType.NotTypeObject,
                    type.ToString()
                    );
            }

            set
            {
                throw new JSONException(
                    JSONException.JSONExceptionType.NotTypeObject,
                    type.ToString()
                    );
            }
        }

        /*
         * Retrieve array tag value
         * @return array tag value
         */
        public JSONArrayTag AsArray()
        {
            throw new JSONException(
                JSONException.JSONExceptionType.NotTypeArray,
                type.ToString()
                );
        }

        /*
         * Retrieve boolean tag value
         * @return boolean tag value
         */
        public bool AsBoolean()
        {
            return this.Value;
        }

        /*
         * Retrieve number tag value
         * @return number tag value
         */
        public float AsFloat()
        {
            throw new JSONException(
                JSONException.JSONExceptionType.NotTypeNumber,
                type.ToString()
                );
        }

        /*
         * Retrieve number tag value
         * @return number tag value
         */
        public int AsInteger()
        {
            throw new JSONException(
                JSONException.JSONExceptionType.NotTypeNumber,
                type.ToString()
                );
        }

        /*
         * Retrieve object tag value
         * @return object tag value
         */
        public JSONObjectTag AsObject()
        {
            throw new JSONException(
                JSONException.JSONExceptionType.NotTypeObject,
                type.ToString()
                );
        }

        /*
         * Retrieve string tag value
         * @return string tag value
         */
        public string AsString()
        {
            throw new JSONException(
                JSONException.JSONExceptionType.NotTypeString,
                type.ToString()
                );
        }

        /*
         * Retrieve tag key string
         * @return tag key string
         */
        public string GetKey()
        {
            return key;
        }

        /*
         * Retrieve tag type
         * @return tag type
         */
        public JSONTagType GetTagType()
        {
            return type;
        }

        /*
         * Serialization routine
         * @param Tab JSON syntax tab amount (default: 0)
         * @return JSON style string representing a tag
         */
        public string Serialize(uint Tab = 0)
        {
            uint tabIter;
            StringBuilder stream = new StringBuilder();

            for (tabIter = 0; tabIter < Tab; ++tabIter)
            {
                stream.Append((char)JSONDefines.JSONWhitespaceType.Tab);
            }

            if (Key.Length > 0)
            {
                stream.Append((char)JSONDefines.JSONSymbolType.StringDelimiter + Key + (char)JSONDefines.JSONSymbolType.StringDelimiter
                    + (char)JSONDefines.JSONSymbolType.PairSeperator + (char)JSONDefines.JSONWhitespaceType.Space);
            }
            stream.Append(Value.ToString().ToLower());

            return stream.ToString();
        }

        /*
         * Set tag key string
         * @param Key tag key string
         */
        public void SetKey(string Key)
        {
            key = Key;
        }

        /*
         * JSON boolean tag string representation
         * @return boolean tag string representation
         */
        public override string ToString()
        {
            return Serialize();
        }

        /*
         * Tag key
         */
        public string Key
        {
            get { return key; }
            set { key = value; }
        }

        /*
         * Tag type
         */
        public JSONTagType Type
        {
            get { return type; }
        }

        /*
         * Tag value
         */
        public bool Value
        {
            get { return objectValue; }
            set { objectValue = value; }
        }

        private string key;
        private JSONTagType type;
        private bool objectValue;
    }

    /*
     * JSON number tag class
     * ---------------------
     * Holds a float value
     * Syntax "<key>" : <float>
     */
    public class JSONNumberTag : IJSONTag
    {

        /*
         * JSON number tag constructor
         * @param Key tag key string
         */
        public JSONNumberTag(string Key)
        {
            key = Key;
            type = JSONTagType.Number;
            objectValue = 0.0F;
        }

        /*
         * JSON number tag constructor
         * @param Key tag key string
         * @param Value tag type
         */
        public JSONNumberTag(string Key, float Value)
        {
            key = Key;
            type = JSONTagType.Number;
            objectValue = Value;
        }

        /*
         * Array indexer overload
         * @param Index array index
         */
        public IJSONTag this[int Index]
        {
            get
            {
                throw new JSONException(
                    JSONException.JSONExceptionType.NotTypeArray,
                    type.ToString()
                    );
            }

            set
            {
                throw new JSONException(
                    JSONException.JSONExceptionType.NotTypeArray,
                    type.ToString()
                    );
            }
        }

        /*
         * Object indexer overload
         * @param Key key string
         */
        public IJSONTag this[string Key]
        {
            get
            {
                throw new JSONException(
                    JSONException.JSONExceptionType.NotTypeObject,
                    type.ToString()
                    );
            }

            set
            {
                throw new JSONException(
                    JSONException.JSONExceptionType.NotTypeObject,
                    type.ToString()
                    );
            }
        }

        /*
         * Retrieve array tag value
         * @return array tag value
         */
        public JSONArrayTag AsArray()
        {
            throw new JSONException(
                JSONException.JSONExceptionType.NotTypeArray,
                type.ToString()
                );
        }

        /*
         * Retrieve boolean tag value
         * @return boolean tag value
         */
        public bool AsBoolean()
        {
            throw new JSONException(
                JSONException.JSONExceptionType.NotTypeBoolean,
                type.ToString()
                );
        }

        /*
         * Retrieve number tag value
         * @return number tag value
         */
        public float AsFloat()
        {
            return this.Value;
        }

        /*
         * Retrieve number tag value
         * @return number tag value
         */
        public int AsInteger()
        {
            return (int)this.Value;
        }

        /*
         * Retrieve object tag value
         * @return object tag value
         */
        public JSONObjectTag AsObject()
        {
            throw new JSONException(
                JSONException.JSONExceptionType.NotTypeObject,
                type.ToString()
                );
        }

        /*
         * Retrieve string tag value
         * @return string tag value
         */
        public string AsString()
        {
            throw new JSONException(
                JSONException.JSONExceptionType.NotTypeString,
                type.ToString()
                );
        }

        /*
         * Retrieve tag key string
         * @return tag key string
         */
        public string GetKey()
        {
            return key;
        }

        /*
         * Retrieve tag type
         * @return tag type
         */
        public JSONTagType GetTagType()
        {
            return type;
        }

        /*
         * Serialization routine
         * @param Tab JSON syntax tab amount (default: 0)
         * @return JSON style string representing a tag
         */
        public string Serialize(uint Tab = 0)
        {
            uint tabIter;
            StringBuilder stream = new StringBuilder();

            for (tabIter = 0; tabIter < Tab; ++tabIter)
            {
                stream.Append((char)JSONDefines.JSONWhitespaceType.Tab);
            }

            if (Key.Length > 0)
            {
                stream.Append((char)JSONDefines.JSONSymbolType.StringDelimiter + Key + (char)JSONDefines.JSONSymbolType.StringDelimiter
                    + (char)JSONDefines.JSONSymbolType.PairSeperator + (char)JSONDefines.JSONWhitespaceType.Space);
            }
            stream.Append(Value);

            return stream.ToString();
        }

        /*
         * Set tag key string
         * @param Key tag key string
         */
        public void SetKey(string Key)
        {
            key = Key;
        }

        /*
         * JSON number tag string representation
         * @return number tag string representation
         */
        public override string ToString()
        {
            return Serialize();
        }

        /*
         * Tag key
         */
        public string Key
        {
            get { return key; }
            set { key = value; }
        }

        /*
         * Tag type
         */
        public JSONTagType Type
        {
            get { return type; }
        }

        /*
         * Tag value
         */
        public float Value
        {
            get { return objectValue; }
            set { objectValue = value; }
        }

        private string key;
        private JSONTagType type;
        private float objectValue;
    }

    /*
     * JSON object tag class
     * ---------------------
     * Holds an object value
     * Syntax "<key>" : { <pair_list> }
     */
    public class JSONObjectTag : Dictionary<string, IJSONTag>, IJSONTag
    {

        /*
         * JSON object tag constructor
         * @param Key tag key string
         */
        public JSONObjectTag(string Key)
        {
            key = Key;
            type = JSONTagType.Object;
        }

        /*
         * JSON object tag constructor
         * @param Key tag key string
         * @param Value tag type
         */
        public JSONObjectTag(string Key, Dictionary<string, IJSONTag> Value)
        {
            key = Key;
            type = JSONTagType.Object;

            foreach (KeyValuePair<string, IJSONTag> tag in Value)
            {
                Add(tag.Key, tag.Value);
            }
        }

        /*
         * Array indexer overload
         * @param Index array index
         */
        public IJSONTag this[int Index]
        {
            get
            {
                throw new JSONException(
                    JSONException.JSONExceptionType.NotTypeArray,
                    type.ToString()
                    );
            }

            set
            {
                throw new JSONException(
                    JSONException.JSONExceptionType.NotTypeArray,
                    type.ToString()
                    );
            }
        }

        /*
         * Retrieve array tag value
         * @return array tag value
         */
        public JSONArrayTag AsArray()
        {
            throw new JSONException(
                JSONException.JSONExceptionType.NotTypeArray,
                type.ToString()
                );
        }

        /*
         * Retrieve boolean tag value
         * @return boolean tag value
         */
        public bool AsBoolean()
        {
            throw new JSONException(
                JSONException.JSONExceptionType.NotTypeBoolean,
                type.ToString()
                );
        }

        /*
         * Retrieve number tag value
         * @return number tag value
         */
        public float AsFloat()
        {
            throw new JSONException(
                JSONException.JSONExceptionType.NotTypeNumber,
                type.ToString()
                );
        }

        /*
         * Retrieve number tag value
         * @return number tag value
         */
        public int AsInteger()
        {
            throw new JSONException(
                JSONException.JSONExceptionType.NotTypeNumber,
                type.ToString()
                );
        }

        /*
         * Retrieve object tag value
         * @return object tag value
         */
        public JSONObjectTag AsObject()
        {
            return this;
        }

        /*
         * Retrieve string tag value
         * @return string tag value
         */
        public string AsString()
        {
            throw new JSONException(
                JSONException.JSONExceptionType.NotTypeString,
                type.ToString()
                );
        }

        /*
         * Retrieve tag key string
         * @return tag key string
         */
        public string GetKey()
        {
            return key;
        }

        /*
         * Retrieve tag type
         * @return tag type
         */
        public JSONTagType GetTagType()
        {
            return type;
        }

        /*
         * Serialization routine
         * @param Tab JSON syntax tab amount (default: 0)
         * @return JSON style string representing a tag
         */
        public string Serialize(uint Tab = 0)
        {
            uint tabIter;
            bool firstIter = true;
            StringBuilder stream = new StringBuilder();

            for (tabIter = 0; tabIter < Tab; ++tabIter)
            {
                stream.Append((char)JSONDefines.JSONWhitespaceType.Tab);
            }

            if (Key.Length > 0)
            {
                stream.Append((char)JSONDefines.JSONSymbolType.StringDelimiter + Key + (char)JSONDefines.JSONSymbolType.StringDelimiter
                    + (char)JSONDefines.JSONSymbolType.PairSeperator + (char)JSONDefines.JSONWhitespaceType.Space);
            }
            stream.Append((char)JSONDefines.JSONSymbolType.ObjectOpen);

            if (Count > 0)
            {
                stream.Append((char)JSONDefines.JSONWhitespaceType.LineFeed);
            }

            foreach (KeyValuePair<string, IJSONTag> tag in this)
            {
                if (!firstIter)
                {
                    stream.Append((char)JSONDefines.JSONSymbolType.PairDelimiter).Append((char)JSONDefines.JSONWhitespaceType.LineFeed);
                }
                else
                {
                    firstIter = false;
                }
                stream.Append(tag.Value.Serialize(Tab + 1));
            }

            if (Count > 0)
            {
                stream.Append((char)JSONDefines.JSONWhitespaceType.LineFeed);

                for (tabIter = 0; tabIter < Tab; ++tabIter)
                {
                    stream.Append((char)JSONDefines.JSONWhitespaceType.Tab);
                }
            }
            stream.Append((char)JSONDefines.JSONSymbolType.ObjectClose);

            return stream.ToString();
        }

        /*
         * Set tag key string
         * @param Key tag key string
         */
        public void SetKey(string Key)
        {
            key = Key;
        }

        /*
         * JSON object tag string representation
         * @return object tag string representation
         */
        public override string ToString()
        {
            return Serialize();
        }

        /*
         * Tag key
         */
        public string Key
        {
            get { return key; }
            set { key = value; }
        }

        /*
         * Tag type
         */
        public JSONTagType Type
        {
            get { return type; }
        }

        private string key;
        private JSONTagType type;
    }

    /*
     * JSON string tag class
     * ---------------------
     * Holds a string value
     * Syntax "<key>" : "<value>"
     */
    public class JSONStringTag : IJSONTag
    {

        /*
         * JSON string tag constructor
         * @param Key tag key string
         */
        public JSONStringTag(string Key)
        {
            key = Key;
            type = JSONTagType.String;
            objectValue = string.Empty;
        }

        /*
         * JSON string tag constructor
         * @param Key tag key string
         * @param Value tag type
         */
        public JSONStringTag(string Key, string Value)
        {
            key = Key;
            type = JSONTagType.String;
            objectValue = Value;
        }

        /*
         * Array indexer overload
         * @param Index array index
         */
        public IJSONTag this[int Index]
        {
            get
            {
                throw new JSONException(
                    JSONException.JSONExceptionType.NotTypeArray,
                    type.ToString()
                    );
            }

            set
            {
                throw new JSONException(
                    JSONException.JSONExceptionType.NotTypeArray,
                    type.ToString()
                    );
            }
        }

        /*
         * Object indexer overload
         * @param Key key string
         */
        public IJSONTag this[string Key]
        {
            get
            {
                throw new JSONException(
                    JSONException.JSONExceptionType.NotTypeObject,
                    type.ToString()
                    );
            }

            set
            {
                throw new JSONException(
                    JSONException.JSONExceptionType.NotTypeObject,
                    type.ToString()
                    );
            }
        }

        /*
         * Retrieve array tag value
         * @return array tag value
         */
        public JSONArrayTag AsArray()
        {
            throw new JSONException(
                JSONException.JSONExceptionType.NotTypeArray,
                type.ToString()
                );
        }

        /*
         * Retrieve boolean tag value
         * @return boolean tag value
         */
        public bool AsBoolean()
        {
            throw new JSONException(
                JSONException.JSONExceptionType.NotTypeBoolean,
                type.ToString()
                );
        }

        /*
         * Retrieve number tag value
         * @return number tag value
         */
        public float AsFloat()
        {
            throw new JSONException(
                JSONException.JSONExceptionType.NotTypeNumber,
                type.ToString()
                );
        }

        /*
         * Retrieve number tag value
         * @return number tag value
         */
        public int AsInteger()
        {
            throw new JSONException(
                JSONException.JSONExceptionType.NotTypeNumber,
                type.ToString()
                );
        }

        /*
         * Retrieve object tag value
         * @return object tag value
         */
        public JSONObjectTag AsObject()
        {
            throw new JSONException(
                JSONException.JSONExceptionType.NotTypeObject,
                type.ToString()
                );
        }

        /*
         * Retrieve string tag value
         * @return string tag value
         */
        public string AsString()
        {
            return this.Value;
        }

        /*
         * Retrieve tag key string
         * @return tag key string
         */
        public string GetKey()
        {
            return key;
        }

        /*
         * Retrieve tag type
         * @return tag type
         */
        public JSONTagType GetTagType()
        {
            return type;
        }

        /*
         * Serialization routine
         * @param Tab JSON syntax tab amount (default: 0)
         * @return JSON style string representing a tag
         */
        public string Serialize(uint Tab = 0)
        {
            uint tabIter;
            StringBuilder stream = new StringBuilder();

            for (tabIter = 0; tabIter < Tab; ++tabIter)
            {
                stream.Append((char)JSONDefines.JSONWhitespaceType.Tab);
            }

            if (Key.Length > 0)
            {
                stream.Append((char)JSONDefines.JSONSymbolType.StringDelimiter + Key + (char)JSONDefines.JSONSymbolType.StringDelimiter
                    + (char)JSONDefines.JSONSymbolType.PairSeperator + (char)JSONDefines.JSONWhitespaceType.Space);
            }
            stream.Append((char)JSONDefines.JSONSymbolType.StringDelimiter).Append(Value).Append((char)JSONDefines.JSONSymbolType.StringDelimiter);

            return stream.ToString();
        }

        /*
         * Set tag key string
         * @param Key tag key string
         */
        public void SetKey(string Key)
        {
            key = Key;
        }

        /*
         * JSON string tag string representation
         * @return string tag string representation
         */
        public override string ToString()
        {
            return Serialize();
        }

        /*
         * Tag key
         */
        public string Key
        {
            get { return key; }
            set { key = value; }
        }

        /*
         * Tag type
         */
        public JSONTagType Type
        {
            get { return type; }
        }

        /*
         * Tag value
         */
        public string Value
        {
            get { return objectValue; }
            set { objectValue = value; }
        }

        private string key;
        private JSONTagType type;
        private string objectValue;
    }
}
