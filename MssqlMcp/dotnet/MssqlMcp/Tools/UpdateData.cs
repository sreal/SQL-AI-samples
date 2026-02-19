// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.ComponentModel;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;

namespace Mssql.McpServer;

public partial class Tools
{
    [McpServerTool(
        Title = "Update Data",
        ReadOnly = false,
        Destructive = true),
        Description("Updates data in a table in a SQL database. Specify the database parameter to update in the correct database.")]
    public async Task<DbOperationResult> UpdateData(
        [Description("UPDATE SQL statement")] string sql,
        [Description("Target database name. Required to update in the correct database. Use ListDatabases to discover available names.")] string? database = null)
    {
        if (IsReadOnlyMode)
        {
            return new DbOperationResult(success: false, error: "UPDATE operation is not allowed in READONLY mode");
        }

        var conn = await _connectionFactory.GetOpenConnectionAsync(database);
        try
        {
            using (conn)
            {
                using var cmd = new Microsoft.Data.SqlClient.SqlCommand(sql, conn);
                var rows = await cmd.ExecuteNonQueryAsync();
                return new DbOperationResult(true, null, rows);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "UpdateData failed: {Message}", ex.Message);
            return new DbOperationResult(false, ex.Message);
        }
    }
}

