﻿// <auto-generated />
using System;
using Know_Me_Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Know_Me_Api.Migrations
{
    [DbContext(typeof(DBContext))]
    [Migration("20191102202258_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.11-servicing-32099")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Know_Me_Api.Models.Role", b =>
                {
                    b.Property<int>("roleId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("role");

                    b.HasKey("roleId");

                    b.ToTable("Role");
                });

            modelBuilder.Entity("Know_Me_Api.Models.UserInfo", b =>
                {
                    b.Property<Guid>("userId")
                        .ValueGeneratedOnAdd();

                    b.Property<bool?>("IsExternal");

                    b.Property<string>("email")
                        .IsRequired();

                    b.Property<string>("firstName")
                        .IsRequired()
                        .HasMaxLength(20);

                    b.Property<bool>("isActive");

                    b.Property<string>("lastName")
                        .IsRequired()
                        .HasMaxLength(20);

                    b.Property<string>("password");

                    b.Property<int?>("roleId");

                    b.Property<string>("userName")
                        .IsRequired()
                        .HasMaxLength(20);

                    b.HasKey("userId");

                    b.HasIndex("roleId");

                    b.ToTable("UserInfo");
                });

            modelBuilder.Entity("Know_Me_Api.Models.UserInfo", b =>
                {
                    b.HasOne("Know_Me_Api.Models.Role", "Role")
                        .WithMany()
                        .HasForeignKey("roleId");
                });
#pragma warning restore 612, 618
        }
    }
}
