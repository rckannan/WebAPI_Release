using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace MSLA.Server.Data
{
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(System.DBNull))]
    [System.Runtime.Serialization.KnownType(typeof(SimpleTable))]
    [System.Runtime.Serialization.DataContract]
    public class SimpleTable: IDisposable
    {
        private List<Dictionary<string, object>> _internalRows = new List<Dictionary<string,object>>();
        private Dictionary<string, string> _Columns = new Dictionary<string, string>();

        public SimpleTable()
        {
            _internalRows = new List<Dictionary<string, object>>();
            _Columns = new Dictionary<string, string>();
        }

        [System.Runtime.Serialization.DataMember]
        public Dictionary<string, string> Columns 
        {
            get
            {
                if (_Columns == null)
                {
                    _Columns = new Dictionary<string, string>();
                }
                return _Columns;
            }
        }

        [System.Runtime.Serialization.DataMember]
        public Dictionary<string, object>[] Rows 
        { 
            get 
            {
                if (_internalRows == null)
                {
                    _internalRows = new List<Dictionary<string, object>>();
                }
                return _internalRows.ToArray(); 
            }
            set 
            {
                if (_internalRows == null)
                {
                    _internalRows = new List<Dictionary<string, object>>();
                }
                _internalRows.AddRange(value); 
            }
        }

        public void AddRow(Dictionary<string, object> item)
        {
            _internalRows.Add(item);
        }

        public void AddColumn(string colName, string colDataType)
        {
            _Columns.Add(colName, colDataType);
        }

        public void Dispose()
        {
            _internalRows.Clear(); 
            _internalRows = null;
            _Columns.Clear();
            _Columns = null;

            //EventLog.WriteEntry("Simple table", "Disponse invoked.." + DateTime.Now.ToString("dd MMM yyyy HH:mm:ss"));
        }
    }
}
