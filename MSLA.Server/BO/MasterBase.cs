using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Data.SqlClient;

namespace MSLA.Server.BO
{
    ///<summary>This class is the base class for all master business objects required to be created. Inherit this class to create your own Master Business Object.</summary>
    [System.Diagnostics.DebuggerStepThrough()]
    [Serializable()]
    public abstract class MasterBase
        : Base.MasterSaveBase
    {
        /// <summary>Constructor. Uses default Company Connection</summary>
        /// <param name="User">User Info</param>
        protected MasterBase(MSLA.Server.Security.IUser User)
            : this(User, MSLA.Server.Data.DBConnectionType.CompanyDB)
        {

        }

        /// <summary>Constructor</summary>
        /// <param name="User">User Info</param>
        /// <param name="DBType">The Connection Type</param>
        protected MasterBase(MSLA.Server.Security.IUser User, Data.DBConnectionType DBType)
            : base(User, DBType)
        {

        }

        ///<summary>Shared Member: Creates a new instance of a data filled Business Object on the server and returns the same to the client.</summary>
        ///<param name="MastCriteria">The Criteria required to create an instance and fetch data based on the criteria.</param>
        ///<param name="User">The User Details (Session Cookie) received on authentication.</param>
        ///<returns>Returns an object of type 'Master Base'</returns>
        public static MasterBase DataPortal_Fetch(IMasterCriteria MastCriteria, Security.IUser User)
        {
            Data.DataPortal SaveManager = Data.DataPortal.CreateDataPortal();
            return (MasterBase)(SaveManager.FetchMaster(MastCriteria, User));
        }

        ///<summary>Saves the current instance of the Business Object by serializing it to the Server. 
        ///Raises a 'ValidationException' if Business Rules are broken.
        ///Calls the MustOverride Methods 'SaveControlTran' And 'MakeLogEntry'.</summary>
        ///<returns>Returns the deserialized object sent to the server after successful save. The returned object would contain data after save.</returns>
        public MasterBase DataPortal_Save()
        {
            //   ****    Validate and throw exceptions if any
            this.DoSaveValidations();

            //   ****    Proceed with Save
            Data.DataPortal SaveManager = Data.DataPortal.CreateDataPortal();
            return (MasterBase)(SaveManager.SaveMaster(this, this.UserInfo));
        }

        ///<summary>Deletes the current document (Record(s) on SQL Server) on the server. 
        ///Raises a 'ValidationException' if Business Rules are broken.
        ///Calls the MustOverride Methods 'DeleteControlTran' And 'MakeLogEntry'.</summary>
        ///<returns>Returns the deserialized object sent to the server after successful delete. The returned object would contain data after delete.</returns>
        public MasterBase DataPortal_Delete()
        {
            //   ****    Validate and throw exceptions if any
            this.DoDeleteValidations();

            //   ****    Proceed with Save
            Data.DataPortal SaveManager = Data.DataPortal.CreateDataPortal();
            return (MasterBase)(SaveManager.DeleteMaster(this, this.UserInfo));
        }

        /// <summary>Override this method to Create or Fetch the fields from the DB</summary>
        /// <param name="MastCriteria">The Master Criteria</param>
        protected internal abstract void FetchOrCreate(IMasterCriteria MastCriteria);

        public Base.SimpleBOMaster ConstructSimpleBO()
        {
            Base.SimpleBOMaster myBO = new Base.SimpleBOMaster();
            myBO.fldMasterItem_ID = this.DocObjectInfo.DocObject_ID;
            myBO.MasterType = this.DocObjectInfo.DocObjectType;
            myBO.fldSerializedMaster_ID = Guid.NewGuid();
            myBO.AccessLevel = this.AccessLevel;
            myBO.AuditMode = this.AuditMode;
            myBO.BrokenDeleteRules = this.BrokenDeleteRules;
            myBO.BrokenSaveRules = this.BrokenSaveRules;
            myBO.DocObjectInfo = this.DocObjectInfo;
            myBO.DocObjectIC_ID = this.GetDocObjectIC_ID;
            myBO.IsDeleteAllowed = this.IsDeleteAllowed;
            myBO.VersionInfo = this.GetVersionInfo;
            myBO.UserInfo.User_ID = this.UserInfo.User_ID;
            myBO.UserInfo.UserName = this.UserInfo.FullUserName;
            myBO.UserInfo.Session_ID = this.UserInfo.Session_ID;
            //persist class to database

            Serialize(myBO, this);

            // get all public static properties of MyClass type
            PropertyInfo[] propertyInfos;
            propertyInfos = this.GetType().GetProperties(BindingFlags.Public |
                                                          BindingFlags.Static | BindingFlags.Instance);

            PropertyInfo[] basePropertyInfos;
            basePropertyInfos = typeof(MasterBase).GetProperties(BindingFlags.Public |
                                                          BindingFlags.Static | BindingFlags.Instance);

            // sort properties by name
            Array.Sort(propertyInfos,
                    delegate(PropertyInfo propertyInfo1, PropertyInfo propertyInfo2)
                    { return propertyInfo1.Name.CompareTo(propertyInfo2.Name); });

            Array.Sort(basePropertyInfos,
                   delegate(PropertyInfo propertyInfo1, PropertyInfo propertyInfo2)
                   { return propertyInfo1.Name.CompareTo(propertyInfo2.Name); });


            Boolean InBase = false;
            // write property names
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                foreach (PropertyInfo basePropertyInfo in basePropertyInfos)
                {
                    InBase = false;
                    if (propertyInfo.Name == basePropertyInfo.Name)
                    {
                        InBase = true;
                        break;
                    }
                }
                if (InBase == false)
                {

                    if (!propertyInfo.CanWrite)
                    {
                        myBO.PropertyIsReadOnly.Add(propertyInfo.Name, true);
                    }

                    if (propertyInfo.PropertyType.IsEnum)
                    {
                        object enumval = Convert.ChangeType(Utilities.ReflectionHelper.GetPropertyValue(this, propertyInfo.Name), Enum.GetUnderlyingType(propertyInfo.PropertyType));
                        myBO.PropertyBag.Add(propertyInfo.Name, enumval);
                    }
                    else if (propertyInfo.GetValue(this, null) is System.Data.DataTable)
                    {
                        myBO.PropertyBag.Add(propertyInfo.Name, Data.DataConnect.ResolveToSimpleTable((System.Data.DataTable)Utilities.ReflectionHelper.GetPropertyValue(this, propertyInfo.Name)));
                    }
                    else if (propertyInfo.PropertyType.IsValueType || propertyInfo.PropertyType == typeof(System.String) || propertyInfo.PropertyType == typeof(System.Byte[]))
                    {
                        myBO.PropertyBag.Add(propertyInfo.Name, Utilities.ReflectionHelper.GetPropertyValue(this, propertyInfo.Name));
                    }
                    //else if(propertyInfo.PropertyType.IsValueType)
                    //{
                    //    myBO.PropertyBag.Add(propertyInfo.Name, Utilities.ReflectionHelper.GetPropertyValue(this, propertyInfo.Name));
                    //}
                    else
                    {
                        //do nothing
                    }
                }
            }
            return myBO;
        }

