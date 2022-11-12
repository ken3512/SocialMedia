using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SocialMedia.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Relationship>()
                .HasOne(r => r.Sender)
                .WithMany(u => u.Sent)
                .HasForeignKey(r => r.SenderId)
                .OnDelete(DeleteBehavior.ClientCascade);

            modelBuilder.Entity<Relationship>()
                .HasOne(r => r.Receiver)
                .WithMany(u => u.Received)
                .HasForeignKey(r => r.ReceiverId)
                .OnDelete(DeleteBehavior.ClientCascade);
            
            modelBuilder.Entity<PostMessage>()
                .HasOne(m => m.Post)
                .WithMany(p => p.Messages)
                .HasForeignKey(m => m.PostId)
                .OnDelete(DeleteBehavior.ClientCascade);

            modelBuilder.Entity<PostMessage>()
                .HasOne(m => m.User)
                .WithMany(u => u.Messages)
                .HasForeignKey(m => m.UserId)
                .OnDelete(DeleteBehavior.ClientCascade);
            
            modelBuilder.Entity<Post>()
                .HasMany(m => m.Likes)
                .WithMany(u => u.Liked);

            modelBuilder.Entity<Post>()
                .HasOne(m => m.User)
                .WithMany(u => u.Posts)
                .OnDelete(DeleteBehavior.ClientCascade);
        }
    
        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Relationship> Relationships { get; set; }
        public DbSet<PostMessage> Messages { get; set; }
    }
}