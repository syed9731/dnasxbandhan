using DNAS.Domain.DTO.Note;
using DNAS.Domain.DTO.Note.Common;
using DNAS.WEB.Models;

using Microsoft.AspNetCore.Mvc;

namespace DNAS.WEB.ViewComponents
{
    public class ApproverViewComponent : ViewComponent
    {   
        public async Task<IViewComponentResult> InvokeAsync(List<CommonApproverModel> viewModel, string NoteStatus,string viewType,string userId)
        {
            var data = new ApproverComponentViewModel
            {
                NoteStatus = NoteStatus,
                ApproverList=viewModel,
                ViewType = viewType,
                UserId = userId

            };
            return await Task.FromResult(View(data));

        }
    }
    

}
