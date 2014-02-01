/*
 * JSONTester.cs
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

using JSON;
using System;

class TestClass
{
    static void Main(string[] args)
    {
        Exception schemaResults;
        string key, path = "..\\..\\test\\test.json", schemaPath = "..\\..\\test\\test.json_schema";

        Console.WriteLine(JSONDefines.LibraryVersion(true, true) + "\n------------");

        try
        {
            // read in a json file/retrieve key
            JSONDocument doc = new JSONDocument(false, path);
            key = JSONDocument.ExtractKey(path);

            // print out json file contents
            Console.WriteLine(doc.ToString(key) + "\n------------");

            // retrieve key-value pairs from json object
            Console.WriteLine("NAME: " + doc[key]["firstName"].AsString() + " " + doc[key]["lastName"].AsString());
            Console.WriteLine("HOMEPHONE: " + ((IJSONPrimitive<string>)doc[key]["phoneNumbers"][0]["number"]).AsValue());
            Console.WriteLine("AGE: " + doc[key]["age"].AsInteger() + " (" + doc[key]["age"].AsFloat() + ")");

            // verify json file matches schema file
            Console.WriteLine("------------\n" + (!JSONDocument.ValidateWithSchema(path, schemaPath, out schemaResults) ? schemaResults.Message : "Schema Match!"));

            // write json contents to new json file
            //doc.Write(key, "..\\..\\test\\test_copy.json");
        }
        catch (JSONException exception)
        {
            Console.WriteLine("EXCEPTION: " + exception.Message);
        }
        Console.ReadLine();
    }
}