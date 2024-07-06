using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MSLA.Server.Validations
{
    /// <summary>This is the abstract Validation Attribute Class. All other Validation Attributes must inherit this attribute.</summary>
    [System.Diagnostics.DebuggerStepThrough()]
    public abstract class ValidationAttribute
        : Attribute
    {
        ///<summary>Mention the user friendly name for the Field.</summary>
        public string DisplayString = String.Empty;
    }

    /// <summary>Use this Attribute for strings that should not be left blank.</summary>
    [System.Diagnostics.DebuggerStepThrough()]
    public class BlankNotAllowedAttribute
        : ValidationAttribute
    {
    }

    /// <summary>Use this Attribute for Long datatypes (usually IDs) that are instantiated as -1 by Default.</summary>
    [System.Diagnostics.DebuggerStepThrough()]
    public class ValueNotAllowedAttribute
        : ValidationAttribute
    {
        /// <summary>Enter the value that is not allowed to be saved. (Default is -1)</summary>
        public long FieldValue = -1;
    }

    /// <summary>Use this Attribute for Date fields that should be within the selected financial year.</summary>
    [System.Diagnostics.DebuggerStepThrough()]
    public class DateRangeAttribute
        : ValidationAttribute
    {
        /// <summary>Enter the Fin Year Alias</summary>
        public string FinYearField = "fldYear";

        /// <summary>The default constructor</summary>
        public DateRangeAttribute()
        {
            base.DisplayString = "Date";
        }
    }

    /// <summary>Use this Attribute for validating the IDs of Masters. This will fire a query and verify that the Master ID exists.</summary>
    [System.Diagnostics.DebuggerStepThrough()]
    public class MasterItemValidAttribute
        : ValidationAttribute
    {
        /// <summary>Setting this to true will validate the master in the table. Default is 'False'.</summary>
        public bool Disable = true;
        /// <summary>The name of the table in the database.</summary>
        public string MasterTable = String.Empty;
        /// <summary>The Field to compare with in the table.</summary>
        public string MasterField = String.Empty;
    }

    /// <summary>Use this Attribute for validating duplicates. This will fire a query and verify if the description/value already exists.</summary>
    [System.Diagnostics.DebuggerStepThrough()]
    public class DuplicateNotAllowedAttribute
        : ValidationAttribute
    {
        /// <summary>The name of the table in the database</summary>
        public string MasterTable = String.Empty;
        /// <summary>The primary key of the table</summary>
        public string MasterField = String.Empty;
        /// <summary>The field to be Checked with</summary>
        public string FieldTobeChecked = String.Empty;
    }


    /// <summary>Use this Attribute for validating duplicate codes. This will fire a query and verify whether the code already exists</summary>
    [System.Diagnostics.DebuggerStepThrough()]
    public class DuplicateCodeNotAllowedAttribute
        : ValidationAttribute
    {
        /// <summary>The name of the table in the database</summary>
        public string MasterTable = String.Empty;
        /// <summary>The primary key of the table</summary>
        public string MasterField = String.Empty;
        /// <summary>The field to be checked with</summary>
        public string FieldTobeChecked = String.Empty;
        /// <summary>The key in tblSettings with True or false value</summary>
        public string SettingKey = String.Empty;
    }
}
