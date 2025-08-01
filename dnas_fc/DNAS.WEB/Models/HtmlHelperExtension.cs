using DNAS.Domain.DTO.Note;
using DNAS.Domain.DTO.Note.Common;
using DNAS.Persistence.DataAccessContents;

using static SkiaSharp.HarfBuzz.SKShaper;
using System.Linq;

namespace DNAS.WEB.Models
{
    public static class HtmlHelperExtension
    {
        public static List<CommonRecomendedApproverModel> ConvertToRecomendedApproverComponent<T>(this IEnumerable<T> recomendedapproverModel) where T:CommonRecomendedApproverModel //IEnumerable<ViewRecomendedApproverModel>
        {
            return recomendedapproverModel.Select(m => new CommonRecomendedApproverModel
            {
                ApprovedTime = m.ApprovedTime,
                ApproverId = m.ApproverId,
                ApproverType = m.ApproverType,
                Deligated_UserId = m.Deligated_UserId,
                Department = m.Department,
                DesignationName = m.DesignationName,
                FirstName = m.FirstName,
                Grade = m.Grade,
                IsApproved = m.IsApproved,
                IsCurrentApprover = m.IsCurrentApprover,
                LastName = m.LastName,
                MiddleName = m.MiddleName,
                NoteId = m.NoteId,
                UserId = m.UserId,
                SkippBy = m.SkippBy,
                SkippTime = m.SkippTime
            }).ToList();
        }
        public static List<CommonApproverModel> ConvertToApproverComponent<T>(this IEnumerable<T> approverModel) where T : CommonApproverModel  //IEnumerable<ViewApproversModel>
        {
            //List<CommonApproverModel> sortedList = new();

            //var ff = approverModel.FirstOrDefault().ApproverId;


            //// Group entries by AddedBy for easy lookup
            //var addedByLookup = approverModel.Where(x=>x.AddedBy.HasValue).ToLookup(x => x.AddedBy!.Value);

            //foreach (var approver in approverModel.Where(x => x.AddedBy == null))
            //{
            //    int userId = Convert.ToInt32(approver.UserId);
            //    // Add entries with SuffixPrefix = 1 (before the current approver)
            //    if (addedByLookup.Contains(userId))
            //    {
            //        var beforeEntries = addedByLookup[userId]
            //            .Where(x => x.SuffixPrefix == 1);
            //        sortedList.AddRange(beforeEntries);
            //    }

            //    // Add the current approver
            //    sortedList.Add(approver);

            //    // Add entries with SuffixPrefix = 2 (after the current approver)
            //    if (addedByLookup.Contains(userId))
            //    {
            //        var afterEntries = addedByLookup[userId]
            //            .Where(x => x.SuffixPrefix == 2);
            //        sortedList.AddRange(afterEntries);
            //    }
            //}

            //return sortedList;
            return approverModel.Select(m => new CommonApproverModel
            {
                ApprovedTime = m.ApprovedTime,
                ApproverId = m.ApproverId,
                Deligated_UserId = m.Deligated_UserId,
                Department = m.Department,
                DesignationName = m.DesignationName,
                FirstName = m.FirstName,
                Grade = m.Grade,
                IsApproved = m.IsApproved,
                IsCurrentApprover = m.IsCurrentApprover,
                LastName = m.LastName,
                MiddleName = m.MiddleName,
                NoteId = m.NoteId,
                UserId = m.UserId,
                SkippBy = m.SkippBy,
                SkippTime = m.SkippTime,
                SuffixPrefix = m.SuffixPrefix,
                AddedBy = m.AddedBy
            }).ToList();
        }
    }
}
