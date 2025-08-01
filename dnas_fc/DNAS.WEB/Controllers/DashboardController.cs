using DNAS.Application.Common.Interface;
using DNAS.Application.Features.DashBoard;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DNAS.Application.Common.Filter;
using System.Security.Claims;

namespace DNAS.WEB.Controllers
{
    [Authorize] 
    [TypeFilter(typeof(UserCurrentAuth))] 
    public class DashboardController(ISender iSender, ICustomLogger logger) : Controller
    {
        private readonly ISender _iSender = iSender;
        private readonly ICustomLogger _logger = logger;
        public async Task<IActionResult> Index()
        {
            try
            {
                var Id = User.FindFirstValue("UserId");
                var request = new DashBoardsCommand { id = Convert.ToInt32(Id) };
                var response = await _iSender.Send(request);

                return View(response.Data);
            }
            catch (Exception ex)
            {
                _logger.LogwriteInfo("exception occur during Dashboard page load" +
                Environment.NewLine + "exception message-" + ex.Message + Environment.NewLine + ex.StackTrace, "User_"+ User.FindFirstValue("UserId"));
                return View();
            }            
        }
    }
}
