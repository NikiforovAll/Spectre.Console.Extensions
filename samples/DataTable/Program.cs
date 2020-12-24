namespace DataTable
{
    using System;
    using System.Data;
    using System.Linq;
    using Bogus;
    using Spectre.Console;
    using SpectreTable = Spectre.Console.Table;
    using Spectre.Console.Extensions.Table;

    // TODO: add example: EF core in memory database + datatable + spectre.console
    // TODO: add unit tests; integration tests
    // https://stackoverflow.com/questions/23697467/returning-datatable-using-entity-framework


    internal static class Program
    {
        private static readonly Faker<Person> FakePersons = new Faker<Person>()
            .RuleFor(p => p.FirstName, f => f.Person.FirstName)
            .RuleFor(p => p.LastName, f => f.Person.LastName)
            .RuleFor(p => p.Address, f => new Faker<Address>()
                .RuleFor(address => address.City, f1 => f1.Address.City())
                .RuleFor(address => address.Street, f2 => f2.Address.StreetName())
                .RuleFor(address => address.ZipCode, f3 => f3.Random.Number(0, 1_000)).Generate());

        private static void Main()
        {
            var dataTable = GenerateDataTable();

            const int peopleSize = 10;
            foreach (var row in FakePersons.Generate(peopleSize).Select(person => ToDataRow(person, dataTable)))
            {
                dataTable.Rows.Add(row);
            }

            var tableToDisplay = dataTable.FromDataTable()
                .Border(TableBorder.Rounded);
            AnsiConsole.Render(tableToDisplay);
        }

        private static DataRow ToDataRow(Person person, DataTable dataTable)
        {
            DataRow row = dataTable.NewRow();
            row[nameof(Person.FirstName)] = person.FirstName;
            row[nameof(Person.LastName)] = person.LastName;
            row[nameof(Person.Address)] = $"{person.Address?.Street}, {person.Address?.City}";
            return row;
        }

        private static DataTable GenerateDataTable()
        {
            DataTable dt = new();
            dt.TableName = "Test";
            _ = dt.Columns.Add(nameof(Person.FirstName), typeof(string));
            _ = dt.Columns.Add(nameof(Person.LastName), typeof(string));
            _ = dt.Columns.Add(nameof(Person.Address), typeof(string));
            return dt;
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
