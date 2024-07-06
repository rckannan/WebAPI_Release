using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MSLA.Server.BO
{
    /// <summary>Abstract Document Base. All Documents must inherit from this class</summary>
    [System.Diagnostics.DebuggerStepThrough()]
    [Serializable()]
    public abstract class DocumentBase
        : Base.SaveBase
    {
        /// <summary>Constructor</summary>
        /// <param name="User">User Info</param>
        protected DocumentBase(Security.IUser User)
            : this(User, MSLA.Server.Data.DBConnectionType.CompanyDB)
        {

        }

        /// <summary>Constructor</summary>
        /// <param name="User">User Info</param>
        /// <param name="DBType">The Database Connection Type</param>
        protected DocumentBase(Security.IUser User, Data.DBConnectionType DBType)
            : base(User, DBType)
        {

        }

        /// <summary>Shared Member: Creates a new instance of a data filled Business Object on the server and returns the same to the client.</summary>
        /// <param name="DocCriteria">The Criteria required to create an instance and fetch data based on the criteria.</param>
        /// <param name="User">The User Details (Session Cookie) received on authentication.</param>
        /// <returns>Returns an object of type 'Document Base'</returns>
        public static DocumentBase DataPortal_Fetch(IDocCriteria DocCriteria, Security.IUser User)
        {
            Data.DataPortal SaveManager = Data.DataPortal.CreateDataPortal();
            return (DocumentBase)SaveManager.Fetch(DocCriteria, User);
        }

        /// <summary>Saves the current instance of the Business Object by serializing it to the Server. 
        /// Raises a 'ValidationException' if Business Rules are broken.
        /// Calls the MustOverride Methods 'SaveControlTran' And 'MakeLogEntry'.</summary>
        /// <returns>Returns the deserialized object sent to the server after successful save. The returned object would contain data after save.</returns>
        public DocumentBase DataPortal_Save(InnerWorkFlow DoAction)
        {
            if (this.IsSaveAllowed)
            {
                //  ****    Validate and throw exceptions if any
                this.DoSaveValidations(DoAction);

                //   ****    Proceed with Save
                Data.DataPortal SaveManager = Data.DataPortal.CreateDataPortal();
                return (DocumentBase)SaveManager.Save(this, DoAction, this.UserInfo);
            }
            else if (this.IsPosted && DoAction == InnerWorkFlow.PushDown)
            {
                //  *****   Validate and throw exceptions if any
                this.DoBeforeUnpostValidations();

                //   ****    Proceed with Save
                Data.DataPortal SaveManager = Data.DataPortal.CreateDataPortal();
                return (DocumentBase)SaveManager.Save(this, DoAction, this.UserInfo);
            }
            else
            {
                throw new Exception("This Document cannot be saved. Save Action denied based on Workflow/Access Rights.");
            }
        }

        /// <summary>Call this method to Modify document after Post</summary>
        /// <param name="Param">Pass any serializable class or structure as a Param.</param>
        public DocumentBase DataPortal_ModifyAfterPost(Object Param)
        {
            if (this.IsPosted)
            {
                //  *** Conduct Validations
                this.DoValidateModifyAfterPost();

                //   ****    Proceed with Save
                Data.DataPortal SaveManager = Data.DataPortal.CreateDataPortal();
                return (DocumentBase)SaveManager.ModifyAfterSave(this, this.UserInfo, Param);
            }
            else
            {
                throw new Exception("This document is not posted. Failed to call Modify actions after save.");
            }
        }

        /// <summary>This method deletes the Document</summary>
        /// <returns>Returns True if delete succeeded</returns>
        public bool DataPortal_Delete()
        {
            //  *****   Validate and throw exceptions
            this.DoDeleteValidations();

            Data.DataPortal SaveManager = Data.DataPortal.CreateDataPortal();
            return SaveManager.Delete(this, this.UserInfo);
        }

        /// <summary>Override this method to Fetch the document from the server</summary>
        /// <param name="DocCriteria">The DocCriteria passed in the Shared DataPortal_Create()</param>
        protected internal abstract void FetchOrCreate(IDocCriteria DocCriteria);


    }
}
