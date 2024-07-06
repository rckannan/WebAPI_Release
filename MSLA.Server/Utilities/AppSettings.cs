using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MSLA.Server.Utilities
{
    /// <summary>This class has only shared methods and is used to read AppConfig, AppPath, etc.</summary>
    [System.Diagnostics.DebuggerStepThrough()]
    public class AppConfig
    {
        /// <summary>Returns the value of the key in App Config</summary>
        /// <param name="index">The Key</param>
        /// <returns>Value of the Key</returns>
        public static object Item(string index)
        {
            Object obj = System.Configuration.ConfigurationManager.AppSettings[index];
            if (obj == null)
            {
                throw new Exception("Configuration Item '" + index + "' in Application Configuration not found.\r\n" +
                                        "Kindly edit the App.Config to include this item.");
            }
            else
            {
                return obj;
            }
        }
    }

}
