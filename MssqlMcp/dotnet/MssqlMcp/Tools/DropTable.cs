// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.ComponentModel;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;

namespace Mssql.McpServer;

public partial class Tools
{
    [McpServerTool(
        Title = "Drop Table",
        ReadOnly = false,
        Destructive = true),
        Description("Drops a table from a SQL database. Specify the database parameter to drop from the correct database.")]
    public async Task<DbOperationResult> DropTable(
        [Description("DROP TABLE SQL statement")] string sql,
        [Description("Target database name (e.g., adgilecloud, adgiledigital, adgileportal, totaltv). Required to drop from the correct database.")] string? database = null)
    {
        if (IsReadOnlyMode)
        {
            return new DbOperationResult(success: false, error: "DROP TABLE operation is not allowed in READONLY mode");
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
            _logger.LogError(ex, "DropTable failed: {Message}", ex.Message);
            return new DbOperationResult(success: false, error: ex.Message);
        }
    }
}
