using System.Linq;
using System.Web.Mvc;

namespace FeatureSelection.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var path = Server.MapPath("~/App_Data/train.csv");

            var orygin = System.IO.File.ReadAllLines(path);

            var oryginCsv = orygin.Select(l => l.Split(';')).ToList();

            return View();
        }
    }
}