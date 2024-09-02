﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WT_API.Data;

#nullable disable

namespace WT_API.Migrations
{
    [DbContext(typeof(Context))]
    [Migration("20240829121844_usersv2")]
    partial class usersv2
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.33")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("WT_API.Models.Author", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("id"), 1L, 1);

                    b.Property<string>("name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("id");

                    b.ToTable("Authors");
                });

            modelBuilder.Entity("WT_API.Models.Chapter", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("id"), 1L, 1);

                    b.Property<string>("link")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("nextChLinkXPath")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("otherNextChLinkXPaths")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("secondaryNextChLinkXPath")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("serialId")
                        .HasColumnType("int");

                    b.Property<string>("title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("id");

                    b.ToTable("Chapters");
                });

            modelBuilder.Entity("WT_API.Models.LikedSerial", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("id"), 1L, 1);

                    b.Property<int>("serialId")
                        .HasColumnType("int");

                    b.Property<int>("userId")
                        .HasColumnType("int");

                    b.HasKey("id");

                    b.ToTable("LikedSerials");
                });

            modelBuilder.Entity("WT_API.Models.Serial", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("id"), 1L, 1);

                    b.Property<int?>("authorId")
                        .HasColumnType("int");

                    b.Property<string>("firstCh")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("home")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("nextChLinkXPath")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("otherNextChLinkXPaths")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("reviewStatus")
                        .HasColumnType("bit");

                    b.Property<string>("secondaryNextChLinkXPath")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("titleXPath")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("id");

                    b.ToTable("Serials");
                });
#pragma warning restore 612, 618
        }
    }
}