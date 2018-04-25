using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CoreMyAppAncket.Models;
using CoreMyAppAncket.Models.AncketVievModels;

namespace CoreMyAppAncket.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Ancket> Anckets { get; set; }
        public DbSet<AncketForm> AncketForms { get; set; }
        public DbSet<AncketResult> AncketResults { get; set; }
        public DbSet<AncketFormResult> AncketFormResults { get; set; }


        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
            builder.Entity<AncketForm>(e =>
            {
                e.HasOne(r => r.Ancket).WithMany(t => t.AncketForms).HasForeignKey(r => r.AncketId).OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<AncketFormResult>(e =>
            {
                e.HasOne(r => r.AncketResult).WithMany(t => t.AncketFormResults).HasForeignKey(r => r.AncketResultId).OnDelete(DeleteBehavior.Cascade);
            });          
           
        }
    }
}
