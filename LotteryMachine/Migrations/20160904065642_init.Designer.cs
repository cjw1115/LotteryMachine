using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using LotteryMachine;

namespace LotteryMachine.Migrations
{
    [DbContext(typeof(LotteryDbContext))]
    [Migration("20160904065642_init")]
    partial class init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.0-rtm-21431");

            modelBuilder.Entity("LotteryMachine.LotteryRecord", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreateTime");

                    b.Property<int>("Level");

                    b.Property<string>("LotteryNum");

                    b.HasKey("ID");

                    b.ToTable("LotteryRecords");
                });
        }
    }
}
