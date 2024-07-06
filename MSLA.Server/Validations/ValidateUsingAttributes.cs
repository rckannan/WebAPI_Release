using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Data.SqlClient;
using System.Data;

namespace MSLA.Server.Validations
{
    /// <summary>This class can be used to validate the Custom Validation Attributes applied to the properties</summary>
    [System.Diagnostics.DebuggerStepThrough()]
    public class ValidateUsingAttributes
    {
        private Security.IUser _UserInfo;

        /// <summary>
        /// Constructor used to create an instance of this class
        /// </summary>
        /// <param name="UInfo">The User Info.</param>
        public ValidateUsingAttributes(Security.IUser UInfo)
        {
            _UserInfo = UInfo;
        }

        /// <summary>Use this method to validate the object</summary>
        /// <param name="BOClass">Any class that implements MSLA.Server.Validations.IValidateUsingAttributes</param>
        public virtual void ValidateObject(IValidateUsingAttributes BOClass)
        {
            PropertyInfo[] Properties = this.GetProperties(BOClass);
            foreach (PropertyInfo Prop in Properties)
            {
                object[] Attribs = Prop.GetCustomAttributes(true);
                foreach (object Attrib in Attribs)
                {
                    if (Attrib.GetType() == typeof(ValueNotAllowedAttribute))
                    { ValidateValueNotAllowed(BOClass, Prop, (ValueNotAllowedAttribute)Attrib); }
                    else if (Attrib.GetType() == typeof(BlankNotAllowedAttribute))
                    { ValidateBlankNotAllowed(BOClass, Prop, (BlankNotAllowedAttribute)Attrib); }
                    else if (Attrib.GetType() == typeof(DateRangeAttribute))
                    { ValidateDateRange(BOClass, Prop, (DateRangeAttribute)Attrib); }
                    else if (Attrib.GetType() == typeof(MasterItemValidAttribute))
                    { ValidateMasterItem(BOClass, Prop, (MasterItemValidAttribute)Attrib); }
                    else if (Attrib.GetType() == typeof(DuplicateNotAllowedAttribute))
                    { DuplicateNotAllowed(BOClass, Prop, (DuplicateNotAllowedAttribute)Attrib); }
                    else if (Attrib.GetType() == typeof(DuplicateCodeNotAllowedAttribute))
                    { DuplicateCodeNotAllowed(BOClass, Prop, (DuplicateCodeNotAllowedAttribute)Attrib); }
                    else
                    {
                        if (Attrib.GetType() == typeof(ValidationAttribute))
                        { ValidateOther(BOClass, Prop, (ValidationAttribute)Attrib); }
                    }
                }
            }
        }

        /// <summary>Validate the String property for null or empty value</summary>
        /// <param name="BOClass">The class object</param>
        /// <param name="Prop">The Property Information</param>
        /// <param name="Attrib">The attributes bound to the property</param>
        protected virtual void ValidateBlankNotAllowed(IValidateUsingAttributes BOClass, PropertyInfo Prop, BlankNotAllowedAttribute Attrib)
        {
            if (Prop.GetValue(BOClass, null) == null)
            {
                BOClass.BrokenSaveRules.Add(BOClass.ToString(), Prop.Name, Attrib.DisplayString + " is empty/blank. Please enter a valid value.");
            }
            else if (Prop.GetValue(BOClass, null).ToString().Trim() == String.Empty)
            {
                BOClass.BrokenSaveRules.Add(BOClass.ToString(), Prop.Name, Attrib.DisplayString + " is empty/blank. Please enter a valid value.");
            }
        }

        /// <summary>Validate the value not allowed</summary>
        /// <param name="BOClass">The class object</param>
        /// <param name="Prop">The Property Information</param>
        /// <param name="Attrib">The attributes bound to the property</param>
        protected virtual void ValidateValueNotAllowed(IValidateUsingAttributes BOClass, PropertyInfo Prop, ValueNotAllowedAttribute Attrib)
        {
            if (Prop.GetValue(BOClass, null) == null)
            {
                BOClass.BrokenSaveRules.Add(BOClass.ToString(), Prop.Name, "Selected " + Attrib.DisplayString + " is invalid. Please enter a valid value.");
            }
            else if (Convert.ToInt64(Prop.GetValue(BOClass, null)) == Attrib.FieldValue)
            {
                BOClass.BrokenSaveRules.Add(BOClass.ToString(), Prop.Name, "Selected " + Attrib.DisplayString + " is invalid. Please enter a valid value.");
            }
        }

