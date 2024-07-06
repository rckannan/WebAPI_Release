using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace MSLA.Server.Data
{
    public partial class DataPortal
    {
        private static DataPortal _DataPortal;
        internal DataPortal()
        {
        }

        public static DataPortal CreateDataPortal()
        {
            if (_DataPortal == null)
            {
                _DataPortal = new DataPortal();
            }
            return _DataPortal;
        }

        /// <summary>This is used to Create an instance of the Master BO</summary>
        /// <param name="Criteria">The Criteria for Master BO</param>
        /// <param name="User">User Info</param>
        /// <returns>Returns Master BO Instance</returns>
        public BO.MasterBase FetchMaster(BO.IMasterCriteria Criteria, Security.IUser User)
        {

            // ****  Create an instance of the business object
            BO.MasterBase CreatedObject = CreateBusinessObject(Criteria, User);

            //   ****    Apply AccessRights on this Object
            Security.AccessRights AR = new Security.AccessRights();
            AR.Assert((Security.IARMaster)CreatedObject, User);

            // ****  Tell the business object to fetch its data
            CreatedObject.FetchOrCreate(Criteria);

            //  ****    Create Version 
            CreatedObject.CreateVersion();

            // return the populated business object as a result
            return CreatedObject;
        } 
        

        /// <summary>This method can be used to save the Master Bo</summary>
        /// <param name="MasterDoc">The Instance of Master BO</param>
        /// <param name="User">User Info</param>
        /// <returns>The saved instance of master BO</returns>
        public Base.MasterSaveBase SaveMaster(Base.MasterSaveBase MasterDoc, Security.IUser User)
        {
            // ****  Tell the business object to Save its data
            MasterDoc.SaveDocument();

            // return the populated business object as a result
            return MasterDoc;
        }

        /// <summary>This method can be used to delete the Master Item</summary>
        /// <param name="MasterDoc">The Master BO</param>
        /// <param name="User">user Info</param>
        public Base.MasterSaveBase DeleteMaster(Base.MasterSaveBase MasterDoc, Security.IUser User)
        {
            // ****  Tell the business object to Save its data
            MasterDoc.DeleteDocument();

            // return the populated business object as a result
            return MasterDoc;
        }

        #region "Creating Master Business Object"

        private BO.MasterBase CreateBusinessObject(BO.IMasterCriteria Criteria, Security.IUser UserInfo)
        {
            // get the type of the actual business object
            Data.DataAccess DALocal = new Data.DataAccess();

            Base.DocMaster ObjType = DALocal.FetchDocMaster(Criteria.DocMasterType, UserInfo);

            // create an instance of the business object
            object Document;
            Document = Utilities.ReflectionHelper.CreateObject(ObjType.DocAssembly, ObjType.DocNameSpace, ObjType.DocObjectName, new Object[] { UserInfo });
            ((BO.MasterBase)Document).SetObjectInfo(ObjType);

            return (BO.MasterBase)Document;
        }

       

        #endregion

        #region "DBConnector Code"

        /// <summary>This method can be used to save the DBConnector Base</summary>
        /// <param name="DBConDoc">The Instance of class inherited from DBConnector</param>
        /// <param name="User">User Info</param>
        /// <returns>The saved instance of master BO</returns>
        public Data.DBConnectorBase SaveDBConnector(Data.DBConnectorBase DBConDoc, Security.IUser User)
        {
            // ****  Tell the business object to Save its data
            DBConDoc.SaveDocument();
            return DBConDoc;
        }


        #endregion
    }
}
