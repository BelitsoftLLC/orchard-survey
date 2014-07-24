using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using Belitsoft.Orchard.Survey.Models;
using Belitsoft.Orchard.Survey.Services;
using Belitsoft.Orchard.Survey.ViewModels;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.Core.Common.Models;
using Orchard.Data;

namespace Belitsoft.Orchard.Survey.Drivers
{
    public class SurveyDriver : ContentPartDriver<SurveyPart>
    {
        #region Fields

        private readonly IContentManager _contentManager;
        private readonly IOrchardServices _services;
        private readonly IAnswerService _answerService;
        private readonly IUserAnswerService _userAnswerService;

        #endregion //Fields

        #region Constructors

        public SurveyDriver(IContentManager contentManager
            , IOrchardServices services
            , IRepository<AnswerRecord> answerRepository
            , IRepository<UserAnswerRecord> userAnswerRepositor
            , IAnswerService answerService)
        {
            _contentManager = contentManager;
            _services = services;
            _answerService = new AnswerService(answerRepository, _contentManager);
            _userAnswerService = new UserAnswerService(userAnswerRepositor, answerService);
        }

        #endregion //Constructors

        #region ContentPartDriver member

        protected override string Prefix
        {
            get { return "Survey"; }
        }

        protected override DriverResult Display(SurveyPart part, string displayType, dynamic shapeHelper)
        {
            return ContentShape("Parts_Survey",
                                () => shapeHelper.Parts_Survey(
                                    View: BuilViewModel(part)));
        }

        protected override DriverResult Editor(SurveyPart part, dynamic shapeHelper)
        {
            part.Answers = part.Id > 0 ? _answerService.GetAnswers(part.Id).ToList() : new List<AnswerRecord>();

            var temp = ContentShape("Parts_Survey_Edit",
                                () => shapeHelper.EditorTemplate(
                                    TemplateName: "Survey",
                                    Model: part,
                                    Prefix: Prefix));
            return temp;
        }

        protected override DriverResult Editor(SurveyPart part, IUpdateModel updater, dynamic shapeHelper)
        {
            if (updater.TryUpdateModel(part, Prefix, null, null))
            {
                var answers = ParseAnswers(_services.WorkContext.HttpContext.Request.Form);
                _answerService.UpdateAnswers(part.Id, answers);
            }
            return Editor(part, shapeHelper);
        }

        #endregion //ContentPartDriver

        #region Help Methods

        private IEnumerable<AnswerRecord> ParseAnswers(NameValueCollection form)
        {
            var numberOfAnswers = form.AllKeys.Count(p => p.Contains("text.Answer"));
            IList<AnswerRecord> records = new List<AnswerRecord>();
            for (int i = 0; i < numberOfAnswers; i++)
            {
                if (form.AllKeys.Contains("text.Answer" + i))
                {
                    var record = new AnswerRecord
                                     {
                                         Text = form["text.Answer" + i]
                                     };

                    if (form.AllKeys.Contains("Survey.answer.Id" + i))
                        record.Id = int.Parse(form["Survey.answer.Id" + i]);

                    if (form.AllKeys.Contains("text.StartValue" + i))
                    {
                        if (!string.IsNullOrWhiteSpace(form["text.StartValue" + i]))
                            record.StartValue = int.Parse(form["text.StartValue" + i] ?? "0");
                    }

                    record.Value = form["radio.Answer"] == i.ToString();
                    records.Add(record);
                }
                else
                {
                    numberOfAnswers++;
                }
               
            }



            return records;
        }

        private EditSurveyViewModel BuilViewModel(SurveyPart part)
        {
            var viewModel = new EditSurveyViewModel();
            //viewModel.UserIsAnswered = _userAnswerService.IsAnsweredSurvey(_services.WorkContext.CurrentUser.Id, part.Id);

            bool userIsAnswered = false;
            var httpCookie = HttpContext.Current.Request.Cookies["survey"];
            if (httpCookie != null)
                userIsAnswered = httpCookie.Value.Split(',').Contains(part.Id.ToString());
            else
                userIsAnswered = _userAnswerService.IsAnsweredSurvey(_services.WorkContext.CurrentUser.Id, part.Id);
            viewModel.UserIsAnswered = userIsAnswered;

            viewModel.Id = part.Id;
            viewModel.NumberOfAnswer = _userAnswerService.GetCountForAllAnswer(part.Id);

            //get question from body part
            viewModel.Body = part.ContentItem.Get(typeof(BodyPart)).As<BodyPart>().Text;
            viewModel.Answers = _answerService.GetAnswers(part.Id).ToList();
            viewModel.Result = new Dictionary<int, int>();

            //get created date value from common part
            var createdUtc = part.ContentItem.Get(typeof (CommonPart)).As<CommonPart>().CreatedUtc;
            if (createdUtc != null)
                viewModel.CreatedDate = createdUtc.Value.ToLocalTime().ToShortDateString();
            foreach (AnswerRecord answerRecord in viewModel.Answers)
            {
                viewModel.Result.Add(answerRecord.Id, _userAnswerService.GetCountForOneAnswer(answerRecord.Id));
            }
            return viewModel;
        }

        #endregion //Help Methods

    }
}