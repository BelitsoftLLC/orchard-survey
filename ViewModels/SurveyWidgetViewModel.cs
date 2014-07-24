using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Belitsoft.Orchard.Survey.ViewModels
{
    public class SurveyWidgetViewModel
    {
        public IEnumerable<SelectListItem> Surveys { get; set; }

        public long SurveyId { get; set; }
    }
}