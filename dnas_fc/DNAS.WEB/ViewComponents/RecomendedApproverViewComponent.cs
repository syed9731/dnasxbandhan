using DNAS.Domain.DTO.Note.Common;
using DNAS.WEB.Models;

using Microsoft.AspNetCore.Mvc;

namespace DNAS.WEB.ViewComponents
{
    public class RecomendedApproverViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(List<CommonRecomendedApproverModel> viewModel, string NoteStatus, string viewType)
        {
            var data = new RecomendedApproverComponentViewModel
            {
                NoteStatus = NoteStatus,
                RecomendedApproverList = viewModel,
                ViewType= viewType
                
            };
            return await Task.FromResult(View(data));

        }
    }
}
