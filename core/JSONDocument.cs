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
                throw new JSONException(
                    JSONException.JSONExceptionType.InvalidKey,
                    "\'" + key + "\'"
                    );
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
                    throw new JSONException(
                        JSONException.JSONExceptionType.KeyNotUnique,
                        "\'" + Key + "\'"
                        );
                }
                else
                {
                    Remove(Key);
                }
            }
            Add(Key, new JSONParser(Input, IsFile).RootTag);
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
                throw new JSONException(JSONException.JSONExceptionType.MissingSchemaTag, JSONDefines.SCHEMA_TAG_ENTRY, exception);
            }

            return entryTag;
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
                throw new JSONException(JSONException.JSONExceptionType.MissingSchemaTag, JSONDefines.SCHEMA_TAG_TYPE, exception);
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
                throw new JSONException(
                    JSONException.JSONExceptionType.KeyNotFound,
                    "\'" + Key + "\'"
                    );
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
            JSONObjectTag entryTag = RetrieveSchemaEntryTag(SchemaTag);
            JSONStringTag typeTag = RetrieveSchemaTypeTag(entryTag);

            // TODO: check optional schema parameters (count, etc.)

            foreach (IJSONTag element in InputTag)
            {

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

            // TODO: check optional schema parameters (range, etc.)
        }

        /*
         * Validate JSON number tag against schema tag
         * @param Input tag input JSON number tag
         * @param SchemaTag schema object tag
         * @param IsFloat determine if input tag is floating-point
         */
        static private void ValidateNumberTag(JSONNumberTag InputTag, JSONObjectTag SchemaTag, bool IsFloat)
        {

            // TODO: check optional schema parameters (range, etc.)
        }

        /*
         * Validate JSON object tag against schema tag
         * @param Input tag input JSON object tag
         * @param SchemaTag schema object tag
         * @param IsRoot determine if input tag is is root tag (default: false)
         */
        static private void ValidateObjectTag(JSONObjectTag InputTag, JSONObjectTag SchemaTag, bool IsRoot = false)
        {
            JSONStringTag subTypeTag, typeTag = RetrieveSchemaTypeTag(SchemaTag);
            JSONObjectTag entryTag = RetrieveSchemaEntryTag(SchemaTag);

            try
            {

                if (!IsRoot && (InputTag.Key != SchemaTag.Key))
                {
                    throw new JSONException(JSONException.JSONExceptionType.ExpectingNamedObjectTag, SchemaTag.Key);
                }

                foreach (KeyValuePair<string, IJSONTag> entry in entryTag)
                {

                    if (!InputTag.ContainsKey(entry.Key))
                    {
                        throw new JSONException(JSONException.JSONExceptionType.ExpectingNamedObjectChildTag, entry.Key);
                    }
                    subTypeTag = RetrieveSchemaTypeTag(entry.Value);

                    switch (subTypeTag.Value)
                    {
                        case JSONDefines.SCHEMA_TAG_ARRAY:

                            if (InputTag[entry.Key].GetTagType() != JSONTagType.Array)
                            {
                                throw new JSONException(JSONException.JSONExceptionType.ExpectingNamedArrayChildTag, entry.Key);
                            }
                            ValidateArrayTag(InputTag[entry.Key].AsArray(), entry.Value.AsObject());
                            break;
                        case JSONDefines.SCHEMA_TAG_BOOLEAN:

                            if (InputTag[entry.Key].GetTagType() != JSONTagType.Boolean)
                            {
                                throw new JSONException(JSONException.JSONExceptionType.ExpectingNamedBooleanChildTag, entry.Key);
                            }
                            ValidateBooleanTag((JSONBooleanTag)InputTag[entry.Key], entry.Value.AsObject());
                            break;
                        case JSONDefines.SCHEMA_TAG_FLOAT:

                            if (InputTag[entry.Key].GetTagType() != JSONTagType.Number)
                            {
                                throw new JSONException(JSONException.JSONExceptionType.ExpectingNamedNumberChildTag, entry.Key);
                            }
                            ValidateNumberTag((JSONNumberTag)InputTag[entry.Key], entry.Value.AsObject(), true);
                            break;
                        case JSONDefines.SCHEMA_TAG_INTEGER:

                            if (InputTag[entry.Key].GetTagType() != JSONTagType.Number)
                            {
                                throw new JSONException(JSONException.JSONExceptionType.ExpectingNamedNumberChildTag, entry.Key);
                            }
                            ValidateNumberTag((JSONNumberTag)InputTag[entry.Key], entry.Value.AsObject(), false);
                            break;
                        case JSONDefines.SCHEMA_TAG_OBJECT:

                            if (InputTag[entry.Key].GetTagType() != JSONTagType.Object)
                            {
                                throw new JSONException(JSONException.JSONExceptionType.ExpectingNamedObjectChildTag, entry.Key);
                            }
                            ValidateObjectTag(InputTag[entry.Key].AsObject(), entry.Value.AsObject());
                            break;
                        case JSONDefines.SCHEMA_TAG_STRING:

                            if (InputTag[entry.Key].GetTagType() != JSONTagType.String)
                            {
                                throw new JSONException(JSONException.JSONExceptionType.ExpectingNamedStringChildTag, entry.Key);
                            }
                            ValidateStringTag((JSONStringTag)InputTag[entry.Key], entry.Value.AsObject());
                            break;
                        default:
                            throw new JSONException(JSONException.JSONExceptionType.UnknownSchemaTagType, subTypeTag.Value);
                    }
                }
            }
            catch (Exception exception)
            {
                throw new JSONException(JSONException.JSONExceptionType.SchemaMismatch, exception);
            }
        }

        /*
         * Validate JSON string tag against schema tag
         * @param Input tag input JSON string tag
         * @param SchemaTag schema object tag
         */
        static private void ValidateStringTag(JSONStringTag InputTag, JSONObjectTag SchemaTag)
        {

            // TODO: check optional schema parameters (length, pattern, etc.)
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
                throw new JSONException(
                    JSONException.JSONExceptionType.KeyNotFound,
                    "\'" + Key + "\'"
                    );
            }

            try
            {
                stream = new StreamWriter(Path);
                stream.WriteLine(objectTag.ToString());
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
