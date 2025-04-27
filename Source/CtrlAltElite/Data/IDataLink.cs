// <copyright file="IDataLink.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SteamStore.Data
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IDataLink
    {
        void OpenConnection();

        void CloseConnection();

        T? ExecuteScalar<T>(string storedProcedure, SqlParameter[] ? sqlParameters = null);

        DataTable ExecuteReader(string storedProcedure, SqlParameter[] ? sqlParameters = null);

        int ExecuteNonQuery(string storedProcedure, SqlParameter[] ? sqlParameters = null);

        SqlConnection GetConnection();
    }
}
