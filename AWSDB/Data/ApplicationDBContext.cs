using AWSDB.Models;
using Microsoft.EntityFrameworkCore;

namespace AWSDB.Data
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions options) : base(options) { }

        public DbSet<LeadDetailsEntity> Articulo { get; set; }
    }

}
