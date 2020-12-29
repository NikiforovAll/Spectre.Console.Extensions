namespace DataTableEfCore
{
    using System.Collections.Generic;
    using System.Linq;
    using DataTableEfCore.Models;
    using Microsoft.EntityFrameworkCore;

    public class PersonContext : DbContext
    {
        public PersonContext(DbContextOptions<PersonContext> options)
            : base(options)
        {
        }

        public DbSet<Person> Persons => this.Set<Person>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            this.SeedData(builder);
        }

        private void SeedData(ModelBuilder builder)
        {
            List<Person> people = new ()
            {
                new ()
                {
                    FirstName = "Sebastian",
                    LastName = "Bach",
                    Age = 65,
                },
                new ()
                {
                    FirstName = "Ludwig",
                    LastName = "Beethoven",
                    Age = 56,
                },
                new ()
                {
                    FirstName = "Franz",
                    LastName = "Schubert",
                    Age = 31,
                },
                new ()
                {
                    FirstName = "Joseph",
                    LastName = "Haydn",
                    Age = 77,
                },
            };
            Enumerable.Range(1, people.Count)
                .ToList()
                .ForEach(i => people[i - 1].PersonId = i);

            builder.Entity<Person>().HasData(people);
            builder.Entity<Person>().ToTable("Composers");
        }
    }
}
