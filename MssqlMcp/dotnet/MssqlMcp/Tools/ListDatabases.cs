// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.ComponentModel;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;

namespace Mssql.McpServer;

public partial class Tools
{
    private const string ListDatabasesQuery = @"SELECT name FROM sys.databases WHERE state = 0 ORDER BY name";

    [McpServerTool(
        Title = "List Databases",
        ReadOnly = true,
        Idempotent = true,
        Destructive = false),
        Description("Lists all available databases on the SQL Server. Call this first to discover database names like adgilecloud, adgiledigital, adgileportal, totaltv, etc. Use the returned database names with other tools via the 'database' parameter.")]
    public async Task<DbOperationResult> ListDatabases()
    {
        try
        {
            using var conn = await _connectionFactory.GetOpenConnectionAsync();
            using var cmd = new SqlCommand(ListDatabasesQuery, conn);
            var databases = new List<string>();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                databases.Add(reader.GetString(0));
            }
            return new DbOperationResult(success: true, data: databases);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ListDatabases failed: {Message}", ex.Message);
            return new DbOperationResult(success: false, error: ex.Message);
        }
    }
}
