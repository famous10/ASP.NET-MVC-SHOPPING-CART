using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebAppECartDemoShopping.Models;
using WebAppECartDemoShopping.ViewModel;

namespace WebAppECartDemoShopping.Controllers
{
    public class ItemController : Controller
    {
        private  ECartBdEntities objCartBdEntities;
        public ItemController()
        {
            objCartBdEntities = new ECartBdEntities();
        }
        // GET: Item
        public ActionResult Index()
        {
            ItemViewModel objItemViewModel = new ItemViewModel();
            objItemViewModel.CategorySelectListItem = (from objCat in objCartBdEntities.Categories
                                                       select new SelectListItem()
                                                       {
                                                         Text = objCat.CategoryName,
                                                         Value = objCat.CategoryId.ToString(),
                                                         Selected = true
                                                       });
            return View(objItemViewModel);
        }
        [HttpPost]
        public JsonResult Index(ItemViewModel objItemViewModel)
        {
            string NewImage = Guid.NewGuid() + Path.GetExtension(objItemViewModel.ImagePath.FileName);
            objItemViewModel.ImagePath.SaveAs(Server.MapPath("~/Images/"+ NewImage));

            Item objItem = new Item();
            objItem.ImagePath = "~/Images/" + NewImage;
            objItem.CategoryId = objItemViewModel.CategoryId;
            objItem.Description = objItemViewModel.Description;
            objItem.ItemCode = objItemViewModel.ItemCode;
            objItem.ItemId = Guid.NewGuid();
            objItem.ItemName = objItemViewModel.ItemName;
            objItem.ItemPrice = objItemViewModel.ItemPrice;
            objCartBdEntities.Items.Add(objItem);
            objCartBdEntities.SaveChanges();

            return Json(new {Success = true, Message = "Item is added Successfully" }, JsonRequestBehavior.AllowGet);
        }
    }
}