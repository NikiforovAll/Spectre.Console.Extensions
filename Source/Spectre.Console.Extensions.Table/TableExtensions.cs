namespace Spectre.Console.Extensions.Table
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Spectre.Console.Rendering;
    using SpectreTable = Spectre.Console.Table;

    /// <summary>
    /// A set of extension methods to display <see cref="System.Data.DataTable"/> and <see cref="System.Data.DataSet"/>.
    /// </summary>
    public static class TableExtensions
    {
        /// <summary>
        /// Builds a table to be displayed.
        /// </summary>
        /// <param name="dataTable">Data source.</param>
        /// <returns>A new <see cref="Spectre.Console.Table"/> with populated data.</returns>
        public static SpectreTable FromDataTable(this DataTable dataTable)
        {
            var consoleTable = new SpectreTable { Title = new TableTitle(dataTable.TableName) };
            foreach (var column in dataTable.Columns.OfType<DataColumn>())
            {
                string title = column.Caption ?? column.ColumnName ?? string.Empty;
                _ = consoleTable.AddColumn(new TableColumn($"[grey]{title}[/]"));
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

        /// <summary>
        /// Builds a collection of tables based on provided <see cref="System.Data"/> instance.
        /// </summary>
        /// <param name="dataSet">Data source.</param>
        /// <returns>A new <see cref="IEnumerable{T}"/> of <see cref="Spectre.Console.Table"/>.</returns>
        public static IEnumerable<SpectreTable> FromDataSet(this DataSet dataSet)
        {
            foreach (var table in dataSet.Tables.OfType<DataTable>())
            {
                yield return table.FromDataTable();
            }
        }

        /// <summary>
        /// Builds a collection of tables based on provided <see cref="System.Data"/> instance.
        /// </summary>
        /// <param name="dataSet">Data source.</param>
        /// <param name="configurePanel">A delegate to configure the container <see cref="Spectre.Console.Panel"/>.</param>
        /// <returns>A rendarable panel.</returns>
        public static IRenderable FromDataSet(this DataSet dataSet, Action<Panel>? configurePanel)
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
