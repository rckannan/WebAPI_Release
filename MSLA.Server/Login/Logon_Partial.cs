using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Runtime.Serialization;
using System.Data;

namespace MSLA.Server.Login
{

    public partial class Logon
    {
        /// <summary>Collection of Main DB</summary>
        [System.Diagnostics.DebuggerNonUserCode]
        [Serializable]
        public class tblMainDB
            : DataTable, IEnumerable
        {
            /// <summary>Constructor</summary>            
            public tblMainDB()
                : base()
            {
                this.TableName = "tblMainDB";
                this.Columns.Add("name", Type.GetType("System.String"));
            }

            /// <summary>Constructor</summary>
            /// <param name="info">Serialization Info</param>
            /// <param name="context">Streamong Context</param>
            protected tblMainDB(SerializationInfo info, StreamingContext context)
                : base(info, context)
            {

            }

            /// <summary>The public enumerator</summary>
            public IEnumerator GetEnumerator()
            {
                return this.Rows.GetEnumerator();
            }

            /// <summary>Builds a New Row</summary>
            /// <param name="builder">The builder</param>
            protected override DataRow NewRowFromBuilder(System.Data.DataRowBuilder builder)
            {
                return new tblMainDBRow(builder);
            }

            /// <summary>Gets the Reflection Row Type</summary>
            protected override Type GetRowType()
            {
                return typeof(tblMainDBRow);
            }

            /// <summary>The default Iterator</summary>
            /// <param name="index">The Index</param>
            public tblMainDBRow this[int index]
            {
                get { return ((tblMainDBRow)(this.Rows[index])); }
            }

            /// <summary>The Main Row Class</summary>
            [Serializable]
            public class tblMainDBRow
                : DataRow
            {
                /// <summary>Constructor</summary>
                /// <param name="rb">Row Builder</param>
                protected internal tblMainDBRow(System.Data.DataRowBuilder rb)
                    : base(rb)
                {

                }

                /// <summary>The Database Name</summary>
                public string fldDatabase
                {
                    get { return ((string)(this["name"])); }
                    set { this["name"] = value; }
                }
            }

        }
    }
}
