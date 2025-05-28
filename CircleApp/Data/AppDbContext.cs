using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CircleApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CircleApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<Post> Posts { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<Report> Reports { get; set; }

        //Explicitly define relationship
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany(u => u.Posts)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.UserId);

            modelBuilder.Entity<Like>()
                .HasKey(l => new { l.PostId, l.UserId });
            modelBuilder.Entity<Like>()
                .HasOne(l => l.Post)
                .WithMany(p => p.Likes)
                .HasForeignKey(l => l.PostId)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Like>()
                .HasOne(l => l.User)
                .WithMany(u => u.Likes)
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Post)
                .WithMany(p => p.Comments)
                .HasForeignKey(c => c.PostId)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Comment>()
                .HasOne(c =>c.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Favorite>()
                .HasKey(f => new { f.PostId, f.UserId });
            modelBuilder.Entity<Favorite>()
                .HasOne(f => f.User)
                .WithMany(u => u.Favorites)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Favorite>()
                .HasOne(f => f.Post)
                .WithMany(p => p.Favorites)
                .HasForeignKey(c => c.PostId)
                .OnDelete(DeleteBehavior.NoAction);


            modelBuilder.Entity<Report>()
                .HasKey(r => new { r.PostId, r.UserId });
            modelBuilder.Entity<Report>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reports)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Report>()
                .HasOne(r => r.Post)
                .WithMany(p => p.Reports)
                .HasForeignKey(r => r.PostId)
                .OnDelete(DeleteBehavior.NoAction);

            base.OnModelCreating(modelBuilder);

        }
        
    }
}