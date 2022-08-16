using QnA.Models.Data.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace QnA.Models.Data
{
    public class Comment : Table
    {
        [ForeignKey("User")]
        public virtual int UserId { get; set; }
        public virtual User User { get; set; }
        [ForeignKey("Question")]
        public virtual int QuestionId { get; set; }
        public virtual Question Question { get; set; }
        [ForeignKey("Answer")]
        public virtual int? AnswerId { get; set; }
        public virtual Answer? Answer { get; set; }
        public virtual string Text { get; set; }
    }
}
