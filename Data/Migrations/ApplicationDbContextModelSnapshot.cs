﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Nebula.Data;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Nebula.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.11")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("ClaimType")
                        .HasColumnType("text");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("text");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("integer");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("text");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("boolean");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("boolean");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("text");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("text");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("boolean");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("text");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("boolean");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("ClaimType")
                        .HasColumnType("text");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("text");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("text");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("text");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("text");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.Property<string>("RoleId")
                        .HasColumnType("text");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("Value")
                        .HasColumnType("text");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("Nebula.Data.Models.Caja", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)");

                    b.Property<bool>("SoftDeleted")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.ToTable("Cajas");
                });

            modelBuilder.Entity("Nebula.Data.Models.CajaDiaria", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<Guid?>("CajaId")
                        .HasColumnType("uuid");

                    b.Property<string>("Month")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)");

                    b.Property<bool>("SoftDeleted")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("State")
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)");

                    b.Property<decimal>("TotalApertura")
                        .HasColumnType("numeric");

                    b.Property<decimal>("TotalCierre")
                        .HasColumnType("numeric");

                    b.Property<decimal>("TotalContabilizado")
                        .HasColumnType("numeric");

                    b.Property<string>("Year")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("CajaId");

                    b.ToTable("CajasDiaria");
                });

            modelBuilder.Entity("Nebula.Data.Models.Contact", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Address")
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)");

                    b.Property<string>("Document")
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)");

                    b.Property<string>("Email")
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)");

                    b.Property<string>("Name")
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)");

                    b.Property<Guid?>("PeopleDocTypeId")
                        .HasColumnType("uuid");

                    b.Property<string>("PhoneNumber1")
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)");

                    b.Property<string>("PhoneNumber2")
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)");

                    b.Property<bool>("SoftDeleted")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.HasIndex("PeopleDocTypeId");

                    b.ToTable("Contacts");
                });

            modelBuilder.Entity("Nebula.Data.Models.PeopleDocType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Description")
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)");

                    b.Property<bool>("SoftDeleted")
                        .HasColumnType("boolean");

                    b.Property<string>("SunatCode")
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)");

                    b.HasKey("Id");

                    b.ToTable("PeopleDocTypes");
                });

            modelBuilder.Entity("Nebula.Data.Models.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Barcode")
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)");

                    b.Property<string>("Description")
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)");

                    b.Property<string>("Icbper")
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)");

                    b.Property<string>("IgvSunat")
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)");

                    b.Property<string>("PathImage")
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)");

                    b.Property<decimal?>("Price")
                        .HasColumnType("numeric");

                    b.Property<bool>("SoftDeleted")
                        .HasColumnType("boolean");

                    b.Property<string>("Type")
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)");

                    b.Property<Guid?>("UndMedidaId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("UndMedidaId");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("Nebula.Data.Models.UndMedida", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)");

                    b.Property<bool>("SoftDeleted")
                        .HasColumnType("boolean");

                    b.Property<string>("SunatCode")
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)");

                    b.HasKey("Id");

                    b.ToTable("UndMedida");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Nebula.Data.Models.CajaDiaria", b =>
                {
                    b.HasOne("Nebula.Data.Models.Caja", "Caja")
                        .WithMany("CajasDiaria")
                        .HasForeignKey("CajaId");

                    b.Navigation("Caja");
                });

            modelBuilder.Entity("Nebula.Data.Models.Contact", b =>
                {
                    b.HasOne("Nebula.Data.Models.PeopleDocType", "PeopleDocType")
                        .WithMany("Contacts")
                        .HasForeignKey("PeopleDocTypeId");

                    b.Navigation("PeopleDocType");
                });

            modelBuilder.Entity("Nebula.Data.Models.Product", b =>
                {
                    b.HasOne("Nebula.Data.Models.UndMedida", "UndMedida")
                        .WithMany("Products")
                        .HasForeignKey("UndMedidaId");

                    b.Navigation("UndMedida");
                });

            modelBuilder.Entity("Nebula.Data.Models.Caja", b =>
                {
                    b.Navigation("CajasDiaria");
                });

            modelBuilder.Entity("Nebula.Data.Models.PeopleDocType", b =>
                {
                    b.Navigation("Contacts");
                });

            modelBuilder.Entity("Nebula.Data.Models.UndMedida", b =>
                {
                    b.Navigation("Products");
                });
#pragma warning restore 612, 618
        }
    }
}
