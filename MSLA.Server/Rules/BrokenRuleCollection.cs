using System;
using System.Collections.Generic;
using System.Text;

namespace MSLA.Server.Rules
{
    /// <summary>A Collection of Broken Rules</summary>
    [System.Diagnostics.DebuggerStepThrough()]
    [Serializable()]
    public class BrokenRuleCollection
        : List<BrokenRule>
    {

        #region "Overloaded Add Methods"
        /// <summary>Adds a new Broken Rule</summary>
        /// <param name="ClassName">The name of the Class</param>
        /// <param name="ClassProperty">The name of the property</param>
        /// <param name="RuleDesc">The Broken Rule Desc.</param>
        public BrokenRule Add(string ClassName, string ClassProperty, string RuleDesc)
        {
            BrokenRule Rule = new BrokenRule(ClassName, ClassProperty, RuleDesc);
            this.Add(Rule);
            return Rule;
        }

        /// <summary>Adds a new Broken Rule</summary>
        /// <param name="ClassName">The name of the Class</param>
        /// <param name="ClassProperty">The name of the property</param>
        /// <param name="RuleDesc">The Broken Rule Desc.</param>
        /// <param name="ErrorRowNo">The Row No.</param>
        public BrokenRule Add(string ClassName, string ClassProperty, string RuleDesc, Int16 ErrorRowNo)
        {
            BrokenRule Rule = new BrokenRule(ClassName, ClassProperty, RuleDesc, ErrorRowNo);
            this.Add(Rule);
            return Rule;
        }

        /// <summary>Adds a new Broken Rule</summary>
        /// <param name="ClassName">The name of the Class</param>
        /// <param name="ClassProperty">The name of the property</param>
        /// <param name="RuleDesc">The Broken Rule Desc.</param>
        /// <param name="ErrorRowNo">The Row No.</param>
        /// <param name="GridName">The Grid Name</param>
        public BrokenRule Add(string ClassName, string ClassProperty, string RuleDesc, Int16 ErrorRowNo, string GridName)
        {
            BrokenRule Rule = new BrokenRule(ClassName, ClassProperty, RuleDesc, ErrorRowNo, GridName);
            this.Add(Rule);
            return Rule;
        }


        #endregion

        /// <summary> Returns Broken Rules Count </summary>
        public long RuleCount
        {
            get
            {
                long RCount = 0;
                foreach (BrokenRule Brule in this)
                {
                    if (Brule.IsWarning == false)
                        RCount += 1;
                }
                return RCount;
            }

        }

        /// <summary> Returns Warnings Count </summary>
        public long WarningCount
        {
            get
            {
                long RCount = 0;
                foreach (BrokenRule Brule in this)
                {
                    if (Brule.IsWarning == true)
                        RCount += 1;
                }
                return RCount;
            }

        }
    }
}
