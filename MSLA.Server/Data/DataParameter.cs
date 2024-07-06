using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace MSLA.Server.Data
{
    /// <summary>Data parameter class</summary>
    [System.Diagnostics.DebuggerStepThrough()]
    [Serializable()]
    public class DataParameter
    {

        public enum EnDataParameterType
        {
            BigInt = 0,
            Binary = 1,
            Bit = 2,
            Char = 3,
            DateTime = 4,
            Decimal = 5,
            Float = 6,
            Image = 7,
            Int = 8,
            Money = 9,
            NChar = 10,
            NText = 11,
            NVarChar = 12,
            Real = 13,
            UniqueIdentifier = 14,
            SmallDateTime = 15,
            SmallInt = 16,
            SmallMoney = 17,
            Text = 18,
            Timestamp = 19,
            TinyInt = 20,
            VarBinary = 21,
            VarChar = 22,
            Variant = 23,
            Xml = 25,
            Udt = 29,
            Structured = 30,
            Date = 31,
            Time = 32,
            DateTime2 = 33,
            DateTimeOffset = 34,
        }

        public enum EnParameterDirection
        {
            Input = 1,
            Output = 2,
            InputOutput = 3,
            ReturnValue = 6,
        }

        private string _ParameterName = String.Empty;
        private EnDataParameterType _DBType = EnDataParameterType.Variant;
        private int _Size = 0;
        private object _Value = System.DBNull.Value;
        private EnParameterDirection _Direction = EnParameterDirection.Input;
        private Byte _Precision = 0;
        private Byte _Scale = 0;

        /// <summary>Constructor</summary>
        public DataParameter()
            : this(String.Empty, EnDataParameterType.Variant, 0)
        {
        }

        /// <summary>Constructor</summary>
        /// <param name="ParamName">The parameter name</param>
        /// <param name="ParamDBType">Data Type of the parameter</param>
        internal DataParameter(string ParamName, EnDataParameterType ParamDBType)
            : this(ParamName, ParamDBType, 0)
        {
        }

        /// <summary>Constructor</summary>
        /// <param name="ParamName">The parameter name</param>
        /// <param name="ParamDBType">Data Type of the parameter</param>
        /// <param name="ParamSize">Size of the parameter</param>
        internal DataParameter(string ParamName, EnDataParameterType ParamDBType, int ParamSize)
        {
            _ParameterName = ParamName;
            _DBType = ParamDBType;
            _Size = ParamSize;
        }

        #region "Public Properties"

        /// <summary>The Parameter Name</summary>
        public String ParameterName
        {
            get { return _ParameterName; }
            set { _ParameterName = value; }
        }

        /// <summary>Parameter Datatype</summary>
        public EnDataParameterType DBType
        {
            get { return _DBType; }
            set { _DBType = value; }
        }

        /// <summary>Parameter Size</summary>
        public int Size
        {
            get { return _Size; }
            set { _Size = value; }
        }

        /// <summary>Parameter Value</summary>
        public object Value
        {
            get { return _Value; }
            set { _Value = value; }
        }

        /// <summary>Parameter Direction</summary>
        public EnParameterDirection Direction
        {
            get { return _Direction; }
            set { _Direction = value; }
        }

        /// <summary>Parameter Precision</summary>
        public Byte Precision
        {
            get { return _Precision; }
            set { _Precision = value; }
        }

        /// <summary>Parameter Scale</summary>
        public Byte Scale
        {
            get { return _Scale; }
            set { _Scale = value; }
        }

        #endregion

    }
}
