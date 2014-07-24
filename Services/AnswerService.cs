using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Belitsoft.Orchard.Survey.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using Orchard.Data;

namespace Belitsoft.Orchard.Survey.Services
{
    public class AnswerService : IAnswerService
    {
        private readonly IRepository<AnswerRecord> _answerRepository;
        private readonly IContentManager _contentManager;

        public AnswerService(IRepository<AnswerRecord> answerRepository, IContentManager contentManager)
        {
            _answerRepository = answerRepository;
            _contentManager = contentManager;
        }

        public void UpdateAnswer(AnswerRecord answer)
        {
            
            if (answer.Id == 0)
                _answerRepository.Create(answer);
            else
                _answerRepository.Update(answer);
            _answerRepository.Flush();
        }

        public void UpdateAnswers(int surveyId, IEnumerable<AnswerRecord> answers)
        {
            var savedAnswer = _answerRepository.Table.Where(p => p.SurveyId == surveyId);
            foreach (AnswerRecord record in answers)
            {
                record.SurveyId = surveyId;
                UpdateAnswer(record);
            }

            savedAnswer = _answerRepository.Table.Where(p => p.SurveyId == surveyId);

            foreach (AnswerRecord record in savedAnswer)
            {
                if (answers.FirstOrDefault(p => p.Id == record.Id) == null)
                {
                    _answerRepository.Delete(record);
                    _answerRepository.Flush();
                }
            }
        }

        public IEnumerable<AnswerRecord> GetAnswers(int surverId)
        {
            return _answerRepository.Table.Where(p => p.SurveyId == surverId);
        }

        public int GetAnswerStartValue(int answerId)
        {
            var answer = _answerRepository.Table.FirstOrDefault(p => p.Id == answerId);

            if (answer != null)
                return answer.StartValue;
            return 0;
        }

        public Dictionary<int, int> GetParts(Dictionary<int, int> results)
        {
            Dictionary<int, int> newResults = new Dictionary<int, int>();

            int allAnswers = results.Sum(r => r.Value);
            int sumParts = 0;

            foreach (var result in results)
            {
                double part = allAnswers != 0 ? (double)result.Value / (double)allAnswers : 0;
                int percents = int.Parse(Math.Round(part * 100, 0).ToString());
                newResults.Add(result.Key, percents);
                sumParts += percents;
            }

            if (sumParts != 100 && sumParts != 0)
            {
                var correlation = 100-sumParts;
                var maxPart = newResults.FirstOrDefault(d=>d.Value.Equals(newResults.Max(t=>t.Value)));
                newResults[maxPart.Key] = maxPart.Value + correlation;
            }

            return newResults;
        }
    }
}