// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.ComponentModel;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;

namespace Mssql.McpServer;

public partial class Tools
{
    [McpServerTool(
        Title = "Insert Data",
        ReadOnly = false,
        Destructive = false),
        Description("Inserts data into a table in a SQL database. Specify the database parameter to insert into the correct database.")]
    public async Task<DbOperationResult> InsertData(
        [Description("INSERT SQL statement")] string sql,
        [Description("Target database name (e.g., adgilecloud, adgiledigital, adgileportal, totaltv). Required to insert into the correct database.")] string? database = null)
    {
        var conn = await _connectionFactory.GetOpenConnectionAsync(database);
        try
        {
            using (conn)
            {
                using var cmd = new Microsoft.Data.SqlClient.SqlCommand(sql, conn);
                var rows = await cmd.ExecuteNonQueryAsync();
                return new DbOperationResult(success: true, rowsAffected: rows);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "InsertData failed: {Message}", ex.Message);
            return new DbOperationResult(success: false, error: ex.Message);
        }
    }
}
