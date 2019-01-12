﻿// <auto-generated />

using CoreJsNoise.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace CoreJsNoise.Migrations
{
    [DbContext(typeof(PodcastsCtx))]
    [Migration("20180706071804_ProducersAndShows")]
    partial class ProducersAndShows
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.1.1-rtm-30846")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("WebApplication1.Domain.Producer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("FeedUrl");

                    b.Property<string>("Name");

                    b.Property<string>("Url");

                    b.HasKey("Id");

                    b.ToTable("Producers");
                });

            modelBuilder.Entity("WebApplication1.Domain.Show", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("Mp3");

                    b.Property<int>("ProducerId");

                    b.Property<string>("Title");

                    b.HasKey("Id");

                    b.HasIndex("ProducerId");

                    b.ToTable("Shows");
                });

            modelBuilder.Entity("WebApplication1.Domain.Show", b =>
                {
                    b.HasOne("WebApplication1.Domain.Producer", "Producer")
                        .WithMany("Shows")
                        .HasForeignKey("ProducerId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
