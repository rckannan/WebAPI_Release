using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;
using System.Runtime.Serialization;

namespace MSLA.Server.Base
{
    public abstract partial class WorkflowBase
    {
        /// <summary>This is a strogly typed table that represents the document routing information.</summary>
        [Serializable]
        public class tblDocWorkflowLevel : DataTable, IEnumerable
        {
            /// <summary>Constructor</summary>
            [System.Diagnostics.DebuggerNonUserCodeAttribute]
            public tblDocWorkflowLevel()
                : base()
            {
                this.TableName = "tblDocWorkflowLevel";
                this.Columns.Add("fldApproveStep_ID", Type.GetType("System.Int16"));
                this.Columns.Add("fldRole_ID", Type.GetType("System.Int64"));
                this.Columns.Add("fldEnAccessLevel", Type.GetType("System.Int16"));
                this.Columns.Add("fldIsInclusive", Type.GetType("System.Boolean"));

            }

            /// <summary>Gets the very first step's Role in the routing table.</summary>
            /// <returns>Role ID</returns>
            public Int64 GetDataEntryRole()
            {
                DataRow[] drs = this.Select("fldApproveStep_ID=1");
                if (drs.Length == 1)
                {
                    return Convert.ToInt64(drs[0]["fldRole_ID"]);
                }
                else
                {
                    throw new Exception("Failed to resove Data Entry Role");
                }
            }

            /// <summary>Get The last step's Role in the routing table.</summary>
            /// <returns>Role ID</returns>
            public Int64 GetAuthoriseRole()
            {
                DataRow[] drs = this.Select("Max(fldApproveStep_ID)");
                if (drs.Length == 1)
                {
                    return Convert.ToInt64(drs[0]["fldRole_ID"]);
                }
                else
                {
                    throw new Exception("Failed to resove Authorise/Post Role");
                }
            }

            /// <summary>Gets the Role that is authorised to conduct the step.</summary>
            /// <param name="ApproveStep_ID">The step Id to query for.</param>
            /// <returns>Role ID</returns>
            public Int64 GetStepRole(Int16 ApproveStep_ID)
            {
                if (ApproveStep_ID == -1 || ApproveStep_ID == 99)
                { return -1; } // Known that it is the limit

                if (ApproveStep_ID == 0)
                {   //  ****This is the step that is created when an existing document is rejected to the Data Entry Role
                    DataRow[] drSteps = this.Select("fldApproveStep_ID=1");
                    if (drSteps.Length == 1)
                    {
                        return Convert.ToInt64(drSteps[0]["fldRole_ID"]);
                    }
                }

                // **** In all other cases, return the actual Role for the Step.
                DataRow[] drs = this.Select("fldApproveStep_ID=" + ApproveStep_ID.ToString());
                if (drs.Length == 1)
                {
                    return Convert.ToInt64(drs[0]["fldRole_ID"]);
                }
                else
                {
                    throw new Exception("Failed to resove the associated Step Role");
                }
            }

