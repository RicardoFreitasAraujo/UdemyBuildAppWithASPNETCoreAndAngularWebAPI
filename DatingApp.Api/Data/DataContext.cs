﻿using DatingApp.Api.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.Api.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Like>().HasKey(k => new { k.LikerId, k.LikeeId });
            modelBuilder.Entity<Like>()
                    .HasOne(u => u.Likee)
                    .WithMany(u => u.Likers)
                    .HasForeignKey(u => u.LikeeId)
                    .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Like>()
             .HasOne(u => u.Liker)
             .WithMany(u => u.Likees)
             .HasForeignKey(u => u.LikerId)
             .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>()
                .HasOne(u => u.Sender)
                .WithMany(m => m.MessageSent)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>()
             .HasOne(u => u.Recipient)
             .WithMany(m => m.MessageReceived)
             .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }

        /*
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }*/

        public DbSet<Value> Values { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Message> Messages { get; set; }
    }
}
