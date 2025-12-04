using System;
using System.Collections.Generic;
using System.Text;

namespace WEB_353505_Horoshko.Domain.Entities
{
    public class CartItem
    {
        public Book Item { get; set; }
        public int Quantity { get; set; } = 1;
        public decimal TotalPrice => Item.Price * Quantity;
    }

}
