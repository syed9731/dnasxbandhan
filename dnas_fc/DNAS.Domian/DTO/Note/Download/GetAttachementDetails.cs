
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNAS.Domain.DTO.Note.Download
{

    public class GetAttachementDetailsModel
    {
        public GetAttachementDetails getAttachementDetails { get; set; } = new GetAttachementDetails();
    }
    public class GetAttachementDetails
    {
        public string AttachmentPath { get; set; } = default!;
        public string DocumentName { get; set; } = default!;

    }
}
