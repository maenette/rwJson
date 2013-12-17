/*
 * JSONDocument.cs
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
         * @param Key JSON key string
         * @param Input JSON data or file path
         * @param IsFile determine if input in a file path
         * @param IsOverridable determine if input is overridable
         */
        public JSONDocument(string Key, string Input, bool IsFile, bool IsOverridable)
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
                        JSONException.JSONExceptionType.EntryNotUnique,
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
                    JSONException.JSONExceptionType.ObjectNotFound,
                    "\'" + Key + "\'"
                    );
            }

            return objectTag.ToString();
        }

        /*
         * Validate JSON data or file
         * @param Input JSON data or file path
         * @param IsFile determine if input in a file path
         * @param InException optional user supplied exception object (filled upon exception, null otherwise)
         * @return true if successful, false otherwise
         */
        static public bool Validate(string Input, bool IsFile, out Exception InException)
        {
            bool result = true;

            InException = null;

            try
            {
                JSONParser parser = new JSONParser(Input, IsFile);
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
                    JSONException.JSONExceptionType.ObjectNotFound,
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