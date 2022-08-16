using Microsoft.AspNetCore.Mvc;
using QnA.Attributes;
using QnA.Models;
using QnA.Models.Data.Shared;

namespace QnA.Controllers
{
    [QnAAuthorize("Admin")]
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly QnADbContext _qnADbContext;
        public AdminController(QnADbContext qnADbContext, ILogger<AdminController> logger)
        {
            _qnADbContext = qnADbContext;
            _logger = logger;
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View(PrepareAdminViewModel());
        }
        [HttpGet, Route("Delete/{id:int}")]
        public IActionResult DeleteQuestion([FromRoute] int id)
        {
            try
            {
                _qnADbContext.Database.BeginTransaction();
                var votes = _qnADbContext.Votes.Where(vote => vote.QuestionId == id);
                _qnADbContext.Votes.RemoveRange(votes);
                var comments = _qnADbContext.Comments.Where(comment => comment.QuestionId == id);
                _qnADbContext.Comments.RemoveRange(comments);
                var answers = _qnADbContext.Answers.Where(answer => answer.QuestionId == id);
                _qnADbContext.Answers.RemoveRange(answers);
                var questionTags = _qnADbContext.QuestionTags.Where(tag => tag.QuestionId == id);
                _qnADbContext.QuestionTags.RemoveRange(questionTags);
                var question = _qnADbContext.Questions.FirstOrDefault(question => question.Id == id);
                _qnADbContext.Questions.Remove(question);
                _qnADbContext.SaveChanges();
                _qnADbContext.Database.CurrentTransaction.Commit();
                var model = PrepareAdminViewModel();
                model.Notification.Message = "Deleted successfully";
                model.Notification.MessageClass = "alert-success";
                return View("Index", model);
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to delete question ${id}", e);
                _qnADbContext.Database.CurrentTransaction.Rollback();
                return RedirectToAction("Error", "Home");
            }
        }
        private AdminViewModel PrepareAdminViewModel()
        {
            var model = new AdminViewModel();
            model.QuestionsGrid.Questions = (from question in _qnADbContext.Questions
                                             join user in _qnADbContext.Users on question.UserId equals user.Id
                                             select new QuestionSummaryViewModel
                                             {
                                                 QuestionId = question.Id,
                                                 Title = question.Title,
                                                 User = user.Name,
                                                 AnswerCount = (from answer in _qnADbContext.Answers where answer.QuestionId == question.Id select 1).Count(),
                                                 TimeStampUtc = question.TimeStampUtc,
                                                 TagIds = (from questionTag in _qnADbContext.QuestionTags where question.Id == questionTag.QuestionId select questionTag.TagId).ToList()
                                             }).ToList();
            return model;
        }
    }
}
