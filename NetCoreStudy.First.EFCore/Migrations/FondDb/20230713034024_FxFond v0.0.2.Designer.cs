﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NetCoreStudy.First.EFCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace NetCoreStudy.First.EFCore.Migrations.FondDb
{
    [DbContext(typeof(FondDbContext))]
    [Migration("20230713034024_FxFond v0.0.2")]
    partial class FxFondv002
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("NetCoreStudy.First.Domain.Entity.Fond.FxContact", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("Email")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("FondContacts");
                });

            modelBuilder.Entity("NetCoreStudy.First.Domain.Entity.Fond.FxFondEvent", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("EventInitiator")
                        .HasColumnType("text");

                    b.Property<string>("Keyword")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("Type")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("FondEvents");
                });

            modelBuilder.Entity("NetCoreStudy.First.Domain.Entity.Fond.FxFondEvent", b =>
                {
                    b.OwnsMany("NetCoreStudy.First.Domain.Entity.Fond.FxFond", "Fonds", b1 =>
                        {
                            b1.Property<string>("FxFondEventId")
                                .HasColumnType("text");

                            b1.Property<string>("Id")
                                .HasColumnType("text");

                            b1.Property<decimal>("Amount")
                                .HasColumnType("numeric");

                            b1.Property<string>("Description")
                                .HasColumnType("text");

                            b1.Property<string>("ExContactId")
                                .HasColumnType("text");

                            b1.Property<string>("InContactId")
                                .HasColumnType("text");

                            b1.HasKey("FxFondEventId", "Id");

                            b1.ToTable("Fonds");

                            b1.WithOwner()
                                .HasForeignKey("FxFondEventId");
                        });

                    b.Navigation("Fonds");
                });
#pragma warning restore 612, 618
        }
    }
}
