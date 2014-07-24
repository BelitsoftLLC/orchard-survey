using System;
using Orchard.Fields.Fields;
using Belitsoft.Orchard.Survey.Models;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;

namespace Belitsoft.Orchard.Survey.Handlers
{
    public class SurveyHandler : ContentHandler
    {
        public SurveyHandler(IRepository<SurveyPartRecord> repository)
        {
            Filters.Add(StorageFilter.For(repository));
            OnRemoved<SurveyPart>((context, part) => repository.Delete(part.Record));

            //add behaivor on survey item indexing
            OnIndexing<SurveyPart>((context, contactPart) =>
            {
                context.DocumentIndex.Add("survey_title", contactPart.Title).Store();
                if (((DateTimeField)contactPart.Get(typeof(DateTimeField), "DueDate")).DateTime > DateTime.Now)
                    context.DocumentIndex.Add("survey_title", contactPart.Title).Analyze().Store();
            });

        }
    }
}