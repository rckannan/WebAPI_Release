using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MSLA.Server.Base
{
    /// <summary>This class implements the Binding base</summary>
    [System.Diagnostics.DebuggerStepThrough()]
    [Serializable()]
    public abstract class BindingBase
        : Object, Rules.IBrokenRules
    {
        /// <summary>Delegate required for Validate Event</summary>
        /// <param name="Sender">Sender object</param>
        /// <param name="e">Event Args</param>
        public delegate void ValidateUIOnSaveDelegate(Object Sender, EventArgs e);
        [NonSerialized()]
        private ValidateUIOnSaveDelegate _ValidateUIOnSave;

        ///<summary>Returns the Collection of Broken Save Rules.</summary>
        public abstract Rules.BrokenRuleCollection BrokenSaveRules
        { get; }

        ///<summary>Returns the Collection of Broken Delete Rules.</summary>
        public abstract Rules.BrokenRuleCollection BrokenDeleteRules
        { get; }

        /// <summary>This is a non serializable event that is invoked on Save. Write any UI related validations here.</summary>
        public event ValidateUIOnSaveDelegate ValidateUIOnSave
        {
            add
            {
                _ValidateUIOnSave = (ValidateUIOnSaveDelegate)System.Delegate.Combine(_ValidateUIOnSave, value);
            }
            remove
            {
                _ValidateUIOnSave = (ValidateUIOnSaveDelegate)System.Delegate.Remove(_ValidateUIOnSave, value);
            }
        }

        /// <summary> For Validating UI, Listen to this event. </summary>
        protected void OnValidateUIOnSave(EventArgs e)
        {
            if (_ValidateUIOnSave != null)
            {
                _ValidateUIOnSave.Invoke(this, e);
            }
        }
    }
}