        public static MasterBase ConstructMasterBO(Base.SimpleBOMaster myBO, Security.IUser UserInfo)
        {
            Byte[] obj_bytes;
            SqlCommand cmm = new SqlCommand();
            SqlDataReader rdr;
            using (SqlConnection cn = Data.DataAccess.GetCn(Data.DBConnectionType.MainDB))
            {
                cmm.Connection = cn;
                if (cn.State == System.Data.ConnectionState.Closed)
                {
                    cn.Open();
                }

                cmm.CommandType = System.Data.CommandType.StoredProcedure;
                cmm.CommandText = "System.spSerializedMasterFetch";
                cmm.Parameters.Add("@SM_ID", System.Data.SqlDbType.UniqueIdentifier).Value = myBO.fldSerializedMaster_ID;
                rdr = cmm.ExecuteReader();

                MasterBase myMaster;

                while (rdr.Read())
                {
                    if (rdr[0] != System.DBNull.Value)
                    {
                        obj_bytes = new Byte[((Byte[])rdr[0]).Length];
                        obj_bytes = (Byte[])rdr[0];
                        System.IO.MemoryStream MStream = new System.IO.MemoryStream();
                        MStream.Write(obj_bytes, 0, obj_bytes.Length);
                        MStream.Flush();
                        MStream.Position = 0;
                        System.Runtime.Serialization.Formatters.Binary.BinaryFormatter sbf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                        object obj = sbf.Deserialize(MStream);
                        myMaster = (MasterBase)obj;

                        foreach (KeyValuePair<string, object> keyval in myBO.PropertyBag)
                        {
                            if (keyval.Value == null)
                            {
                                Utilities.ReflectionHelper.SetPropertyValue(myMaster, keyval.Key, keyval.Value);
                            }
                            else if (keyval.Value.GetType().IsEnum)
                            {
                                if (!myBO.PropertyIsReadOnly.ContainsKey(keyval.Key))
                                {
                                    Utilities.ReflectionHelper.SetPropertyValue(myMaster, keyval.Key, keyval.Value);
                                }
                            }
                            else if (keyval.Value is Data.SimpleTable)
                            {
                                object obj1 = Utilities.ReflectionHelper.GetPropertyValue(myMaster, keyval.Key);
                                ((System.Data.DataTable)obj1).Rows.Clear();
                                System.Data.DataRow drNew;

                                foreach (KeyValuePair<string, string> dc in ((keyval.Value) as Data.SimpleTable).Columns)
                                {
                                    if (((System.Data.DataTable)obj1).Columns.Contains(dc.Key))
                                    {
                                    }
                                    else
                                    {
                                        ((System.Data.DataTable)obj1).Columns.Add(dc.Key, Type.GetType(dc.Value));
                                    }
                                }

                                foreach (Dictionary<string, object> dr in ((keyval.Value) as Data.SimpleTable).Rows)
                                {
                                    drNew = ((System.Data.DataTable)obj1).NewRow();
                                    foreach (KeyValuePair<string, string> dc in ((keyval.Value) as Data.SimpleTable).Columns)
                                    {
                                        if (dr[dc.Key] == null)
                                        {
                                            drNew[dc.Key] = System.DBNull.Value;
                                        }
                                        else
                                        {
                                            drNew[dc.Key] = dr[dc.Key];
                                        }
                                    }
                                    ((System.Data.DataTable)obj1).Rows.Add(drNew);
                                }
                                ((System.Data.DataTable)obj1).AcceptChanges();
                            }
                            else if (keyval.Value.GetType().IsValueType || keyval.Value.GetType() == typeof(System.String) || keyval.Value.GetType() == typeof(System.Byte[]))
                            {
                                PropertyInfo myProperty = myMaster.GetType().GetProperty(keyval.Key);
                                if (myProperty.CanWrite)
                                {
                                    if (!myBO.PropertyIsReadOnly.ContainsKey(keyval.Key))
                                    {
                                        Utilities.ReflectionHelper.SetPropertyValue(myMaster, keyval.Key, keyval.Value);
                                    }
                                }
                            }
                        }
                        return myMaster;
                    }
                }
            }
            return null;
        }

        internal void Serialize(Base.SimpleBOMaster myBO, MasterBase myMaster)
        {
            System.IO.MemoryStream MStream = new System.IO.MemoryStream();
            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter sbf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            sbf.Serialize(MStream, myMaster);
            Byte[] obj_bytes;
            obj_bytes = new Byte[MStream.Length];
            MStream.Position = 0;
            MStream.Read(obj_bytes, 0, (int)MStream.Length);
            MStream.Flush();
            MStream.Close();

            SqlCommand cmm = new SqlCommand();
            cmm.CommandType = System.Data.CommandType.StoredProcedure;
            cmm.CommandText = "System.spSerializedMasterAdd";

            cmm.Parameters.Add("@SM_ID", System.Data.SqlDbType.UniqueIdentifier).Value = myBO.fldSerializedMaster_ID;
            cmm.Parameters.Add("@S_Value", System.Data.SqlDbType.VarBinary, obj_bytes.Length).Value = obj_bytes;
            Data.DataConnect.ExecCMM(myMaster.UserInfo, ref cmm, Data.DBConnectionType.MainDB);
        }
    }
}
