using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Client;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using WebProject.Models;

namespace WebProject.Controllers
{
    [Authorize]
    public class postingController : Controller
    {
        private string connectionString = "Data Source = (localdb)\\MSSQLLocalDB;Initial Catalog = YammerDB; Integrated Security = True;";
        private readonly ILogger<postingController> _logger;
        private readonly IWebHostEnvironment _env;
        public postingController(ILogger<postingController> logger, IWebHostEnvironment env)
        {
            _logger = logger;
            _env = env;
        }

        private void Shuffle<T>(List<T> list)
        {
            Random random = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
        public IActionResult Index()
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            PostingRepository postingRepository = new PostingRepository(connectionString);
            List<postANDimage> posts = postingRepository.getAll();
            Shuffle<postANDimage>(posts);

            string userName = postingRepository.getUserName(userId);
            ViewBag.UserId = userId;
            ViewBag.UserName = userName;
            ViewBag.profilePath = postingRepository.getUserProfilePictureFromDb(userId);

            //check if new user then add userId in userProfile table to edit profile
            postingRepository.checkAndAddUserId(userId, userName);

            IRepository<userProfile> repo = new GenericRepository<userProfile>(connectionString);
            var Id = repo.GetId(userId);
            var userProfilesDetails = repo.GetById(Id);
            userProfilesDetails.profilePath = postingRepository.setPath(userProfilesDetails.profilePath);
            userIndexViewModel indexView = new userIndexViewModel();
            indexView.UserProfile = userProfilesDetails;
            indexView.Posts = posts;
            string key = "welcomeMsg";
            string msg = string.Empty;
            if (HttpContext.Request.Cookies.ContainsKey(key))
            {
                string name = HttpContext.Request.Cookies[key];
                msg = $"Welcome back {name} !";
            }
            else
            {
                HttpContext.Response.Cookies.Append(key, userName);
                msg = $"Welcome {userName} to our company !";
            }
            ViewBag.cookieMsg = msg;
            ViewBag.DBmsg = TempData["msg"];
            return View(indexView);
        }
        [HttpPost]
        public IActionResult addProfilePicture(IFormFile image)
        {
            string wwwRootPath = _env.WebRootPath;
            string path = Path.Combine(wwwRootPath, "userProfileImage");

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            var file = image;
            PostingRepository postingRepository = new PostingRepository(connectionString);
            if (file.Length > 0)
            {
                string filePath = Path.Combine(path, file.FileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }
                string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                postingRepository.addUserProfilePictureInDb(filePath, userId);
            }

            return RedirectToAction("editProfile", "posting");
        }

        [HttpPost]
        public IActionResult AddPost(string publication, List<IFormFile> picture)
        {
            TempData["postUploaded"] = false;
            if (publication!=null && picture!=null)
            {

            string wwwRootPath = _env.WebRootPath;
            string path = Path.Combine(wwwRootPath, "uploadFiles");

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            Posting post = new Posting();
            post.userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            post.publication = publication;
            post.postDate = DateTime.Now;

            List<string> images = new List<string>();
            foreach (var file in picture)
            {
                if (file.Length > 0)
                {
                    string filePath = Path.Combine(path, file.FileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                        images.Add(filePath);
                    }
                }
            }
            PostingRepository postingRepository = new PostingRepository(connectionString);
            postingRepository.add(post, images);
            TempData["postUploaded"] = true;
            TempData["msg"] = "Post Uploaded Successfully !";
            }

            return RedirectToAction("index", "posting");
        }
        public IActionResult deletePost(int Id)
        {
            string UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            PostingRepository postingRepository = new PostingRepository(connectionString);
            postingRepository.deletePost(Id, UserId);
            TempData["postUploaded"] = true;
            TempData["msg"] = "post deleted successfully !";
            return RedirectToAction("index", "posting");
        }

        [HttpPost]
        public IActionResult updatePost(int Id, string publication)
        {
            PostingRepository postingRepository = new PostingRepository(connectionString);
            postingRepository.updatePost(Id, publication);
            TempData["postUploaded"] = true;
            TempData["msg"] = "post updated successfully !";
            return RedirectToAction("index", "posting");
        }

        public ViewResult editPost(int Id)
        {
            postANDimage postData = new postANDimage();
            IRepository<Posting> repo = new GenericRepository<Posting>(connectionString);
            postData.posting = repo.GetById(Id);

            PostingRepository postingRepo = new PostingRepository(connectionString);
            postData.imagePath = postingRepo.getPostImages(Id);

            string UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            postData.profilePath = postingRepo.getUserProfilePictureFromDb(UserId);
            postData.userName = postingRepo.getUserName(UserId);

            return View(postData);
        }

        public IActionResult viewProfile(int Id)
        {
            userProfileRepository userProfileRepository = new userProfileRepository(connectionString);
            userProfile user = userProfileRepository.UserDeatils(Id);
            user.profilePath = new PostingRepository(connectionString).setPath(user.profilePath);
            return View(user);
        }
        public IActionResult editProfile(int Id)
        {
            userProfileRepository userProfileRepository = new userProfileRepository(connectionString);
            userProfile userProfiles = userProfileRepository.UserDeatils(Id);
            userProfiles.profilePath = new PostingRepository(connectionString).setPath(userProfiles.profilePath);
            TempData["ProfileUpdated"] = false;
            return View(userProfiles);
        }

        [HttpPost]
        public IActionResult editProfile(int Id, string UserName, string Fname, string Lname, string OrgName, string country, string phone, string DOB, string lives, string depart, string about, string profilePath)
        {
            userProfile UserProfile = new userProfile();
            UserProfile.Id = Id;
            UserProfile.Fname = Fname;
            UserProfile.Lname = Lname;
            UserProfile.UserName = Fname + Lname;
            UserProfile.OrganizationName = OrgName;
            UserProfile.country = country;
            UserProfile.PhoneNumber = phone;
            UserProfile.DOB = DOB;
            UserProfile.lives = lives;
            UserProfile.department = depart;
            UserProfile.about = about;
            UserProfile.userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            UserProfile.profilePath = profilePath;

            userProfileRepository uPrepository = new userProfileRepository(connectionString);
            uPrepository.add(UserProfile);
            TempData["ProfileUpdated"] = true;
            TempData["msg"] = "Profile Updated Successfully !";
            return RedirectToAction("index", "posting");
        }


        public IActionResult messages()
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            PostingRepository postingRepository = new PostingRepository(connectionString);
            ViewBag.senderName = postingRepository.getUserName(userId);
            ViewBag.senderId = userId;
            ViewBag.profilePath = postingRepository.getUserProfilePictureFromDb(userId);

            userProfileRepository user = new userProfileRepository(connectionString);
            List<userProfile> users = user.GetAll();

            return View(users);
        }
        [HttpGet]
        [Route("messages")]
        public async Task<IActionResult> GetMessages([FromQuery] string userId, [FromQuery] string receiverId)
        {
            try
            {
                PostingRepository repo = new PostingRepository(connectionString);
                List<messages> msgs = repo.GetMsg(userId, receiverId);
                // Fetch messages from the database based on userId
                
                return Ok(msgs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
