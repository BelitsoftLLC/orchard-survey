using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Belitsoft.Orchard.Survey.Models;

namespace Belitsoft.Orchard.Survey.ViewModels
{
    public class EditSurveyViewModel
    {
        public string Title { get; set; }

        public int Id { get; set; }

        public string Body
        {
            get; 
            set;
        }

        public bool IsNotShowResultAfterAnswer
        {
            get;
            set;
        }

        public bool UserIsAnswered { get; set; }

        public IList<AnswerRecord> Answers { get; set; }

        public Dictionary<int, int> Result { get; set; }

        public Dictionary<int, int> Parts { get; set; }

        public int NumberOfAnswer { get; set; }

        public string CreatedDate { get; set; }

        public bool IsPublish { get; set; }

        public bool IsExpired { get; set; }
    }
}