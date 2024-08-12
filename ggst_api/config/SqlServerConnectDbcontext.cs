using ggst_api.entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace ggst_api.config
{
    public partial class SqlServerConnectDbcontext : DbContext
    {
        public virtual DbSet<PlayerInfoEntity> PlayerInfoEntities { get; set; }

        public virtual DbSet<TbLoginUser> TbLoginUsers { get; set; }

        public virtual DbSet<TbUser2Character> TbUser2Characters { get; set; }

        public SqlServerConnectDbcontext(DbContextOptions<SqlServerConnectDbcontext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TbLoginUser>(entity =>
            {
                entity.HasKey(e => e.LoginUserId).HasName("PRIMARY");

                entity.ToTable("tb_login_user");

                entity.HasIndex(e => e.LoginUserAccount, "login_user_account_UNIQUE").IsUnique();

                entity.Property(e => e.LoginUserId)
                    .HasColumnType("int(11)")
                    .HasColumnName("login_user_id");
                entity.Property(e => e.LoginUserAccount)
                    .HasMaxLength(45)
                    .HasColumnName("login_user_account");
                entity.Property(e => e.LoginUserName)
                    .HasMaxLength(45)
                    .HasColumnName("login_user_name");
                entity.Property(e => e.LoginUserPassword)
                    .HasMaxLength(45)
                    .HasColumnName("login_user_password");
                entity.Property(e => e.LoginUserRole)
                    .HasColumnType("int(11)")
                    .HasColumnName("login_user_role");

            });

            modelBuilder.Entity<PlayerInfoEntity>(entity =>
            {
                entity.HasKey(e => new { e.id, e.character }).HasName("PRIMARY");

                entity.ToTable("tb_player_info");

                entity.Property(e => e.id)
                    .HasMaxLength(50)
                    .HasColumnName("id");
                entity.Property(e => e.character)
                    .HasMaxLength(50)
                    .HasColumnName("character");
                entity.Property(e => e.character_short)
                    .HasMaxLength(50)
                    .HasDefaultValueSql("'NULL'")
                    .HasColumnName("character_short");
                entity.Property(e => e.cheater_status)
                    .HasMaxLength(50)
                    .HasDefaultValueSql("'NULL'")
                    .HasColumnName("cheater_status");
                entity.Property(e => e.game_count)
                    .HasDefaultValueSql("'NULL'")
                    .HasColumnType("int(11)")
                    .HasColumnName("game_count");
                entity.Property(e => e.hidden_status)
                    .HasMaxLength(50)
                    .HasDefaultValueSql("'NULL'")
                    .HasColumnName("hidden_status");
                entity.Property(e => e.name)
                    .HasMaxLength(50)
                    .HasColumnName("name");
                entity.Property(e => e.platform)
                    .HasMaxLength(50)
                    .HasDefaultValueSql("'NULL'")
                    .HasColumnName("platform");
                entity.Property(e => e.rating_deviation)
                    .HasDefaultValueSql("'NULL'")
                    .HasColumnType("int(11)")
                    .HasColumnName("rating_deviation");
                entity.Property(e => e.rating_value)
                    .HasDefaultValueSql("'NULL'")
                    .HasColumnType("int(11)")
                    .HasColumnName("rating_value");
                entity.Property(e => e.vip_status)
                    .HasMaxLength(50)
                    .HasDefaultValueSql("'NULL'")
                    .HasColumnName("vip_status");
            });

            modelBuilder.Entity<TbUser2Character>(entity =>
            {
                entity.HasKey(e => e.TbUser2CharacterId).HasName("PRIMARY");

                entity.ToTable("tb_user_2_character");

                entity.HasIndex(e => e.UserId, "tb_player_info_idx");

                entity.Property(e => e.TbUser2CharacterId)
                    .HasColumnType("int(11)")
                    .HasColumnName("tb_user_2_character_id");
                entity.Property(e => e.PlayUuid)
                    .HasMaxLength(50)
                    .HasColumnName("play_uuid");
                entity.Property(e => e.UserId)
                    .HasColumnType("int(11)")
                    .HasColumnName("user_id");

                entity.HasOne(d => d.User).WithMany(p => p.TbUser2Characters)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_tb_login_user");

            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
