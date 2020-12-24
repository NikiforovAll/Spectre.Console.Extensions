namespace DataTable
{
    using System;
    using System.Data;
    using System.Linq;
    using Bogus;
    using Spectre.Console;
    using Spectre.Console.Extensions.Table;
    using DataSet = System.Data.DataSet;

    internal static class Program
    {
        private static readonly Faker<Person> FakePersons = new Faker<Person>()
            .RuleFor(p => p.FirstName, f => f.Person.FirstName)
            .RuleFor(p => p.LastName, f => f.Person.LastName)
            .RuleFor(p => p.Address, f => new Faker<Address>()
                .RuleFor(address => address.City, f1 => f1.Address.City())
                .RuleFor(address => address.Street, f2 => f2.Address.StreetName())
                .RuleFor(address => address.ZipCode, f3 => f3.Random.Number(0, 1_000)).Generate());

        private static void Main(string[] args)
        {
            var dataSet = GenerateDataSet();

            var panel = dataSet
                .FromDataSet((cfg) =>
                    cfg.HeaderAlignment(Justify.Left)
                        .BorderColor(Color.Grey));

            AnsiConsole.Render(panel);
        }

        private static DataRow ToDataRow(Person person, DataTable dataTable)
        {
            DataRow row = dataTable.NewRow();
            row[nameof(Person.FirstName)] = person.FirstName;
            row[nameof(Person.LastName)] = person.LastName;
            row[nameof(Person.Address)] = $"{person.Address?.Street}, {person.Address?.City}";
            return row;
        }

        private static DataSet GenerateDataSet()
        {
            DataSet ds = new();
            ds.DataSetName = "PersonDataSet";
            ds.Tables.Add(GenerateDataTable("People1"));
            ds.Tables.Add(GenerateDataTable("People2"));
            return ds;

            static DataTable GenerateDataTable(string tableName)
            {
                DataTable dt = new();
                dt.TableName = tableName;
                _ = dt.Columns.Add(nameof(Person.FirstName), typeof(string));
                _ = dt.Columns.Add(nameof(Person.LastName), typeof(string));
                _ = dt.Columns.Add(nameof(Person.Address), typeof(string));

                const int peopleSize = 10;
                foreach (var row in FakePersons.Generate(peopleSize).Select(person => ToDataRow(person, dt)))
                {
                    dt.Rows.Add(row);
                }

                return dt;
            }
        }
    }

    internal class Person
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public Address Address { get; set; }
    }

    internal class Address
    {
        public string City { get; set; }

        public string Street { get; set; }

        public int ZipCode { get; set; }
    }
}
