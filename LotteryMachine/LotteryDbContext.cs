using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotteryMachine
{
    public class LotteryDbContext:DbContext
    {
        public DbSet<LotteryRecord> LotteryRecords { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=lotteryrecord.db");
        }
    }
}
