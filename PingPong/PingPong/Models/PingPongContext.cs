﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace PingPong.Models
{
    public partial class PingPongContext : DbContext
    {
        public PingPongContext()
        {
        }

        public PingPongContext(DbContextOptions<PingPongContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Game> Games { get; set; }
        public virtual DbSet<Player> Players { get; set; }
        public virtual DbSet<Team> Teams { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=DESKTOP-4JOHSKQ;Initial Catalog=PingPong;Integrated Security=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Game>(entity =>
            {
                entity.ToTable("games");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Date)
                    .HasColumnType("datetime")
                    .HasColumnName("date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.LoseScore).HasColumnName("lose_score");

                entity.Property(e => e.TeamA).HasColumnName("team_a");

                entity.Property(e => e.TeamB).HasColumnName("team_b");

                entity.Property(e => e.Victor).HasColumnName("victor");

                entity.Property(e => e.WinScore).HasColumnName("win_score");

                /*entity.HasOne(d => d.TeamANavigation)
                    .WithMany(p => p.GameTeamANavigations)
                    .HasForeignKey(d => d.TeamA)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_games_teams");

                entity.HasOne(d => d.TeamBNavigation)
                    .WithMany(p => p.GameTeamBNavigations)
                    .HasForeignKey(d => d.TeamB)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_games_teams1");

                entity.HasOne(d => d.VictorNavigation)
                    .WithMany(p => p.GameVictorNavigations)
                    .HasForeignKey(d => d.Victor)
                    .HasConstraintName("FK_games_teams2");*/
            });

            modelBuilder.Entity<Player>(entity =>
            {
                entity.ToTable("players");

                entity.HasIndex(e => e.Username, "UQ__players__F3DBC572BBF9E480")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DateJoined)
                    .HasColumnType("datetime")
                    .HasColumnName("date_joined")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("first_name");

                entity.Property(e => e.ImageUrl)
                    .HasColumnType("text")
                    .HasColumnName("image_url")
                    .HasDefaultValueSql("('https://cdn0.iconfinder.com/data/icons/streamline-emoji-1/48/018-slightly-smiling-face-256.png')");

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("last_name");

                entity.Property(e => e.Username)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("username");
            });

            modelBuilder.Entity<Team>(entity =>
            {
                entity.ToTable("teams");

                entity.HasIndex(e => e.Teamname, "UQ__teams__A97CE74621BF8768")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DateFormed)
                    .HasColumnType("datetime")
                    .HasColumnName("date_formed")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.PlayerA).HasColumnName("player_a");

                entity.Property(e => e.PlayerB).HasColumnName("player_b");

                entity.Property(e => e.Teamname)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("teamname");

                /*entity.HasOne(d => d.PlayerANavigation)
                    .WithMany(p => p.TeamPlayerANavigations)
                    .HasForeignKey(d => d.PlayerA)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_teams_players1");

                entity.HasOne(d => d.PlayerBNavigation)
                    .WithMany(p => p.TeamPlayerBNavigations)
                    .HasForeignKey(d => d.PlayerB)
                    .HasConstraintName("FK_teams_players");*/
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
