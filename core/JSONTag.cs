/*
 * JSONTag.cs
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
using System.Text;

namespace JSON
{

    /*
     * JSON abstract tag class
     * -----------------------
     * Holds JSON tag key and type information. All tags derive
     * from this class.
     */
    public abstract class JSONTag
    {
        /* 
         * Tag types
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
         * JSON tag constructor
         * @param Key tag key string
         * @param Type tag type
         */
        public JSONTag(string Key, JSONTagType Type)
        {
            key = Key;
            type = Type;
        }

        /*
         * Abstract serialization routine (required by all derivative classes)
         * @param Tab JSON syntax tab amount (default: 0)
         * @return JSON style string representing a tag
         */
        public abstract string Serialize(uint Tab = 0);

        /*
         * JSON tag string representation
         * @return tag string representation
         */
        public override string ToString()
        {
            StringBuilder stream = new StringBuilder();

            stream.Append("[" + type.ToString() + "] \"" + key + "\"");

            return stream.ToString();
        }

        /*
         * Tag key string
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
     * JSON array tag class
     * --------------------
     * Holds an array of JSON types (not always of the same type)
     * Syntax "<key>" : [ <pair_list> ]
     */
    public class JSONArrayTag : JSONTag
    {

        /*
         * JSON array tag constructor
         * @param Key tag key string
         */
        public JSONArrayTag(string Key)
            : base(Key, JSONTagType.Array)
        {
            objectList = new List<JSONTag>();
        }

        /*
         * JSON array tag constructor
         * @param Key tag key string
         * @param Value tag type
         */
        public JSONArrayTag(string Key, List<JSONTag> Value)
            : base(Key, JSONTagType.Array)
        {
            objectList = Value;
        }

        /*
         * Serialization routine
         * @param Tab JSON syntax tab amount (default: 0)
         * @return JSON style string representing a tag
         */
        public override string Serialize(uint Tab = 0)
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
            stream.Append((char)JSONDefines.JSONSymbolType.ArrayOpen);

            if (Value.Count > 0)
            {
                stream.Append((char)JSONDefines.JSONWhitespaceType.LineFeed);
            }

            foreach (JSONTag tag in Value)
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

            if (Value.Count > 0)
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
         * JSON array tag string representation
         * @return array tag string representation
         */
        public override string ToString()
        {
            return Serialize();
        }

        /*
         * Tag value
         */
        public List<JSONTag> Value
        {
            get { return objectList; }
            set { objectList = value; }
        }

        private List<JSONTag> objectList;
    }

    /*
     * JSON boolean tag class
     * ----------------------
     * Holds a boolean value
     * Syntax "<key>" : (true|false)
     */
    public class JSONBooleanTag : JSONTag
    {

        /*
         * JSON boolean tag constructor
         * @param Key tag key string
         */
        public JSONBooleanTag(string Key)
            : base(Key, JSONTagType.Boolean)
        {
            objectValue = false;
        }

        /*
         * JSON boolean tag constructor
         * @param Key tag key string
         * @param Value tag type
         */
        public JSONBooleanTag(string Key, bool Value)
            : base(Key, JSONTagType.Boolean)
        {
            objectValue = Value;
        }

        /*
         * Serialization routine
         * @param Tab JSON syntax tab amount (default: 0)
         * @return JSON style string representing a tag
         */
        public override string Serialize(uint Tab = 0)
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
            stream.Append((Value ? JSONDefines.BOOLTRUE : JSONDefines.BOOLFALSE));

            return stream.ToString();
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
         * Tag value
         */
        public bool Value
        {
            get { return objectValue; }
            set { objectValue = value; }
        }

        private bool objectValue;
    }

    /*
     * JSON number tag class
     * ---------------------
     * Holds a double value
     * Syntax "<key>" : <double>
     */
    public class JSONNumberTag : JSONTag
    {

        /*
         * JSON number tag constructor
         * @param Key tag key string
         */
        public JSONNumberTag(string Key)
            : base(Key, JSONTagType.Number)
        {
            objectValue = 0.0;
        }

        /*
         * JSON boolean tag constructor
         * @param Key tag key string
         * @param Value tag type
         */
        public JSONNumberTag(string Key, double Value)
            : base(Key, JSONTagType.Number)
        {
            objectValue = Value;
        }

        /*
         * Serialization routine
         * @param Tab JSON syntax tab amount (default: 0)
         * @return JSON style string representing a tag
         */
        public override string Serialize(uint Tab = 0)
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
         * JSON number tag string representation
         * @return number tag string representation
         */
        public override string ToString()
        {
            return Serialize();
        }

        /*
         * Tag value
         */
        public double Value
        {
            get { return objectValue; }
            set { objectValue = value; }
        }

        private double objectValue;
    }

    /*
     * JSON object tag class
     * ---------------------
     * Holds an object value
     * Syntax "<key>" : { <pair_list> }
     */
    public class JSONObjectTag : JSONTag
    {

        /*
         * JSON object tag constructor
         * @param Key tag key string
         */
        public JSONObjectTag(string Key)
            : base(Key, JSONTagType.Object)
        {
            objectMap = new Dictionary<string, JSONTag>();
        }

        /*
         * JSON object tag constructor
         * @param Key tag key string
         * @param Value tag type
         */
        public JSONObjectTag(string Key, Dictionary<string, JSONTag> Value)
            : base(Key, JSONTagType.Object)
        {
            objectMap = Value;
        }

        /*
         * Serialization routine
         * @param Tab JSON syntax tab amount (default: 0)
         * @return JSON style string representing a tag
         */
        public override string Serialize(uint Tab = 0)
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

            if (Value.Count > 0)
            {
                stream.Append((char)JSONDefines.JSONWhitespaceType.LineFeed);
            }

            foreach (KeyValuePair<string, JSONTag> tag in Value)
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

            if (Value.Count > 0)
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
         * JSON object tag string representation
         * @return object tag string representation
         */
        public override string ToString()
        {
            return Serialize();
        }

        /*
         * Tag value
         */
        public Dictionary<string, JSONTag> Value
        {
            get { return objectMap; }
            set { objectMap = value; }
        }

        private Dictionary<string, JSONTag> objectMap;
    }

    /*
     * JSON string tag class
     * ---------------------
     * Holds a string value
     * Syntax "<key>" : "<value>"
     */
    public class JSONStringTag : JSONTag
    {

        /*
         * JSON string tag constructor
         * @param Key tag key string
         */
        public JSONStringTag(string Key)
            : base(Key, JSONTagType.String)
        {
            return;
        }

        /*
         * JSON string tag constructor
         * @param Key tag key string
         * @param Value tag type
         */
        public JSONStringTag(string Key, string Value)
            : base(Key, JSONTagType.String)
        {
            objectValue = Value;
        }

        /*
         * Serialization routine
         * @param Tab JSON syntax tab amount (default: 0)
         * @return JSON style string representing a tag
         */
        public override string Serialize(uint Tab = 0)
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
         * JSON string tag string representation
         * @return string tag string representation
         */
        public override string ToString()
        {
            return Serialize();
        }

        /*
         * Tag value
         */
        public string Value
        {
            get { return objectValue; }
            set { objectValue = value; }
        }

        private string objectValue;
    }
}
