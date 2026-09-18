using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractMonthlyClaimSystem_
{
    public class ClaimContext : DbContext
    {
        public ClaimContext()
        {
        }

        public ClaimContext(DbContextOptions<ClaimContext>options): base(options)
        {
            
        }

        
        //Claim table /entities in the database
        public DbSet<Claim> Claims { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source = cliam.db");
        }
    }
}
