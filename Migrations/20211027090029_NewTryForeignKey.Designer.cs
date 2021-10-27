﻿// <auto-generated />
using System;
using Assignment1;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DataAccessConsoleAssignment.Migrations
{
    [DbContext(typeof(MoviesContext))]
    [Migration("20211027090029_NewTryForeignKey")]
    partial class NewTryForeignKey
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.11")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Assignment1.Movies", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("ReleaseDate")
                        .HasColumnType("Date");

                    b.Property<string>("Title")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("ID");

                    b.ToTable("Movies");
                });

            modelBuilder.Entity("Assignment1.Screenings", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("DateTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("MovieID")
                        .HasColumnType("int");

                    b.Property<int?>("MoviesID")
                        .HasColumnType("int");

                    b.Property<short>("Seats")
                        .HasColumnType("smallint");

                    b.HasKey("ID");

                    b.HasIndex("MoviesID");

                    b.ToTable("Screenings");
                });

            modelBuilder.Entity("Assignment1.Screenings", b =>
                {
                    b.HasOne("Assignment1.Movies", "Movies")
                        .WithMany()
                        .HasForeignKey("MoviesID");

                    b.Navigation("Movies");
                });
#pragma warning restore 612, 618
        }
    }
}
