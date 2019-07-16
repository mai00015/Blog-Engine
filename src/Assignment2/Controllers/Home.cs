using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Assignment2.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;
using System.Collections;
using System.Text;


// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Assignment2.Controllers
{
    public class Home : Controller
    {
        private Assignment2DataContext _blogContext;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public Home(Assignment2DataContext context)
        {
            _blogContext = context;
        }
        public IActionResult Index()
        {
            var photo = _blogContext.Photos.ToList();
            ViewData["photo"] = photo;
            return View(_blogContext.BlogPosts.ToList());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IActionResult Login()
        {
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }
        public IActionResult AddBlogPost()
        {
            return View();
        }
        public IActionResult ViewBadWords()
        {
            return View(_blogContext.BadWords.ToList());
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
        public IActionResult EditBlogPost(int id)
        {
            var blogToUpdate = _blogContext.BlogPosts.Where(b => b.BlogPostId == id).FirstOrDefault();
            var photoList = _blogContext.Photos.Where(p => p.BlogPostId == id).ToList();

            ViewData["photos"] = photoList;

            return View(blogToUpdate);
        }
        public IActionResult CreateAccount(User user)
        {
            _blogContext.Users.Add(user);
            _blogContext.SaveChanges();

            return RedirectToAction("Login");
        }
        public IActionResult EditProfile(int id)
        {
            var userToUpdate = _blogContext.Users.Where(b => b.UserId == id).FirstOrDefault();
            return View(userToUpdate);
        }
        public IActionResult UpdateAccount(User updateUser)
        {
            var userId = Convert.ToInt32(Request.Form["id"]);
            var userToUpdate = _blogContext.Users.Where(b => b.UserId == userId).FirstOrDefault();

            userToUpdate.FirstName = updateUser.FirstName;
            userToUpdate.LastName = updateUser.LastName;
            userToUpdate.Password = updateUser.Password;
            userToUpdate.RoleId = updateUser.RoleId;
            userToUpdate.City = updateUser.City;
            userToUpdate.Address = updateUser.Address;
            userToUpdate.DateOfBirth = updateUser.DateOfBirth;
            userToUpdate.EmailAddress = updateUser.EmailAddress;
            userToUpdate.PostalCode = updateUser.PostalCode;
            userToUpdate.Country = updateUser.Country;

            _blogContext.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult CheckAccount(User user)
        {
            var user_detail = _blogContext.Users
                .Where(u => u.EmailAddress.Equals(user.EmailAddress) && u.Password.Equals(user.Password))
                .ToList();

            if (user_detail.Count() == 0 || user_detail == null)
            {
                TempData["error"] = "Sorry email address or password is invalid";
                //HttpContext.Session.SetString("error", "Sorry email address or password is invalid");
                return RedirectToAction("Login");
            }
            else
            {
                // var d = user_detail.Single().UserId;
                HttpContext.Session.SetInt32("UserId", user_detail.ElementAtOrDefault(0).UserId);
                HttpContext.Session.SetString("FirstName", user_detail.ElementAtOrDefault(0).FirstName);
                HttpContext.Session.SetString("LastName", user_detail.ElementAtOrDefault(0).LastName);
                HttpContext.Session.SetString("Email", user_detail.ElementAtOrDefault(0).EmailAddress);
                HttpContext.Session.SetInt32("RoleId", user_detail.ElementAtOrDefault(0).RoleId);
                return RedirectToAction("Index");
            }

        }
        public IActionResult DisplayFullBlogPost(int id)
        {
            var getDetail = _blogContext.BlogPosts.Where(b => b.BlogPostId == id).FirstOrDefault();

            var userMain = _blogContext.Users.Where(u => u.UserId == getDetail.UserId).FirstOrDefault();

            var photoDetail = _blogContext.Photos.Where(p => p.BlogPostId == id).ToList();

            var listComment = _blogContext.Comments
                .Join(_blogContext.Users,
                c => c.UserId,
                u => u.UserId, (c, u) => new { comment = c, user = u })
                .Where(b => b.comment.BlogPostId == id).ToList();
            //HttpContext.Session.SetString("detail", getDetail);

            ViewData["getdetail"] = getDetail;
            ViewData["getuser"] = userMain;
            ViewData["getphoto"] = photoDetail;

            var readListComment = new List<CommentDetail>();

            foreach (var item in listComment)
            {
                var commentDetail = new CommentDetail();
                commentDetail.comment = item.comment;
                commentDetail.firstname = item.user.FirstName;
                commentDetail.lastname = item.user.LastName;
                readListComment.Add(commentDetail);
            }

            ViewData["getcomment"] = readListComment;


            //   var ok1 = JsonConvert.SerializeObject(getDetail);
            // var yeah = JsonConvert.DeserializeObject<BlogPost>(ok1);



            /* Work but too long
            HttpContext.Session.SetString("getdetail", JsonConvert.SerializeObject(getDetail));
            HttpContext.Session.SetString("getuser", JsonConvert.SerializeObject(userMain));
            HttpContext.Session.SetString("getcomment", JsonConvert.SerializeObject(listComment));
            */
            //user = userMain.ToString();

            return View();
        }
        public IActionResult PostComment(Comment newComment)
        {
            var blogId = Convert.ToInt32(Request.Form["BlogPostId"]);
            var userid = Convert.ToInt32(HttpContext.Session.GetInt32("UserId"));

            newComment.BlogPostId = blogId;
            newComment.UserId = userid;

            string[] words = newComment.Content.Split(' ');

            var checkBadWord = _blogContext.BadWords.Select(w => w).ToList();

            for (int i = 0; i < words.Count(); i++)
            {
                foreach (var bad in checkBadWord)
                {
                    if (bad.Word.Equals(words[i]))
                    {
                        words[i] = "*****";
                    }
                }
            }
            newComment.Content = string.Join(" ", words);


            _blogContext.Comments.Add(newComment);
            _blogContext.SaveChanges();

            return RedirectToAction("DisplayFullBlogPost", new { id = blogId });
        }
        public IActionResult CreateNewBlog(BlogPost newBlog)
        {
            var userid = Convert.ToInt32(HttpContext.Session.GetInt32("UserId"));
            newBlog.UserId = userid;
            _blogContext.BlogPosts.Add(newBlog);
            _blogContext.SaveChanges();

            return RedirectToAction("Index");
        }
        public IActionResult UpdateBlog(BlogPost update)
        {

            var blogId = Convert.ToInt32(Request.Form["id"]);
            var blogToUpdate = _blogContext.BlogPosts.Where(b => b.BlogPostId == blogId).FirstOrDefault();

            blogToUpdate.Title = update.Title;
            blogToUpdate.Content = update.Content;
            blogToUpdate.Posted = update.Posted;
            blogToUpdate.IsAvailable = update.IsAvailable;
            blogToUpdate.ShortDescription = update.ShortDescription;
            _blogContext.SaveChanges();

            return RedirectToAction("Index");
        }
        public async Task<IActionResult> UploadFileNow(ICollection<IFormFile> files)
        {
            var blogId = Convert.ToInt32(Request.Form["id"]);

            // get your storage accounts connection string
            var storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=cst8359;AccountKey=ecMPpNU6vimZKMDTJG4seALrY7Kq7UJYjgl0/yLanXn857C8xtUJ2sF4ciB6wy9gg+e/YeYbRTaly2DVOxWhXQ==");

            // create an instance of the blob client
            var blobClient = storageAccount.CreateCloudBlobClient();

            // create a container to hold your blob (binary large object.. or something like that)
            // naming conventions for the curious https://msdn.microsoft.com/en-us/library/dd135715.aspx
            var container = blobClient.GetContainerReference("khasmaisphotostorage");
            await container.CreateIfNotExistsAsync();

            // set the permissions of the container to 'blob' to make them public
            var permissions = new BlobContainerPermissions();
            permissions.PublicAccess = BlobContainerPublicAccessType.Blob;
            await container.SetPermissionsAsync(permissions);

            // for each file that may have been sent to the server from the client
            foreach (var file in files)
            {
                try
                {
                    // create the blob to hold the data
                    var blockBlob = container.GetBlockBlobReference(file.FileName);
                    if (await blockBlob.ExistsAsync())
                        await blockBlob.DeleteAsync();

                    using (var memoryStream = new MemoryStream())
                    {
                        // copy the file data into memory
                        await file.CopyToAsync(memoryStream);

                        // navigate back to the beginning of the memory stream
                        memoryStream.Position = 0;

                        // send the file to the cloud
                        await blockBlob.UploadFromStreamAsync(memoryStream);
                    }

                    // add the photo to the database if it uploaded successfully

                    var photo = new Photo();

                    photo.BlogPostId = blogId;
                    photo.Url = blockBlob.Uri.AbsoluteUri;
                    photo.Filename = file.FileName;

                    _blogContext.Photos.Add(photo);
                    _blogContext.SaveChanges();

                }
                catch
                {

                }
            }

            return RedirectToAction("EditBlogPost", new { id = blogId });
        }
        public IActionResult DeleteFile(int id, int blogid)
        {
            var photoToDelete = _blogContext.Photos.Where(p => p.PhotoId == id).FirstOrDefault();
            _blogContext.Photos.Remove(photoToDelete);
            _blogContext.SaveChanges();

            return RedirectToAction("EditBlogPost", new { id = blogid });
        }
        public IActionResult DeleteWords(int id)
        {
    
            var wordToDelete = _blogContext.BadWords.Where(b => b.BadWordId == id).FirstOrDefault();

            _blogContext.BadWords.Remove(wordToDelete);
            _blogContext.SaveChanges();

            return RedirectToAction("ViewBadWords");
        }
        public IActionResult DeleteBlogPost(int id)
        {
            var blogToDelete = _blogContext.BlogPosts.Where(b => b.BlogPostId == id).FirstOrDefault();

            var commentToDelete = _blogContext.Comments.Where(c => c.BlogPostId == id).ToList();


            if (commentToDelete.Count() != 0)
            {
                foreach(var comment in commentToDelete)
                {
                    _blogContext.Comments.Remove(comment);
                    _blogContext.SaveChanges();

                }
            }

            _blogContext.BlogPosts.Remove(blogToDelete);
            _blogContext.SaveChanges();

            return RedirectToAction("Index");
        }
        public IActionResult AddBadWord()
        {
            var word = Request.Form["bad"];

            var badWord = new BadWord();
            badWord.Word = word;

            _blogContext.BadWords.Add(badWord);
            _blogContext.SaveChanges();

            return RedirectToAction("ViewBadWords");
        }
    }
}
