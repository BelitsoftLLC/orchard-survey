using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Belitsoft.Orchard.Survey.Models;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;

namespace Belitsoft.Orchard.Survey.Handlers
{
    public class SurveyWidgetHandler : ContentHandler
    {
        public SurveyWidgetHandler(IRepository<SurveyWidgetPartRecord> repository)
        {
            Filters.Add(StorageFilter.For(repository));
            OnRemoved<SurveyWidget>((context, part) => repository.Delete(part.Record));
        }
    }
}