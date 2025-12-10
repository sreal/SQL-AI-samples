// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Data.SqlClient;

namespace Mssql.McpServer;

public class SqlConnectionFactory : ISqlConnectionFactory
{
    public async Task<SqlConnection> GetOpenConnectionAsync(string? database = null)
    {
        var connectionString = GetConnectionString();

        if (!string.IsNullOrEmpty(database))
        {
            var builder = new SqlConnectionStringBuilder(connectionString);
            builder.InitialCatalog = database;
            connectionString = builder.ConnectionString;
        }

        // Let ADO.Net handle connection pooling
        var conn = new SqlConnection(connectionString);
        await conn.OpenAsync();
        return conn;
    }

    private static string GetConnectionString()
    {
        var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");

        return string.IsNullOrEmpty(connectionString)
            ? throw new InvalidOperationException("Connection string is not set in the environment variable 'CONNECTION_STRING'.")
            : connectionString;
    }
}
