using Microsoft.EntityFrameworkCore;
using System;
using TokenService.Data.Entities;
using TokenService.Data.Seed;

namespace TokenService.Data
{
    public class TokenServiceContext : DbContext
    {
        public TokenServiceContext(DbContextOptions<TokenServiceContext> options)
            : base(options)
        {
        }

        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>(entity => {
                
                entity.ToTable("Roles", "Security");

                entity.Property(r => r.Name)
                    .HasMaxLength(100);

                entity.HasKey(r => r.Name)
                    .HasName("PK_Roles");

                entity.HasData(RoleSeed.Roles());

                entity.HasAnnotation("READONLY_ANNOTATION", true);

                entity.HasMany(r => r.UserRoles)
                    .WithOne(u => u.Role)
                    .HasForeignKey(u => u.RoleId)
                    .HasConstraintName("FK_UserRoles_Roles")
                    .IsRequired(true);
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.ToTable("UserRoles", "Security");

                entity.HasIndex(u => u.UserId)
                    .HasName("IX_UserRoles_UserId");

                entity.HasIndex(u => u.RoleId)
                    .HasName("IX_UserRoles_RoleId");

                entity.Property(u => u.RoleId)
                    .HasMaxLength(100);

                entity.Property(u => u.UserId)
                    .HasMaxLength(100);

                entity.HasKey(u => new { u.RoleId, u.UserId });

            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users", "Security");

                entity.HasKey(r => r.Id)
                    .HasName("PK_Users");

                entity.Property(r => r.Id)
                    .HasMaxLength(100);

                entity.Property(r => r.Username)
                    .HasMaxLength(100)
                    .IsRequired(true);

                entity.Property(r => r.Email)
                    .HasMaxLength(256)
                    .IsRequired(true);

                entity.Property(r => r.SecurityStamp)
                    .HasMaxLength(100)
                    .IsRequired(true);

                entity.Property(r => r.PasswordHash)
                    .HasMaxLength(100)
                    .IsRequired(true);

                entity.HasIndex(r => r.Username)
                    .HasName("IX_Users_Username")
                    .IsUnique();

                entity.HasIndex(r => r.Email)
                    .HasName("IX_Users_Email")
                    .IsUnique();

                entity.HasMany(r => r.UserRoles)
                    .WithOne(u => u.User)
                    .HasForeignKey(u => u.UserId)
                    .HasConstraintName("FK_UserRoles_Users")
                    .IsRequired(true);

            });
        }
    }
}
