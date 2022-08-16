using QnA.Models.Data.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace QnA.Models.Data
{
    public class QuestionTag : Table
    {
        [ForeignKey("Question")]
        public virtual int QuestionId { get; set; }
        public virtual Question Question { get; set; }
        [ForeignKey("Tag")]
        public virtual int TagId { get; set; }
        public virtual Tag Tag { get; set; }
    }
}
