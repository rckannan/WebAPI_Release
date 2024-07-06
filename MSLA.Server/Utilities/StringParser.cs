using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MSLA.Server.Utilities
{
    /// <summary>A String Parser Class that can be used to replace XML Encodes</summary>
    [System.Diagnostics.DebuggerStepThrough()]
    public class StringParser
    {
        /// <summary>Removes Single Quotes from the string and prefixes them with another single quote. Can be used in SQL Statements</summary>
        /// <param name="ItemDesc">The String to be modified</param>
        public static String ParseSingleQuote(String ItemDesc)
        {
            return ItemDesc.Replace("'", "''");
        }

        /// <summary>Encodes XML items</summary>
        /// <param name="XMLValue">The String containing XML tags</param>
        public static String EncodeXML(String XMLValue)
        {
            String Result;
            Result = XMLValue.Replace("&", "&amp;");
            Result = Result.Replace("<", "&lt;");
            Result = Result.Replace(">", "&gt;");
            Result = Result.Replace("'", "&apos;");
            Result = Result.Replace(Convert.ToString('"'), "&quot;");
            return Result;
        }

        /// <summary>Decodes XML Items</summary>
        /// <param name="XMLValue">The XML Encoded String</param>
        public static String DecodeXML(String XMLValue)
        {
            String Result;
            Result = XMLValue.Replace("&lt;", "<");
            Result = Result.Replace("&gt;", ">");
            Result = Result.Replace("&apos;", "'");
            Result = Result.Replace("&quot;", Convert.ToString('"'));
            Result = Result.Replace("&amp;", "&");
            return Result;
        }
    }
}
