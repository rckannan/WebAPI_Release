using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace MSLA.Server.Security
{
    /// <summary>Custom Workflow Manager</summary>
    public partial class CustomWorkflowManager
    {
        /// <summary>Asserts the Access Rights</summary>
        /// <param name="Doc">Document that implements IARCustomWorkflow</param>
        /// <param name="UserInfo">UserInfo</param>
        static public void Assert(IARCustomWorkflow Doc, Security.IUser UserInfo)
        {
            //  ***     First set the Custom Validator Info
            if (Doc.WFValidatorInfo.ValidatorAssembly == null)
            {
                Doc.SetWFValidatorInfo(GetWFValidator(Doc, UserInfo));
            }

            //  ****    Create an Instance of Custom Validator Info and pass the Doc into the same
            if (Doc.WFValidatorInfo.ValidatorClassType == "WFValidatorBase")
            {
                Base.WFValidatorBase Validator = new MSLA.Server.Base.WFValidatorBase(UserInfo);
                Validator.AssertAccess((MSLA.Server.Base.WorkflowBase)Doc);
            }
            else
            {
                Base.WFValidatorBase Validator = (Base.WFValidatorBase)Utilities.ReflectionHelper.CreateObject(Doc.WFValidatorInfo.ValidatorAssembly,
                                                        Doc.WFValidatorInfo.ValidatorNameSpace,
                                                        Doc.WFValidatorInfo.ValidatorObject,
                                                        new object[] { UserInfo });
                Validator.AssertAccess((MSLA.Server.Base.WorkflowBase)Doc);
            }

            //  ****    Based on the returned object, set the normal workflow for the document

        }

        private static WFValidatorInfo GetWFValidator(IARCustomWorkflow Doc, Security.IUser UserInfo)
        {
            //  ****    Fetch the Document Workflow Settings
            SqlCommand Cmm = new SqlCommand();
            Cmm.CommandType = CommandType.StoredProcedure;
            Cmm.CommandText = "System.spDocWorkflowFetch";

            Cmm.Parameters.Add("@DocObject_ID", SqlDbType.BigInt).Value = ((Base.SaveBase)Doc).DocObjectInfo.DocObject_ID;
            Cmm.Parameters.Add("@ValidatorClassType", SqlDbType.VarChar, 50).Value = String.Empty;
            Cmm.Parameters["@ValidatorClassType"].Direction = ParameterDirection.InputOutput;
            Cmm.Parameters.Add("@ValidatorAssembly", SqlDbType.VarChar, 120).Value = String.Empty;
            Cmm.Parameters["@ValidatorAssembly"].Direction = ParameterDirection.InputOutput;
            Cmm.Parameters.Add("@ValidatorNameSpace", SqlDbType.VarChar, 250).Value = String.Empty;
            Cmm.Parameters["@ValidatorNameSpace"].Direction = ParameterDirection.InputOutput;
            Cmm.Parameters.Add("@ValidatorObject", SqlDbType.VarChar, 250).Value = String.Empty;
            Cmm.Parameters["@ValidatorObject"].Direction = ParameterDirection.InputOutput;

            Data.DataConnect.ExecCMM(UserInfo, ref Cmm, MSLA.Server.Data.DBConnectionType.MainDB);

            WFValidatorInfo CVI = new WFValidatorInfo();
            if (Cmm.Parameters["@ValidatorClassType"].Value.ToString() != String.Empty)
            {
                CVI.ValidatorClassType = Cmm.Parameters["@ValidatorClassType"].Value.ToString();
                CVI.ValidatorAssembly = Cmm.Parameters["@ValidatorAssembly"].Value.ToString();
                CVI.ValidatorNameSpace = Cmm.Parameters["@ValidatorNameSpace"].Value.ToString();
                CVI.ValidatorObject = Cmm.Parameters["@ValidatorObject"].Value.ToString();
            }
            else
            {
                CVI.ValidatorClassType = "WFValidatorBase";
                CVI.ValidatorAssembly = "MSLA.Server";
                CVI.ValidatorNameSpace = "MSLA.Server.Base";
                CVI.ValidatorObject = "WFValidatorBase";
            }
            return CVI;
        }
    }
}
