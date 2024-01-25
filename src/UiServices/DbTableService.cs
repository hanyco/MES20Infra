



using Library.Collections;
using Library.Data.SqlServer.Dynamics;
using Library.Exceptions.Validations;
using Library.Validations;

namespace Services;

internal sealed class DbTableService : IDbTableService
{
    public async Task<IEnumerable<DbColumnViewModel>> GetColumnsAsync(string connectionString, string tableName, CancellationToken token = default)
    {
        Check.MustBeNotNull(connectionString);
        Check.MustBeNotNull(tableName);

        var db = await Database.GetDatabaseAsync(connectionString, cancellationToken: token);
        Check.MustBeNotNull(db, () => "Not connected to database or database not found.");
        return db.Tables[tableName].NotNull($"Table '{tableName}' not found.").Columns.Select(DbColumnViewModel.FromDbColumn);
    }

    public async Task<IReadOnlyList<Node<DbObjectViewModel>>> GetTablesTreeViewItemAsync(GetTablesTreeViewItemOptions options, CancellationToken token = default)
    {
        var (connectionString, gatherColumns, reporter) = options;
        reporter?.Report(description: "Please wait a while.");
        var db = await Database.GetDatabaseAsync(connectionString, cancellationToken: token);
        Check.MustBeNotNull(db, () => new NotFoundValidationException("Not connected to database or database not found. 💀"));

        List<Node<DbObjectViewModel>> result = [];
        Node<DbObjectViewModel> schemaNode;
        Node<DbObjectViewModel> tableNode;
        Node<DbObjectViewModel> tableColumnsNode;
        Node<DbObjectViewModel> tablesNode;

        await Task.Factory.StartNew(() =>
        {
            reporter?.Report(description: "Initializing...");
            var max = db.GetTablesCount() + 1;
            var tables = db.Tables.Compact<Table>().ToList<Table>();
            var schemas = tables.Select(t => t.Schema).Compact().Distinct().ToList();
            var index = 1;
            foreach (var schema in schemas)
            {
                schemaNode = new(new(schema));
                tablesNode = new(new("Tables"));
                foreach (var table in tables.Where<Table>(x => x.Schema == schema))
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