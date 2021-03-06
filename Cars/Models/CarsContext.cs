using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cars.Models
{
    public class CarsContext : IdentityDbContext<ApplicationUser>
    {
        public CarsContext(DbContextOptions<CarsContext> options) : base(options)
        {
        }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<CustomerContact> CustomerContacts { get; set; } 
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderDetails> OrderDetails { get; set; }
        public virtual DbSet<OrderDetailsType> OrderDetailsType { get; set; }
        public virtual DbSet<Vehicle> Vehicle { get; set; }
        public virtual DbSet<Workflow> Workflows { get; set; }
        public virtual DbSet<VendorLocation> VendorLocations { get; set; }       
        public virtual DbSet<WorkflowOrderDetailsLog> WorkflowOrderDetailsLogs { get; set; }
        public virtual DbSet<Status> Status { get; set; }
        public virtual DbSet<OrderDetailsStatusLog> OrderDetailsStatusLogs { get; set; }
        public virtual DbSet<BranchModel> Branches { get; set; }
        public virtual DbSet<UserBranchModel> UserBranches { get; set; }
        public virtual DbSet<QuotationDocument> QuotationDocuments { get; set; }
        public virtual DbSet<QuotationStatusLogs> QuotationStatusLogs { get; set; }
        public virtual DbSet<Quotation> Quotations { get; set; }
        public virtual DbSet<Finance> Finances { get; set; }
        public virtual DbSet<FinanceDocument> FinanceDocuments { get; set; }
        public virtual DbSet<Runner> Runners { get; set; }
        public virtual DbSet<StatusLogDocument> StatusLogDocuments { get; set; }
        public virtual DbSet<Inventory> Inventory { get; set; }
        public virtual DbSet<InventoryDocument> InventoryDocuments { get; set; }
        public virtual DbSet<NotificationUser> NotificationUser { get; set; }
        public virtual DbSet<Notification> Notification { get; set; }

        public virtual DbSet<TeamDuration> TeamDurations { get; set; }

        public virtual DbSet<TeamMemberAllowed> TeamMemberAlloweds { get; set; }

        public virtual DbSet<UsersLogs> UsersLogs { get; set; }
        public virtual DbSet<Brand> Brand { get; set; }
        public virtual DbSet<BrandModel> BrandModels { get; set; }
        public virtual DbSet<ModelYear> ModelYears { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}