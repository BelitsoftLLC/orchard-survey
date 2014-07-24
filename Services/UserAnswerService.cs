using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Belitsoft.Orchard.Survey.Models;
using Orchard.ContentManagement;
using Orchard.Data;

namespace Belitsoft.Orchard.Survey.Services
{
    public class UserAnswerService : IUserAnswerService
    {
        private readonly IRepository<UserAnswerRecord> _userAnswerRepository;
        private readonly IAnswerService _answerService;

        public UserAnswerService(IRepository<UserAnswerRecord> userAnswerRepositor, IAnswerService answerService)
        {
            _userAnswerRepository = userAnswerRepositor;
            _answerService = answerService;
        }

        public UserAnswerRecord GetUserAnswer(int surveyId)
        {
            return _userAnswerRepository.Table.FirstOrDefault(p => p.SurveyId == surveyId);
        }

        public IEnumerable<UserAnswerRecord> GetUserAnswers(int surveyId)
        {
            return _userAnswerRepository.Table.Where(p => p.SurveyId == surveyId);
        }

        public void UpdateUserAnswer(UserAnswerRecord answerRecord)
        {
            if (answerRecord.Id == 0)
                _userAnswerRepository.Create(answerRecord);
            else
                _userAnswerRepository.Update(answerRecord);
            _userAnswerRepository.Flush();
        }

        public bool IsAnsweredSurvey(int userId, int surveyId)
        {
            return _userAnswerRepository.Table.FirstOrDefault(p => p.SurveyId == surveyId && p.UserId == userId) != null;
        }

        public int GetCountForOneAnswer(int answerId)
        {
            var numberOfStartValue = _answerService.GetAnswerStartValue(answerId);

            return _userAnswerRepository.Count(p => p.AnswerId == answerId) + numberOfStartValue;
        }

        public int GetCountForAllAnswer(int surveyId)
        {
            var answers = _answerService.GetAnswers(surveyId);

            return _userAnswerRepository.Count(p => p.SurveyId == surveyId) + answers.Sum(answerRecord => answerRecord.StartValue);
        }
    }
}