using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Belitsoft.Orchard.Survey.Models;
using Orchard;

namespace Belitsoft.Orchard.Survey.Services
{
    public interface IUserAnswerService : IDependency
    {
        UserAnswerRecord GetUserAnswer(int surveyId);

        void UpdateUserAnswer(UserAnswerRecord answerRecord);

         IEnumerable<UserAnswerRecord> GetUserAnswers(int surveyId);

         bool IsAnsweredSurvey(int userId, int surveyId);

         int GetCountForOneAnswer(int answerId);

         int GetCountForAllAnswer(int surveyId);
    }
}
