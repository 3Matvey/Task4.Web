using Microsoft.EntityFrameworkCore;
using Task4.Web.Models;

namespace Task4.Web.Data
{
    public sealed class AppDbContext(DbContextOptions<AppDbContext> options) 
        : DbContext(options)
    {
        public DbSet<User> Users => Set<User>();

        public DbSet<EmailConfirmationToken> EmailConfirmationTokens => Set<EmailConfirmationToken>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("pgcrypto");

            ConfigureUser(modelBuilder);
            ConfigureEmailToken(modelBuilder);
        }

        private static void ConfigureUser(ModelBuilder modelBuilder)
        {
            var b = modelBuilder.Entity<User>();

            b.ToTable("users");

            b.HasKey(x => x.Id);

            b.Property(x => x.Id)
                .HasDefaultValueSql("gen_random_uuid()");

            b.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200);

            b.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(320);

            b.Property(x => x.PasswordHash)
                .IsRequired();

            b.Property(x => x.Status)
                .IsRequired();

            b.Property(x => x.CreatedAtUtc)
                .IsRequired();

            b.HasIndex(x => x.Email)
                .IsUnique();
        }

        private static void ConfigureEmailToken(ModelBuilder modelBuilder)
        {
            var b = modelBuilder.Entity<EmailConfirmationToken>();

            b.ToTable("email_confirmation_tokens");

            b.HasKey(x => x.Id);

            b.Property(x => x.Id)
                .HasDefaultValueSql("gen_random_uuid()");

            b.Property(x => x.Token)
                .IsRequired();

            b.HasIndex(x => x.Token)
                .IsUnique();

            b.Property(x => x.CreatedAtUtc)
                .IsRequired();

            b.Property(x => x.ExpiresAtUtc)
                .IsRequired();

            b.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
