using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using System.Web.Security;
using VideosAccessPoint.Attribute;
using VideosAccessPoint.Models;

namespace VideosAccessPoint.Controllers
{
    public class AuthentificationController : Controller
    {
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [MultipleButton(Name = "action", Argument = "register")]
        public ActionResult Register(Register newUser)
        {

            User user = null;
            if (ModelState.IsValid)
            {
                if (newUser.Password.Trim() != newUser.ConfirmPassword.Trim())
                {
                    ModelState.AddModelError("ConfirmPassword", "Password fields must be equal");
                }

                // check user in DB
                using (VideoContext db = new VideoContext())
                {
                    user = db.Users.FirstOrDefault(u => u.Name == newUser.Name);
                }

                if(user == null)
                {
                    using (VideoContext db = new VideoContext())
                    {
                        db.Users.Add(new Models.User { Name = newUser.Name, Password = CalculateMD5Hash(newUser.Password) });
                        db.SaveChanges();
                        return RedirectToAction("RegisterSuccess", "Authentification");  
                    }  
                }
                else
                {
                    ModelState.AddModelError("Name", "User with this name exists");
                }
            }
            return View(newUser);
        }

        public ActionResult RegisterSuccess()
        {
            return View();
        }

        // Clear fields
        [HttpPost, ActionName("Register")]
        [MultipleButton(Name = "action", Argument = "clear")]
        public ActionResult Clear()
        {
            return RedirectToAction("Register", "Authentification");
        }


        [HttpGet]
        public ActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [MultipleButton(Name = "action", Argument = "signin")]
        public ActionResult SignIn(Login newUser)
        {
            if (ModelState.IsValid)
            {
                User user = null;
                using (VideoContext db = new VideoContext())
                {
                     user = db.Users.FirstOrDefault(u => u.Name == newUser.Name);
                }
                if(user != null)
                {
                    // hashing password
                    string passwordHash = CalculateMD5Hash(newUser.Password);
                    using (VideoContext db = new VideoContext())
                    {
                        user = db.Users.FirstOrDefault(u => u.Name == newUser.Name && u.Password == passwordHash);
                    }
                    if(user != null)
                    {
                        FormsAuthentication.SetAuthCookie(newUser.Name, true);
                        Session["genreStatus"] = "all";
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError("", $"Name or Password are invalid");
                    }              
                }
                else
                {
                    ModelState.AddModelError("Name", $"User with this name isn't exist. Please sign up as a new user.");
                }
            }
            return View(newUser);
        }


        // Clear fields
        [HttpPost, ActionName("SignIn")]
        [MultipleButton(Name = "action", Argument = "clearSignin")]
        public ActionResult ClearSignIn()
        {
            return RedirectToAction("SignIn", "Authentification");
        }

        // Hashing procedure for password
        public string CalculateMD5Hash(string input)
        {
            // calculate MD5 hash from input
            MD5 md5 = MD5.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);
            //  convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }

        // sign out session
        public ActionResult SignOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }
    }

    
}