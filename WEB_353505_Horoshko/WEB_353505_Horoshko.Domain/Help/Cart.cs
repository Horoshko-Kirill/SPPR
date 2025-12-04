using System;
using System.Collections.Generic;
using System.Text;
using WEB_353505_Horoshko.Domain.Entities;

namespace WEB_353505_Horoshko.Domain.Help
{
    public class Cart : ICart
    {
        public virtual Dictionary<int, CartItem> CartItems { get; set; } = new();

        public virtual void AddToCart(Book item)
        {
            if (CartItems.TryGetValue(item.Id, out var cartItem))
                cartItem.Quantity++;
            else
                CartItems[item.Id] = new CartItem { Item = item, Quantity = 1 };
        }

        public virtual void RemoveItems(int id)
        {
            CartItems.Remove(id);
        }

        public virtual void ClearAll()
        {
            CartItems.Clear();
        }

        public int Count => CartItems.Sum(i => i.Value.Quantity);

        public double TotalPrice => (double)CartItems.Sum(i => i.Value.TotalPrice);
    }
}
