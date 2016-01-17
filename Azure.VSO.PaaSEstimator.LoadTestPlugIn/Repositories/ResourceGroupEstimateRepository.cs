using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Azure.VSO.PaaSEstimator.LoadTestPlugIn.Models;

namespace Azure.VSO.PaaSEstimator.LoadTestPlugIn.Repositories
{
    public class ResourceGroupEstimateRepository : DbContext
    {
        public ResourceGroupEstimateRepository(string connectionString) 
            : base (connectionString)
        {
        }

        public DbSet<ResourceGroupCostEstimate> ResourceGroupCostEstimates { get; set; }
        public DbSet<ResourceCostEstimate> ResourceCostEstimates { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}
