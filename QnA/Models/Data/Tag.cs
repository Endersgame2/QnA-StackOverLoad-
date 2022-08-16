using QnA.Models.Data.Shared;

namespace QnA.Models.Data
{
    public class Tag : Table
    {
        public virtual string Name { get; set; }
        public virtual ICollection<QuestionTag> QuestionTags { get; set; }
    }
}
