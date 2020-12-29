using System.Data.Common;
namespace DataTable
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Linq;
    using DataTableEfCore;
    using DataTableEfCore.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Spectre.Console;
    using Spectre.Console.Extensions.Table;
    using StackExchange.Profiling;
    using StackExchange.Profiling.Data;

    internal static class Program
    {
        public static void Main()
        {
            var serviceProvider = BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            var ctx = scope.ServiceProvider.GetRequiredService<PersonContext>();

            // personContext.Database.EnsureCreated();
            try
            {
                var profiler = MiniProfiler.StartNew("DataTableEfCore Demo");
                using (profiler.Step("Retrieve..."))
                {
                    IQueryable<Person> query = ctx.Persons;
                    var dataTable = RetrieveDataTable(ctx, query);
                    var dataset = new DataSet
                    {
                        DataSetName = "Greatest of all time",
                    };
                    dataset.Tables.Add(RetrieveDataTable(ctx, query));
                    using (profiler.Step("Display..."))
                    {
                        AnsiConsole.Render(
                            dataset.FromDataSet(configurePanel: opt =>
                                opt.BorderColor(Color.Aqua)));
                    }
                }

                _ = profiler.Stop();
                var diagnosticsPanel = new Panel(profiler.RenderPlainText()).RoundedBorder();
                diagnosticsPanel.Header = new PanelHeader("Diagnostics");
                AnsiConsole.Render(diagnosticsPanel);
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteException(ex, ExceptionFormats.ShortenEverything);
            }
        }

        private static ServiceProvider BuildServiceProvider()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddDbContext<PersonContext>(opt =>
                opt.UseSqlite("Data Source=people.db")
                .LogTo(Console.WriteLine, LogLevel.Information));

            var serviceProvider = services.BuildServiceProvider();
            return serviceProvider;
        }

        private static DataTable RetrieveDataTable(PersonContext personContext, IQueryable<Person> query)
        {
            var connection = personContext.Database.GetDbConnection();
            connection = new ProfiledDbConnection(connection, MiniProfiler.Current);
            connection.Open();
            using var cmd = connection.CreateCommand();
            cmd.Connection = connection;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = query.ToQueryString();
            using DbDataReader reader = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(reader);
            return dt;
        }
    }
}
