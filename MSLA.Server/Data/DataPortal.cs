using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MSLA.Server.Data
{
    /// <summary>Server Data Portal. Anchored Object on the server</summary>
    [System.Diagnostics.DebuggerStepThrough()]
    public partial class DataPortal
        : MarshalByRefObject
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

        #region "Business Object Code"

        /// <summary>This method actually fetches an instance of the object from the server based upon the Criteria</summary>
        /// <param name="Criteria">The Criteria to fetch the document</param>
        /// <param name="UserInfo">The Logged in User Info</param>
        public BO.DocumentBase Fetch(BO.IDocCriteria Criteria, Security.IUser UserInfo)
        {
            // ****  Create an instance of the business object
            BO.DocumentBase CreatedObject = CreateBusinessObject(Criteria, UserInfo);

            //  ****    Create Allocation Instance
            //this.CreateAllocInstance(CreatedObject, Criteria, UserInfo);

            // ****  Tell the business object to fetch its data
            CreatedObject.FetchOrCreate(Criteria);

            // **** Fetch the status if the document is opend for edit
            if (CreatedObject.fldVoucher_ID != String.Empty)
            { CreatedObject.GetWorkflowES(new MSLA.Server.Base.SaveResult(CreatedObject.fldVoucher_ID)); }

            //  ****    Tell the Allocations within the BO to Fetch
            //foreach (Allocation.AllocationSaveBase Alloc in CreatedObject.Allocations)
            //{ Alloc.FetchAlloc(Criteria); }

            //   ****    Apply AccessRights on the Opened Document 
            //   ****    Apply AccessRights on this Object
            Security.AccessRights AR = new Security.AccessRights();
            AR.Assert((Security.IARDocument)CreatedObject, UserInfo);

            //  ****    Set the AuditMode
            SetAuditMode(CreatedObject, UserInfo);

            //  ****    Create Version 
            CreatedObject.CreateVersion();

            //  ****    After having successfully created the object, set the Instance Creation (IC) ID
            if (CreatedObject.fldVoucher_ID == String.Empty)
            { SetInstanceCreationControl(CreatedObject, UserInfo); }

            // return the populated business object as a result
            return CreatedObject;
        }

        /// <summary>Saves the Document by calling its appropriate methods.</summary>
        /// <param name="Document">The Document to be saved</param>
        /// <param name="DoAction">The Action to perform for Status while saving</param>
        /// <param name="UserInfo">The User Ifo login session</param>
        public Base.SaveBase Save(Base.SaveBase Document, Base.SaveBase.InnerWorkFlow DoAction, Security.IUser UserInfo)
        {
            // ****  Tell the business object to Save its data
            Document.SaveDocument(DoAction);

            //   ****    Apply AccessRights on this Object
            Security.AccessRights AR = new Security.AccessRights();
            AR.Assert((Security.IARDocument)Document, UserInfo);

            // return the populated business object as a result
            return Document;
        }

        /// <summary>This method can be called to modify a document after save. Ensure that this is done very carefully</summary>
        /// <param name="Document">The Documnet to be saved</param>
        /// <param name="UserInfo">The User Info</param>
        /// <param name="Param">Any Serializable Class or structure</param>
        /// <returns>Returns the BO after save</returns>
        public Base.SaveBase ModifyAfterSave(Base.SaveBase Document, Security.IUser UserInfo, Object Param)
        {
            // ****  Tell the business object to Save its data
            Document.AfterPostAction(Param);

            //   ****    Apply AccessRights on this Object
            Security.AccessRights AR = new Security.AccessRights();
            AR.Assert((Security.IARDocument)Document, UserInfo);

            // return the populated business object as a result
            return Document;
        }

        /// <summary>Deletes a document by calling its delete method</summary>
        /// <param name="Document">The Document to delete</param>
        /// <param name="UserInfo">The User Info Login session information</param>
        public bool Delete(Base.SaveBase Document, Security.IUser UserInfo)
        {
            // ****  Tell the business object to Delete its data
            return Document.DeleteDocument();
        }

        #endregion

        #region "Creating Business Object"
        private BO.DocumentBase CreateBusinessObject(BO.IDocCriteria Criteria, Security.IUser UserInfo)
        {
            // get the type of the actual business object
            Data.DataAccess DataAccessLocal = new Data.DataAccess();
            Base.DocObject ObjType = DataAccessLocal.FetchDocObject(Criteria.DocObjectType, UserInfo);

            // create an instance of the business object
            Object Document;
            Document = Utilities.ReflectionHelper.CreateObject(ObjType.DocAssembly, ObjType.DocNameSpace, ObjType.DocObjectName, new Object[] { UserInfo });
            ((BO.DocumentBase)Document)._DocObjectInfo = ObjType;

            //   ****    Make Sure that the ControlTable and Voucher Field are not empty. Else the document will fail to save
            if (ObjType.VoucherField == String.Empty)
            {
                throw new Exception("The VoucherField for document '" + ObjType.DocObjectType + "' is not mentioned in tblDocObjects. The Document will fail to Save.");
            }
            else if (ObjType.TableName == String.Empty)
            {
                throw new Exception("The Control Table for document '" + ObjType.DocObjectType + "' is not mentioned in tblDocObjects. The Document will fail to Save.");
            }

            return (BO.DocumentBase)Document;
        }

        //private void CreateAllocInstance(BO.DocumentBase BoDoc, BO.IDocCriteria Criteria, Security.IUser UserInfo)
        //{

        //    // get the type of the actual business object
        //    Data.DataAccess DataAccessLocal = new Data.DataAccess();
        //    System.Data.SqlClient.SqlCommand cmm = new System.Data.SqlClient.SqlCommand();
        //    cmm.CommandType = System.Data.CommandType.StoredProcedure;
        //    cmm.CommandText = "System.spDocAllocCollection";
        //    System.Data.DataTable dtAlloc = new System.Data.DataTable();
        //    DataAccessLocal.FillDtInternal(cmm, UserInfo, MSLA.Server.Data.DBConnectionType.MainDB, ref dtAlloc);

        //    foreach (System.Data.DataRow dr in dtAlloc.Rows)
        //    {
        //        // create an instance of the business object
        //        Object Alloc;
        //        try
        //        {
        //            Alloc = Utilities.ReflectionHelper.CreateObject(dr["fldDocAssembly"].ToString(), dr["fldDocNameSpace"].ToString(), dr["fldDocObject"].ToString(), new Object[] { UserInfo, (Base.SaveBase)BoDoc });

        //            ((Allocation.AllocationSaveBase)Alloc).SetAllocType(dr["fldDocAllocType"].ToString());
        //            BoDoc.Allocations.Add((Allocation.AllocationSaveBase)Alloc);
        //        }
        //        catch (Exception ex)
        //        {
        //            BoDoc.ConstructionExceptions.Add(ex);
        //        }
        //    }

        //}

        private void SetAuditMode(BO.DocumentBase BoDoc, Security.IUser UserInfo)
        {
            // Get the Audit Mode
            Data.DataAccess DataAccessLocal = new Data.DataAccess();
            System.Data.SqlClient.SqlCommand cmm = new System.Data.SqlClient.SqlCommand();
            cmm.CommandType = System.Data.CommandType.StoredProcedure;
            cmm.CommandText = "AuditTrail.spAuditModeFetch";

            cmm.Parameters.Add("@Branch_ID", System.Data.SqlDbType.BigInt).Value = BoDoc.fldBranch_ID;
            cmm.Parameters.Add("@DocObjectType", System.Data.SqlDbType.VarChar, 50).Value = BoDoc.DocObjectInfo.DocObjectType;
            cmm.Parameters.Add("@EnAuditMode", System.Data.SqlDbType.SmallInt).Value = BoDoc.AuditMode;
            cmm.Parameters["@EnAuditMode"].Direction = System.Data.ParameterDirection.InputOutput;

            DataAccessLocal.ExecCMM(cmm, UserInfo, MSLA.Server.Data.DBConnectionType.CompanyDB);
            BoDoc.SetAuditMode((Base.VersionBase.EnAuditMode)(Enum.ToObject(typeof(Base.VersionBase.EnAuditMode), cmm.Parameters["@EnAuditMode"].Value)));
        }

        private void SetInstanceCreationControl(BO.DocumentBase Doc, Security.IUser UInfo)
        {
            System.Data.SqlClient.SqlCommand Cmm = new System.Data.SqlClient.SqlCommand();
            Cmm.CommandType = System.Data.CommandType.StoredProcedure;
            Cmm.CommandText = "System.spDocObjectICCreate";
            Cmm.Parameters.Add("@DocObjectType", System.Data.SqlDbType.VarChar, 100).Value = Doc.DocObjectInfo.DocObjectType;
            Cmm.Parameters.Add("@UserSession_ID", System.Data.SqlDbType.UniqueIdentifier).Value = UInfo.Session_ID;
            Cmm.Parameters.Add("@DocObjectIC_ID", System.Data.SqlDbType.BigInt).Value = -1;
            Cmm.Parameters["@DocObjectIC_ID"].Direction = System.Data.ParameterDirection.InputOutput;

            Data.DataAccess DataAccessLocal = new Data.DataAccess();
            DataAccessLocal.ExecCMM(Cmm, UInfo, MSLA.Server.Data.DBConnectionType.CompanyDB);
            Doc.SetDocObjectIC_ID(Convert.ToInt64(Cmm.Parameters["@DocObjectIC_ID"].Value));
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
