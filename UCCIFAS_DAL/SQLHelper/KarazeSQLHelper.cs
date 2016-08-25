using System.Data;
using System.Data.Sql;
using System.Data.Common;
using System.Data.SqlClient;
using System.Configuration;
using System;

namespace Serene1Test2.Modules
{
    public class KarazeSQLHelper
    {
        #region Members 
        private static string _ConString = string.Empty;
        private SqlConnection Con = null;
        private SqlCommand Comm = null;
        #endregion

        #region Properties
        public static string ConString
        {
            get { return _ConString; }
            set { _ConString = value; }
        }
        #endregion

        #region Procedures & Functions
        public KarazeSQLHelper(string ConnectionStringName = "DefaultConnection")
        {
            if (GetConnectionStringByName(ConnectionStringName) == null)
            {
                throw new Exception("Could not find connection string named [ " + ConnectionStringName + " ] in the Application configuration file");
                return;
            }
            _ConString = ConfigurationManager.ConnectionStrings[ConnectionStringName].ConnectionString;
        }

        // Retrieves a connection string by name.
        // Returns Nothing if the name is not found.
        public static string GetConnectionStringByName(string name)
        {
            // Assume failure
            string returnValue = null;

            // Look for the name in the connectionStrings section.
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[name];

            // If found, return the connection string.
            if ((settings != null))
            {
                returnValue = settings.ConnectionString;
            }

            return returnValue;
        }

        // Retrieve a connection string by specifying the providerName.
        // Assumes one connection string per provider in the config file.
        public bool CanConnect()
        {
            bool Check = false;
            Con = new SqlConnection(_ConString);
            try
            {
                ReOpenConnection();
                Check = true;
            }
            catch (Exception Ex)
            {
                Check = false;
            }
            finally
            {
                CloseDbConnection();
            }
            return Check;
        }

        public void SetSqlStatement(string query, CommandType _Type)
        {
            Con = new SqlConnection(_ConString);
            Comm = Con.CreateCommand();
            Comm.CommandText = query;
            Comm.CommandType = _Type;
        }

        public void Parameter(string ParameterName, SqlDbType _Type, object ParameterValue, ParameterDirection ParamDir = ParameterDirection.Input)
        {
            SqlParameter Param = Comm.CreateParameter();
            Param.ParameterName = ParameterName;
            Param.Value = (ParameterValue == null ? DBNull.Value : ParameterValue);
            Param.SqlDbType = _Type;
            Param.Direction = ParamDir;
            Comm.Parameters.Add(Param);
        }

        public void Parameter(string ParameterName, SqlDbType _Type, int TypeSize, string ParameterValue, ParameterDirection ParamDir = ParameterDirection.Input)
        {
            SqlParameter Param = Comm.CreateParameter();
            Param.ParameterName = ParameterName;
            Param.Value = (ParameterValue == null ? (object)DBNull.Value : ParameterValue);
            Param.SqlDbType = _Type;
            Param.Size = TypeSize;
            Param.Direction = ParamDir;
            Comm.Parameters.Add(Param);
        }

        public void Parameter(string parameterName, object parameterValue)
        {
            Comm.Parameters.AddWithValue(parameterName, (parameterValue == null ? DBNull.Value : parameterValue));
        }

        public void Parameter(string parameterName, object parameterValue, string TypeName)
        {
            SqlParameter Param = Comm.CreateParameter();
            Param.ParameterName = parameterName;
            Param.Value = (parameterValue == null ? DBNull.Value : parameterValue);
            Param.SqlDbType = SqlDbType.Structured;
            Param.TypeName = TypeName;
            Param.Direction = ParameterDirection.Input;
            Comm.Parameters.Add(Param);
        }

        public void Parameter(string parameterName, string _Type, object parameterValue)
        {
            Comm.Parameters.AddWithValue(parameterName, (parameterValue == null ? DBNull.Value : parameterValue)).TypeName = _Type;
        }

        public object GetParamValue(string parameterName)
        {
            return Comm.Parameters[parameterName].Value;
        }
        public DataTable FillData()
        {
            DataTable dt = new DataTable();
            try
            {
                ReOpenConnection();
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = Comm;
                da.Fill(dt);
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
            finally
            {
                CloseDbConnection();
            }
            return dt;
        }
        public int ExecuteNonQuery(bool CloseConnection = true)
        {
            int Rows = 0;
            try
            {
                ReOpenConnection();
                Rows = Comm.ExecuteNonQuery();
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
            finally
            {
                if (CloseConnection)
                {
                    CloseDbConnection();
                }
            }
            return Rows;
        }
        public object ExecuteScalar()
        {
            object Res = null;
            try
            {
                ReOpenConnection();
                Res = Comm.ExecuteScalar();
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
            finally
            {
                CloseDbConnection();
            }
            return Res;
        }
        private void CloseDbConnection()
        {
            if (Con.State == ConnectionState.Open)
            {
                Con.Close();
            }
        }
        private void ReOpenConnection()
        {
            if ((Con != null))
            {
                if (Con.State == ConnectionState.Closed)
                {
                    Con.Open();
                }
            }
        }

        #endregion
    }
}