using System.Collections.Generic;
using Belitsoft.Orchard.Survey.Models;
using Orchard;

namespace Belitsoft.Orchard.Survey.Services
{
    public interface  ISurveyService : IDependency
    {
        SurveyPart GetSurvey(int surveyId);

        IEnumerable<SurveyPart> GetSurveys();

        IEnumerable<SurveyPart> GetLastSurveys(int count, int? page = null);

        double GetCountOfPage(int count);

        double GetCountOfPageFiltered(int count, int allcount);
    }
}
