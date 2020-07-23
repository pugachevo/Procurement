using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using Procurement.DAL;
using Procurement.Models;
using System.Threading.Tasks;

namespace Procurement.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<ProContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(ProContext context)
        {
            var users = new List<Users>();

            for (var i = 0; i < 1000; i++)
            {
                var user = new Users { Name = $"User {i}", StartDate = DateTime.Now };
                users.Add(user);
            }
            users[0].Name = "Joe";
            users[0].StartDate = DateTime.Parse("2010-09-01");

            users[1].Name = "Arturo";
            users[1].StartDate = DateTime.Parse("2010-09-01");

            users[2].Name = "Laura";
            users[2].StartDate = DateTime.Parse("2020-03-04");

            users[3].Name = "Nino";
            users[3].StartDate = DateTime.Now;

            users.ForEach(u => context.Users.AddOrUpdate(p => p.Name, u));
            context.SaveChanges();

            foreach (var task in users.Select(user => PerformDatabaseOperations(context, user.UserID)))
            {
                task.Wait();
            }
        }

        private static async Task PerformDatabaseOperations(ProContext context, int id)
        {
            var gen = new Random();
            var date = new DateTime(2000, 1, 1);
            var range = (DateTime.Today - date).Days;
            var rnd = new Random();

            var orders = new List<Orders>();

            for (var i = 0; i < 100; i++)
            {
                orders.Add(new Orders { Sum = rnd.Next(1, 1000), Date = date.AddDays(gen.Next(range)), UserID = id });
            }

            orders.ForEach(s => context.Orders.AddOrUpdate(p => p.ID, s));
            await context.SaveChangesAsync(); ;
        }
    }
}