        /// <summary>
        /// Validate date range for the fin year
        /// </summary>
        /// <param name="BOClass">The class object</param>
        /// <param name="Prop">The Property Information</param>
        /// <param name="Attrib">The attributes bound to the property</param>        
        protected virtual void ValidateDateRange(IValidateUsingAttributes BOClass, PropertyInfo Prop, DateRangeAttribute Attrib)
        {
            // string FinYear = this.GetPropertyValue((string)(object)BOClass.ToString(), Attrib.FinYearField);
            string FinYear = (String)this.GetPropertyValue(BOClass, Attrib.FinYearField);

            if (FinYear == String.Empty)
            {
                BOClass.BrokenSaveRules.Add(BOClass.ToString(), Prop.Name, "Financial Year does not contain a valid value. Date Range validation failed.");
            }
            else
            {
                SqlCommand cmm = new SqlCommand();
                cmm.CommandType = CommandType.StoredProcedure;
                cmm.CommandText = "System.spDateRangeValidation";

                cmm.Parameters.Add("@Year", SqlDbType.VarChar, 4).Value = FinYear;
                cmm.Parameters.Add("@Date", SqlDbType.DateTime, 0).Value = ((DateTime)Prop.GetValue(BOClass, null)).ToString(MSLA.Server.Constants.SQLDateFormat);
                cmm.Parameters.Add("@IsValid", SqlDbType.Bit, 0).Direction = ParameterDirection.Output;

                Data.DataConnect.ExecCMM(this._UserInfo, ref cmm, Data.DBConnectionType.CompanyDB);

                if (!Convert.ToBoolean(cmm.Parameters["@IsValid"].Value))
                {
                    BOClass.BrokenSaveRules.Add(BOClass.ToString(), Prop.Name, "Invalid " + Attrib.DisplayString + ". The Document date does not belong to the associated Financial Year.");
                }

            }
        }


        /// <summary>
        /// Validate master item
        /// </summary>
        /// <param name="BOClass">The class object</param>
        /// <param name="Prop">The Property Information</param>
        /// <param name="Attrib">The attributes bound to the property</param>        
        protected virtual void ValidateMasterItem(IValidateUsingAttributes BOClass, PropertyInfo Prop, MasterItemValidAttribute Attrib)
        {

            if (!Attrib.Disable)
            {
                SqlCommand cmm = new SqlCommand();
                cmm.CommandType = CommandType.StoredProcedure;
                cmm.CommandText = "System.spMasterItemValidation";

                cmm.Parameters.Add("@MasterTable", SqlDbType.VarChar, 120).Value = Attrib.MasterTable;
                cmm.Parameters.Add("@MasterField", SqlDbType.DateTime, 120).Value = Attrib.MasterField;
                cmm.Parameters.Add("@MasterItem_ID", SqlDbType.BigInt, 0).Value = Prop.GetValue(BOClass, null);
                cmm.Parameters.Add("@IsValid", SqlDbType.Bit, 0).Direction = ParameterDirection.Output;

                Data.DataConnect.ExecCMM(this._UserInfo, ref cmm, Data.DBConnectionType.CompanyDB);
                if ((int)cmm.Parameters["@IsValid"].Value != 1)

                    BOClass.BrokenSaveRules.Add((string)(object)BOClass.ToString(), Prop.Name, "Invalid " + Attrib.DisplayString + ". Please rectify before proceeding.");
            }


        }

        /// <summary>
        /// Validate for duplicate description in Master Items
        /// </summary>
        /// <param name="BOClass">The class object</param>
        /// <param name="Prop">The Property Information</param>
        /// <param name="Attrib">The attributes bound to the property</param>        
        protected virtual void DuplicateNotAllowed(IValidateUsingAttributes BOClass, PropertyInfo Prop, DuplicateNotAllowedAttribute Attrib)
        {
            long MasterFieldValue;
            MasterFieldValue = (long)this.GetPropertyValue(BOClass, Attrib.MasterField);

            if (MasterFieldValue == 0)
            {
                BOClass.BrokenSaveRules.Add(BOClass.ToString(), Prop.Name, "Invalid ID. Duplicate Validation failed.");
            }

            SqlCommand cmm = new SqlCommand();

            cmm.CommandType = CommandType.StoredProcedure;
            cmm.CommandText = "System.spDuplicatesNotAllowed";

            cmm.Parameters.Add("@MasterTable", SqlDbType.VarChar, 120).Value = Attrib.MasterTable;
            cmm.Parameters.Add("@MasterField", SqlDbType.VarChar, 120).Value = Attrib.MasterField;
            cmm.Parameters.Add("@MasterFieldValue", SqlDbType.BigInt, 0).Value = MasterFieldValue;
            cmm.Parameters.Add("@FieldTobeChecked", SqlDbType.VarChar, 120).Value = Attrib.FieldTobeChecked;
            cmm.Parameters.Add("@FieldValue", SqlDbType.VarChar, 500).Value = Prop.GetValue(BOClass, null);
            cmm.Parameters.Add("@IsValid", SqlDbType.Bit, 0).Direction = ParameterDirection.Output;

            Data.DataConnect.ExecCMM(this._UserInfo, ref cmm, Data.DBConnectionType.CompanyDB);

            if ((bool)cmm.Parameters["@IsValid"].Value != true)
            {
                BOClass.BrokenSaveRules.Add(BOClass.ToString(), Prop.Name, "Duplicate " + Attrib.DisplayString + ". Please rectify before proceeding.");
            }

        }

