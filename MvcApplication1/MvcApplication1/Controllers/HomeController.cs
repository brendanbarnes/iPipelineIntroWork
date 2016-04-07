using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebMatrix.Data;
using System.Drawing;
using System.Web.Mvc;

namespace MvcApplication1.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "This is a photo album that has multiple users.";

            return View();
        }

        public ActionResult Upload()
        {
            ViewBag.Message = "Upload your photos here.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Contact Info.";

            return View();
        }

        public ActionResult Pics()
        {
            ViewBag.Message = "Your pics page.";

            return View();
        }


        public ActionResult LoadNextImages(int o)
        {
            var db = Database.Open("DefaultConnection");
            var forPagingWork = "SELECT * FROM PicsDBs ORDER BY Id desc OFFSET (" + o + ") ROWS FETCH NEXT 10 ROWS ONLY";

            var newImages = db.Query(forPagingWork).ToArray();
            String[,] pictures = new String[newImages.Length, 2];
            for (int i = 0; i < newImages.Length; i++)
            {
                pictures[i, 0] = Convert.ToBase64String(newImages[i].pic);
                pictures[i, 1] = newImages[i].Id + "";
            }


            var stuff = new JsonResult { Data = pictures, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            var curious = stuff.MaxJsonLength;
            stuff.MaxJsonLength = int.MaxValue;

            return stuff;
        }

        public ActionResult LoadNextImagesForUser(int o)
        {
            var db = Database.Open("DefaultConnection");
            var userId = "SELECT * FROM UserProfile WHERE UserName = '" + User.Identity.Name + "'";
            var qt = db.Query(userId);
            int userIdNum = qt.ToArray()[0].UserId;
            var forPagingWork = "SELECT * FROM PicsDBs WHERE user_id = "+ userIdNum + " ORDER BY Id desc OFFSET (" + o + ") ROWS FETCH NEXT 10 ROWS ONLY";

            var newImages = db.Query(forPagingWork).ToArray();

            String[,] pictures;
            if (newImages.Length != 0)
            {
                pictures = new String[newImages.Length,2];
                for (int i = 0; i < newImages.Length; i++)
                {
                    pictures[i, 0] = Convert.ToBase64String(newImages[i].pic);
                    pictures[i, 1] = newImages[i].Id + "";
                }
            }
            else
            {
                pictures = null;
            }
            
            var stuff = new JsonResult { Data = pictures, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            var curious = stuff.MaxJsonLength;
            stuff.MaxJsonLength = int.MaxValue;

            return stuff;
        }

        public ActionResult DeleteImageWithId(int id)
        {
            var db = Database.Open("DefaultConnection");
            var deleteQuery = "DELETE FROM PicsDBs WHERE id = " + id;
            db.Execute(deleteQuery);

            return null;
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase file)
        {
            if (file != null)
            {
                Image bytes = Image.FromStream(file.InputStream);

                var db = Database.Open("DefaultConnection");
                
                var maxIdValQueryString = "SELECT * FROM PicsDBs ORDER BY Id desc";
                var picId = 0;
                if (User.Identity.Name != "") {
                    var userId = "SELECT * FROM UserProfile WHERE UserName = '" + User.Identity.Name + "'";
                    var qt = db.Query(userId);
                    picId = qt.ToArray()[0].UserId;
                }
                var t = db.Query(maxIdValQueryString);
                var q = t.ToArray()[0].Id;

                var updateString = "INSERT INTO PicsDBs (Id, pic, user_id) VALUES (@0, @1, @2)";
                ImageConverter converter = new ImageConverter();
                byte[] imgArray = (byte[])converter.ConvertTo(bytes, typeof(byte[]));
                db.Execute(updateString, q + 1, imgArray, picId);


                ViewBag.Message = "File uploaded successfully";
                
            
            }
            return View();
        }
    }
}
