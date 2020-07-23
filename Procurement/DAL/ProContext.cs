using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using Procurement.Models;

namespace Procurement.DAL
{
    public class ProContext : DbContext
    {
        public DbSet<Users> Users { get; set; }
        public DbSet<Orders> Orders { get; set; }
       
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            
            modelBuilder.Entity<Users>().MapToStoredProcedures();
        }
    }
}