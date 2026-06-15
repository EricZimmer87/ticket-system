using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TicketSystem.Models;

namespace TicketSystem.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Comment> Comments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User -> Comment relationship
            // Prevent deleting a User if Comments exist
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.AppUser)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.AppUserId)
                .OnDelete(DeleteBehavior.Restrict); // Can use Restrict or NoAction

            // Configure User -> Ticket relationship
            // Prevent deleting a User if Tickets exist
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.CreatedByUser)
                .WithMany(u => u.CreatedTickets)
                .HasForeignKey(t => t.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict); // Can use Restrict or NoAction

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.UpdatedByUser)
                .WithMany(u => u.UpdatedTickets)
                .HasForeignKey(t => t.UpdatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
