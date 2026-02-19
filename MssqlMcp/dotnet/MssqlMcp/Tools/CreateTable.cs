// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.ComponentModel;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;

namespace Mssql.McpServer;

public partial class Tools
{
    [McpServerTool(
        Title = "Create Table",
        ReadOnly = false,
        Destructive = false),
        Description("Creates a new table in a SQL database. Specify the database parameter to create the table in the correct database.")]
    public async Task<DbOperationResult> CreateTable(
        [Description("CREATE TABLE SQL statement")] string sql,
        [Description("Target database name. Required to create table in the correct database. Use ListDatabases to discover available names.")] string? database = null)
    {
        if (IsReadOnlyMode)
        {
            return new DbOperationResult(success: false, error: "CREATE TABLE operation is not allowed in READONLY mode");
        }

        var conn = await _connectionFactory.GetOpenConnectionAsync(database);
        try
        {
            using (conn)
            {
                using var cmd = new Microsoft.Data.SqlClient.SqlCommand(sql, conn);
                _ = await cmd.ExecuteNonQueryAsync();
                return new DbOperationResult(success: true);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CreateTable failed: {Message}", ex.Message);
            return new DbOperationResult(success: false, error: ex.Message);
        }
    }
}
