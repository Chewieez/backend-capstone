using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RepPortal.Models;

namespace RepPortal.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<ApplicationUser> ApplicationUser { get; set; }
        public DbSet<Store> Store { get; set; }
        public DbSet<Status> Status { get; set; }
        public DbSet<State> State { get; set; }
        public DbSet<UserState> UserState { get; set; }
        public DbSet<Flag> Flag { get; set; }
        public DbSet<StoreFlag> StoreFlag { get; set; }
        public DbSet<StoreNote> StoreNote { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            builder.Entity<Store>()
                .Property(s => s.DateCreated)
                .HasDefaultValueSql("strftime('%Y-%m-%d %H:%M:%S')");

            builder.Entity<StoreNote>()
               .Property(s => s.DateCreated)
               .HasDefaultValueSql("strftime('%Y-%m-%d %H:%M:%S')");

            builder.Entity<StoreFlag>()
               .Property(s => s.DateCreated)
               .HasDefaultValueSql("strftime('%Y-%m-%d %H:%M:%S')");
        }
    }
}
