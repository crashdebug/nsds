using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using NSDS.Data;

namespace src.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20170305230525_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.0-rtm-22752");

            modelBuilder.Entity("WebApplication.Models.Client", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Address")
                        .IsRequired();

                    b.Property<bool>("Enabled");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<int>("PoolId");

                    b.HasKey("Id");

                    b.HasIndex("PoolId");

                    b.ToTable("Clients");
                });

            modelBuilder.Entity("WebApplication.Models.Module", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ClientId");

                    b.Property<string>("Endpoint")
                        .IsRequired();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("Version");

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.ToTable("Modules");
                });

            modelBuilder.Entity("WebApplication.Models.Package", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("Version");

                    b.HasKey("Id");

                    b.ToTable("Packages");
                });

            modelBuilder.Entity("WebApplication.Models.Pool", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Pools");
                });

            modelBuilder.Entity("WebApplication.Models.Client", b =>
                {
                    b.HasOne("WebApplication.Models.Pool", "Pool")
                        .WithMany("Clients")
                        .HasForeignKey("PoolId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("WebApplication.Models.Module", b =>
                {
                    b.HasOne("WebApplication.Models.Client", "Client")
                        .WithMany("Modules")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
