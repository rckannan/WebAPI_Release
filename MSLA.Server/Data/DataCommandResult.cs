using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MSLA.Server.Data
{
    /// <summary>The result of output parameters after execution of Command</summary>
    [System.Diagnostics.DebuggerStepThrough()]
    [Serializable()]
    public class DataCommandResult
    {
        private Dictionary<string, object> _ResultCollection;

        /// <summary>Returns a collection of Param Name and Values</summary>
        internal DataCommandResult()
        {
            _ResultCollection = new Dictionary<String, Object>();
        }

        /// <summary>Can be used to add execution results</summary>
        /// <param name="ParamName">Parameter name</param>
        /// <param name="ParamValue">Parameter value</param>
        public void AddResult(string ParamName, object ParamValue)
        {
            _ResultCollection.Add(ParamName, ParamValue);
        }

        /// <summary>Gets value of a requested parameter</summary>
        /// <param name="ParamName">Name of the Parameter</param>
        public object GetValue(string ParamName)
        {
            return _ResultCollection[ParamName];
        }

    }
}
