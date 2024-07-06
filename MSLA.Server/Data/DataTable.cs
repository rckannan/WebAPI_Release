using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MSLA.Server.Data
{
    [System.Runtime.Serialization.DataContract]
    public class DataTable : IEnumerable, IDisposable
    {
        List<DataColumn> columns = null;
        [System.Runtime.Serialization.DataMember]
        public List<DataColumn> Columns
        {
            get
            {
                if (columns == null)
                {
                    columns = new List<DataColumn>();
                }

                return columns;
            }
            set
            {
                columns = value;
            }
        }

        List<DataRow> rows = null;
        [System.Runtime.Serialization.DataMember]
        public List<DataRow> Rows
        {
            get
            {
                if (rows == null)
                {
                    rows = new List<DataRow>();
                }

                return rows;
            }
            set
            {
                rows = value;
            }
        }

        public object NewRow()
        {
            if (queryable != null)
            {
                return Activator.CreateInstance(queryable.ElementType);
            }

            return null;
        }

        #region IEnumerable Members

        IQueryable queryable = null;
        public IEnumerator GetEnumerator()
        {
            if (queryable == null)
            {
                var type = ClassFactory.Instance.GetDynamicClass(this.Columns.Select(c => new DynamicProperty(c.ColumnName, c.DataType)));
                var propertyInfos = type.GetProperties().ToList();

                var list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(type));
                foreach (var row in this.Rows)
                {
                    var item = Activator.CreateInstance(type);
                    propertyInfos.ForEach(p => p.SetValue(item, row[p.Name], null));

                    list.Add(item);
                }

                queryable = list.AsQueryable();
            }

            return queryable.GetEnumerator();
        }

        #endregion

        public IList ToList()
        {
            var enumerator = GetEnumerator();
            var list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(queryable.ElementType));
            while (enumerator.MoveNext())
            {
                list.Add(enumerator.Current);
            }
            return list;
        }

        static Type GetNonNullableType(Type type)
        {
            return IsNullableType(type) ? type.GetGenericArguments()[0] : type;
        }

        static bool IsNullableType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        public void Dispose()
        {
            columns.Clear();
            columns = null;
            Rows.Clear();
            Rows = null;
        }
    }

    public class DataColumn
    {
        public DataColumn()
        {
            DataType = typeof(object);
        }

        [System.Runtime.Serialization.DataMember]
        public Type DataType { get; set; }

        [System.Runtime.Serialization.DataMember]
        public string ColumnName { get; set; }
    }

 
    public class DataRow : Dictionary<string, object>
    {
        //
    }
}