            /// <summary>Returns True if the step is last step.</summary>
            /// <param name="Step_ID">The step to query</param>
            /// <returns>True for last step, else False</returns>
            public bool IsLastStep(Int16 Step_ID)
            {
                DataRow[] drs = this.Select("fldApproveStep_ID>" + Step_ID.ToString());
                if (drs.Length == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            /// <summary>Serialization Constructor</summary>
            /// <param name="info">Serialization Info</param>
            /// <param name="context">Serialization Context</param>
            [System.Diagnostics.DebuggerNonUserCodeAttribute]
            protected tblDocWorkflowLevel(SerializationInfo info, StreamingContext context)
                : base(info, context)
            {

            }

            /// <summary>Enumerator</summary>
            /// <returns>The collection of Rows</returns>
            [System.Diagnostics.DebuggerNonUserCodeAttribute]
            public IEnumerator GetEnumerator()
            {
                return this.Rows.GetEnumerator();
            }

            /// <summary>Constructor for New Row</summary>
            /// <param name="builder">The Builder Info</param>
            /// <returns>A newly created row</returns>
            [System.Diagnostics.DebuggerNonUserCodeAttribute]
            protected override DataRow NewRowFromBuilder(System.Data.DataRowBuilder builder)
            {
                return new tblDocWorkflowLevelRow(builder);
            }

            /// <summary>Gets the Row Type</summary>
            /// <returns>System Type</returns>
            [System.Diagnostics.DebuggerNonUserCodeAttribute]
            protected override Type GetRowType()
            {
                return typeof(tblDocWorkflowLevelRow);
            }

            /// <summary>Default Property</summary>
            /// <param name="index">Index</param>
            /// <returns>row instance</returns>
            [System.Diagnostics.DebuggerNonUserCodeAttribute]
            public tblDocWorkflowLevelRow this[int index]
            {
                get { return (tblDocWorkflowLevelRow)this.Rows[index]; }
            }

            /// <summary>Strongly typed row</summary>
            [Serializable]
            public class tblDocWorkflowLevelRow : DataRow
            {
                /// <summary>Constructor</summary>
                /// <param name="rb">the rowbuilder</param>
                [System.Diagnostics.DebuggerNonUserCodeAttribute]
                protected internal tblDocWorkflowLevelRow(System.Data.DataRowBuilder rb)
                    : base(rb)
                {

                }

                /// <summary>The Approved Step of the Document.</summary>
                public Int16 fldApproveStep_ID
                {
                    get { return (Int16)this["fldApproveStep_ID"]; }
                    set { this["fldApproveStep_ID"] = value; }
                }

                /// <summary>The Approved Step related Role</summary>
                public Int64 fldRole_ID
                {
                    get { return (Int64)this["fldRole_ID"]; }
                    set { this["fldRole_ID"] = value; }
                }

                /// <summary>The Access Level to the Document</summary>
                public MSLA.Server.Security.EnAccessLevel fldEnAccessLevel
                {
                    get { return (MSLA.Server.Security.EnAccessLevel)(Int16)this["fldEnAccessLevel"]; }
                    set { this["fldEnAccessLevel"] = value; }
                }

                /// <summary>Is Inclusive</summary>
                public Boolean fldIsInclusive
                {
                    get { return (Boolean)this["fldIsInclusive"]; }
                    set { this["fldIsInclusive"] = value; }
                }


            }
        }

        /// <summary>This is a strogly typed table that represents role information associated with the connected user.</summary>
        [Serializable]
        public class tblUserWithRole : DataTable, IEnumerable
        {
            /// <summary>Constructor</summary>
            [System.Diagnostics.DebuggerNonUserCodeAttribute]
            public tblUserWithRole()
                : base()
            {
                this.TableName = "tblUserWithRole";
                this.Columns.Add("fldRole_ID", Type.GetType("System.Int64"));
                this.Columns.Add("fldUser_ID", Type.GetType("System.Int64"));
            }

            /// <summary>Serialization Constructor</summary>
            /// <param name="info">Serialization Info</param>
            /// <param name="context">Serialization Context</param>
            [System.Diagnostics.DebuggerNonUserCodeAttribute]
            protected tblUserWithRole(SerializationInfo info, StreamingContext context)
                : base(info, context)
            {

            }

            /// <summary>Enumerator</summary>
            /// <returns>The collection of Rows</returns>
            [System.Diagnostics.DebuggerNonUserCodeAttribute]
            public IEnumerator GetEnumerator()
            {
                return this.Rows.GetEnumerator();
            }

            /// <summary>Constructor for New Row</summary>
            /// <param name="builder">The Builder Info</param>
            /// <returns>A newly created row</returns>
            [System.Diagnostics.DebuggerNonUserCodeAttribute]
            protected override DataRow NewRowFromBuilder(System.Data.DataRowBuilder builder)
            {
                return new tblUserWithRoleRow(builder);
            }

            /// <summary>Gets the Row Type</summary>
            /// <returns>System Type</returns>
            [System.Diagnostics.DebuggerNonUserCodeAttribute]
            protected override Type GetRowType()
            {
                return typeof(tblUserWithRoleRow);
            }

            /// <summary>Default Property</summary>
            /// <param name="index">Index</param>
            /// <returns>row instance</returns>
            [System.Diagnostics.DebuggerNonUserCodeAttribute]
            public tblUserWithRoleRow this[int index]
            {
                get { return (tblUserWithRoleRow)this.Rows[index]; }
            }

            /// <summary>Strongly typed row for tblUserWithRole</summary>
            [Serializable]
            public class tblUserWithRoleRow : DataRow
            {
                /// <summary>Constructor</summary>
                /// <param name="rb">the rowbuilder</param>
                [System.Diagnostics.DebuggerNonUserCodeAttribute]
                protected internal tblUserWithRoleRow(System.Data.DataRowBuilder rb)
                    : base(rb)
                {

                }

                /// <summary>Role ID associated with the User ID</summary>
                public Int64 fldRole_ID
                {
                    get { return (Int64)this["fldRole_ID"]; }
                    set { this["fldRole_ID"] = value; }
                }

                /// <summary>User ID</summary>
                public Int64 fldUser_ID
                {
                    get { return (Int64)this["fldUser_ID"]; }
                    set { this["fldUser_ID"] = value; }
                }

            }
        }
    }
}
