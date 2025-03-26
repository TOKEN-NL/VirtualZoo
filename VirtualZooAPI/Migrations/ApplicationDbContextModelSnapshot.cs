﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using VirtualZooAPI.Data;

#nullable disable

namespace VirtualZooAPI.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("VirtualZooShared.Models.Animal", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("ActivityPattern")
                        .HasColumnType("int");

                    b.Property<int>("CategoryId")
                        .HasColumnType("int");

                    b.Property<int>("DietaryClass")
                        .HasColumnType("int");

                    b.Property<int>("EnclosureId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Prey")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("SecurityRequirement")
                        .HasColumnType("int");

                    b.Property<int>("Size")
                        .HasColumnType("int");

                    b.Property<double>("SpaceRequirement")
                        .HasColumnType("float");

                    b.Property<string>("Species")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.HasIndex("EnclosureId");

                    b.ToTable("Animals");
                });

            modelBuilder.Entity("VirtualZooShared.Models.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("VirtualZooShared.Models.Enclosure", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("Climate")
                        .HasColumnType("int");

                    b.Property<int>("HabitatType")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("SecurityLevel")
                        .HasColumnType("int");

                    b.Property<double>("Size")
                        .HasColumnType("float");

                    b.Property<int>("ZooId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ZooId");

                    b.ToTable("Enclosures");
                });

            modelBuilder.Entity("VirtualZooShared.Models.Zoo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Zoos");
                });

            modelBuilder.Entity("VirtualZooShared.Models.Animal", b =>
                {
                    b.HasOne("VirtualZooShared.Models.Category", null)
                        .WithMany("Animals")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("VirtualZooShared.Models.Enclosure", null)
                        .WithMany("Animals")
                        .HasForeignKey("EnclosureId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("VirtualZooShared.Models.Enclosure", b =>
                {
                    b.HasOne("VirtualZooShared.Models.Zoo", null)
                        .WithMany("Enclosures")
                        .HasForeignKey("ZooId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("VirtualZooShared.Models.Category", b =>
                {
                    b.Navigation("Animals");
                });

            modelBuilder.Entity("VirtualZooShared.Models.Enclosure", b =>
                {
                    b.Navigation("Animals");
                });

            modelBuilder.Entity("VirtualZooShared.Models.Zoo", b =>
                {
                    b.Navigation("Enclosures");
                });
#pragma warning restore 612, 618
        }
    }
}
