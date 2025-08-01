using DNAS.Domain.DTO.Note.Common;

namespace DNAS.WEB.Models
{
    public class ApproverComponentViewModel
    {
        public List<CommonApproverModel> ApproverList { get; set; } = new();
        public string? NoteStatus { set; get; }
        public string? ViewType { get; set; }
        public string UserId { get; set; } = string.Empty;
    }
    public class RecomendedApproverComponentViewModel
    {
        public List<CommonRecomendedApproverModel> RecomendedApproverList { get; set; } = new();
        public string? NoteStatus { set; get; }
        public string? ViewType { get; set; }
    }

    
}
