using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WEB_353505_Horoshko.Models;
using WEB_353505_Horoshko.Models.ViewsModel;

namespace WEB_353505_Horoshko.Controllers
{
    public class Home : Controller
    {
        public IActionResult Index()
        {

            List<ListDemo> data = new List<ListDemo>
            {
                new ListDemo {Id = 1, Name="Item 1"},
                new ListDemo {Id = 2, Name="Item 2"},
                new ListDemo {Id = 3, Name="Item 3"}
            };

            ListDemoViewModel model = new ListDemoViewModel();

            model.Items = new SelectList(data, "Id", "Name");

            ViewData["Head"] = "Лабораторная работа №2";
            return View(model);
        }
    }
}
