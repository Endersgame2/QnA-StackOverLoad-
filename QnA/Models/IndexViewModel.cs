using Microsoft.AspNetCore.Mvc.Rendering;
using QnA.Enums;
using QnA.Models.Shared;

namespace QnA.Models
{
    public class IndexViewModel
    {
        public OrderMethod OrderMethod { get; set; }
        public int PageNumber { get; set; }
        public int TagId { get; set; }
        public MultiSelectList? TagOptions { get; set; }
        public QuestionsGridViewModel? QuestionsGrid { get; set; }
        public IndexViewModel()
        {
            QuestionsGrid = new QuestionsGridViewModel();
            OrderMethod = OrderMethod.Recent;
            PageNumber = 1;
            TagId = 0;
        }
    }
}
