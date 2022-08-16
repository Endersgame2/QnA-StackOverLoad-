using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QnA.Enums;
using QnA.Models;
using QnA.Models.Data;
using QnA.Models.Data.Shared;
using System.Diagnostics;

namespace QnA.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly QnADbContext _qnADbContext;

        public HomeController(ILogger<HomeController> logger, QnADbContext qnADbContext)
        {
            _logger = logger;
            _qnADbContext = qnADbContext;
        }
        [HttpGet]
        public IActionResult Index([FromQuery] IndexViewModel model = null)
        {
            if(model == null)
            {
                model = new IndexViewModel();
            }
            var tags = _qnADbContext.Tags.ToList();
            tags = tags.Prepend(new Tag()
            {
                Id = 0,
                Name = "-Unset-"
            }).ToList();
            model.TagOptions = new MultiSelectList(tags, "Id", "Name");
            var query = (from question in _qnADbContext.Questions
                         join user in _qnADbContext.Users on question.UserId equals user.Id
                         select new QuestionSummaryViewModel
                         {
                             QuestionId = question.Id,
                             Title = question.Title,
                             User = user.Name,
                             AnswerCount = (from answer in _qnADbContext.Answers where answer.QuestionId == question.Id select 1).Count(),
                             TimeStampUtc = question.TimeStampUtc,
                             TagIds = (from questionTag in _qnADbContext.QuestionTags where question.Id == questionTag.QuestionId select questionTag.TagId).ToList()
                         });
            if(model.TagId > 0)
            {
                query = query.Where(x => x.TagIds.Contains(model.TagId));
            }
            switch (model.OrderMethod)
            {
                case OrderMethod.Recent:
                    model.QuestionsGrid.Questions = query.OrderByDescending(x => x.TimeStampUtc).Skip((model.PageNumber - 1) * 10).Take(10).ToList();
                    break;
                case OrderMethod.Answered:
                    model.QuestionsGrid.Questions = query.OrderByDescending(x => x.AnswerCount).Skip((model.PageNumber - 1) * 10).Take(10).ToList();
                    break;
                default:
                    return BadRequest();
            }
            return View(model);
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}