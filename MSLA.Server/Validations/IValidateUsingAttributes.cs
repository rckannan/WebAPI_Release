using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MSLA.Server.Validations
{
    /// <summary>This interface is to be implemented by the class that proposes to use ValidateUsing Attributes</summary>
    public interface IValidateUsingAttributes
    {
        /// <summary>Gets the Broken Rules Collection</summary>
        Rules.BrokenRuleCollection BrokenSaveRules { get; }
    }
}
