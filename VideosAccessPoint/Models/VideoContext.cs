using System.Data.Entity;

namespace VideosAccessPoint.Models
{
    public class VideoContext : DbContext
    {
        public DbSet<User> Users { set; get; }
        public DbSet<Video> Videos { set; get; }
    }
}