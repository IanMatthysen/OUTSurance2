using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public static class SQLHelper
    {
        #region Private Methods

        private static string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["IanOutsuranceAssessment"].ConnectionString;
        }

        #endregion Private Methods

        #region Public Methods

        public static int ExecuteStoredProcedure(string storedProcedureName)
        {
            int numAffectedRows = 0;

            numAffectedRows = ExecuteParameterizedStoredProcedure(storedProcedureName, null);

            return numAffectedRows;
        }

        public static int ExecuteParameterizedStoredProcedure(string storedProcedureName, List<SqlParameter> parameters)
        {
            int affectedRecords = 0;

            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand command = new SqlCommand(storedProcedureName, connection))
                {
                    command.CommandTimeout = 600;
                    command.CommandType = CommandType.StoredProcedure;
                    if (parameters != null)
                    {
                        foreach (SqlParameter parameter in parameters)
                        {
                            command.Parameters.AddWithValue(parameter.ParameterName, parameter.Value);
                        }
                    }
                    connection.Open();
                    affectedRecords = command.ExecuteNonQuery();
                    connection.Close();
                }
            }

            return affectedRecords;
        }

        //execute stored procedure and return dataset

        /// <summary>
        /// Executes the stored procedure named storedProcedureName and return a DataSet
        /// </summary>
        /// <param name="storedProcedureName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static DataSet GetDataFromStoredProcedure(string storedProcedureName, SqlParameter[] parameters)
        {
            DataSet dataSet = new DataSet();

            using (SqlConnection sqlConnection = new SqlConnection(GetConnectionString()))
            {
                sqlConnection.Open();

                using (SqlCommand sqlCommand = new SqlCommand(storedProcedureName, sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.CommandTimeout = 600;

                    if (parameters != null)
                    {
                        foreach (var parameter in parameters)
                        {
                            sqlCommand.Parameters.AddWithValue(parameter.ParameterName, parameter.Value);
                        }
                    }

                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
                    {
                        sqlDataAdapter.Fill(dataSet);
                    }
                }
            }

            return dataSet;
        }

        public static DataSet GetDataFromStoredProcedure(string storedProcedureName)
        {
            return GetDataFromStoredProcedure(storedProcedureName, null);
        }

        public static List<string> GetStoredProcedureResultsAsList(string storedProcedureName, List<SqlParameter> parameters, List<int> columnsIndicesToIncludeInListItem, string dataTableValuesSeparator, string nullValueIndicator, bool appendNullValuesWithSeparator)
        {
            List<string> result = new List<string>();

            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand command = new SqlCommand(storedProcedureName, connection))
                {
                    command.CommandTimeout = 600;
                    command.CommandType = CommandType.StoredProcedure;
                    if (parameters != null)
                    {
                        foreach (SqlParameter parameter in parameters)
                        {
                            command.Parameters.AddWithValue(parameter.ParameterName, parameter.Value);
                        }
                    }
                    connection.Open();
                    SqlDataReader dataReader;
                    dataReader = command.ExecuteReader();

                    while (dataReader.Read())
                    {
                        string resultingListItem = String.Empty;

                        if (columnsIndicesToIncludeInListItem != null)
                        {
                            for (int a = 0; a < columnsIndicesToIncludeInListItem.Count; a++)
                            {
                                if (dataReader.IsDBNull(dataReader.GetOrdinal(dataReader.GetName(columnsIndicesToIncludeInListItem[a]))))
                                {
                                    if (!String.IsNullOrEmpty(resultingListItem.Trim()))
                                    {
                                        if (appendNullValuesWithSeparator)
                                        {
                                            resultingListItem += String.Format("{0}{1}", dataTableValuesSeparator, nullValueIndicator);
                                        }
                                    }
                                }
                                else
                                {
                                    //episode.AirDate = dataReader.GetDateTime(5);

                                    if (!String.IsNullOrEmpty(resultingListItem.Trim()))
                                    {
                                        resultingListItem += String.Format("{0}{1}", dataTableValuesSeparator, dataReader.GetValue(columnsIndicesToIncludeInListItem[a]) as string);
                                    }
                                    else
                                    {
                                        resultingListItem = dataReader.GetValue(columnsIndicesToIncludeInListItem[a]) as string;
                                    }
                                }
                            }
                        }
                        else
                        {
                            for (int a = 0; a < dataReader.FieldCount; a++)
                            {
                                if (dataReader.IsDBNull(a))
                                {
                                    if (!String.IsNullOrEmpty(resultingListItem.Trim()))
                                    {
                                        if (appendNullValuesWithSeparator)
                                        {
                                            resultingListItem += String.Format("{0}{1}", dataTableValuesSeparator, nullValueIndicator);
                                        }
                                    }
                                }
                                else
                                {
                                    //episode.AirDate = dataReader.GetDateTime(5);

                                    if (!String.IsNullOrEmpty(resultingListItem.Trim()))
                                    {
                                        resultingListItem += String.Format("{0}{1}", dataTableValuesSeparator, dataReader.GetValue(a) as string);
                                    }
                                    else
                                    {
                                        resultingListItem = dataReader.GetValue(a) as string;
                                    }
                                }
                            }
                        }

                        if (!String.IsNullOrEmpty(resultingListItem))
                        {
                            result.Add(resultingListItem);
                        }
                    }
                    connection.Close();
                }
            }

            return result;
        }

        public static List<string> GetStoredProcedureResultsAsList(string storedProcedureName, List<int> columnsIndicesToIncludeInListItem, string dataTableValuesSeparator, string nullValueIndicator, bool appendNullValuesWithSeparator)
        {
            return GetStoredProcedureResultsAsList(storedProcedureName, null, columnsIndicesToIncludeInListItem, dataTableValuesSeparator, nullValueIndicator, appendNullValuesWithSeparator);
        }

        public static List<string> GetStoredProcedureResultsAsList(string storedProcedureName, string dataTableValuesSeparator, string nullValueIndicator, bool appendNullValuesWithSeparator)
        {
            return GetStoredProcedureResultsAsList(storedProcedureName, null, null, dataTableValuesSeparator, nullValueIndicator, appendNullValuesWithSeparator);
        }

        #endregion Public Methods
    }
}
