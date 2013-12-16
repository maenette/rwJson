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
     * Holds a root object tag, representing a JSON document
     */
    public class JSONDocument
    {

        /*
         * JSON document constructor
         */
        public JSONDocument()
        {
            rootTags = new List<JSONObjectTag>();
        }

        /*
         * JSON document constructor
         * @param Input JSON data or file path
         * @param IsFile determine if input in a file path
         */
        public JSONDocument(string Path, bool IsFile)
        {
            Read(Path, IsFile);
        }

        /*
         * Read in JSON data or file
         * @param Input JSON data or file path
         * @param IsFile determine if input in a file path
         */
        public void Read(string Input, bool IsFile)
        {
            JSONParser parser = new JSONParser(Input, IsFile);
            rootTags = parser.RootTags;
        }

        /*
         * JSON document string representation
         * @return document string representation
         */
        public override string ToString()
        {
            bool firstIter = true;
            StringBuilder stream = new StringBuilder();

            foreach (JSONObjectTag tag in rootTags)
            {
                if (!firstIter)
                {
                    stream.Append((char)JSONDefines.JSONSymbolType.PairDelimiter).Append((char)JSONDefines.JSONWhitespaceType.LineFeed);
                }
                else
                {
                    firstIter = false;
                }
                stream.Append(tag.ToString());
            }

            return stream.ToString();
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
         * @param Path JSON file path
         */
        public void Write(string Path)
        {
            StreamWriter stream;

            try
            {
                stream = new StreamWriter(Path);
                stream.WriteLine(rootTags.ToString());
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
         * JSON root tag
         */
        public List<JSONObjectTag> RootTags
        {
            get { return rootTags; }
        }

        private List<JSONObjectTag> rootTags;
    }
}
