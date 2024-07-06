using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace MSLA.Server.Utilities
{
    /// <summary>The XML Formatter Class</summary>
    public class XMLFormatter
    {
        /// <summary>XML Formattter Class. Can be used to convert all the field values of a BO into XML.</summary>
        public XMLFormatter()
        {

        }

        /// <summary>Use this method to derive the BO in XML as Bytes.</summary>
        /// <param name="BoDoc">The BO to be serialized</param>
        /// <returns>An Array of Bytes (consisting of XMl Data)</returns>
        public Byte[] Serialize(MSLA.Server.Base.VersionBase BoDoc)
        {
            Byte[] Result;
            using (System.IO.MemoryStream MStream = new System.IO.MemoryStream())
            {
                System.Xml.XmlTextWriter xmlDoc = new System.Xml.XmlTextWriter(MStream, null);
                xmlDoc.WriteStartElement(BoDoc.ToString());
                xmlDoc.WriteAttributeString("AuditTrailVersion", "2.0");
                xmlDoc.WriteAttributeString("ObjType", "Document");
                //  Write BO properties (simple properties and tables exposed as properties)
                this.WriteProperties(BoDoc, xmlDoc);
                //  Write the end element and close
                xmlDoc.WriteEndElement();
                xmlDoc.Flush();
                MStream.Flush();

                Result = new byte[MStream.Length];
                MStream.Position = 0;
                MStream.Read(Result, 0, (int)MStream.Length);
            }
            return Result;
        }

        /// <summary>Use this method to Serialize the XML data in the Memory Stream</summary>
        /// <param name="BoDoc">The BO to Serialize</param>
        /// <param name="MStream">The Memory Stream</param>
        public void Serialize(MSLA.Server.Base.VersionBase BoDoc, System.IO.Stream MStream)
        {
            System.Xml.XmlTextWriter xmlDoc = new System.Xml.XmlTextWriter(MStream, null);
            xmlDoc.WriteStartElement(BoDoc.ToString());
            this.WriteProperties(BoDoc, xmlDoc);
            xmlDoc.WriteEndElement();
            xmlDoc.Flush();
            MStream.Flush();
            MStream.Position = 0;
        }


        private void WriteProperties(Object BoDoc, System.Xml.XmlTextWriter XmlDoc)
        {
            PropertyInfo[] Properties = BoDoc.GetType().GetProperties();
            object[] Attrib;
            foreach (PropertyInfo Prop in Properties)
            {
                if (Prop.CanRead)
                {
                    Attrib = Prop.GetCustomAttributes(typeof(MSLA.Server.Base.VersionBase.VersionNotRequiredAttribute), true);
                    if (Attrib.Length == 0)
                    {

                        if (Type.GetTypeCode(Prop.PropertyType) != TypeCode.Object)
                        {
                            this.WriteValue(Prop.Name, Prop.GetValue(BoDoc, null), XmlDoc);
                        }
                        else if (Prop.GetValue(BoDoc, null) is System.Data.DataTable)
                        {
                            this.WriteTable(Prop.Name, (System.Data.DataTable)Prop.GetValue(BoDoc, null), XmlDoc);
                        }
                        //else if (Prop.GetValue(BoDoc, null) is Allocation.AllocationSaveCollection)
                        //{
                        //    this.WriteAlloc((Allocation.IAllocCollection)BoDoc, XmlDoc);
                        //}
                        else
                        {

                            this.WriteNonSerializedValue(Prop.Name, XmlDoc);
                        }
                    }
                }
            }

        }

        private void WriteTable(string PropName, System.Data.DataTable dt, System.Xml.XmlTextWriter XmlDoc)
        {
            XmlDoc.WriteStartElement(PropName);
            XmlDoc.WriteAttributeString("ObjType", "DataTable");
            int i = 0;
            foreach (System.Data.DataRow dr in dt.Rows)
            {
                i++;
                XmlDoc.WriteStartElement(PropName + "_Row");
                XmlDoc.WriteAttributeString("ObjType", "DataRow");
                XmlDoc.WriteAttributeString("RowIndex", i.ToString());
                foreach (System.Data.DataColumn Col in dt.Columns)
                {
                    if (Type.GetTypeCode(dr[Col].GetType()) != TypeCode.Object)
                    {
                        this.WriteValue(Col.ColumnName, dr[Col], XmlDoc);
                    }
                    else if (dr[Col] is System.Data.DataTable)
                    {
                        this.WriteTable(Col.ColumnName, (System.Data.DataTable)dr[Col], XmlDoc);
                    }

                }
                XmlDoc.WriteEndElement();
            }
            XmlDoc.WriteEndElement();
        }

        private void WriteValue(string PropName, object PropValue, System.Xml.XmlTextWriter XmlDoc)
        {
            XmlDoc.WriteStartElement(PropName);
            XmlDoc.WriteAttributeString("ObjType", "Property");
            if (PropValue != null)
            { XmlDoc.WriteValue(PropValue.ToString()); }
            else
            { XmlDoc.WriteValue(String.Empty); }
            XmlDoc.WriteEndElement();
        }

        private void WriteNonSerializedValue(string PropName, System.Xml.XmlTextWriter XmlDoc)
        {
            XmlDoc.WriteStartElement(PropName);
            XmlDoc.WriteAttributeString("ObjType", "Property");
            XmlDoc.WriteAttributeString("ObjState", "NonSerialized");
            XmlDoc.WriteEndElement();
        }

        //private void WriteAlloc(Allocation.IAllocCollection BoDoc, System.Xml.XmlTextWriter XmlDoc)
        //{
        //    XmlDoc.WriteStartElement("Allocations");
        //    XmlDoc.WriteAttributeString("ObjType", "Allocations");
        //    foreach (Allocation.AllocationSaveBase Alloc in BoDoc.Allocations)
        //    {
        //        XmlDoc.WriteStartElement(Alloc.ToString());
        //        XmlDoc.WriteAttributeString("ObjType", "AllocClass");
        //        this.WriteProperties(Alloc, XmlDoc);
        //        XmlDoc.WriteEndElement();
        //    }
        //    XmlDoc.WriteEndElement();
        //}
    }
}
