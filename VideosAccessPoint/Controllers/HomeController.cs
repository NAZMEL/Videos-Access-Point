using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VideosAccessPoint.Models;
using System.Data.Entity;
using System.IO;
using System.Activities;
using VideosAccessPoint.Attribute;
using PagedList.Mvc;
using PagedList;

namespace VideosAccessPoint.Controllers
{
    public class HomeController : Controller
    {
        VideoContext db = new VideoContext();
        
        [Authorize]
        public ActionResult Index(int? page = 1)
        {
            IEnumerable<Video> videos = GetDataFromVideosDB();

            // count posts on 1 page
            int pageSize = 10;
            // number page
            int pageNumber = (page ?? 1);
            return View(videos.ToList().ToPagedList(pageNumber, pageSize));  
        }

       
        [Authorize]
        [HttpGet]
        public ActionResult AddNewVideo()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult AddNewVideo(Video newVideo, HttpPostedFileBase uploadVideo)
        {
            Video titleVideo = db.Videos.FirstOrDefault(v => v.Title == newVideo.Title.Trim());

            if(titleVideo == null)
            {
                if (uploadVideo != null)
                {
                    string fileFormat = Path.GetExtension(uploadVideo.FileName);
                    // control format of uploaded file
                    if (fileFormat == ".mp4" || fileFormat == ".swf" || fileFormat == ".ogg" || fileFormat == ".wmv" || fileFormat == ".mpeg" || fileFormat == ".muv")
                    {
                        // control uploaded size
                        if (uploadVideo.ContentLength > 100040000)  // Size of file must be 100Mb or smaller
                        {
                            ModelState.AddModelError("Select video", $"Your video has a size of {uploadVideo.ContentLength.ToString()} bytes which exceeded the limit of 100040000 bytes.  Please save your video in size of 100Mb or less and then try again.");
                            return View();

                        }

                        newVideo.UserId = GetUserId();
                        newVideo.Date = DateTime.Now;


                        if (newVideo.Title == string.Empty)
                        {
                            newVideo.Title = Path.GetFileName(uploadVideo.FileName);
                        }
                        newVideo.VideoFormat = string.Concat(newVideo.Title, fileFormat);

                        // saving video in UploadedVideos folder
                        uploadVideo.SaveAs(Server.MapPath($"~/UploadedVideos/{newVideo.VideoFormat}"));

                        db.Videos.Add(newVideo);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("Select Video", "Type of video must be .mp4, .swf, .ogg, .wmv, .mpeg or .muv");
                        return View();
                    }

                    
                }
                else
                {
                    ModelState.AddModelError("Select Video", "Video haven't uploaded. Please upload video.");
                    return View();
                }
            }
            else
            {
                ModelState.AddModelError("Title", "Video with this title exists. Write another title or upload another video.");
            }
            return View();
        }

        // Get all data about one video with the passed id
        [Authorize]
        [HttpGet]
        public ActionResult Article(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            Video video = db.Videos.Include(p => p._User).Where(v => v.Id == id).FirstOrDefault();
            if (video == null)
            {
                return RedirectToAction("Index");
            }
            return View(video);
        }


        // delete data about one video with the passed VideoId
        [Authorize]
        [HttpGet]
        public ActionResult Delete(int VideoId)
        {
            Video video = db.Videos.FirstOrDefault(v => v.Id == VideoId);
            if (video == null)
            {
                return RedirectToAction("Article/" + VideoId);
            }
            string path = video.VideoFormat;

            db.Videos.Remove(video);
            db.SaveChanges();

            // delete video from directory 
            System.IO.File.Delete(Server.MapPath($"~/UploadedVideos/{path}"));

            return RedirectToAction("Index");
        }

        // edit data about one video with the passed VideoId
        [Authorize]
        [HttpGet]
        public ActionResult Edit(int VideoId)
        {
            Video video = db.Videos.Include(p => p._User).Where(v => v.Id == VideoId).FirstOrDefault();
            if (video == null)
            {
                return RedirectToAction("Index");
            }
            return View(video);
        }

        // edit data about one video with the passed video object
        [Authorize]
        [HttpPost]
        public ActionResult Edit(Video editVideo)
        {
            editVideo.Date = DateTime.Now;
            db.Entry(editVideo).State = EntityState.Modified;  //UPDATE DATABASE
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Sorting(string genre)
        {
            Session["genreStatus"] = genre;
            return RedirectToAction("Index");
        }

        // Get data from DB for one User in date sort
        private IEnumerable<Video> GetDataFromVideosDB()
        {
            var genre = Session["genreStatus"];
            try
            {
                if (genre.ToString() != "all")
                {
                    return db.Videos.Include(u => u._User).Where(v => v._User.Name == User.Identity.Name).Where(v => v.Genre == genre.ToString()).OrderByDescending(v => v.Date);
                }
            }
            catch (System.NullReferenceException) { }

            return db.Videos.Include(u => u._User).Where(v => v._User.Name == User.Identity.Name).OrderByDescending(v => v.Date);
        }
      
        // Get user's Id
        private int GetUserId()
        {
           return  db.Users.Where(user => user.Name == User.Identity.Name).Select(user => user.Id).First();
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}