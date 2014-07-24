using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Orchard.Core.Title.Models;
using Belitsoft.Orchard.Survey.Models;
using Belitsoft.Orchard.Survey.Services;
using Belitsoft.Orchard.Survey.ViewModels;
using NHibernate.Linq;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.Core.Common.Models;
using Orchard.Data;
using Orchard.Fields.Fields;
using Orchard.Security;

namespace Belitsoft.Orchard.Survey.Drivers
{
    public class SurveyWidgetDriver : ContentPartDriver<SurveyWidget>
    {
        #region Fields

        private readonly IContentManager _contentManager;
        private readonly IOrchardServices _services;
        private readonly IRepository<SurveyPartRecord> _surveyRepository;
        private readonly IAnswerService _answerService;
        private readonly IUserAnswerService _userAnswerService;
        private readonly IAuthenticationService _authenticationService;
        private readonly ISurveyService _surveyService;

        #endregion //Fields

        #region Constructors

        public SurveyWidgetDriver(IContentManager contentManager
            , IOrchardServices services
            , IRepository<AnswerRecord> answerRepository
            , IRepository<UserAnswerRecord> userAnswerRepositor
            , IRepository<SurveyPartRecord> surveyRepository
            , IAuthenticationService authenticationService
            , IAnswerService answerService
            , ISurveyService surveyService)
        {
            _contentManager = contentManager;
            _services = services;
            _surveyRepository = surveyRepository;
            _authenticationService = authenticationService;
            _surveyService = surveyService;
            _answerService = new AnswerService(answerRepository, _contentManager);
            _userAnswerService = new UserAnswerService(userAnswerRepositor, answerService);
        }

        #endregion Constructors

        #region ContentPartDriver members

        protected override string Prefix
        {
            get { return "SurveyWidget"; }
        }

        protected override DriverResult Display(SurveyWidget part, string displayType, dynamic shapeHelper)
        {
            //get survey part by 
            var surveyPart = _contentManager.Get((int)part.SurveyId).Get(typeof(SurveyPart)).As<SurveyPart>();
            var model = BuildViewModel(surveyPart);
            HttpContext.Current.Response.Headers.Remove("__r");
            return ContentShape("Parts_SurveyWidget",
                                () => shapeHelper.Parts_SurveyWidget(
                                    View: model));
        }

        protected override DriverResult Editor(SurveyWidget part, dynamic shapeHelper)
        {
            var surveyWidgetViewModel = new SurveyWidgetViewModel
                                            {
                                                SurveyId = part.SurveyId
                                            };

            var surveys = _surveyService.GetSurveys().Select(p => new SelectListItem()
                                                                      {
                                                                          Text = p.ContentItem.Get(typeof(TitlePart)).As<TitlePart>().Title,
                                                                          Value = p.Id.ToString(CultureInfo.InvariantCulture)
                                                                      }).ToList();
            surveyWidgetViewModel.Surveys = surveys;

            var tempData = ContentShape("Parts_SurveyWidget_Edit",
                                () => shapeHelper.EditorTemplate(
                                    TemplateName: "SurveyWidget",
                                    Model: surveyWidgetViewModel));
            return tempData;
        }

        protected override DriverResult Editor(SurveyWidget part, IUpdateModel updater, dynamic shapeHelper)
        {
            part.SurveyId = int.Parse(HttpContext.Current.Request.Form["SurveyId"]);
            updater.TryUpdateModel(part, Prefix, null, null);
            return Editor(part, shapeHelper);
        }

        #endregion //ContentPartDriver members

        #region Help methods

        private EditSurveyViewModel BuildViewModel(SurveyPart part)
        {
            if (part == null)
                return new EditSurveyViewModel();
            var viewModel = new EditSurveyViewModel();
            viewModel.Id = part.Id;
            
            viewModel.NumberOfAnswer = _userAnswerService.GetCountForAllAnswer(part.Id);
            viewModel.Body = part.ContentItem.Get(typeof(BodyPart)).As<BodyPart>().Text;
            viewModel.Answers = _answerService.GetAnswers(part.Id).ToList();
            viewModel.IsNotShowResultAfterAnswer = part.IsNotShowResultAfterAnswer;
            viewModel.Result = new Dictionary<int, int>();

            //get value of datetime field and check for past-due
            viewModel.IsExpired = ((DateTimeField)part.Get(typeof(DateTimeField), "DueDate")).DateTime.ToUniversalTime() < DateTime.UtcNow;

            //get value of datetime field and check on publishing
            viewModel.IsPublish =
                part.Fields.FirstOrDefault(p => p.Name == "DueDate").As<DateTimeField>().DateTime.ToUniversalTime() > DateTime.UtcNow;
            var createdUtc = part.ContentItem.Get(typeof(CommonPart)).As<CommonPart>().CreatedUtc;
            if (createdUtc != null)
                viewModel.CreatedDate = createdUtc.Value.ToLocalTime().ToShortDateString();

            viewModel.UserIsAnswered = UserIsAnswered(viewModel);

            foreach (AnswerRecord answerRecord in viewModel.Answers)
            {
                viewModel.Result.Add(answerRecord.Id, _userAnswerService.GetCountForOneAnswer(answerRecord.Id));
            }

           // HttpContext.Current.Request.QueryString.Clear();
            return viewModel;
        }

        private bool UserIsAnswered(EditSurveyViewModel viewModel)
        {
            bool userIsAnswered = false;
            var httpCookie = HttpContext.Current.Request.Cookies["survey"];

            if (_services.WorkContext.CurrentUser != null)
            {
                if (httpCookie != null)
                    userIsAnswered = httpCookie.Value.Split(',').Contains(viewModel.Id.ToString());
                else
                    userIsAnswered = _userAnswerService.IsAnsweredSurvey(_services.WorkContext.CurrentUser.Id, viewModel.Id);
            }
            else
            {
                if (httpCookie != null)
                    userIsAnswered = httpCookie.Value.Split(',').Contains(viewModel.Id.ToString());
            }

            return userIsAnswered;
        }

        #endregion //Help methods
    }
}