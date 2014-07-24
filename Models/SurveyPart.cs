using System.Collections.Generic;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;

namespace Belitsoft.Orchard.Survey.Models
{
    public class SurveyPartRecord: ContentPartRecord
    {
        public virtual string Title { get; set; }

        public virtual bool IsNotShowResultAfterAnswer { get; set; }
    }

    public class SurveyPart : ContentPart<SurveyPartRecord>
    {
        public string Title
        {
            get { return Record.Title; }
            set { Record.Title = value; }
        }

        public bool IsNotShowResultAfterAnswer
        {
            get { return Record.IsNotShowResultAfterAnswer; }
            set { Record.IsNotShowResultAfterAnswer = value; }
        }

        public IList<AnswerRecord> Answers { get; set; }
    }
}