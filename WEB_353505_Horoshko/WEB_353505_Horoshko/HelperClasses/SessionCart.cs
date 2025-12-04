using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using WEB_353505_Horoshko.Domain.Entities;
using WEB_353505_Horoshko.Domain.Help;

namespace WEB_353505_Horoshko.API.Models
{
    public class SessionCart : Cart
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SessionCart(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;


            var session = _httpContextAccessor.HttpContext.Session;
            var sessionData = session.GetString("cart");
            if (!string.IsNullOrEmpty(sessionData))
            {
                var data = JsonConvert.DeserializeObject<Cart>(sessionData);
                if (data != null)
                {
                    this.CartItems = data.CartItems;
                }
            }
        }

        private void Save()
        {
            var session = _httpContextAccessor.HttpContext.Session;
            session.SetString("cart", JsonConvert.SerializeObject(this));
        }

        public override void AddToCart(Book book)
        {
            base.AddToCart(book);
            Save();
        }

        public override void RemoveItems(int id)
        {
            base.RemoveItems(id);
            Save();
        }

        public override void ClearAll()
        {
            base.ClearAll();
            Save();
        }
    }
}
