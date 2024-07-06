using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace MSLA.Client.Data
{
     public class DataTable  : IEnumerable
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

        public DataRow this[int i]
        {
            get { return Rows[i]; }
        }


        public IQueryable updatedRows
        {
            get { return queryable; }
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
            return Rows.GetEnumerator();

            //if (queryable == null)
            //{
            //    var type = ClassFactory.Instance.GetDynamicClass(this.Columns.Select(c => new DynamicProperty(c.ColumnName, c.DataType)));
            //    var propertyInfos = type.GetProperties().ToList();

            //    var list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(type));

            //    foreach (var row in this.Rows)
            //    {
            //        var item = Activator.CreateInstance(type);


            //        foreach (var p in propertyInfos)
            //        {
            //            if (row.ContainsKey(p.Name))
            //            {
            //                object obj1 = row[p.Name];
            //                if (obj1 != System.DBNull.Value)
            //                {
            //                    p.SetValue(item, row[p.Name], null);
            //                }
            //            }
            //        }

            //        list.Add(item);
            //    }

            //    //foreach (var row in this.Rows)
            //    //{
            //    //    var item = Activator.CreateInstance(type);
            //    //    propertyInfos.ForEach(p => p.SetValue(item, row[p.Name], null));

            //    //    list.Add(item);
            //    //}

            //    queryable = list.AsQueryable();
            //}

            //return queryable.GetEnumerator();

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
        public DataRow()
            : base()
        {

        }

        public DataRow(Dictionary<string, object> item)
            : base(item)
        {

        }

        public RowBag RowValue
        {
            get { return new RowBag(this); }
        }


        public class RowBag
        {
            private DataRow Parent;
            public RowBag(DataRow parent)
            {
                Parent = parent;
            }

            public object this[String key]
            {
                get{return Parent[key];}
                set{Parent[key]=value;}
            }
        }

    }
}
