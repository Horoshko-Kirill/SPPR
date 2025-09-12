using Microsoft.AspNetCore.Mvc;

namespace WEB_353505_Horoshko.Components
{
    public class CartViewComponent : ViewComponent
    {

        public IViewComponentResult Invoke()
        {
            return View();
        }

    }
}
