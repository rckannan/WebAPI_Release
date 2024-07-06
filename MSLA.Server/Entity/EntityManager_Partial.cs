using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace MSLA.Server.Entity
{
    public partial class EntityManager
    {
        private static void GenerateDocActionScripts(Security.IUser UserInfo, string schema, string table, string primaryKey, string sequenceTable, Data.DBConnectionType DBType)
        {
            ActionScript tableAction = new ActionScript();
            tableAction.TableName = schema + "." + table;
            tableAction.TableType = ActionScript.enTableType.Master;

            StringBuilder Template = new StringBuilder();
            Template.Append(LoadedTemplate.GetValue("DocControlAddUpdate"));

            // Contains the Comma seperated list of field names
            System.Text.StringBuilder FieldList = new System.Text.StringBuilder();
            System.Text.StringBuilder ParamList = new System.Text.StringBuilder();
            // Contains the parameters with field list for Update
            System.Text.StringBuilder UpdateFildParamList = new System.Text.StringBuilder();

            // Update statements and Insert statement parameters
            List<string> ExcludeFromUpdate = null;
            List<string> ExcludeParamDef = new List<string>();

            // **** Gets the column collection
            DataTable dtcolumns = FetchTableFields(UserInfo, schema, table, DBType);

            if (!ContainsKey(dtcolumns, primaryKey))
            {
                throw new Exception("Primary Key '" + primaryKey + "' not found in field collection for '" + schema + "." + table + "'. Failed to generate Action Scripts.");
            }

            // **** Get the list of fields to be Excluded from the Update Statement.
            ExcludeFromUpdate = GetExclusionList("DocControlAddUpdate", "UpdateStatement");
            ExcludeFromUpdate.Add(primaryKey);

            // **** Get the list of fields for which parameters are not needed in SP.
            ExcludeParamDef = GetExclusionList("DocControlAddUpdate", "Parameters");

            // *** For each field in the table, proceed to create the field/Param list
            foreach (DataRow TableCol in dtcolumns.Rows)
            {

                // **** If a computed column, no need for an insert or update
                if (!Convert.ToBoolean(TableCol["fldIsComputed"]))
                {
                    // Generate Field List for Insert
                    if (FieldList.Length > 0)
                    { FieldList.Append(", "); }
                    FieldList.Append(TableCol["fldColumnName"].ToString());

                    // Generate Param List for Insert
                    if (ParamList.Length > 0)
                    { ParamList.Append(", "); }
                    ParamList.Append(TableCol["fldColumnName"].ToString().Replace("fld", "@"));

                    // Generate Combination of Field and Pram list for Update
                    // Step 1: Append ","

                    //Step 2: Exclude Fileds not to be updated
                    if (!ExcludeParamDef.Contains(TableCol["fldColumnName"].ToString()))
                    {
                        if (UpdateFildParamList.Length > 0)
                        { UpdateFildParamList.Append(", "); }
                        UpdateFildParamList.AppendLine(TableCol["fldColumnName"].ToString() + " = " + TableCol["fldColumnName"].ToString().Replace("fld", "@"));
                    }


                    // Add the field Parameter to Command Object
                    if (!ExcludeParamDef.Contains(TableCol["fldColumnName"].ToString()))
                    {
                        ActionParam actionParam = GenerateParam(TableCol["fldColumnName"].ToString(), TableCol["fldDataType"].ToString(),
                                            Convert.ToInt32(TableCol["fldLength"]), Convert.ToByte(TableCol["fldPrecision"]),
                                            Convert.ToByte(TableCol["fldScale"]));
                        if (TableCol["fldColumnName"].ToString().Equals(primaryKey) || TableCol["fldColumnName"].ToString().Equals("fldLastUpdated"))
                        {
                            actionParam.Direction = ParameterDirection.InputOutput;
                        }
                        tableAction.AddUpdateParams.Add(actionParam);
                    }

                }
            }

            // **** Replaces the parameter values in the string containing the template.
            // ***** Places the <> for which escape characters have been provided in XML
            Template.Replace("&lt;", "<");
            Template.Replace("&gt;", ">");
            // Replace constructed strings
            Template.Replace("?TableName", table);
            Template.Replace("?Schema", schema);
            Template.Replace("?PrimaryKeyField", primaryKey);
            Template.Replace("?PrimaryKeyParam", primaryKey.Replace("fld", "@"));
            Template.Replace("?FieldList", FieldList.ToString());
            Template.Replace("?ParamList", ParamList.ToString());
            Template.Replace("?UpdateFieldParamList", UpdateFildParamList.ToString());
            Template.Replace("?SequenceTable", sequenceTable);

            tableAction.AddUpdateScript = Template.ToString();

            // Start the Fetch Script Generation
            GenerateActionFetch(UserInfo, schema, table, primaryKey, Data.DBConnectionType.CompanyDB, tableAction, dtcolumns);

            // Start the Delete Script Generation
            GenerateActionDelete(UserInfo, schema, table, primaryKey, DBType, tableAction, dtcolumns);

            // Add to Script Cache
            CachedTables.Add(tableAction.TableName, tableAction);

        }

        private static void GenerateMasterActionScripts(Security.IUser UserInfo, string schema, string table, string primaryKey, Data.DBConnectionType DBType)
        {

            ActionScript tableAction = new ActionScript();
            tableAction.TableName = schema + "." + table;
            tableAction.TableType = ActionScript.enTableType.Master;


            StringBuilder Template = new StringBuilder();
            Template.Append(LoadedTemplate.GetValue("MasterAddUpdate"));

            // Contains the Comma seperated list of field names
            System.Text.StringBuilder FieldList = new System.Text.StringBuilder();
            System.Text.StringBuilder ParamList = new System.Text.StringBuilder();
            // Contains the parameters with field list for Update
            System.Text.StringBuilder UpdateFildParamList = new System.Text.StringBuilder();

            // Update statements and Insert statement parameters
            List<string> ExcludeFromUpdate = null;
            List<string> ExcludeParamDef = new List<string>();

            // **** Gets the column collection
            DataTable dtcolumns = FetchTableFields(UserInfo, schema, table, DBType);

            if (!ContainsKey(dtcolumns, primaryKey))
            {
                throw new Exception("Primary Key '" + primaryKey + "' not found in field collection for '" + schema + "." + table + "'. Failed to generate Action Scripts.");
            }

            // **** Get the list of fields to be Excluded from the Update Statement.
            ExcludeFromUpdate = GetExclusionList("MasterAddUpdate", "UpdateStatement");
            ExcludeFromUpdate.Add(primaryKey);

            // *** For each field in the table, proceed to create the field/Param list
            foreach (DataRow TableCol in dtcolumns.Rows)
            {

                // **** If a computed column, no need for an insert or update
                if (!Convert.ToBoolean(TableCol["fldIsComputed"]))
                {
                    // Generate Field List for Insert
                    if (FieldList.Length > 0)
                    { FieldList.Append(", "); }
                    FieldList.Append(TableCol["fldColumnName"].ToString());

                    // Generate Param List for Insert
                    if (ParamList.Length > 0)
                    { ParamList.Append(", "); }
                    ParamList.Append(TableCol["fldColumnName"].ToString().Replace("fld", "@"));

                    // Generate Combination of Field and Pram list for Update
                    // Step 1: Append ","

                    //Step 2: Exclude Fileds not to be updated
                    if (!ExcludeParamDef.Contains(TableCol["fldColumnName"].ToString()))
                    {
                        if (UpdateFildParamList.Length > 0)
                        { UpdateFildParamList.Append(", "); }
                        UpdateFildParamList.AppendLine(TableCol["fldColumnName"].ToString() + " = " + TableCol["fldColumnName"].ToString().Replace("fld", "@"));
                    }

                    // Add the field Parameter to Command Object
                    if (!ExcludeParamDef.Contains(TableCol["fldColumnName"].ToString()))
                    {
                        ActionParam actionParam = GenerateParam(TableCol["fldColumnName"].ToString(), TableCol["fldDataType"].ToString(),
                                            Convert.ToInt32(TableCol["fldLength"]), Convert.ToByte(TableCol["fldPrecision"]),
                                            Convert.ToByte(TableCol["fldScale"]));
                        if (TableCol["fldColumnName"].ToString().Equals(primaryKey) || TableCol["fldColumnName"].ToString().Equals("fldLastUpdated"))
                        {
                            actionParam.Direction = ParameterDirection.InputOutput;
                        }
                        tableAction.AddUpdateParams.Add(actionParam);
                    }

                }
            }

            // Add default parameters (This is remporarily disabled)
            //tableAction.AddUpdateParams.Add(new ActionParam() { ParameterName = "@Overwrite", SqlDbType = System.Data.SqlDbType.Bit });

            // **** Replaces the parameter values in the string containing the template.
            // ***** Places the <> for which escape characters have been provided in XML
            Template.Replace("&lt;", "<");
            Template.Replace("&gt;", ">");
            // Replace constructed strings
            Template.Replace("?TableName", table);
            Template.Replace("?Schema", schema);
            Template.Replace("?PrimaryKeyField", primaryKey);
            Template.Replace("?PrimaryKeyParam", primaryKey.Replace("fld", "@"));
            Template.Replace("?FieldList", FieldList.ToString());
            Template.Replace("?ParamList", ParamList.ToString());
            Template.Replace("?UpdateFieldParamList", UpdateFildParamList.ToString());

            tableAction.AddUpdateScript = Template.ToString();

            // Start the Fetch Script Generation
            GenerateActionFetch(UserInfo, schema, table, primaryKey, DBType, tableAction, dtcolumns);

            // Start the Delete Script Generation
            GenerateActionDelete(UserInfo, schema, table, primaryKey, DBType, tableAction, dtcolumns);

            // Add to Script Cache
            CachedTables.Add(tableAction.TableName, tableAction);

        }

        private static void GenerateTranActionScripts(Security.IUser UserInfo, string schema, string table, string foreignKey, Data.DBConnectionType DBType)
        {
            ActionScript tableAction = new ActionScript();
            tableAction.TableName = schema + "." + table;
            tableAction.TableType = ActionScript.enTableType.TranTable;

            StringBuilder Template = new StringBuilder();
            Template.Append(LoadedTemplate.GetValue("TranAdd"));

            // Contains the Comma seperated list of field names
            System.Text.StringBuilder FieldList = new System.Text.StringBuilder();
            System.Text.StringBuilder ParamList = new System.Text.StringBuilder();

            // **** Gets the column collection
            DataTable dtcolumns = FetchTableFields(UserInfo, schema, table, DBType);

            if (!ContainsKey(dtcolumns, foreignKey))
            {
                throw new Exception("Foreign Key '" + foreignKey + "' not found in field collection for '" + schema + "." + table + "'. Failed to generate Action Scripts.");
            }


            // *** For each field in the table, proceed to create the field/Param list
            foreach (DataRow TableCol in dtcolumns.Rows)
            {

                // **** If a computed column, no need for an insert or update
                if (!Convert.ToBoolean(TableCol["fldIsComputed"]))
                {
                    // Generate Field List for Insert
                    if (FieldList.Length > 0)
                    { FieldList.Append(", "); }
                    FieldList.Append(TableCol["fldColumnName"].ToString());

                    // Generate Param List for Insert
                    if (ParamList.Length > 0)
                    { ParamList.Append(", "); }
                    ParamList.Append(TableCol["fldColumnName"].ToString().Replace("fld", "@"));

                    tableAction.AddUpdateParams.Add(GenerateParam(TableCol["fldColumnName"].ToString(), TableCol["fldDataType"].ToString(),
                                        Convert.ToInt32(TableCol["fldLength"]), Convert.ToByte(TableCol["fldPrecision"]),
                                        Convert.ToByte(TableCol["fldScale"])));

                }
            }

            // **** Replaces the parameter values in the string containing the template.
            // ***** Places the <> for which escape characters have been provided in XML
            Template.Replace("&lt;", "<");
            Template.Replace("&gt;", ">");
            // Replace constructed strings
            Template.Replace("?TableName", table);
            Template.Replace("?Schema", schema);
            Template.Replace("?FieldList", FieldList.ToString());
            Template.Replace("?ParamList", ParamList.ToString());

            tableAction.AddUpdateScript = Template.ToString();

            // Start the Fetch Script Generation
            GenerateActionFetch(UserInfo, schema, table, foreignKey, DBType, tableAction, dtcolumns);

            // Start the Delete Script Generation
            GenerateActionDelete(UserInfo, schema, table, foreignKey, DBType, tableAction, dtcolumns);

            // Add to Script Cache
            CachedTables.Add(tableAction.TableName, tableAction);
        }

        private static void GenerateActionFetch(Security.IUser UserInfo, String Schema, String Table, string PrimaryKey, Data.DBConnectionType DBType,
                ActionScript tableAction, DataTable dtColumns)
        {
            StringBuilder Template = new StringBuilder();
            Template.Append(LoadedTemplate.GetValue("FetchStatement"));

            // Contains the Comma seperated list of field names
            System.Text.StringBuilder FieldList = new System.Text.StringBuilder();


            List<string> ExcludeFromFetch = GetExclusionList("DocControlAddUpdate", "FetchStatement");

            // *** For each field in the table, proceed to create the field/Param list
            foreach (DataRow TableCol in dtColumns.Rows)
            {

                // Ignore Excluded Items
                if (!ExcludeFromFetch.Contains(TableCol["fldColumnName"].ToString()))
                {
                    // Generate Field List for Insert
                    if (FieldList.Length > 0)
                    { FieldList.Append(", "); }
                    FieldList.Append(TableCol["fldColumnName"].ToString());

                    if (TableCol["fldColumnName"].ToString().Equals(PrimaryKey))
                    {
                        tableAction.FetchParams.Add(GenerateParam(TableCol["fldColumnName"].ToString(), TableCol["fldDataType"].ToString(),
                                                Convert.ToInt32(TableCol["fldLength"]), Convert.ToByte(TableCol["fldPrecision"]),
                                                Convert.ToByte(TableCol["fldScale"])));
                    }
                }
            }


            // **** Replaces the parameter values in the string containing the template.
            // Replace constructed strings
            Template.Replace("?TableName", Table);
            Template.Replace("?Schema", Schema);
            Template.Replace("?PrimaryKeyField", PrimaryKey);
            Template.Replace("?PrimaryKeyParam", PrimaryKey.Replace("fld", "@"));
            Template.Replace("?FieldList", FieldList.ToString());

            tableAction.FetchScript = Template.ToString();
        }

        private static void GenerateActionDelete(Security.IUser UserInfo, String Schema, String Table, string PrimaryKey, Data.DBConnectionType DBType,
                ActionScript tableAction, DataTable dtColumns)
        {

            StringBuilder Template = new StringBuilder();
            Template.Append(LoadedTemplate.GetValue("DeleteScript"));


            // **** Replaces the parameter values in the string containing the template.
            // Replace constructed strings
            Template.Replace("?TableName", Table);
            Template.Replace("?Schema", Schema);
            Template.Replace("?PrimaryKeyField", PrimaryKey);
            Template.Replace("?PrimaryKeyParam", PrimaryKey.Replace("fld", "@"));

            // *** Create criteria field based on primary/foreign key
            foreach (DataRow TableCol in dtColumns.Rows)
            {
                // Generate Field List for Insert
                if (TableCol["fldColumnName"].ToString().Equals(PrimaryKey))
                {
                    tableAction.DeleteParams.Add(GenerateParam(TableCol["fldColumnName"].ToString(), TableCol["fldDataType"].ToString(),
                                            Convert.ToInt32(TableCol["fldLength"]), Convert.ToByte(TableCol["fldPrecision"]),
                                            Convert.ToByte(TableCol["fldScale"])));
                }
            }

            tableAction.DeleteScript = Template.ToString();
        }

        private static System.Data.DataTable FetchTableFields(Security.IUser UserInfo, string schema, string table, Data.DBConnectionType DBType)
        {
            SqlCommand cmm = new SqlCommand();
            cmm.CommandType = System.Data.CommandType.StoredProcedure;
            cmm.CommandText = "System.spTableFieldCollection";

            cmm.Parameters.Add("@schema", System.Data.SqlDbType.VarChar, 50).Value = schema;
            cmm.Parameters.Add("@table", System.Data.SqlDbType.VarChar, 50).Value = table;
            DataTable dtResult = new DataTable();
            dtResult = Data.DataConnect.FillDt(dtResult, cmm, UserInfo, DBType);
            return dtResult;
        }

        private static List<string> GetExclusionList(String parentNode, String type)
        {
            // ***** Gets the exclusion list of fields.Type stands for the Node which contains the list(Comma seperated)
            string excludeFields;
            string[] fields;
            List<string> FieldList = new List<string>();

            excludeFields = (string)ExcludedFields.GetValue(parentNode, type);
            // **** Splits the string and adds the item into an List.
            fields = excludeFields.Split(Convert.ToChar(","));
            foreach (string field in fields)
            {
                FieldList.Add(field);
            }
            return FieldList;
        }

        private static ActionParam GenerateParam(string fieldName, string dataType, int length, byte precision, byte scale)
        {
            ActionParam Param = new ActionParam();
            Param.ParameterName = fieldName.Replace("fld", "@");
            if (dataType == "varchar")
            {
                Param.SqlDbType = System.Data.SqlDbType.VarChar;
                Param.Size = length;
            }
            else if (dataType == "text")
            {
                Param.SqlDbType = System.Data.SqlDbType.Text;
                Param.Size = length;
            }
            else if (dataType == "datetime")
            {
                Param.SqlDbType = System.Data.SqlDbType.DateTime;
            }
            else if (dataType == "numeric")
            {
                Param.SqlDbType = System.Data.SqlDbType.Decimal;
                Param.Precision = precision;
                Param.Scale = scale;
            }
            else if (dataType == "bigint")
            {
                Param.SqlDbType = System.Data.SqlDbType.BigInt;
            }
            else if (dataType == "int")
            {
                Param.SqlDbType = System.Data.SqlDbType.Int;
            }
            else if (dataType == "smallint")
            {
                Param.SqlDbType = System.Data.SqlDbType.SmallInt;
            }
            else if (dataType == "tinyint")
            {
                Param.SqlDbType = System.Data.SqlDbType.TinyInt;
            }
            else if (dataType == "bit")
            {
                Param.SqlDbType = System.Data.SqlDbType.Bit;
            }
            else if (dataType == "varbinary")
            {
                Param.SqlDbType = System.Data.SqlDbType.VarBinary;
                Param.Size = length;
            }
            else if (dataType == "char")
            {
                Param.SqlDbType = System.Data.SqlDbType.Char;
                Param.Size = length;
            }

            else
            {
                throw new Exception("Unknown datatype! " + dataType);
            }
            return Param;
        }

        private static bool ContainsKey(DataTable dtColumns, string primaryKey)
        {
            foreach (DataRow dr in dtColumns.Rows)
            {
                if (dr["fldColumnName"].ToString().Equals(primaryKey))
                { return true; }
            }
            return false;
        }
    }
}
