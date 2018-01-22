using SETZ.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SETZ.Controllers
{
    public class CartController : Controller
    {
        // GET: Cart
        DB_A28A4D_setzEntities setzDB = new DB_A28A4D_setzEntities();

        [Authorize]
        public ActionResult Index()
        {
            CartModel model = new CartModel();
            var identity = (System.Web.HttpContext.Current.User as SETZ.MyPrincipal).Identity as SETZ.MyIdentity;
            int userID = identity.User.ID;
            Cart cart;
            model.items = new List<CartItemModel>();
            
            if (setzDB.Carts.Where(c => c.UserID == userID).Count() > 0)
            {
                cart = setzDB.Carts.Where(c => c.UserID == userID).FirstOrDefault();

                int total = 0;
                int itemCount = 0;
                foreach (var item in setzDB.CartItems.Where(c => c.CartID == cart.ID))
                {
                    Product product = setzDB.Products.Find(item.ProductID);
                    CartItemModel cartitem = new CartItemModel();
                    cartitem.ID = item.ID;
                    cartitem.productID = item.ProductID ?? 0;
                    cartitem.size = item.Size;
                    cartitem.colour = item.Colour;
                    cartitem.name = item.Name;
                    cartitem.quantity = item.Quantity ?? 1;
                    cartitem.image = "default-img.jpg";
                    var relativePath = "~/Images/Product/" + product.Image;
                    var absolutePath = HttpContext.Server.MapPath(relativePath);
                    if (System.IO.File.Exists(absolutePath))
                        cartitem.image = product.Image;
                    cartitem.price = item.Price ?? 0;
                    model.items.Add(cartitem);
                    total += item.Price ?? 0;
                    itemCount++;
                }
                model.totalPrice = total;
                model.numberOfItems = itemCount;
            }
            
            return View(model);
        }

        public String CartPrice()
        {
            if((System.Web.HttpContext.Current.User as SETZ.MyPrincipal)!= null)
            {
                var identity = (System.Web.HttpContext.Current.User as SETZ.MyPrincipal).Identity as SETZ.MyIdentity;
                Cart cart;

                int totalPrice = 0;
                int numberOfItems = 0;

                if (identity != null && setzDB.Carts.Where(c => c.UserID == identity.User.ID).Count() > 0)
                {
                    cart = setzDB.Carts.Where(c => c.UserID == identity.User.ID).FirstOrDefault();

                    int total = 0;
                    int itemCount = 0;
                    foreach (var item in setzDB.CartItems.Where(c => c.CartID == cart.ID))
                    {
                        total += item.Price ?? 0;
                        itemCount++;
                    }
                    totalPrice = total;
                    numberOfItems = itemCount;
                }

                string cartPrice = totalPrice + "(" + numberOfItems + " товаров)";
                return cartPrice;
            }
            return "";
        }

        public ActionResult Delete(int? id)
        {
            CartItem item = setzDB.CartItems.Find(id);
            setzDB.CartItems.Remove(item);
            setzDB.SaveChanges();
            return RedirectToAction("Index", "Cart");
        }

        [HttpGet]
        public ActionResult Order()
        {
            OrderAddressModel model = new OrderAddressModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Order(OrderAddressModel model)
        {
            if (ModelState.IsValid)
            {
                var identity = (System.Web.HttpContext.Current.User as SETZ.MyPrincipal).Identity as SETZ.MyIdentity;
                int userID = identity.User.ID;
                Order order = new Order();
                Cart cart = setzDB.Carts.Where(c => c.UserID == userID).FirstOrDefault();
                
                order.NumberOfItems = cart.NumberOfProducts;
                order.Price = cart.TotalPrice;
                order.UserID = cart.UserID;
                order.Date = DateTime.Now;
                order.Status = "Ожидание подтверждения";

                order.Address = model.Address;
                order.Phone = model.Phone;
                order.Email = model.Email;
                order.Details = model.Details;

                setzDB.Orders.Add(order);
                setzDB.SaveChanges();

                Order saved = setzDB.Orders.Where(o => o.UserID == userID).ToList().LastOrDefault();
                List<CartItem> item = setzDB.CartItems.Where(c => c.CartID == cart.ID).ToList();
                for (int i = 0; i < item.Count(); i++)
                {
                    Product product = setzDB.Products.Find(item[i].ProductID);
                    product.OrderCount++;
                    setzDB.SaveChanges();
                    OrderItem orderitem = new OrderItem();
                    orderitem.Articul = item[i].Articul;
                    orderitem.Colour = item[i].Colour;
                    orderitem.Name = item[i].Name;
                    orderitem.OrderID = saved.ID;
                    orderitem.Price = item[i].Price;
                    orderitem.ProductID = item[i].ProductID;
                    orderitem.Size = item[i].Size;
                    orderitem.Quantity = item[i].Quantity;
                    setzDB.OrderItems.Add(orderitem);
                    setzDB.SaveChanges();
                }

                deleteCart();
                //return RedirectToAction("Registered");
                return RedirectToAction("Thanks", "Cart", new { id = saved.ID});
            }

            return View(model);
        }

        public ActionResult Thanks(int? id)
        {
            int orderNumber = 16081994 + id ?? 0 ;
            ViewBag.OrderNumber = orderNumber;
            return View();
        }

        public ActionResult Elsom()
        {
            return View();
        }

        public void deleteCart()
        {
            var identity = (System.Web.HttpContext.Current.User as SETZ.MyPrincipal).Identity as SETZ.MyIdentity;
            int userID = identity.User.ID;
            Cart cart = setzDB.Carts.Where(c => c.UserID == userID).FirstOrDefault();
            int count = setzDB.CartItems.Where(c => c.CartID == cart.ID).Count();

            for (int i = 0; i < count; i++)
            {
                CartItem item = setzDB.CartItems.FirstOrDefault();
                setzDB.CartItems.Remove(item);
                setzDB.SaveChanges();
            }
            setzDB.Carts.Remove(cart);
            setzDB.SaveChanges();
        }
    }
}