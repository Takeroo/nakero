using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SETZ.Models
{
    public class OrderModel
    {
        public Order order { get; set; }
        public User user { get; set; }
        public string status { get; set; }
        public List<OrderItemModel> orderItems { get; set; }
    }

    public class OrderItemModel
    {
        public OrderItem item { get; set; }
        public string image { get; set; }
    }
    public class OrderAddressModel
    {
        [Required(ErrorMessage = "Требуется номер телефона", AllowEmptyStrings = false)]
        public string Phone { get; set; }
        [Required(ErrorMessage = "Требуется адрес доставки", AllowEmptyStrings = false)]
        public string Address { get; set; }

        public string Email { get; set; }

        public string Details { get; set; }
    }
}