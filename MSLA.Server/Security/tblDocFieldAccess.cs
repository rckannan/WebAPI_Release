using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;
using System.Runtime.Serialization;

namespace MSLA.Server.Security
{
    /// <summary>This is the Field Access table used in Custom Workflow</summary>
    [Serializable]
    public class tblDocFieldAccess : DataTable, IEnumerable
    {
        /// <summary>Constructor</summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute]
        public tblDocFieldAccess()
            : base()
        {
            this.TableName = "tblDocFieldAccess";
            this.Columns.Add("fldApproveStep_ID", Type.GetType("System.Int16"));
            this.Columns.Add("fldFieldName", Type.GetType("System.String"));
            this.Columns.Add("fldFieldPath", Type.GetType("System.String"));
            this.Columns.Add("fldEnFieldAccess", Type.GetType("System.Int16"));
            this.Columns.Add("fldParentDocField_ID", Type.GetType("System.Int64"));
            this.Columns.Add("fldParentFieldName", Type.GetType("System.String"));
        }

        /// <summary>Serialization Constructor</summary>
        /// <param name="info">Serialization Info</param>
        /// <param name="context">Serialization Context</param>
        [System.Diagnostics.DebuggerNonUserCodeAttribute]
        protected tblDocFieldAccess(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }

        /// <summary>Gets the Enumerator</summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute]
        public IEnumerator GetEnumerator()
        {
            return this.Rows.GetEnumerator();
        }

        /// <summary>New Row from Builder</summary>
        /// <param name="builder">The Row Builder instance</param>
        [System.Diagnostics.DebuggerNonUserCodeAttribute]
        protected override DataRow NewRowFromBuilder(System.Data.DataRowBuilder builder)
        {
            return new tblDocFieldAccessRow(builder);
        }

        /// <summary>Gets the reflection type for the row</summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute]
        protected override Type GetRowType()
        {
            return typeof(tblDocFieldAccessRow);
        }

        /// <summary>Default Enumerator</summary>
        /// <param name="index"></param>
        /// <returns></returns>
        [System.Diagnostics.DebuggerNonUserCodeAttribute]
        public tblDocFieldAccessRow this[int index]
        {
            get { return (tblDocFieldAccessRow)this.Rows[index]; }
        }

        /// <summary>tblDocFieldAccessRow</summary>
        [Serializable]
        public class tblDocFieldAccessRow : DataRow
        {
            /// <summary>Constructor</summary>
            /// <param name="rb">Row Builder</param>
            [System.Diagnostics.DebuggerNonUserCodeAttribute]
            protected internal tblDocFieldAccessRow(System.Data.DataRowBuilder rb)
                : base(rb)
            {

            }

            /// <summary>Gets or Sets the Approve Step ID</summary>
            public Int16 fldApproveStep_ID
            {
                get { return (Int16)this["fldApproveStep_ID"]; }
                set { this["fldApproveStep_ID"] = value; }
            }

            /// <summary>Gets or Sets the Field Name</summary>
            public String fldFieldName
            {
                get { return (String)this["fldFieldName"]; }
                set { this["fldFieldName"] = value; }
            }

            /// <summary>Gets or Sets the Field Path</summary>
            public String fldFieldPath
            {
                get { return (String)this["fldFieldPath"]; }
                set { this["fldFieldPath"] = value; }
            }

            /// <summary>Gets or Sets the Field Access Level</summary>
            public EnFieldAccess fldEnFieldAccess
            {
                get { return (EnFieldAccess)(Int16)this["fldEnFieldAccess"]; }
                set { this["fldEnFieldAccess"] = value; }
            }

            /// <summary>Gets or Sets the Parent Doc Field ID</summary>
            public Int64 fldParentDocField_ID
            {
                get { return (Int64)this["fldParentDocField_ID"]; }
                set { this["fldParentDocField_ID"] = value; }
            }

            /// <summary>Gets or Sets the Parent Field Name</summary>
            public String fldParentFieldName
            {
                get { return (String)this["fldParentFieldName"]; }
                set { this["fldParentFieldName"] = value; }
            }
        }
    }
}
