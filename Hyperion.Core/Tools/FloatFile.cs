
using System;
using System.IO;
using System.Collections.Generic;

namespace Hyperion.Core
{
    public static class FloatFile
    {
        public static bool ReadFloatFile (string filename, List<double> values)
        {
            FileStream stream = File.OpenRead (filename);
            StreamReader reader = new StreamReader (stream);

            char c;
            int lineNumber = 1;
            bool inNumber = false;
            char[] curNumber = new char[32];
            int curNumberPos = 0;
            while (!reader.EndOfStream)
            {
                c = (char)reader.Read ();

                if (c == '\n')
                    ++lineNumber;
                if (inNumber)
                {
                    if (char.IsDigit (c) || c == '.' || c == 'e' || c == '-' || c == '+')
                        curNumber[curNumberPos++] = c;
                    else
                    {
                        curNumber[curNumberPos++] = '\0';
                        values.Add (double.Parse (new string (curNumber)));
                        inNumber = false;
                        curNumberPos = 0;
                    }
                }
                else
                {
                    if (char.IsDigit (c) || c == '.' || c == '-' || c == '+')
                    {
                        inNumber = true;
                        curNumber[curNumberPos++] = c;
                    }
                    else if (c == '#')
                    {
                        while ((c = (char)reader.Read ()) != '\n' && !reader.EndOfStream)
                            ;
                        ++lineNumber;
                    }
                    else if (!char.IsWhiteSpace (c))
                        ;
                }
            }
            return true;
        }
    }
}
