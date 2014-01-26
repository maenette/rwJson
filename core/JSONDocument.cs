/*
 * JSONDocument.cs
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
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace JSON
{

    /*
     * JSON document class
     * -------------------
     * Holds a root object tag dictionary, representing a JSON document
     */
    public class JSONDocument : Dictionary<string, JSONObjectTag>
    {

        /*
         * JSON document constructor
         * @param IsOverridable determine if input is overridable
         */
        public JSONDocument(bool IsOverridable)
        {
            isOverridable = IsOverridable;
        }

        /*
         * JSON document constructor
         * @param IsOverridable determine if input is overridable
         * @param Path JSON file path
         */
        public JSONDocument(bool IsOverridable, string Path)
        {
            isOverridable = IsOverridable;
            ReadAppend(ExtractKey(Path), Path, true);
        }

        /*
         * JSON document constructor
         * @param IsOverridable determine if input is overridable
         * @param Key JSON key string
         * @param Input JSON data or file path
         * @param IsFile determine if input in a file path
         */
        public JSONDocument(bool IsOverridable, string Key, string Input, bool IsFile)
        {
            isOverridable = IsOverridable;
            ReadAppend(Key, Input, IsFile);
        }

        /*
         * Add new JSON document object
         * @param Key JSON key string
         */
        public void AddNew(string Key)
        {
            ReadAppend(Key, string.Empty, false);
        }

        /*
         * Extract key from JSON file path
         * @param Input JSON file path
         * @return JSON file path
         */
        public static string ExtractKey(string Input)
        {
            Match keyMatch;
            string key = string.Empty;

            keyMatch = Regex.Match(Input, JSONDefines.KEY_PATTERN, RegexOptions.IgnoreCase);

            if (keyMatch.Success)
            {
                key = keyMatch.Groups[1].Value;
            }

            if (key.Equals(string.Empty))
            {
                throw new JSONException(JSONException.JSONExceptionType.InvalidKey, key);
            }

            return key;
        }

        /*
         * Extract root path from JSON file path
         * @param Input JSON file path
         * @return JSON file path
         */
        public static string ExtractRootPath(string Input)
        {
            Match rootPathMatch;
            string rootPath = string.Empty;

            rootPathMatch = Regex.Match(Input, JSONDefines.ROOT_PATH_PATTERN, RegexOptions.IgnoreCase);

            if (rootPathMatch.Success)
            {
                rootPath = rootPathMatch.Groups[1].Value;
            }

            return rootPath;
        }

        /*
         * Determine if schema tag type is optional
         * @param SchemaTag schema tag object
         * @return true if schema tag is optional
         */
        static private bool IsSchemaTagOptional(JSONObjectTag SchemaTag)
        {
            IJSONTag unknownTag;
            bool optional = false;

            if (SchemaTag.ContainsKey(JSONDefines.SCHEMA_TAG_OPTIONAL))
            {
                unknownTag = SchemaTag[JSONDefines.SCHEMA_TAG_OPTIONAL];

                if (unknownTag.GetTagType() != JSONTagType.Boolean)
                {
                    throw new JSONException(JSONException.JSONExceptionType.InvalidSchemaTagType, unknownTag.GetTagType().ToString());
                }
                optional = ((JSONBooleanTag)unknownTag).AsBoolean();
            }

            return optional;
        }

        /*
         * Determine if schema tag contains a wildcard
         * @param SchemaTag schema tag object
         * @return wildcard schema tag, or null
         */
        static private JSONObjectTag IsSchemaTagWildcard(JSONObjectTag SchemaTag)
        {
            IJSONTag unknownTag;
            JSONObjectTag wildcardTag = null;

            if (SchemaTag.ContainsKey(JSONDefines.SCHEMA_TAG_WILDCARD))
            {
                unknownTag = SchemaTag[JSONDefines.SCHEMA_TAG_WILDCARD];

                if (unknownTag.GetTagType() == JSONTagType.Object)
                {
                    wildcardTag = ((JSONObjectTag)unknownTag).AsObject();
                }
            }

            return wildcardTag;
        }

        /*
         * Read in JSON file
         * @param Path JSON file path
         */
        public void Read(string Path)
        {
            Read(ExtractKey(Path), Path, true);
        }

        /*
         * Read in JSON data or file
         * @param Key JSON key string
         * @param Input JSON data or file path
         * @param IsFile determine if input in a file path
         */
        public void Read(string Key, string Input, bool IsFile)
        {
            Clear();
            ReadAppend(Key, Input, IsFile);
        }

        /*
         * Read and append JSON file
         * @param Path JSON file path
         */
        public void ReadAppend(string Path)
        {
            ReadAppend(ExtractKey(Path), Path, true);
        }

        /*
         * Read and append JSON data or file
         * @param Key JSON key string
         * @param Input JSON data or file path
         * @param IsFile determine if input in a file path
         */
        public void ReadAppend(string Key, string Input, bool IsFile)
        {
            JSONObjectTag objectTag;

            if (TryGetValue(Key, out objectTag))
            {
                if (!isOverridable)
                {
                    throw new JSONException(JSONException.JSONExceptionType.KeyNotUnique, Key);
                }
                else
                {
                    Remove(Key);
                }
            }
            Add(Key, new JSONParser(Input, IsFile).RootTag);
        }

        /*
         * Retrieve first child tag from input tag
         * @param InputTag input JSON tag object
         * @return First child JSON tag object
         */
        static private IJSONTag RetrieveFirstChildTag(JSONObjectTag InputTag)
        {
            IJSONTag childTag;
            IEnumerator<KeyValuePair<string, IJSONTag>> inputTagEnum;

            inputTagEnum = InputTag.GetEnumerator();

            if ((InputTag.Count == 0) || (!inputTagEnum.MoveNext()))
            {
                throw new JSONException(JSONException.JSONExceptionType.InternalException, 
                    new JSONException(JSONException.JSONExceptionType.NoChildTagExists, InputTag.ToString()));
            }
            childTag = inputTagEnum.Current.Value;

            return childTag;
        }

        /*
         * Retrieve schema entry type
         * @param SchemaTag schema tag object
         * @return object tag object
         */
        static private JSONObjectTag RetrieveSchemaEntryTag(IJSONTag SchemaTag)
        {
            IJSONTag unknownTag;
            JSONObjectTag entryTag;

            try
            {
                unknownTag = SchemaTag[JSONDefines.SCHEMA_TAG_ENTRY];

                if (unknownTag.GetTagType() != JSONTagType.Object)
                {
                    throw new JSONException(JSONException.JSONExceptionType.InvalidSchemaTagType, unknownTag.GetTagType().ToString());
                }
                entryTag = (JSONObjectTag)unknownTag;
            }
            catch (Exception exception)
            {
                throw new JSONException(JSONException.JSONExceptionType.MissingRequiredSchemaTag, JSONDefines.SCHEMA_TAG_ENTRY, exception);
            }

            return entryTag;
        }

        static private JSONTagType RetrieveSchemaRequiredType(IJSONTag SchemaTag)
        {
            IJSONTag unknownTag;
            JSONStringTag entryTag;
            JSONTagType requiredType;

            try
            {
                unknownTag = SchemaTag[JSONDefines.SCHEMA_TAG_TYPE];

                if (unknownTag.GetTagType() != JSONTagType.String)
                {
                    throw new JSONException(JSONException.JSONExceptionType.InvalidSchemaTagType, unknownTag.GetTagType().ToString());
                }
                entryTag = (JSONStringTag)unknownTag;
            }
            catch (Exception exception)
            {
                throw new JSONException(JSONException.JSONExceptionType.MissingRequiredSchemaTag, JSONDefines.SCHEMA_TAG_TYPE, exception);
            }

            switch (entryTag.Value)
            {
                case JSONDefines.SCHEMA_TAG_ARRAY:
                    requiredType = JSONTagType.Array;
                    break;
                case JSONDefines.SCHEMA_TAG_BOOLEAN:
                    requiredType = JSONTagType.Boolean;
                    break;
                case JSONDefines.SCHEMA_TAG_FLOAT:
                case JSONDefines.SCHEMA_TAG_INTEGER:
                    requiredType = JSONTagType.Number;
                    break;
                case JSONDefines.SCHEMA_TAG_OBJECT:
                    requiredType = JSONTagType.Object;
                    break;
                case JSONDefines.SCHEMA_TAG_STRING:
                    requiredType = JSONTagType.String;
                    break;
                default:
                    throw new JSONException(JSONException.JSONExceptionType.UnknownSchemaTagType, entryTag.Value);
            }

            return requiredType;
        }

        /*
         * Retrieve schema tag type
         * @param SchemaTag schema tag object
         * @return string tag object
         */
        static private JSONStringTag RetrieveSchemaTypeTag(IJSONTag SchemaTag)
        {
            IJSONTag unknownTag;
            JSONStringTag typeTag;

            try
            {
                unknownTag = SchemaTag[JSONDefines.SCHEMA_TAG_TYPE];

                if (unknownTag.GetTagType() != JSONTagType.String)
                {
                    throw new JSONException(JSONException.JSONExceptionType.InvalidSchemaTagType, unknownTag.GetTagType().ToString());
                }
                typeTag = (JSONStringTag)unknownTag;
            }
            catch (Exception exception)
            {
                throw new JSONException(JSONException.JSONExceptionType.MissingRequiredSchemaTag, JSONDefines.SCHEMA_TAG_TYPE, exception);
            }

            return typeTag;
        }

        /*
         * JSON document object string representation
         * @param Key JSON key string
         * @return document object string representation
         */
        public string ToString(string Key)
        {
            JSONObjectTag objectTag;

            if (!TryGetValue(Key, out objectTag))
            {
                throw new JSONException(JSONException.JSONExceptionType.KeyNotFound, Key);
            }

            return objectTag.ToString();
        }

        /*
         * Validate JSON file
         * @param Path JSON file path
         * @param InException optional user supplied exception object (filled upon exception, null otherwise)
         * @return true if successful, false otherwise
         */
        static public bool Validate(string Path, out Exception InException)
        {
            return Validate(Path, true, out InException);
        }

        /*
         * Validate JSON data or file
         * @param Input JSON data or file path
         * @param IsFile determine if input is a file path
         * @param InException optional user supplied exception object (filled upon exception, null otherwise)
         * @return true if successful, false otherwise
         */
        static public bool Validate(string Input, bool IsFile, out Exception InException)
        {
            bool result = true;
            JSONParser inputParser;

            InException = null;

            try
            {
                inputParser = new JSONParser(Input, IsFile);
            }
            catch (Exception exception)
            {
                InException = exception;
                result = false;
            }

            return result;
        }

        /*
         * Validate JSON array tag against schema tag
         * @param Input tag input JSON array tag
         * @param SchemaTag schema object tag
         */
        static private void ValidateArrayTag(JSONArrayTag InputTag, JSONObjectTag SchemaTag)
        {
            int elementCount;
            IJSONTag optionalTag;
            JSONObjectTag entryTag = RetrieveSchemaEntryTag(SchemaTag);
            JSONTagType requiredType = RetrieveSchemaRequiredType(entryTag);

            if (SchemaTag.ContainsKey(JSONDefines.SCHEMA_TAG_COUNT))
            {
                optionalTag = SchemaTag[JSONDefines.SCHEMA_TAG_COUNT];

                if (optionalTag.GetTagType() != JSONTagType.Number)
                {
                    throw new JSONException(JSONException.JSONExceptionType.ExpectingNamedNumberChildTag, JSONDefines.SCHEMA_TAG_COUNT);
                }
                elementCount = ((JSONNumberTag)optionalTag).AsInteger();

                if (InputTag.Count != elementCount)
                {
                    throw new JSONException(JSONException.JSONExceptionType.ArrayChildCountMismatch, InputTag.GetKey() 
                        + " (must contain " + elementCount + " element(s))");
                }
            }

            foreach (IJSONTag element in InputTag)
            {

                if (element.GetTagType() != requiredType)
                {

                    switch (requiredType)
                    {
                        case JSONTagType.Array:
                            throw new JSONException(JSONException.JSONExceptionType.ExpectingArrayChildTag, element.ToString());
                        case JSONTagType.Boolean:
                            throw new JSONException(JSONException.JSONExceptionType.ExpectingBooleanChildTag, element.ToString());
                        case JSONTagType.Number:
                            throw new JSONException(JSONException.JSONExceptionType.ExpectingNumericChildTag, element.ToString());
                        case JSONTagType.Object:
                            throw new JSONException(JSONException.JSONExceptionType.ExpectingObjectChildTag, element.ToString());
                        case JSONTagType.String:
                            throw new JSONException(JSONException.JSONExceptionType.ExpectingStringChildTag, element.ToString());
                        default:
                            throw new JSONException(JSONException.JSONExceptionType.UnknownSchemaTagType, requiredType.ToString());
                    }
                }

                switch (element.GetTagType())
                {
                    case JSONTagType.Array:
                        ValidateArrayTag(element.AsArray(), entryTag);
                        break;
                    case JSONTagType.Boolean:
                        ValidateBooleanTag((JSONBooleanTag)element, entryTag);
                        break;
                    case JSONTagType.Number:
                        ValidateNumberTag((JSONNumberTag)element, entryTag, true);
                        break;
                    case JSONTagType.Object:
                        ValidateObjectTag(element.AsObject(), entryTag, true);
                        break;
                    case JSONTagType.String:
                        ValidateStringTag((JSONStringTag)element, entryTag);
                        break;
                    default:
                        throw new JSONException(JSONException.JSONExceptionType.UnknownSchemaTagType, element.GetTagType().ToString());
                }
            }
        }

        /*
         * Validate JSON boolean tag against schema tag
         * @param Input tag input JSON boolean tag
         * @param SchemaTag schema object tag
         */
        static private void ValidateBooleanTag(JSONBooleanTag InputTag, JSONObjectTag SchemaTag)
        {
            IJSONTag optionalTag;
            JSONArrayTag rangeArrayTag;
            bool allowFalse = false, allowTrue = false;

            if (SchemaTag.ContainsKey(JSONDefines.SCHEMA_TAG_RANGE))
            {
                optionalTag = SchemaTag[JSONDefines.SCHEMA_TAG_RANGE];

                if (optionalTag.GetTagType() != JSONTagType.Array)
                {
                    throw new JSONException(JSONException.JSONExceptionType.ExpectingNamedArrayChildTag, JSONDefines.SCHEMA_TAG_RANGE);
                }
                rangeArrayTag = (JSONArrayTag)optionalTag;

                if ((rangeArrayTag.Count < JSONDefines.SCHEMA_RANGE_MIN_COUNT) || (rangeArrayTag.Count > JSONDefines.SCHEMA_RANGE_MAX_COUNT))
                {
                    throw new JSONException(JSONException.JSONExceptionType.ExpectingRangeArrayCount, InputTag.GetKey()
                        + " (must contain values in the range [" + JSONDefines.SCHEMA_RANGE_MIN_COUNT + " - " + JSONDefines.SCHEMA_RANGE_MAX_COUNT + "])");
                }

                foreach (IJSONTag element in rangeArrayTag)
                {

                    if (element.GetTagType() != JSONTagType.Boolean)
                    {
                        throw new JSONException(JSONException.JSONExceptionType.ExpectingRangeValue, InputTag.GetKey());
                    }

                    if (((JSONBooleanTag)element).AsBoolean())
                    {

                        if (allowTrue)
                        {
                            throw new JSONException(JSONException.JSONExceptionType.DuplicateBooleanRangeValue, InputTag.GetKey());
                        }
                        allowTrue = true;
                    }
                    else
                    {

                        if (allowFalse)
                        {
                            throw new JSONException(JSONException.JSONExceptionType.DuplicateBooleanRangeValue, InputTag.GetKey());
                        }
                        allowFalse = true;
                    }
                }

                if (InputTag.AsBoolean() && !allowTrue)
                {
                    throw new JSONException(JSONException.JSONExceptionType.BooleanOutOfRange, InputTag.GetKey()
                        + " (" + InputTag.AsBoolean() + " is not allowed)");
                }
                else if (!InputTag.AsBoolean() && !allowFalse)
                {
                    throw new JSONException(JSONException.JSONExceptionType.BooleanOutOfRange, InputTag.GetKey()
                        + " (" + InputTag.AsBoolean() + " is not allowed)");
                }
            }
        }

        /*
         * Validate JSON number tag against schema tag
         * @param Input tag input JSON number tag
         * @param SchemaTag schema object tag
         * @param IsFloat determine if input tag is floating-point
         */
        static private void ValidateNumberTag(JSONNumberTag InputTag, JSONObjectTag SchemaTag, bool IsFloat)
        {
            IJSONTag optionalTag;
            JSONArrayTag rangeArrayTag;
            float maxFloatValue, minFloatValue;
            int maxIntegerValue, minIntegerValue;

            if (SchemaTag.ContainsKey(JSONDefines.SCHEMA_TAG_RANGE))
            {
                optionalTag = SchemaTag[JSONDefines.SCHEMA_TAG_RANGE];

                if (optionalTag.GetTagType() != JSONTagType.Array)
                {
                    throw new JSONException(JSONException.JSONExceptionType.ExpectingNamedArrayChildTag, JSONDefines.SCHEMA_TAG_RANGE);
                }
                rangeArrayTag = (JSONArrayTag)optionalTag;

                if ((rangeArrayTag.Count < JSONDefines.SCHEMA_RANGE_MIN_COUNT) || (rangeArrayTag.Count > JSONDefines.SCHEMA_RANGE_MAX_COUNT))
                {
                    throw new JSONException(JSONException.JSONExceptionType.ExpectingRangeArrayCount, InputTag.GetKey()
                        + " (must contain values in the range [" + JSONDefines.SCHEMA_RANGE_MIN_COUNT + " - " + JSONDefines.SCHEMA_RANGE_MAX_COUNT + "])");
                }

                foreach (IJSONTag element in rangeArrayTag)
                {

                    if (element.GetTagType() != JSONTagType.Number)
                    {
                        throw new JSONException(JSONException.JSONExceptionType.ExpectingRangeValue, InputTag.GetKey());
                    }
                }

                if (IsFloat)
                {
                    minFloatValue = rangeArrayTag[JSONDefines.SCHEMA_RANGE_MIN_OFFSET].AsFloat();
                    maxFloatValue = rangeArrayTag[(rangeArrayTag.Count == JSONDefines.SCHEMA_RANGE_MIN_COUNT) 
                        ? JSONDefines.SCHEMA_RANGE_MIN_OFFSET : JSONDefines.SCHEMA_RANGE_MAX_OFFSET].AsFloat();

                    if (minFloatValue > maxFloatValue)
                    {
                        throw new JSONException(JSONException.JSONExceptionType.InvalidValueRange, InputTag.GetKey());
                    }

                    if (InputTag.AsFloat() < minFloatValue)
                    {
                        throw new JSONException(JSONException.JSONExceptionType.ValueOutOfRange, InputTag.GetKey() 
                            + " (must contain a value >= " + minFloatValue + ")");
                    }

                    if(InputTag.AsFloat() > maxFloatValue)
                    {
                        throw new JSONException(JSONException.JSONExceptionType.ValueOutOfRange, InputTag.GetKey()
                            + " (must contain a value <= " + maxFloatValue + ")");
                    }
                }
                else
                {
                    minIntegerValue = rangeArrayTag[JSONDefines.SCHEMA_RANGE_MIN_OFFSET].AsInteger();
                    maxIntegerValue = rangeArrayTag[(rangeArrayTag.Count == JSONDefines.SCHEMA_RANGE_MIN_COUNT)
                        ? JSONDefines.SCHEMA_RANGE_MIN_OFFSET : JSONDefines.SCHEMA_RANGE_MAX_OFFSET].AsInteger();

                    if (minIntegerValue > maxIntegerValue)
                    {
                        throw new JSONException(JSONException.JSONExceptionType.InvalidValueRange, InputTag.GetKey());
                    }

                    if (InputTag.AsInteger() < minIntegerValue)
                    {
                        throw new JSONException(JSONException.JSONExceptionType.ValueOutOfRange, InputTag.GetKey()
                            + " (must contain a value >= " + minIntegerValue + ")");
                    }

                    if (InputTag.AsInteger() > maxIntegerValue)
                    {
                        throw new JSONException(JSONException.JSONExceptionType.ValueOutOfRange, InputTag.GetKey()
                            + " (must contain a value <= " + maxIntegerValue + ")");
                    }
                }
            }
        }

        /*
         * Validate JSON object tag against schema tag
         * @param Input tag input JSON object tag
         * @param SchemaTag schema object tag
         * @param IsRoot determine if input tag is is root tag (default: false)
         */
        static private void ValidateObjectTag(JSONObjectTag InputTag, JSONObjectTag SchemaTag, bool IsRoot = false)
        {
            int elementCount;
            IJSONTag optionalTag;
            JSONStringTag typeTag = RetrieveSchemaTypeTag(SchemaTag);
            JSONObjectTag entryTag = RetrieveSchemaEntryTag(SchemaTag), wildcardTag;

            try
            {

                if (!IsRoot && (InputTag.Key != SchemaTag.Key))
                {
                    throw new JSONException(JSONException.JSONExceptionType.ExpectingNamedObjectTag, SchemaTag.Key);
                }

                if (SchemaTag.ContainsKey(JSONDefines.SCHEMA_TAG_COUNT))
                {
                    optionalTag = SchemaTag[JSONDefines.SCHEMA_TAG_COUNT];

                    if (optionalTag.GetTagType() != JSONTagType.Number)
                    {
                        throw new JSONException(JSONException.JSONExceptionType.ExpectingNamedNumberChildTag, JSONDefines.SCHEMA_TAG_COUNT);
                    }
                    elementCount = ((JSONNumberTag)optionalTag).AsInteger();

                    if (InputTag.Count != elementCount)
                    {
                        throw new JSONException(JSONException.JSONExceptionType.ObjectChildCountMismatch, InputTag.GetKey() 
                            + " (must contain " + elementCount + " element(s))");
                    }
                }

                foreach (KeyValuePair<string, IJSONTag> entry in entryTag)
                {

                    if (IsSchemaTagOptional(entry.Value.AsObject()) && !InputTag.ContainsKey(entry.Key))
                    {
                        continue;
                    }

                    if (!InputTag.ContainsKey(entry.Key))
                    {
                        wildcardTag = IsSchemaTagWildcard(entryTag);

                        if (wildcardTag == null)
                        {
                            throw new JSONException(JSONException.JSONExceptionType.ExpectingNamedObjectChildTag, entry.Key);
                        }
                        ValidateObjectChildTag(InputTag, new KeyValuePair<string, IJSONTag>(RetrieveFirstChildTag(InputTag).GetKey(), wildcardTag));
                    }
                    else
                    {
                        ValidateObjectChildTag(InputTag, entry);
                    }
                }
            }
            catch (Exception exception)
            {
                throw new JSONException(JSONException.JSONExceptionType.SchemaMismatch, exception);
            }
        }

        /*
         * Validate JSON object child tag against schema tag
         * @param Input tag input JSON object tag
         * @param EntryTagPair schema mapped tag object
         */
        static private void ValidateObjectChildTag(JSONObjectTag InputTag, KeyValuePair<string, IJSONTag> EntryTagPair)
        {
            JSONStringTag subTypeTag = RetrieveSchemaTypeTag(EntryTagPair.Value);

            switch (subTypeTag.Value)
            {
                case JSONDefines.SCHEMA_TAG_ARRAY:

                    if (InputTag[EntryTagPair.Key].GetTagType() != JSONTagType.Array)
                    {
                        throw new JSONException(JSONException.JSONExceptionType.ExpectingNamedArrayChildTag, EntryTagPair.Key);
                    }
                    ValidateArrayTag(InputTag[EntryTagPair.Key].AsArray(), EntryTagPair.Value.AsObject());
                    break;
                case JSONDefines.SCHEMA_TAG_BOOLEAN:

                    if (InputTag[EntryTagPair.Key].GetTagType() != JSONTagType.Boolean)
                    {
                        throw new JSONException(JSONException.JSONExceptionType.ExpectingNamedBooleanChildTag, EntryTagPair.Key);
                    }
                    ValidateBooleanTag((JSONBooleanTag)InputTag[EntryTagPair.Key], EntryTagPair.Value.AsObject());
                    break;
                case JSONDefines.SCHEMA_TAG_FLOAT:

                    if (InputTag[EntryTagPair.Key].GetTagType() != JSONTagType.Number)
                    {
                        throw new JSONException(JSONException.JSONExceptionType.ExpectingNamedNumberChildTag, EntryTagPair.Key);
                    }
                    ValidateNumberTag((JSONNumberTag)InputTag[EntryTagPair.Key], EntryTagPair.Value.AsObject(), true);
                    break;
                case JSONDefines.SCHEMA_TAG_INTEGER:

                    if (InputTag[EntryTagPair.Key].GetTagType() != JSONTagType.Number)
                    {
                        throw new JSONException(JSONException.JSONExceptionType.ExpectingNamedNumberChildTag, EntryTagPair.Key);
                    }
                    ValidateNumberTag((JSONNumberTag)InputTag[EntryTagPair.Key], EntryTagPair.Value.AsObject(), false);
                    break;
                case JSONDefines.SCHEMA_TAG_OBJECT:

                    if (InputTag[EntryTagPair.Key].GetTagType() != JSONTagType.Object)
                    {
                        throw new JSONException(JSONException.JSONExceptionType.ExpectingNamedObjectChildTag, EntryTagPair.Key);
                    }
                    ValidateObjectTag(InputTag[EntryTagPair.Key].AsObject(), EntryTagPair.Value.AsObject());
                    break;
                case JSONDefines.SCHEMA_TAG_STRING:

                    if (InputTag[EntryTagPair.Key].GetTagType() != JSONTagType.String)
                    {
                        throw new JSONException(JSONException.JSONExceptionType.ExpectingNamedStringChildTag, EntryTagPair.Key);
                    }
                    ValidateStringTag((JSONStringTag)InputTag[EntryTagPair.Key], EntryTagPair.Value.AsObject());
                    break;
                default:
                    throw new JSONException(JSONException.JSONExceptionType.UnknownSchemaTagType, subTypeTag.Value);
            }
        }

        /*
         * Validate JSON string tag against schema tag
         * @param Input tag input JSON string tag
         * @param SchemaTag schema object tag
         */
        static private void ValidateStringTag(JSONStringTag InputTag, JSONObjectTag SchemaTag)
        {
            Match patternMatch;
            IJSONTag optionalTag;
            JSONStringTag patternTag;
            JSONArrayTag lengthArrayTag;
            int minLengthValue, maxLengthValue;

            if (SchemaTag.ContainsKey(JSONDefines.SCHEMA_TAG_LENGTH))
            {
                optionalTag = SchemaTag[JSONDefines.SCHEMA_TAG_LENGTH];

                if (optionalTag.GetTagType() != JSONTagType.Array)
                {
                    throw new JSONException(JSONException.JSONExceptionType.ExpectingNamedArrayChildTag, JSONDefines.SCHEMA_TAG_LENGTH);
                }
                lengthArrayTag = (JSONArrayTag)optionalTag;

                if ((lengthArrayTag.Count < JSONDefines.SCHEMA_LENGTH_MIN_COUNT) || (lengthArrayTag.Count > JSONDefines.SCHEMA_LENGTH_MAX_COUNT))
                {
                    throw new JSONException(JSONException.JSONExceptionType.ExpectingLengthArrayCount, InputTag.GetKey()
                        + " (must contain values of the length [" + JSONDefines.SCHEMA_LENGTH_MIN_COUNT + " - " + JSONDefines.SCHEMA_LENGTH_MAX_COUNT + "])");
                }

                foreach (IJSONTag element in lengthArrayTag)
                {

                    if (element.GetTagType() != JSONTagType.Number)
                    {
                        throw new JSONException(JSONException.JSONExceptionType.ExpectingLengthValue, InputTag.GetKey());
                    }
                }
                minLengthValue = lengthArrayTag[JSONDefines.SCHEMA_LENGTH_MIN_OFFSET].AsInteger();
                maxLengthValue = lengthArrayTag[(lengthArrayTag.Count == JSONDefines.SCHEMA_LENGTH_MIN_COUNT)
                    ? JSONDefines.SCHEMA_LENGTH_MIN_OFFSET : JSONDefines.SCHEMA_LENGTH_MAX_OFFSET].AsInteger();

                if (minLengthValue > maxLengthValue)
                {
                    throw new JSONException(JSONException.JSONExceptionType.InvalidValueLength, InputTag.GetKey());
                }

                if (InputTag.AsString().Length < minLengthValue)
                {
                    throw new JSONException(JSONException.JSONExceptionType.ValueOutOfRange, InputTag.GetKey()
                        + " (must contain a value of length >= " + minLengthValue + ")");
                }

                if (InputTag.AsString().Length > maxLengthValue)
                {
                    throw new JSONException(JSONException.JSONExceptionType.ValueOutOfRange, InputTag.GetKey()
                        + " (must contain a value of length <= " + maxLengthValue + ")");
                }
            }

            if (SchemaTag.ContainsKey(JSONDefines.SCHEMA_TAG_PATTERN))
            {
                optionalTag = SchemaTag[JSONDefines.SCHEMA_TAG_PATTERN];

                if (optionalTag.GetTagType() != JSONTagType.String)
                {
                    throw new JSONException(JSONException.JSONExceptionType.ExpectingNamedStringChildTag, JSONDefines.SCHEMA_TAG_PATTERN);
                }
                patternTag = (JSONStringTag)optionalTag;
                patternMatch = Regex.Match(InputTag.AsString(), patternTag.AsString(), RegexOptions.None);

                if (!patternMatch.Success)
                {
                    throw new JSONException(JSONException.JSONExceptionType.StringPatternMismatch, InputTag.GetKey() 
                        + " (must match the pattern \'" + patternTag.AsString() + "\')");
                }
            }
        }

        /*
         * Validate JSON data or file against JSON schema data or file
         * @param Input JSON data or file path
         * @param SchemaInput Schema JSON data or file path
         * @param InException optional user supplied exception object (filled upon exception, null otherwise)
         * @return true if successful, false otherwise
         */
        static public bool ValidateWithSchema(string Input, string SchemaInput, out Exception InException)
        {
            return ValidateWithSchema(Input, true, SchemaInput, true, out InException);
        }

        /*
         * Validate JSON data or file against JSON schema data or file
         * @param Input JSON data or file path
         * @param IsFile determine if input is a file path
         * @param SchemaInput Schema JSON data or file path
         * @param IsSchemaFile determine if schema is a file file path
         * @param InException optional user supplied exception object (filled upon exception, null otherwise)
         * @return true if successful, false otherwise
         */
        static public bool ValidateWithSchema(string Input, bool IsFile, string SchemaInput, bool IsSchemaFile, out Exception InException)
        {
            bool result = true;
            JSONObjectTag inputTag, schemaTag;

            InException = null;

            try
            {
                inputTag = new JSONParser(Input, IsFile).RootTag;
                schemaTag = new JSONParser(SchemaInput, IsSchemaFile).RootTag;
                ValidateObjectTag(inputTag, schemaTag, true);
            }
            catch (Exception exception)
            {
                InException = exception;
                result = false;
            }

            return result;
        }

        /*
         * Write JSON data to file
         * @param Key JSON key string
         * @param Path JSON file path
         */
        public void Write(string Key, string Path)
        {
            StreamWriter stream;
            JSONObjectTag objectTag;

            if (!TryGetValue(Key, out objectTag))
            {
                throw new JSONException(JSONException.JSONExceptionType.KeyNotFound, Key);
            }

            try
            {
                stream = new StreamWriter(Path);
                stream.WriteLine(objectTag.ToString());
                stream.Close();
            }
            catch (Exception exception)
            {
                throw new JSONException(JSONException.JSONExceptionType.FileException, exception);
            }
        }

        /*
         * JSON tag override flag
         */
        public bool IsOverridable
        {
            get { return isOverridable; }
        }

        private bool isOverridable;
    }
}
