using System.Web.Mvc;
using Orchard;
using Orchard.Mvc;

namespace Belitsoft.Orchard.Survey.Controllers
{
    public class ShapesController: Controller
    {
        private readonly IOrchardServices _services;

        public ShapesController(IOrchardServices services)
        {
            _services = services;
        }

        public ActionResult SurveyShape(int id)
        {
            var shape = _services.New.Parts_Survey();
            return new ShapeResult(this, shape);
        }
    }
}