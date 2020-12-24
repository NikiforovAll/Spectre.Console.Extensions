namespace Spectre.Console.Extensions.Table
{
    using System.Data;
    using System.Linq;
    using SpectreTable = Spectre.Console.Table;

    public static class TableExtensions
    {
        public static SpectreTable FromDataTable(this DataTable dataTable)
        {
            var consoleTable = new SpectreTable();
            consoleTable.Title = new TableTitle(dataTable.TableName);
            foreach (var column in dataTable.Columns.OfType<DataColumn>())
            {
                string title = column.Caption ?? column.ColumnName ?? string.Empty;
                title = $"[grey]{title}[/]";
                _ = consoleTable.AddColumn(new TableColumn(title));
            }

            foreach (var row in dataTable.Rows.OfType<DataRow>())
            {
                _ = consoleTable.AddRow(row
                    .ItemArray
                    .OfType<string>()
                    .ToArray());
            }

            return consoleTable;
        }
    }
}
