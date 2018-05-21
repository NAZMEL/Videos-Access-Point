using System.Collections.Generic;


namespace VideosAccessPoint.Models
{
    public class User
    {
        public int Id { set; get; }
        public string Name { set; get; }
        public string Password { set; get; }
        public ICollection<Video> _Video { set; get; }
    }
}