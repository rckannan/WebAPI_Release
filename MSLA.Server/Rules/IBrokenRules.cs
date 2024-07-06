using System;
using System.Collections.Generic;
using System.Text;

namespace MSLA.Server.Rules
{
    interface IBrokenRules
    {
        BrokenRuleCollection BrokenSaveRules
        { get; }
        BrokenRuleCollection BrokenDeleteRules
        { get; }
    }
}
