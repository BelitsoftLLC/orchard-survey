using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Belitsoft.Orchard.Survey.Models;
using Belitsoft.Orchard.Survey.Services;
using Orchard;
using Orchard.Data;
using Orchard.Fields.Fields;
using Orchard.Mvc.Extensions;

namespace Belitsoft.Orchard.Survey.Controllers
{
    public class SurveyController : Controller
    {

        private readonly IOrchardServices _services;
        private readonly ISurveyService _surveyService;
        private readonly IUserAnswerService _userAnswerService;


        public SurveyController(IOrchardServices services
            , IRepository<UserAnswerRecord> userAnswerRepositor
            , IAnswerService answerService
            , ISurveyService surveyService)
        {
            _services = services;
            _surveyService = surveyService;
            _userAnswerService = new UserAnswerService(userAnswerRepositor, answerService);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Add(FormCollection form)
        {
            var record = new UserAnswerRecord
            {
                SurveyId = int.Parse(form["surveyId"] ?? "0"),
                AnswerId = int.Parse(form["question"] ?? "0")
            };
            var httpCookie = System.Web.HttpContext.Current.Request.Cookies["survey"];
            var cookieValues = new List<string>();
            if (httpCookie != null)
            {
                cookieValues.AddRange(httpCookie.Value.Split(','));
            }
            cookieValues.Add(record.SurveyId.ToString());
            var cookie = new HttpCookie("survey", string.Join(",", cookieValues))
                             {
                                 Expires = DateTime.MaxValue
                             };
             System.Web.HttpContext.Current.Response.Cookies.Add(cookie);



            if (_services.WorkContext.CurrentUser != null)
                record.UserId = _services.WorkContext.CurrentUser.Id;

            var survey = _surveyService.GetSurvey(record.SurveyId);



            if (((DateTimeField)survey.Get(typeof(DateTimeField), "DueDate")).DateTime.ToUniversalTime() >= DateTime.UtcNow)
                _userAnswerService.UpdateUserAnswer(record);

            if (Request.UrlReferrer != null)
                return this.RedirectLocal(Request.UrlReferrer.ToString(), "~/");

            return new EmptyResult();
        }



    }
}