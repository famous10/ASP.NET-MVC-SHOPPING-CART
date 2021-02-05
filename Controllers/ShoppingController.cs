using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebAppECartDemoShopping.Models;
using WebAppECartDemoShopping.ViewModel;

namespace WebAppECartDemoShopping.Controllers
{
    public class ShoppingController : Controller
    {
        private ECartBdEntities objCartBdEntities;
        private List<ShoppingCartModel> listOfShoppingCartModels;
        public ShoppingController()
        {
            objCartBdEntities = new ECartBdEntities();
            listOfShoppingCartModels = new List<ShoppingCartModel>();
        }
        // GET: Shopping
        public ActionResult Index()
        {
            IEnumerable<ShoppingViewModel> listOfShoppingViewModels = (from objItem in objCartBdEntities.Items
                                                                       join
                                                                       objCate in objCartBdEntities.Categories
                                                                       on objItem.CategoryId equals objCate.CategoryId
                                                                       select new ShoppingViewModel()
                                                                       {
                                                                           ImagePath = objItem.ImagePath,
                                                                           ItemName = objItem.ItemName,
                                                                           Description = objItem.Description,
                                                                           ItemPrice = objItem.ItemPrice,
                                                                           ItemId = objItem.ItemId,
                                                                           Category = objCate.CategoryName,
                                                                           ItemCode = objItem.ItemCode,
                                                                       }
                                                  
                                                                       ).ToList();
            return View(listOfShoppingViewModels);
        }
        [HttpPost]
        public JsonResult Index(string ItemId)
        {
      
            ShoppingCartModel objShoppingCartModel = new ShoppingCartModel(); 
            Item objItem = objCartBdEntities.Items.Single(model => model.ItemId.ToString() == ItemId);
            if (Session ["CartCounter"]  != null)
            {
                listOfShoppingCartModels = Session["CartItem"] as List<ShoppingCartModel>;
            }
                
            if (listOfShoppingCartModels.Any(model => model.ItemId == ItemId))
            {
                objShoppingCartModel = listOfShoppingCartModels.Single(model => model.ItemId == ItemId);
                objShoppingCartModel.Quantity = objShoppingCartModel.Quantity + 1;
                objShoppingCartModel.Total = objShoppingCartModel.Quantity * objShoppingCartModel.UnitPrice;
            }
            else
            {
                objShoppingCartModel.ItemId = ItemId;
                objShoppingCartModel.ImagePath = objItem.ImagePath;
                objShoppingCartModel.ItemName = objItem.ItemName;
                objShoppingCartModel.Quantity = 1;
                objShoppingCartModel.Total = objItem.ItemPrice;
                objShoppingCartModel.UnitPrice = objItem.ItemPrice;
                listOfShoppingCartModels.Add(objShoppingCartModel);


            }
            Session["CartCounter"] = listOfShoppingCartModels.Count;
            Session["CartItem"] = listOfShoppingCartModels;
            return Json(new { success = true,counter = listOfShoppingCartModels.Count}, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ShoppingCart()
        {
            listOfShoppingCartModels = Session["CartItem"] as List<ShoppingCartModel>;
            return View(listOfShoppingCartModels);
        }

        [HttpPost]
       public ActionResult AddOrder()
        {
            int OrderId = 0;
            listOfShoppingCartModels = Session["CartItem"] as List<ShoppingCartModel>;
            Order orderObj = new Order()
            {
                OrderDate = DateTime.Now,
                OrderNumber = String.Format("{0:ddmmyyyyHHmmsss}", DateTime.Now)
            };
            objCartBdEntities.Orders.Add(orderObj);
            objCartBdEntities.SaveChanges();
            OrderId = orderObj.OrderId;

            foreach (var item in listOfShoppingCartModels)
            {
                OrderDetail objOrderDetail = new OrderDetail();
                objOrderDetail.Total = item.Total;
                objOrderDetail.ItemId = item.ItemId;
                objOrderDetail.OrderId = OrderId;
                objOrderDetail.Quantity = item.Quantity;
                objOrderDetail.UnitPrice = item.UnitPrice;
                objCartBdEntities.OrderDetails.Add(objOrderDetail);
                objCartBdEntities.SaveChanges();
            }
            Session["CartItem"] = null;
            Session["CartCounter"] = null;
            return RedirectToAction("Index");
        }
    }
}