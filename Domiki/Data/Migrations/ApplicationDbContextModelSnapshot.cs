﻿// <auto-generated />
using System;
using Domiki.Web.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Domiki.Web.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.22")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Domiki.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers", (string)null);
                });

            modelBuilder.Entity("Domiki.Web.Data.Domik", b =>
                {
                    b.Property<int>("PlayerId")
                        .HasColumnType("int")
                        .HasColumnOrder(1);

                    b.Property<int>("Id")
                        .HasColumnType("int")
                        .HasColumnOrder(2);

                    b.Property<int>("Level")
                        .HasColumnType("int");

                    b.Property<int>("TypeId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpgradeCalculateDate")
                        .HasColumnType("datetime2");

                    b.Property<double?>("UpgradeSeconds")
                        .HasColumnType("float");

                    b.HasKey("PlayerId", "Id");

                    b.ToTable("Domiks");
                });

            modelBuilder.Entity("Domiki.Web.Data.DomikType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("LogicName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("MaxCount")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("DomikTypes");
                });

            modelBuilder.Entity("Domiki.Web.Data.DomikTypeLevel", b =>
                {
                    b.Property<int>("DomikTypeId")
                        .HasColumnType("int")
                        .HasColumnOrder(1);

                    b.Property<int>("Value")
                        .HasColumnType("int")
                        .HasColumnOrder(2);

                    b.Property<int>("MaxManufactureCount")
                        .HasColumnType("int");

                    b.Property<int>("UpgradeSeconds")
                        .HasColumnType("int");

                    b.HasKey("DomikTypeId", "Value");

                    b.ToTable("DomikTypeLevels");
                });

            modelBuilder.Entity("Domiki.Web.Data.DomikTypeLevelModificator", b =>
                {
                    b.Property<int>("DomikTypeLevelDomikTypeId")
                        .HasColumnType("int")
                        .HasColumnOrder(1);

                    b.Property<int>("DomikTypeLevelValue")
                        .HasColumnType("int")
                        .HasColumnOrder(2);

                    b.Property<int>("ModificatorTypeId")
                        .HasColumnType("int")
                        .HasColumnOrder(3);

                    b.Property<int>("Value")
                        .HasColumnType("int");

                    b.HasKey("DomikTypeLevelDomikTypeId", "DomikTypeLevelValue", "ModificatorTypeId");

                    b.HasIndex("ModificatorTypeId");

                    b.ToTable("DomikTypeLevelModificators");
                });

            modelBuilder.Entity("Domiki.Web.Data.DomikTypeLevelReceipt", b =>
                {
                    b.Property<int>("DomikTypeLevelDomikTypeId")
                        .HasColumnType("int")
                        .HasColumnOrder(1);

                    b.Property<int>("DomikTypeLevelValue")
                        .HasColumnType("int")
                        .HasColumnOrder(2);

                    b.Property<int>("ReceiptId")
                        .HasColumnType("int")
                        .HasColumnOrder(3);

                    b.HasKey("DomikTypeLevelDomikTypeId", "DomikTypeLevelValue", "ReceiptId");

                    b.HasIndex("ReceiptId");

                    b.ToTable("DomikTypeLevelReceipts");
                });

            modelBuilder.Entity("Domiki.Web.Data.DomikTypeLevelResource", b =>
                {
                    b.Property<int>("DomikTypeLevelDomikTypeId")
                        .HasColumnType("int")
                        .HasColumnOrder(1);

                    b.Property<int>("DomikTypeLevelValue")
                        .HasColumnType("int")
                        .HasColumnOrder(2);

                    b.Property<int>("ResourceTypeId")
                        .HasColumnType("int")
                        .HasColumnOrder(3);

                    b.Property<int>("Value")
                        .HasColumnType("int");

                    b.HasKey("DomikTypeLevelDomikTypeId", "DomikTypeLevelValue", "ResourceTypeId");

                    b.HasIndex("ResourceTypeId");

                    b.ToTable("DomikTypeLevelResources");
                });

            modelBuilder.Entity("Domiki.Web.Data.Manufacture", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("DomikId")
                        .HasColumnType("int");

                    b.Property<int>("DomikPlayerId")
                        .HasColumnType("int");

                    b.Property<DateTime>("FinishDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("PlodderCount")
                        .HasColumnType("int");

                    b.Property<int>("ReceiptId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("DomikPlayerId", "DomikId");

                    b.ToTable("Manufactures");
                });

            modelBuilder.Entity("Domiki.Web.Data.ModificatorType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("LogicName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("ModificatorTypes");
                });

            modelBuilder.Entity("Domiki.Web.Data.Player", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("AspNetUserId")
                        .IsRequired()
                        .HasMaxLength(450)
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<Guid>("Version")
                        .IsConcurrencyToken()
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("AspNetUserId")
                        .IsUnique();

                    b.ToTable("Players");
                });

            modelBuilder.Entity("Domiki.Web.Data.Receipt", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("DurationSeconds")
                        .HasColumnType("int");

                    b.Property<string>("LogicName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PlodderCount")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Receipts");
                });

            modelBuilder.Entity("Domiki.Web.Data.ReceiptResource", b =>
                {
                    b.Property<int>("ReceiptId")
                        .HasColumnType("int")
                        .HasColumnOrder(1);

                    b.Property<int>("ResourceTypeId")
                        .HasColumnType("int")
                        .HasColumnOrder(2);

                    b.Property<bool>("IsInput")
                        .HasColumnType("bit")
                        .HasColumnOrder(3);

                    b.Property<int>("Value")
                        .HasColumnType("int");

                    b.HasKey("ReceiptId", "ResourceTypeId", "IsInput");

                    b.HasIndex("ResourceTypeId");

                    b.ToTable("ReceiptResources");
                });

            modelBuilder.Entity("Domiki.Web.Data.Resource", b =>
                {
                    b.Property<int>("PlayerId")
                        .HasColumnType("int")
                        .HasColumnOrder(2);

                    b.Property<int>("TypeId")
                        .HasColumnType("int")
                        .HasColumnOrder(1);

                    b.Property<int>("Value")
                        .HasColumnType("int");

                    b.HasKey("PlayerId", "TypeId");

                    b.ToTable("Resources");
                });

            modelBuilder.Entity("Domiki.Web.Data.ResourceType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("LogicName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("ResourceTypes");
                });

            modelBuilder.Entity("Duende.IdentityServer.EntityFramework.Entities.DeviceFlowCodes", b =>
                {
                    b.Property<string>("UserCode")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("ClientId")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<DateTime>("CreationTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Data")
                        .IsRequired()
                        .HasMaxLength(50000)
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("DeviceCode")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<DateTime?>("Expiration")
                        .IsRequired()
                        .HasColumnType("datetime2");

                    b.Property<string>("SessionId")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("SubjectId")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.HasKey("UserCode");

                    b.HasIndex("DeviceCode")
                        .IsUnique();

                    b.HasIndex("Expiration");

                    b.ToTable("DeviceCodes", (string)null);
                });

            modelBuilder.Entity("Duende.IdentityServer.EntityFramework.Entities.Key", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Algorithm")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<string>("Data")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("DataProtected")
                        .HasColumnType("bit");

                    b.Property<bool>("IsX509Certificate")
                        .HasColumnType("bit");

                    b.Property<string>("Use")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("Version")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("Use");

                    b.ToTable("Keys");
                });

            modelBuilder.Entity("Duende.IdentityServer.EntityFramework.Entities.PersistedGrant", b =>
                {
                    b.Property<string>("Key")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("ClientId")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<DateTime?>("ConsumedTime")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("CreationTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Data")
                        .IsRequired()
                        .HasMaxLength(50000)
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<DateTime?>("Expiration")
                        .HasColumnType("datetime2");

                    b.Property<string>("SessionId")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("SubjectId")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Key");

                    b.HasIndex("ConsumedTime");

                    b.HasIndex("Expiration");

                    b.HasIndex("SubjectId", "ClientId", "Type");

                    b.HasIndex("SubjectId", "SessionId", "Type");

                    b.ToTable("PersistedGrants", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("ProviderKey")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("Name")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("Domiki.Web.Data.DomikTypeLevel", b =>
                {
                    b.HasOne("Domiki.Web.Data.DomikType", "DomikType")
                        .WithMany("Levels")
                        .HasForeignKey("DomikTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("DomikType");
                });

            modelBuilder.Entity("Domiki.Web.Data.DomikTypeLevelModificator", b =>
                {
                    b.HasOne("Domiki.Web.Data.ModificatorType", "ModificatorType")
                        .WithMany()
                        .HasForeignKey("ModificatorTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domiki.Web.Data.DomikTypeLevel", "DomikTypeLevel")
                        .WithMany()
                        .HasForeignKey("DomikTypeLevelDomikTypeId", "DomikTypeLevelValue")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("DomikTypeLevel");

                    b.Navigation("ModificatorType");
                });

            modelBuilder.Entity("Domiki.Web.Data.DomikTypeLevelReceipt", b =>
                {
                    b.HasOne("Domiki.Web.Data.Receipt", "Receipt")
                        .WithMany()
                        .HasForeignKey("ReceiptId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domiki.Web.Data.DomikTypeLevel", "DomikTypeLevel")
                        .WithMany()
                        .HasForeignKey("DomikTypeLevelDomikTypeId", "DomikTypeLevelValue")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("DomikTypeLevel");

                    b.Navigation("Receipt");
                });

            modelBuilder.Entity("Domiki.Web.Data.DomikTypeLevelResource", b =>
                {
                    b.HasOne("Domiki.Web.Data.ResourceType", "ResourceType")
                        .WithMany()
                        .HasForeignKey("ResourceTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domiki.Web.Data.DomikTypeLevel", "DomikTypeLevel")
                        .WithMany()
                        .HasForeignKey("DomikTypeLevelDomikTypeId", "DomikTypeLevelValue")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("DomikTypeLevel");

                    b.Navigation("ResourceType");
                });

            modelBuilder.Entity("Domiki.Web.Data.Manufacture", b =>
                {
                    b.HasOne("Domiki.Web.Data.Domik", "Domik")
                        .WithMany("Manufactures")
                        .HasForeignKey("DomikPlayerId", "DomikId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Domik");
                });

            modelBuilder.Entity("Domiki.Web.Data.ReceiptResource", b =>
                {
                    b.HasOne("Domiki.Web.Data.Receipt", "Receipt")
                        .WithMany("Resources")
                        .HasForeignKey("ReceiptId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domiki.Web.Data.ResourceType", "ResourceType")
                        .WithMany()
                        .HasForeignKey("ResourceTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Receipt");

                    b.Navigation("ResourceType");
                });

            modelBuilder.Entity("Domiki.Web.Data.Resource", b =>
                {
                    b.HasOne("Domiki.Web.Data.Player", "Player")
                        .WithMany("Resources")
                        .HasForeignKey("PlayerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Player");
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
                    b.HasOne("Domiki.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Domiki.Models.ApplicationUser", null)
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

                    b.HasOne("Domiki.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Domiki.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Domiki.Web.Data.Domik", b =>
                {
                    b.Navigation("Manufactures");
                });

            modelBuilder.Entity("Domiki.Web.Data.DomikType", b =>
                {
                    b.Navigation("Levels");
                });

            modelBuilder.Entity("Domiki.Web.Data.Player", b =>
                {
                    b.Navigation("Resources");
                });

            modelBuilder.Entity("Domiki.Web.Data.Receipt", b =>
                {
                    b.Navigation("Resources");
                });
#pragma warning restore 612, 618
        }
    }
}
