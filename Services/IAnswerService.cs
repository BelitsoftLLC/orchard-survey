using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Belitsoft.Orchard.Survey.Models;
using Orchard;

namespace Belitsoft.Orchard.Survey.Services
{
    public interface IAnswerService : IDependency
    {
        int GetAnswerStartValue(int answerId);

        void UpdateAnswer(AnswerRecord answer);

        void UpdateAnswers(int surveyId, IEnumerable<AnswerRecord> answers);

        IEnumerable<AnswerRecord> GetAnswers(int surverId);

        Dictionary<int, int> GetParts(Dictionary<int, int> results);
    }
}
