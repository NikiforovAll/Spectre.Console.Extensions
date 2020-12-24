namespace Spectre.Console.Extensions.Table
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Rendering;
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

        public static IEnumerable<SpectreTable> FromDataSet(this DataSet dataSet)
        {
            foreach (var table in dataSet.Tables.OfType<DataTable>())
            {
                yield return table.FromDataTable();
            }
        }

        public static IRenderable FromDataSet(this DataSet dataSet, Action<Panel> configurePanel)
        {
            var grid = new Grid();
            _ = grid.AddColumn();
            foreach (var table in dataSet.FromDataSet())
            {
                grid.AddRow(table);
            }

            var panel = new Panel(grid)
                .Header(dataSet.DataSetName);
            configurePanel?.Invoke(panel);
            return panel;
        }
    }
}
