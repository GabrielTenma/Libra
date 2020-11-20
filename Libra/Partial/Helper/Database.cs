using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data;
 
namespace Libra
{
    partial class Helper
    {
        /*
         *  Partial Class Helper
         *  Database Function
         *  Create Function with Keycode Name 'DB'
         *  Example 'DBnamefunction()';
         */


        /// <summary>
        /// Create New Connection Database
        /// </summary>
        /// <param name="Hostname"></param>
        /// <param name="DBName"></param>
        /// <param name="Username"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        public static Connection.Database DBCreateConnection(string Hostname, string DBName, string Username, string Password)
        {
            Connection.Database DB = new Connection.Database();
            DB.MyConnectionString = string.Format("server={0};uid={1};pwd={2};database={3}", Hostname, Username, Password, DBName);
            return DB;
        }


        /// <summary>
        /// Function For strQuery sql return boolean
        /// </summary>
        /// <param name="SerialByte"></param>
        /// <param name="DBConfig"></param>
        /// <returns></returns>
        public static bool DBRowsAffected(string strstrQuery, Connection.Database DBConfig)
        {
            bool result = false;

            using (MySqlConnection conn = new MySqlConnection())
            {
                DBConfig.OpenConnection(conn);
                using (var cmd = new MySqlCommand(strstrQuery, conn))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                            result = true;
                    }
                }
            }
            return result;
        }


        /// <summary>
        /// Function For strQuery SQL Inserting Row
        /// </summary>
        /// <param name="strQuery"></param>
        /// <param name="DBConfig"></param>
        public static void DBRowsInsert(string strQuery, Connection.Database DBConfig)
        {
            DBConfig.InsertToMySql(strQuery);
        }


        /// <summary>
        /// Function For Queries Result to Datatable
        /// </summary>
        public static System.Data.DataTable DBRowsDatatable(string strQuery, Connection.Database DBConfig)
        {
            return DBConfig.MySqlToDataTable(strQuery);
        }


        /// <summary>
        /// Function For Queries Result to String
        /// </summary>
        /// <param name="ColumnName">Target Column Name</param>
        /// <param name="strQuery">strQuery</param>
        /// <param name="DBConfig">Database Configuration</param>
        /// <returns></returns>
        public static string DBRowsString(string strColumnName, string strQuery, Connection.Database DBConfig)
        {
            string result = "NULL";

            using (MySqlConnection conn = new MySqlConnection())
            {
                DBConfig.OpenConnection(conn);
                using (var cmd = new MySqlCommand(strQuery, conn))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            result = reader.GetString(strColumnName);
                        }
                    }
                }
            }

            return result;
        }
        /*

                /// <summary>
                /// Function For Build Autogenerate strQuery from Datatable to String(SQL strQuery)
                /// </summary>
                /// <param name="table">Datatable Target</param>
                /// <param name="table_name">Table Name</param>
                /// <param name="DBConfig">Database Configuration</param>
                /// <returns></returns>
                public static string DBBulkInsertBuilder(ref DataTable table, string table_name, Connection.Database DBConfig)
                {
                    return DBConfig.BulkInsert(ref table, table_name);
                }
        */

        /// <summary>
        /// Function For Build Dictonary from table/datatable
        /// </summary>
        /// <param name="strQuery"></param>
        /// <param name="strKeyColumnName"></param>
        /// <param name="strValueColumnName"></param>
        /// <param name="DBConfig"></param>
        /// <returns></returns>
        public static Dictionary<string, string> DBTableToDict(string strQuery, string strKeyColumnName, string strValueColumnName, Connection.Database DBConfig)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            DataTable dtQueryTable = DBConfig.MySqlToDataTable(strQuery);

            foreach (DataRow dr in dtQueryTable.Rows)
            {
                result.Add(GetDataFromRow(dr, strKeyColumnName), GetDataFromRow(dr, strValueColumnName));
            }

            return result;
        }
    }
}
