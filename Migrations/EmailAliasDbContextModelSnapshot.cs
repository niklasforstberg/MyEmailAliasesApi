﻿// <auto-generated />
using System;
using EmailAliasApi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace MyEmailAliasesApi.Migrations
{
    [DbContext(typeof(EmailAliasDbContext))]
    partial class EmailAliasDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("EmailAliasApi.Models.EmailAlias", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("AliasAddress")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("LastUpdated")
                        .HasColumnType("datetime2");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("EmailAliases");
                });

            modelBuilder.Entity("EmailAliasApi.Models.EmailForwarding", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("EmailAliasId")
                        .HasColumnType("int");

                    b.Property<string>("ForwardingAddress")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("EmailAliasId");

                    b.ToTable("EmailForwardings");
                });

            modelBuilder.Entity("EmailAliasApi.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Role")
                        .HasColumnType("int");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("EmailAliasApi.Models.EmailAlias", b =>
                {
                    b.HasOne("EmailAliasApi.Models.User", "User")
                        .WithMany("EmailAliases")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("EmailAliasApi.Models.EmailForwarding", b =>
                {
                    b.HasOne("EmailAliasApi.Models.EmailAlias", "EmailAlias")
                        .WithMany("ForwardingAddresses")
                        .HasForeignKey("EmailAliasId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("EmailAlias");
                });

            modelBuilder.Entity("EmailAliasApi.Models.EmailAlias", b =>
                {
                    b.Navigation("ForwardingAddresses");
                });

            modelBuilder.Entity("EmailAliasApi.Models.User", b =>
                {
                    b.Navigation("EmailAliases");
                });
#pragma warning restore 612, 618
        }
    }
}
