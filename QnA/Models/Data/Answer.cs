﻿using QnA.Models.Data.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace QnA.Models.Data
{
    public class Answer : Table
    {
        [ForeignKey("User")]
        public virtual int UserId { get; set; }
        public virtual User User { get; set; }
        [ForeignKey("Question")]
        public virtual int QuestionId { get; set; }
        public virtual Question Question { get; set; }
        public virtual string Text { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Vote> Votes { get; set; }
    }
}
