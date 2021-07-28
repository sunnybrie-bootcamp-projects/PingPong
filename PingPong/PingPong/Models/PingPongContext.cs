using System;
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
                entity.HasNoKey();

                entity.ToTable("games");

                entity.Property(e => e.Date)
                    .IsRequired()
                    .IsRowVersion()
                    .IsConcurrencyToken()
                    .HasColumnName("date");

                entity.Property(e => e.LoseScore).HasColumnName("lose_score");

                entity.Property(e => e.TeamA).HasColumnName("team_a");

                entity.Property(e => e.TeamB).HasColumnName("team_b");

                entity.Property(e => e.Victor).HasColumnName("victor");

                entity.Property(e => e.WinScore).HasColumnName("win_score");

                entity.HasOne(d => d.TeamANavigation)
                    .WithMany()
                    .HasForeignKey(d => d.TeamA)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_games_teams");

                entity.HasOne(d => d.TeamBNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.TeamB)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_games_teams1");

                entity.HasOne(d => d.VictorNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.Victor)
                    .HasConstraintName("FK_games_teams2");
            });

            modelBuilder.Entity<Player>(entity =>
            {
                entity.ToTable("players");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DateJoined)
                    .IsRequired()
                    .IsRowVersion()
                    .IsConcurrencyToken()
                    .HasColumnName("date_joined");

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("first_name");

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

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DateFormed)
                    .IsRowVersion()
                    .IsConcurrencyToken()
                    .HasColumnName("date_formed");

                entity.Property(e => e.PlayerA).HasColumnName("player_a");

                entity.Property(e => e.PlayerB).HasColumnName("player_b");

                entity.Property(e => e.Teamname)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("teamname");

                entity.HasOne(d => d.PlayerANavigation)
                    .WithMany(p => p.TeamPlayerANavigations)
                    .HasForeignKey(d => d.PlayerA)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_teams_players1");

                entity.HasOne(d => d.PlayerBNavigation)
                    .WithMany(p => p.TeamPlayerBNavigations)
                    .HasForeignKey(d => d.PlayerB)
                    .HasConstraintName("FK_teams_players");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
