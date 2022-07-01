using ForumApp.Store.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
namespace ForumApp.Store
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public DbSet<Attachments>Attachments { get; set; }
        public DbSet<Messages>Messages { get; set; }

        public DbSet<Topics>Topics { get; set; }
        public DbSet<ForumSections> ForumSections { get; set; }
        public DbSet<ModeratedSections> ModeratedSections { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            


            modelBuilder.Entity<Attachments>().HasKey(x => x.Id);
            modelBuilder.Entity<Attachments>()
                .HasOne(x => x.Message)
                .WithMany(x => x.Attachments)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Messages>().HasKey(x => x.Id);
            modelBuilder.Entity<Messages>()
                .HasOne(x => x.Topic)
                .WithMany(x => x.Messages)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Topics>().HasKey(x => x.Id);
            modelBuilder.Entity<Topics>()
                .HasOne(x => x.ForumSection)
                .WithMany(x => x.Topics)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ModeratedSections>().HasKey(x => x.Id);
                
            

        }
    }
}
