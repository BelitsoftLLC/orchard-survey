using System;
using System.Collections.Generic;
using System.Linq;
using Belitsoft.Orchard.Survey.Models;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;

namespace Belitsoft.Orchard.Survey.Services
{
    public class SurveyService: ISurveyService
    {
        private readonly IContentManager _contentManager;

        public SurveyService(IContentManager contentManager)
        {
            _contentManager = contentManager;
        }


        public SurveyPart GetSurvey(int surveyId)
        {
            IContentQuery<SurveyPart> query = _contentManager.Query<SurveyPart>(VersionOptions.Published, "Survey")
                                                                .Where<CommonPartRecord>(p => p.PublishedUtc != null && p.Id == surveyId)
                                                                .OrderByDescending(p => p.PublishedUtc);
            return query.List().FirstOrDefault();
        }


        public IEnumerable<SurveyPart> GetSurveys()
        {
            IContentQuery<SurveyPart> query = _contentManager.Query<SurveyPart>(VersionOptions.Published, "Survey")
                                                                .Where<CommonPartRecord>(p => p.PublishedUtc != null)
                                                                .OrderByDescending(p => p.PublishedUtc);
            List<SurveyPart> surveys = query.List().ToList();

            return surveys;
        }

        public IEnumerable<SurveyPart> GetLastSurveys(int count, int? page = null)
        {
            IContentQuery<SurveyPart> query = _contentManager.Query<SurveyPart>(VersionOptions.Published, "Survey")
                                                                .Where<CommonPartRecord>(p => p.PublishedUtc != null)
                                                                .OrderByDescending(p => p.PublishedUtc);
            List<SurveyPart> surveyParts = query.List().ToList();

            if (page != null)
            {
                var temp = surveyParts.Skip((page.Value - 1) * count)
                    .Take(count);
                return temp.ToList();
            }

            return surveyParts.Take(count);
        }

        public double GetCountOfPage(int count)
        {
            IContentQuery<SurveyPart> query = _contentManager.Query<SurveyPart>(VersionOptions.Published, "Survey")
                                                               .Where<CommonPartRecord>(p => p.PublishedUtc != null)
                                                               .OrderByDescending(p => p.PublishedUtc);
            List<SurveyPart> surveys = query.List().ToList();


            return Math.Ceiling(surveys.Count() / (float)count / 1.0);
        }

        public double GetCountOfPageFiltered(int count, int allcount)
        {
            return Math.Ceiling(allcount / (float)count / 1.0);
        }
    }
}