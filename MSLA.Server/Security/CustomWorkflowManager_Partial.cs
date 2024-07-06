using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MSLA.Server.Security
{
    public partial class CustomWorkflowManager
    {

    }

    /// <summary>WFValidator Info. Contains Reflection Info. System.tblDocWorkflow</summary>
    [Serializable]
    public struct WFValidatorInfo
    {
        /// <summary>The Class Type name</summary>
        public string ValidatorClassType;
        /// <summary>The Validator Assembly</summary>
        public string ValidatorAssembly;
        /// <summary>The Validator name space</summary>
        public string ValidatorNameSpace;
        /// <summary>The validator object</summary>
        public string ValidatorObject;
    }
}
