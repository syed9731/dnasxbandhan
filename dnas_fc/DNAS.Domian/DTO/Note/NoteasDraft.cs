using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNAS.Domian.DTO.Note
{
    public class NoteasDraft
    {
        public class ContentUpdate
        {
            public string noteid { get; set; } = string.Empty;
            public string NoteBody { get; set; } = string.Empty;
        }


        public class ContentNew
        {
            public string NoteBody { get; set; } = string.Empty;

        }

        public class Savetemplate
        {
            public string userid { get; set; } = string.Empty;
            public string catid { get; set; } = string.Empty;
            [RegularExpression(@"[^<>]*", ErrorMessage = "HTML Tag are not allowed")]
            public string tempname { get; set; } = string.Empty;
            [RegularExpression(@"[^<>]*", ErrorMessage = "HTML Tag are not allowed")]
            public string notetitle { get; set; } = string.Empty;
            public string notebody { get; set; } = string.Empty;
        }
    }

}
