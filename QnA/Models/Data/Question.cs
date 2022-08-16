using QnA.Models.Data.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace QnA.Models.Data
{
    public class Question : Table
    {
        [ForeignKey("User")]
        public virtual int? UserId { get; set; }
        public virtual User User { get; set; }
        public virtual string Title { get; set; }
        public virtual string Text { get; set; }
        public virtual DateTime TimeStampUtc { get; set; }
        [ForeignKey("Answer")]
        public virtual int? AcceptedAnswerId { get; set; }
        public virtual Answer Answer { get; set; }
        public virtual ICollection<Answer> Answers { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<QuestionTag> QuestionTags { get; set; }
        public virtual ICollection<Vote> Votes { get; set; }

    }
}
