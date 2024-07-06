using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace MSLA.Server.Data
{
    /// <summary>The Data Parameter Collection Class</summary>
    [System.Diagnostics.DebuggerStepThrough()]
    [Serializable()]
    public class DataParameterCollection
        : List<DataParameter>
    {
        internal DataParameterCollection()
            : base()
        {
        }

        /// <summary>Add a new Parameter to the collection</summary>
        /// <param name="ParameterName">parameter Name</param>
        /// <param name="ParamDBType">Data Type</param>
        /// <param name="Size">Size</param>
        public DataParameter Add(string ParameterName, DataParameter.EnDataParameterType ParamDBType, int Size)
        {
            DataParameter DPara = new DataParameter(ParameterName, ParamDBType, Size);
            this.Add(DPara);
            return DPara;
        }


        DataParameter this[string index]
        {
            get
            {
                foreach (DataParameter DParam in this)
                {
                    if (DParam.ParameterName == index)
                    {
                        return DParam;
                    }
                }
                return null;
            }
        }
    }
}
