using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;

namespace Belitsoft.Orchard.Survey.Models
{
    public class SurveyWidgetPartRecord : ContentPartRecord
    {
        //storage id of survey, which should show on widget
        public virtual long SurveyId { get; set; }
    }

    public class SurveyWidget : ContentPart<SurveyWidgetPartRecord>
    {
        public long SurveyId
        {
            get { return Record.SurveyId; }
            set { Record.SurveyId = value; }
        }
        
    }
}