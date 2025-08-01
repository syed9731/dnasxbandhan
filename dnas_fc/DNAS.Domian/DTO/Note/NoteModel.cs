using Microsoft.AspNetCore.Http;

using System.ComponentModel.DataAnnotations;
using DNAS.Domain.HtmlRestrict;
using DNAS.Domain.NoSpecialCharacter;
namespace DNAS.Domian.DTO.Note
{
    public class NoteModel
    {
        [HtmlRestrict]
        public string NoteId { get; set; } = string.Empty;
        [HtmlRestrict]
        public string UserId { get; set; } = string.Empty;
        [HtmlRestrict]
        public string TemplateId { get; set; } = string.Empty;
        [Required(ErrorMessage = "Category is required.")]
        [HtmlRestrict]
        public string CategoryId { get; set; } = string.Empty;
        [HtmlRestrict]
        public string? ExpenseIncurredAtId { get; set; } = string.Empty;
        [HtmlRestrict]
        public string? NatureOfExpensesId { get; set; } = string.Empty;
        public string CreatorDepartment { get; set; } = string.Empty;
        public string NoteState { get; set; } = string.Empty;
        [Required(ErrorMessage = "Note title is required.")]
        [StringLength(500, ErrorMessage = "Maximum 500 characters allowed")]
        [HtmlRestrict]
        [NoSpecialCharacter(ErrorMessage ="Note title contains special character!")]
        public string NoteTitle { get; set; } = string.Empty;
        //[Range(0, double.MaxValue, ErrorMessage = "Please enter only number")]
        [HtmlRestrict]
        public string? CapitalExpenditure { get; set; } = string.Empty;
        //[Range(0, double.MaxValue, ErrorMessage = "Please enter only number")]
        [HtmlRestrict]
        public string? OperationalExpenditure { get; set; } = string.Empty;
        //[Range(0, double.MaxValue, ErrorMessage = "Please enter only number")]
        [HtmlRestrict]
        public string? TotalAmount { get; set; } = string.Empty;
        [Required(ErrorMessage = "Note Body is required.")]
        //[ScriptTagRestrict]
        public string NoteBody { get; set; } = string.Empty;
        public string DateOfCreation { get; set; } = string.Empty;
        public string WithdrawDate { get; set; } = string.Empty;
        public string NoteStatus { get; set; } = string.Empty;
        public string CurrentApproverId { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string? CategoryName { get; set; } = string.Empty;
        public string ApproverIdList { get; set; } = string.Empty;
        [HtmlRestrict]
        public string NoteUID { get;set; } = string.Empty;
        public List<IFormFile>? AttachFiles { get; set; }
        public string? RecomendedApproverIdList { get; set; } = string.Empty;
        public string MajorRevision { get; set; } = string.Empty;
        public bool IsAmend { get; set; } = false;
        public string notetype {  get; set; } = string.Empty;
    }
}
