using System;
using System.Collections.Generic;
using System.Text;

namespace MSLA.Server.Rules
{
    /// <summary>The Broken Rule Structure. This would store the Broken Business Rule.</summary>
    [System.Diagnostics.DebuggerStepThrough()]
    [Serializable()]
    public struct BrokenRule
    {
        private string _ClassName;
        private string _ClassProperty;
        private string _RuleDesc;
        private Int32 _ErrorRowNo;
        private string _GridName;
        private bool _IsWarning;

        #region "Constructor"

        /// <summary>The Constructor</summary>
        /// <param name="ClassName">The Class name</param>
        /// <param name="ClassProperty">The property in the class that has the broken the rule</param>
        /// <param name="RuleDesc">The description of the broken rule</param>
        public BrokenRule(string ClassName, string ClassProperty, string RuleDesc)
            : this(ClassName, ClassProperty, RuleDesc, -1, string.Empty)
        {
        }

        /// <summary>The Constructor</summary>
        /// <param name="ClassName">The Class name</param>
        /// <param name="ClassProperty">The property in the class that has the broken the rule</param>
        /// <param name="RuleDesc">The description of the broken rule</param>
        /// <param name="ErrorRowNo">The row No in the grid that has broken the rule</param>
        public BrokenRule(string ClassName, string ClassProperty, string RuleDesc, Int32 ErrorRowNo)
            : this(ClassName, ClassProperty, RuleDesc, ErrorRowNo, string.Empty)
        {
        }

        /// <summary>The Constructor</summary>
        /// <param name="ClassName">The Class name</param>
        /// <param name="ClassProperty">The property in the class that has the broken the rule</param>
        /// <param name="RuleDesc">The description of the broken rule</param>
        /// <param name="ErrorRowNo">The row No in the grid that has broken the rule</param>
        /// <param name="GridName">The Grid name</param>
        public BrokenRule(string ClassName, string ClassProperty, string RuleDesc, Int32 ErrorRowNo, string GridName)
            : this(ClassName, ClassProperty, RuleDesc, ErrorRowNo, GridName, false)
        {

        }

        /// <summary>The Constructor</summary>
        /// <param name="ClassName">The Class name</param>
        /// <param name="ClassProperty">The property in the class that has the broken the rule</param>
        /// <param name="RuleDesc">The description of the broken rule</param>
        /// <param name="ErrorRowNo">The row No in the grid that has broken the rule</param>
        /// <param name="GridName">The Grid name</param>
        /// <param name="IsWarning">True for a warning, will be validated only on Post</param>
        public BrokenRule(string ClassName, string ClassProperty, string RuleDesc, Int32 ErrorRowNo, string GridName, bool IsWarning)
        {
            _GridName = GridName;
            _ClassName = ClassName;
            _ClassProperty = ClassProperty;
            _RuleDesc = RuleDesc;
            _ErrorRowNo = ErrorRowNo;
            _GridName = GridName;
            _IsWarning = IsWarning;
        }

        #endregion

        #region "Structure Properties"
        /// <summary>Gets the name of the class to which this broken rule is associated</summary>
        public string ClassName
        {
            get { return _ClassName; }
        }

        /// <summary>Gets the name of the property which has broken the rule</summary>
        public string ClassProperty
        {
            get { return _ClassProperty; }
        }

        /// <summary>Gets the rule description</summary>
        public string RuleDesc
        {
            get
            {
                if (_ErrorRowNo == -1)
                { return _RuleDesc; }
                else
                {
                    string result = string.Empty;
                    if (_GridName == string.Empty)
                    { result = "Data "; }
                    else
                    { result = _GridName; }
                    result += " grid row no.: " + Convert.ToString((_ErrorRowNo + 1)).PadLeft(2, Convert.ToChar("0")) + " -> " + _RuleDesc;
                    return result;
                }
            }
        }

        /// <summary>Gets the Row# that caused the broken rule</summary>
        public Int32 ErrorRowNo
        {
            get { return _ErrorRowNo; }
        }

        /// <summary>Gets the Grid name that caused the broken rule</summary>
        public string GridName
        {
            get { return _GridName; }
        }

        /// <summary>Gets if the Current rule is a warning.</summary>
        public bool IsWarning
        {
            get { return _IsWarning; }
        }
        #endregion

    }
}
