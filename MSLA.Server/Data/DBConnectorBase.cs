using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace MSLA.Server.Data
{
    /// <summary>A simple abstract class that provides Serialization and DB Connectivity</summary>
    [Serializable]
    public abstract class DBConnectorBase
    {
        #region "Private Variables"

        Security.IUser _UserInfo;
        DBConnectionType _CnType;

        #endregion


        #region "Constructor and Initialisation"

        /// <summary>Constructor</summary>
        /// <param name="User">User Info</param>
        /// <param name="cnType">The Connection Type</param>
        protected DBConnectorBase(Security.IUser User, DBConnectionType cnType)
        {
            _UserInfo = User;
            _CnType = cnType;
        }

        #endregion

        #region "Friend/Static Save Methods"

        /// <summary>Calls the Save method on the data portal</summary>
        /// <returns>An instance of the same class after save</returns>
        public DBConnectorBase DataPortal_Save()
        {
            //   ****    Proceed with Save
            Data.DataPortal SaveManager = DataPortal.CreateDataPortal();
            return SaveManager.SaveDBConnector(this, this.UserInfo);
        }

        internal void SaveDocument()
        {
            SqlTransaction cnTran = null;

            using (SqlConnection cn = Data.DataAccess.GetCn(_CnType))
            {
                SaveResult DocSaveResult;
                try
                {
                    //  First Open the Connection and create a transaction
                    cn.Open();
                    cnTran = cn.BeginTransaction(IsolationLevel.ReadCommitted);

                    //  ****    Save Vch Control And Tran
                    DocSaveResult = this.SaveControlTran(cn, cnTran);

                    //   ****    Commit if there are no errors
                    cnTran.Commit();
                    cnTran = null;

                    //   ****    Always call aftersave
                    AfterSave(DocSaveResult);
                }
                catch (Exception ex)
                {
                    if (cnTran != null)
                    {
                        try
                        { cnTran.Rollback(); }
                        catch { }
                    }
                   // Exceptions.ServiceExceptionHandler.HandleException(UserInfo.User_ID, UserInfo.Session_ID.ToString(), ex);
                    throw ex;
                }
                finally
                {
                    if (cn.State != ConnectionState.Closed)
                    { cn.Close(); }
                }
            }
        }


        #endregion

        #region "Protected/Abstract Methods"

        /// <summary>Gets the User Info</summary>
        protected Security.IUser UserInfo
        { get { return _UserInfo; } }

        /// <summary>Override this method to save the Master Document and return Master Save Result</summary>
        /// <param name="cn">The Open Connection</param>
        /// <param name="cnTran">The Open Transaction</param>
        protected abstract SaveResult SaveControlTran(SqlConnection cn, SqlTransaction cnTran);

        ///<summary>This method is called after the document is saved successfully. Write any After Save Code here.</summary>
        protected abstract void AfterSave(SaveResult LocalSaveResult);

        #endregion

        /// <summary>The Save result class that is returned after save</summary>
        public class SaveResult
        {

        }



    }
}
