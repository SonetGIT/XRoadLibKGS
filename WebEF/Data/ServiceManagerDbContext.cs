using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebEF.Models;

namespace WebEF.Data
{
    public class ServiceManagerDbContext : DbContext
    {
        public ServiceManagerDbContext(DbContextOptions<ServiceManagerDbContext> options)
            : base(options)
        {

        }

        public DbSet<TransmitHistoryItem> TransmitHistoryItems { get; set; }
        public DbSet<ServiceDetail> ServiceDetails { get; set; }
        public DbSet<ServiceCode> ServiceCodes { get; set; }
    }
}