        /// <summary>
        /// Validate for duplicate code in master items
        /// </summary>
        /// <param name="BOClass">The class object</param>
        /// <param name="Prop">The Property Information</param>
        /// <param name="Attrib">The attributes bound to the property</param>
        protected virtual void DuplicateCodeNotAllowed(IValidateUsingAttributes BOClass, PropertyInfo Prop, DuplicateCodeNotAllowedAttribute Attrib)
        {
            long MasterFieldValue;
            MasterFieldValue = (long)this.GetPropertyValue(BOClass, Attrib.MasterField);

            if (MasterFieldValue == 0)
            {
                BOClass.BrokenSaveRules.Add(BOClass.ToString(), Prop.Name, "Invalid ID. Duplicate Code Validation failed.");
            }


            SqlCommand cmm = new SqlCommand();
            cmm.CommandType = CommandType.StoredProcedure;
            cmm.CommandText = "System.spDuplicateCodeNotAllowed";

            cmm.Parameters.Add("@MasterTable", SqlDbType.VarChar, 120).Value = Attrib.MasterTable;
            cmm.Parameters.Add("@SettingKey", SqlDbType.VarChar, 50).Value = Attrib.SettingKey;
            cmm.Parameters.Add("@MasterField", SqlDbType.VarChar, 120).Value = Attrib.MasterField;
            cmm.Parameters.Add("@MasterFieldValue", SqlDbType.BigInt, 0).Value = MasterFieldValue;
            cmm.Parameters.Add("@FieldTobeChecked", SqlDbType.VarChar, 120).Value = Prop.Name;
            cmm.Parameters.Add("@FieldValue", SqlDbType.VarChar, 500).Value = Prop.GetValue(BOClass, null);
            cmm.Parameters.Add("@IsValid", SqlDbType.Bit, 0).Direction = ParameterDirection.Output;
            cmm.Parameters.Add("@ErrMsg", SqlDbType.VarChar, 500).Value = String.Empty;
            cmm.Parameters["@ErrMsg"].Direction = ParameterDirection.InputOutput;

            Data.DataConnect.ExecCMM(this._UserInfo, ref cmm, Data.DBConnectionType.CompanyDB);

            if ((bool)cmm.Parameters["@IsValid"].Value != true)
            {
                BOClass.BrokenSaveRules.Add(BOClass.ToString(), Prop.Name, (string)cmm.Parameters["@ErrMsg"].Value + ". Please rectify before proceeding.");
            }

        }

        /// <summary>
        /// Override this method to do custom validation
        /// </summary>
        /// <param name="BOClass">The class object</param>
        /// <param name="Prop">The Property Information</param>
        /// <param name="Attrib">The attributes bound to the property</param>
        protected virtual void ValidateOther(IValidateUsingAttributes BOClass, PropertyInfo Prop, ValidationAttribute Attrib)
        {
            BOClass.BrokenSaveRules.Add(BOClass.ToString(), Prop.Name, "Validation failed for " + Attrib.DisplayString + ".");
        }

        #region "Helper Methods"
        private PropertyInfo[] GetProperties(object BOClass)
        {
            return BOClass.GetType().GetProperties(BindingFlags.GetProperty |
                                                    BindingFlags.Instance |
                                                    BindingFlags.Public);
        }

        private object GetPropertyValue(object BOClass, string FieldName)
        {
            return BOClass.GetType().GetProperty(FieldName, BindingFlags.GetProperty |
                                                            BindingFlags.Instance |
                                                            BindingFlags.Public).GetValue(BOClass, null);
        }
        #endregion

    }
}
