using Microsoft.EntityFrameworkCore;
using NatsDemo.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NatsDemo.Core.Data
{
    public class NatsDemoDbContext : DbContext
    {
        public NatsDemoDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Auction> Auctions { get; set; }
        public DbSet<Car> Carss { get; set; }
        public DbSet<Bid> Bids { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Auction>()
            .HasOne(a => a.Car)
            .WithMany()
            .HasForeignKey(a => a.CarId);

            modelBuilder.Entity<Bid>()
                .HasOne(b => b.Auction)
                .WithMany(a => a.Bids)
                .HasForeignKey(b => b.AuctionId);
        }
    }
}
