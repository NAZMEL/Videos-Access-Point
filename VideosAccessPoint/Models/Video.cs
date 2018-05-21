using System;
using System.ComponentModel.DataAnnotations;

namespace VideosAccessPoint.Models
{
    public class Video
    {
        public int Id { set; get; }

        [Required(ErrorMessage = "The Title field must not be empty")]
        public string Title { set; get; }
        public string Description { set; get; }
        public string Genre { set; get; }
        public string VideoFormat { set; get; }
        public DateTime Date { set; get; }
        public int? UserId { set; get; }

        public User _User { get; set; }
    }
}