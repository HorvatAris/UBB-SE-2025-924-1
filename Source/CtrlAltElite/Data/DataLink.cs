// <copyright file="DataLink.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

public class DataLink : SteamStore.Data.IDataLink
{
    private readonly string connectionString;
    private SqlConnection sqlConnection;


    public DataLink(IConfiguration configuration)
    {
        this.connectionString = configuration.GetConnectionString("ConnectionString");
        try
        {
            this.sqlConnection = new SqlConnection(this.connectionString);
        }
        catch (Exception exception)
        {
            throw new Exception($"Error initializing SQL connection: {this.connectionString}", exception);
        }
    }

    public SqlConnection GetConnection()
    {
        if (this.sqlConnection == null)
        {
            this.sqlConnection = new SqlConnection(this.connectionString);
        }

        return this.sqlConnection;
    }

    public void OpenConnection()
    {
        if (this.sqlConnection.State != ConnectionState.Open)
        {
            this.sqlConnection.Open();
        }
    }

    public void CloseConnection()
    {
        if (this.sqlConnection.State != ConnectionState.Closed)
        {
            this.sqlConnection.Close();
        }
    }

    public T ? ExecuteScalar<T>(string storedProcedure, SqlParameter[] ? sqlParameters = null)
    {
        try
        {
            this.OpenConnection();
            using (SqlCommand command = new SqlCommand(storedProcedure, this.sqlConnection))
            {
                command.CommandType = CommandType.StoredProcedure;

                if (sqlParameters != null)
                {
                    command.Parameters.AddRange(sqlParameters);
                }

                var result = command.ExecuteScalar();
                if (result == DBNull.Value || result == null)
                {
                    return default;
                }

                return (T)Convert.ChangeType(result, typeof(T));
            }
        }
        catch (Exception exception)
        {
            throw new Exception($"Error - ExecutingScalar : {exception.Message}");
        }
        finally
        {
            this.CloseConnection();
        }
    }

    public DataTable ExecuteReader(string storedProcedure, SqlParameter[] ? sqlParameters = null)
    {
        try
        {
            this.OpenConnection();
            using (SqlCommand command = new SqlCommand(storedProcedure, this.sqlConnection))
            {
                command.CommandType = CommandType.StoredProcedure;

                if (sqlParameters != null)
                {
                    command.Parameters.AddRange(sqlParameters);
                }

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    DataTable dataTable = new DataTable();
                    dataTable.Load(reader);
                    return dataTable;
                }
            }
        }
        catch (Exception exception)
        {
            throw new Exception($"Error - ExecuteReader : {exception.Message}");
        }
        finally
        {
            this.CloseConnection();
        }
    }

    public int ExecuteNonQuery(string storedProcedure, SqlParameter[] ? sqlParameters = null)
    {
        try
        {
            this.OpenConnection();
            using (SqlCommand command = new SqlCommand(storedProcedure, this.sqlConnection))
            {
                command.CommandType = CommandType.StoredProcedure;

                if (sqlParameters != null)
                {
                    command.Parameters.AddRange(sqlParameters);
                }

                return command.ExecuteNonQuery();
            }
        }
        catch (Exception exception)
        {
            throw new Exception($"Error - ExecuteNonQuery : {exception.Message}");
        }
        finally
        {
            this.CloseConnection();
        }
    }
}