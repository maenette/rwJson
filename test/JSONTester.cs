/*
 * JSONTester.cs
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
using JSON;

class TestClass
{
    static void Main(string[] args)
    {
        System.Console.WriteLine(JSONDefines.LIBNAME + " " + JSONDefines.LibraryVersion());

        try
        {
            // read in a json file
            JSONDocument doc = new JSONDocument("..\\..\\test\\test.json", true);

            // print out json file contents
            System.Console.WriteLine(doc.ToString());

            // write json contents to new json file
            //doc.Write("..\\..\\test\\test_copy.json");
        }
        catch (JSONException exception)
        {
            Console.WriteLine("EXCEPTION: " + exception.Message);
        }

        System.Console.ReadLine();
    }
}