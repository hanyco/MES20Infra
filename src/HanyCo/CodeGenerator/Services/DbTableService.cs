using HanyCo.Infra.CodeGen.Domain.Services;
using HanyCo.Infra.CodeGen.Domain.ViewModels;

using Library.Collections;
using Library.Data.SqlServer.Dynamics;
using Library.Exceptions.Validations;
using Library.Validations;

using Microsoft.Extensions.Configuration;

using Services.Helpers;

namespace Services.CodeGen;

internal sealed class DbTableService(IConfiguration configuration) : IDbTableService
{
    public async Task<IEnumerable<DbColumnViewModel>> GetColumns(string tableName, CancellationToken token = default)
    {
        var table = await GetTable(tableName, token);
        return table.NotNull($"Table '{tableName}' not found.").Columns.Select(DbColumnViewModel.FromDbColumn);
    }

    private async Task<Database> GetDb(CancellationToken token)
    {
        var connectionString = configuration.GetApplicationConnectionString();
        
        var db = await Database.GetDatabaseAsync(connectionString, cancellationToken: token);
        Check.MustBeNotNull(db, () => "Not connected to database or database not found.");
        return db;
    }

    public async Task<DbColumnViewModel> GetIdentityColumn(string tableName, CancellationToken token = default)
    {
        var table = await GetTable(tableName, token);
        var idCol = table.NotNull($"Table '{tableName}' not found.").Columns.FirstOrDefault(static x => x.IsIdentity || x.Name == "Id").NotNull();
        return DbColumnViewModel.FromDbColumn(idCol);
    }

    private async Task<Table?> GetTable(string tableName, CancellationToken token)
    {
        Check.MustBeNotNull(tableName);
        var db = await GetDb(token);
        return db.Tables[tableName];
    }

    public async Task<IReadOnlyList<Node<DbObjectViewModel>>> GetTablesTree(GetTablesTreeViewItemOptions options, CancellationToken token = default)
    {
        var (gatherColumns, reporter) =  (options.GatherColumns, options.Reporter);

        reporter?.Report(description: "Please wait a while.");
        var db = await this.GetDb(token);

        List<Node<DbObjectViewModel>> result = [];
        Node<DbObjectViewModel> schemaNode;
        Node<DbObjectViewModel> tableNode;
        Node<DbObjectViewModel> tableColumnsNode;
        Node<DbObjectViewModel> tablesNode;

        await Task.Factory.StartNew(() =>
        {
            reporter?.Report(description: "Initializing...");
            var max = db.GetTablesCount() + 1;
            var tables = db.Tables.Compact().ToList();
            var schemas = tables.Select(t => t.Schema).Compact().Distinct().ToList();
            var index = 1;
            foreach (var schema in schemas)
            {
                schemaNode = new(new(schema));
                tablesNode = new(new("Tables"));
                foreach (var table in tables.Where(x => x.Schema == schema))
                {
                    var value = DbTableViewModel.FromDbTable(table);
                    reporter?.Report(new(max, index++, $"Reading `{value}`..."));
                    tableNode = new(value, table.Name);
                    if (gatherColumns)
                    {
                        tableColumnsNode = new(new("Columns"));
                        foreach (var column in table.Columns)
                        {
                            _ = tableColumnsNode.AddChild(DbColumnViewModel.FromDbColumn(column));
                        }
                        _ = tableNode.AddChild(tableColumnsNode);
                    }
                    _ = tablesNode.AddChild(tableNode);
                }
                _ = schemaNode.AddChild(tablesNode);

                result.Add(schemaNode);
            }
        }, token);
        reporter?.End();
        return result.AsReadOnly();
    }
}