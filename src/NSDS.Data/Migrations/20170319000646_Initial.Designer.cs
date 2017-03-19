using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using NSDS.Data;

namespace NSDS.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20170319000646_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.1");

            modelBuilder.Entity("NSDS.Core.BaseVersion", b =>
                {
                    b.Property<string>("Version")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.HasKey("Version");

                    b.ToTable("Versions");

                    b.HasDiscriminator<string>("Discriminator").HasValue("BaseVersion");
                });

            modelBuilder.Entity("NSDS.Core.Command", b =>
                {
                    b.Property<string>("Name")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.HasKey("Name");

                    b.ToTable("Command");

                    b.HasDiscriminator<string>("Discriminator").HasValue("Command");
                });

            modelBuilder.Entity("NSDS.Data.Models.ClientDataModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Address")
                        .IsRequired();

                    b.Property<DateTime>("Created");

                    b.Property<bool>("Enabled");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<int>("PoolId");

                    b.HasKey("Id");

                    b.HasIndex("PoolId");

                    b.ToTable("Clients");
                });

            modelBuilder.Entity("NSDS.Data.Models.ClientModuleDataModel", b =>
                {
                    b.Property<int>("ClientId");

                    b.Property<int>("ModuleId");

                    b.Property<string>("VersionId");

                    b.HasKey("ClientId", "ModuleId");

                    b.HasIndex("ModuleId");

                    b.HasIndex("VersionId");

                    b.ToTable("ClientModules");
                });

            modelBuilder.Entity("NSDS.Data.Models.DeploymentCommandsDataModel", b =>
                {
                    b.Property<string>("CommandName");

                    b.Property<int>("DeploymentId");

                    b.Property<int>("Order");

                    b.HasKey("CommandName", "DeploymentId");

                    b.HasIndex("DeploymentId");

                    b.ToTable("DeploymentCommands");
                });

            modelBuilder.Entity("NSDS.Data.Models.DeploymentDataModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Deployments");
                });

            modelBuilder.Entity("NSDS.Data.Models.ModuleDataModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("DeploymentId");

                    b.Property<string>("Endpoint")
                        .IsRequired();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("DeploymentId");

                    b.ToTable("Modules");
                });

            modelBuilder.Entity("NSDS.Data.Models.PackageDataModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<int?>("DeploymentId");

                    b.Property<int>("ModuleId");

                    b.Property<string>("ModuleName");

                    b.Property<string>("VersionId");

                    b.HasKey("Id");

                    b.HasIndex("DeploymentId");

                    b.HasIndex("ModuleId");

                    b.HasIndex("VersionId");

                    b.ToTable("Packages");
                });

            modelBuilder.Entity("NSDS.Data.Models.PoolDataModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Pools");
                });

            modelBuilder.Entity("NSDS.Core.DateVersion", b =>
                {
                    b.HasBaseType("NSDS.Core.BaseVersion");


                    b.ToTable("DateVersion");

                    b.HasDiscriminator().HasValue("DateVersion");
                });

            modelBuilder.Entity("NSDS.Web.Commands.SshCommand", b =>
                {
                    b.HasBaseType("NSDS.Core.Command");


                    b.ToTable("SshCommand");

                    b.HasDiscriminator().HasValue("SshCommand");
                });

            modelBuilder.Entity("NSDS.Data.Models.ClientDataModel", b =>
                {
                    b.HasOne("NSDS.Data.Models.PoolDataModel", "Pool")
                        .WithMany("Clients")
                        .HasForeignKey("PoolId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("NSDS.Data.Models.ClientModuleDataModel", b =>
                {
                    b.HasOne("NSDS.Data.Models.ClientDataModel", "Client")
                        .WithMany("Modules")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("NSDS.Data.Models.ModuleDataModel", "Module")
                        .WithMany("Clients")
                        .HasForeignKey("ModuleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("NSDS.Core.BaseVersion", "Version")
                        .WithMany()
                        .HasForeignKey("VersionId");
                });

            modelBuilder.Entity("NSDS.Data.Models.DeploymentCommandsDataModel", b =>
                {
                    b.HasOne("NSDS.Core.Command", "Command")
                        .WithMany()
                        .HasForeignKey("CommandName")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("NSDS.Data.Models.DeploymentDataModel", "Deployment")
                        .WithMany("DeploymentCommands")
                        .HasForeignKey("DeploymentId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("NSDS.Data.Models.ModuleDataModel", b =>
                {
                    b.HasOne("NSDS.Data.Models.DeploymentDataModel", "Deployment")
                        .WithMany()
                        .HasForeignKey("DeploymentId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("NSDS.Data.Models.PackageDataModel", b =>
                {
                    b.HasOne("NSDS.Data.Models.DeploymentDataModel", "Deployment")
                        .WithMany()
                        .HasForeignKey("DeploymentId");

                    b.HasOne("NSDS.Data.Models.ModuleDataModel", "Module")
                        .WithMany()
                        .HasForeignKey("ModuleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("NSDS.Core.BaseVersion", "Version")
                        .WithMany()
                        .HasForeignKey("VersionId");
                });
        }
    }
}
