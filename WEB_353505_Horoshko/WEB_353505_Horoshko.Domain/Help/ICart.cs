using System;
using System.Collections.Generic;
using System.Text;
using WEB_353505_Horoshko.Domain.Entities;

namespace WEB_353505_Horoshko.Domain.Help
{
    public interface ICart
    {
        Dictionary<int, CartItem> CartItems { get; }
        void AddToCart(Book book);
        void RemoveItems(int id);
        void ClearAll();
    }
}
