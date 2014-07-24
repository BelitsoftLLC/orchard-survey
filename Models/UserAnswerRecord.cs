namespace Belitsoft.Orchard.Survey.Models
{
    public class UserAnswerRecord
    {
        public virtual int Id { get; set; }

        public virtual int AnswerId { get; set; }

        public virtual int SurveyId { get; set; }

        public virtual int UserId { get; set; }
    }
}