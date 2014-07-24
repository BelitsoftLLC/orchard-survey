using System.ComponentModel.DataAnnotations;

namespace Belitsoft.Orchard.Survey.Models
{
    public class AnswerRecord
    {
        public virtual int Id { get; set; }
        public virtual bool Value { get; set; }
        [Required]
        public virtual string Text { get; set; }
        public virtual int SurveyId { get; set; }
        public virtual int StartValue { get; set; }
    }

}