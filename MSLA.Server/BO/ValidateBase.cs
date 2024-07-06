using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MSLA.Server.BO
{
    [System.Diagnostics.DebuggerStepThrough()]
    abstract class ValidateBase
    {
        protected Rules.IBrokenRules _ParentObject = null;
        protected ValidateBase(Rules.IBrokenRules ParentObject)
        {
            _ParentObject = ParentObject;
        }

        protected internal abstract void ValidateBeforeSave();

        protected internal abstract void ValidateBeforePost();

        protected internal abstract void ValidateBeforeUnpost();

        protected internal abstract void ValidateBeforeDelete();

    }
}
