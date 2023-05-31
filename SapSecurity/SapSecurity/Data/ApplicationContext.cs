using Microsoft.EntityFrameworkCore;
using SapSecurity.Model;

namespace SapSecurity.Data
{
    public class ApplicationContext : DbContext
    {

        #region Fields
        public DbSet<SensorDetail> SensorDetails { get; set; }
        public DbSet<SensorGroup> SensorGroups { get; set; }
        public DbSet<SensorLog> SensorLogs { get; set; }
        public DbSet<Zone> Zones { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<LoginInfo> LoginInfos { get; set; }
        public DbSet<CameraImage> CameraImages { get; set; }


        #endregion
        #region Methods


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }

        #endregion
        #region Utilities



        #endregion
        #region Ctor

        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
        }
        #endregion



    }
}
