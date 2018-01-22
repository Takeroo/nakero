using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SETZ.Models
{
    public class CartModel
    {
        public List<CartItemModel> items { get; set; }
        public int price { get; set; }
        public int numberOfItems { get; set; }
        public int deliveryPrice { get; set; }
        public int totalPrice { get; set; }
    }

    public class CartItemModel
    {
        public int ID { get; set; }
        public int price { get; set; }
        public string size { get; set; }
        public string colour { get; set; }
        public string name { get; set; }
        public string image { get; set; }
        public int productID { get; set; }
        public int quantity { get; set; }
    }
}